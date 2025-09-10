using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShopAdmin.Authorize;
using ShopAdmin.Dto;
using ShopAdmin.Dto.Products;
using ShopAdmin.Helper;
using ShopAdmin.Models;
using ShopAdmin.WHttpMessage;

namespace ShopAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAccessRole]
    public class ProductController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetTable(SearchDto itemSearch)
        {
            HttpMessage httpMessage = new HttpMessage(true);
            try
            {
                var query = db.Products.Include(x => x.Category).AsNoTracking().Where(x => true);
                if (!string.IsNullOrEmpty(itemSearch.Ten))
                {
                    query = query.Where(x => x.SKU.Contains(itemSearch.Ma) || x.Name.Contains(itemSearch.Ten));
                }
                var result = await query.Select(x => new
                {
                    Id = x.Id,
                    SKU = x.SKU,
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    CategoryName = x.Category.Name
                }).OrderBy(x => x.Id).Skip(itemSearch.Skip).Take(itemSearch.PageSize).ToListAsync().ConfigureAwait(false);
                httpMessage.Body.Data = result;
                httpMessage.Body.Pagination = new HttpMessagePagination
                {
                    NumberRowsOnPage = itemSearch.PageSize,
                    PageNumber = itemSearch.PageNum,
                    TotalRowsOnPage = result.Count,
                    TotalRows = await query.CountAsync().ConfigureAwait(false)
                };
                return Json(httpMessage);
            }
            catch (Exception ex)
            {
                httpMessage.IsOk = false;
                httpMessage.Body.MsgNoti = new HttpMessageNoti("500", null, ex.Message);
                return Json(httpMessage);
            }
        }

        public async Task<JsonResult> GetItem(int id)
        {
            HttpMessage httpMessage = new HttpMessage(true);
            try
            {
                var item = await db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
                if (item == null)
                {
                    httpMessage.IsOk = false;
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("400", null, "Không tìm thấy thông tin");
                    return Json(httpMessage);
                }

                var productVariant = await db.ProductVariants.AsNoTracking().Where(x => x.ProductId == id).Select(x => new
                {
                    Id = x.Id,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    Sku = x.Sku,
                    Stock = x.Quantity,
                }).ToListAsync().ConfigureAwait(false);

                var images = await db.ProductImages.AsNoTracking().Where(x => x.ProductId == id).Select(x => new
                {
                    x.ImageUrl,
                    x.ImageName,
                    x.Id
                }).ToListAsync().ConfigureAwait(false);
                httpMessage.Body.Data = new { ProductItem = item, Variants = productVariant, Images = images };
                return Json(httpMessage);
            }
            catch (Exception ex)
            {
                httpMessage.IsOk = false;
                httpMessage.Body.MsgNoti = new HttpMessageNoti("500", null, ex.Message);
                return Json(httpMessage);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Create([FromForm] List<IFormFile> images)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                HttpMessage httpMessage = new HttpMessage(true);
                try
                {
                    string product = HttpContext.Request.Form["product"].ToString();
                    string attrs = HttpContext.Request.Form["attrs"].ToString();
                    var entity = JsonConvert.DeserializeObject<Product>(product);
                    var listAttribute = JsonConvert.DeserializeObject<List<ProductAttributeValues>>(attrs);

                    entity.Stock = entity.Quantity;
                    entity.Images = await SaveImage(db, images, entity.SKU);
                    entity.Attributes = listAttribute.Select(x => new ProductAttribute
                    {
                        Name = x.Name,
                        Values = x.Values.Select(y => new ProductAttributeValue
                        {
                            Value = y
                        }).ToList()
                    }).ToList();

                    await db.Products.AddAsync(entity);
                    await db.SaveChangesAsync();

                    List<List<string>> lists = new List<List<string>>();
                    foreach (var attr in listAttribute)
                    {
                        lists.Add(attr.Values);
                    }
                    var attributes = CartesianProduct(lists);
                    var productVariants = attributes.Select(x => new ProductVariant
                    {
                        ProductId = entity.Id,
                        Price = entity.Price,
                        Quantity = entity.Quantity,
                        Sku = $"{entity.SKU}-{string.Join('-', x)}",
                        Stock = entity.Stock,
                        VariantValues = db.ProductAttributeValues.Include(y => y.ProductAttribute).Where(y => x.Contains(y.Value) && y.ProductAttribute.ProductId == entity.Id).Select(y => new ProductVariantValue
                        {
                            ProductAttributeValueId = y.Id
                        }).ToList()
                    }).ToList();
                    await db.ProductVariants.AddRangeAsync(productVariants);
                    await db.SaveChangesAsync();

                    await tran.CommitAsync();

                    httpMessage.Body.MsgNoti = new HttpMessageNoti("200", null, "Thêm mới thành công");
                    return Json(httpMessage);
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    httpMessage.IsOk = false;
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("500", null, ex.Message);
                    return Json(httpMessage);
                }
            }
        }

        [HttpPost]
        public async Task<JsonResult> Update([FromForm] List<IFormFile> images)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                HttpMessage httpMessage = new HttpMessage(true);
                try
                {
                    string product = HttpContext.Request.Form["product"].ToString();
                    var entity = JsonConvert.DeserializeObject<Product>(product);

                    var item = await db.Products.FirstOrDefaultAsync(x => x.Id == entity.Id).ConfigureAwait(false);

                    item.SKU = entity.SKU;
                    item.Name = entity.Name;
                    item.CategoryId = entity.CategoryId;
                    item.Quantity = entity.Quantity + (entity.Stock - item.Stock);
                    item.Stock = entity.Stock;
                    item.Description = entity.Description;
                    item.Price = entity.Price;

                    db.Entry(item).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    var productImages = await SaveImage(db, images, entity.SKU);
                    productImages.ForEach(x => x.ProductId = item.Id);
                    await db.ProductImages.AddRangeAsync(productImages);
                    await db.SaveChangesAsync();

                    await tran.CommitAsync();

                    httpMessage.Body.MsgNoti = new HttpMessageNoti("200", null, "Cập nhật thành công");
                    return Json(httpMessage);
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    httpMessage.IsOk = false;
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("500", null, ex.Message);
                    return Json(httpMessage);
                }
            }
        }

        public async Task<JsonResult> UpdateVariant(int id, string sku, int stock)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                HttpMessage httpMessage = new HttpMessage(true);
                try
                {
                    var item = await db.ProductVariants.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
                    item.Sku = sku;
                    item.Quantity = item.Quantity + (stock - item.Stock);
                    item.Stock = stock;
                    db.Entry(item).State = EntityState.Modified;
                    await db.SaveChangesAsync().ConfigureAwait(false);
                    await tran.CommitAsync().ConfigureAwait(false);

                    httpMessage.Body.Description = "Cập nhật thành công";
                    return Json(httpMessage);
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync().ConfigureAwait(false);
                    httpMessage.IsOk = false;
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("500", null, ex.Message);
                    return Json(httpMessage);
                }
            }
        }
        public async Task<JsonResult> Delete(int id)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                HttpMessage httpMessage = new HttpMessage(true);
                try
                {
                    var item = await db.Products.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
                    db.Products.Remove(item);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                    await tran.CommitAsync().ConfigureAwait(false);

                    await RemoveFolderImg(item.SKU);

                    httpMessage.Body.Description = "Xóa sản phẩm thành công";
                    return Json(httpMessage);
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync().ConfigureAwait(false);
                    httpMessage.IsOk = false;
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("500", null, ex.Message);
                    return Json(httpMessage);
                }
            }
        }

        public async Task<JsonResult> RemoveImg(int id)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                HttpMessage httpMessage = new HttpMessage(true);
                try
                {
                    var item = await db.ProductImages.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", item.ImageUrl.TrimStart('/'));

                    db.ProductImages.Remove(item);
                    await db.SaveChangesAsync().ConfigureAwait(false);

                    System.IO.File.Delete(filePath);

                    await tran.CommitAsync().ConfigureAwait(false);


                    httpMessage.Body.Description = "Xóa ảnh thành công";
                    return Json(httpMessage);
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync().ConfigureAwait(false);
                    httpMessage.IsOk = false;
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("500", null, ex.Message);
                    return Json(httpMessage);
                }
            }
        }

        public async Task<JsonResult> GetListVariant(List<ProductAttributeValues> attrs, Product product)
        {
            HttpMessage httpMessage = new HttpMessage(true);

            try
            {
                List<List<string>> lists = new List<List<string>>();
                foreach (var attr in attrs)
                {
                    lists.Add(attr.Values);
                }
                var attributes = CartesianProduct(lists);

                var productVariants = attributes.Select(x => new
                {
                    Price = product.Price,
                    Quantity = product.Quantity,
                    Sku = $"{product.SKU}-{string.Join('-', x)}",
                    Stock = product.Quantity,
                }).ToList();

                httpMessage.Body.Data = productVariants;
                return Json(httpMessage);
            }
            catch (Exception ex)
            {
                httpMessage.IsOk = false;
                httpMessage.Body.MsgNoti = new HttpMessageNoti("500", null, ex.Message);
                return Json(httpMessage);
            }
        }

        private async Task<List<ProductImage>> SaveImage(ShopDbContext db, List<IFormFile> images, string sku)
        {
            // Thư mục lưu ảnh
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductImg", SlugHelper.GenerateSlug(sku));
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var lstImage = new List<ProductImage>();
            foreach (var file in images)
            {
                if (file.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    lstImage.Add(new ProductImage
                    {
                        ImageUrl = $"/ProductImg/{SlugHelper.GenerateSlug(sku)}/{fileName}",
                        ImageName = fileName
                    });
                    // TODO: Lưu đường dẫn file vào DB (vd: ProductImages table)
                    // Insert(ProductId, "/uploads/products/" + fileName);
                }
            }
            return lstImage;
        }

        private async Task RemoveFolderImg(string folderName)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductImg", folderName);
            var dir = new DirectoryInfo(folderPath);
            dir.Delete(true);
        }
        private static List<List<string>> CartesianProduct(List<List<string>> lists)
        {
            return lists.Aggregate(
                new List<List<string>> { new List<string>() },
                (acc, list) =>
                    (from accseq in acc
                     from item in list
                     select new List<string>(accseq) { item }).ToList()
            );
        }
    }
}
