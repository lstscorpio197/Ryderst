using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAdmin.Dto.Products;
using ShopAdmin.Helper;
using ShopAdmin.Models;
using ShopAdmin.WHttpMessage;

namespace ShopAdmin.Areas.User.Controllers
{
    [Area("User")]
    public class ProductController : Controller
    {
        public async Task<IActionResult> Index(string keyword = null, decimal? maxprice = null, string sort = null)
        {
            ViewBag.Search = keyword;
            ViewBag.Maxprice = maxprice;
            ViewBag.Sort = sort;
            return View(await GetList(keyword, maxprice, sort));
        }
        public async Task<IActionResult> Detail(string name, int id)
        {
            return View(await GetItem(id));
        }
        public async Task<JsonResult> GetRelatedItems(int id, int cateId)
        {
            using (var db = new ShopDbContext())
            {
                HttpMessage httpMessage = new HttpMessage(true);

                var products = await db.Products.Include(x => x.Images).AsNoTracking().Where(x => x.CategoryId == cateId && x.Id != id).Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Price,
                    x.PriceDiscount,
                    ImageUrl = x.Images.Select(x => x.ImageUrl).FirstOrDefault(),
                    Slug = SlugHelper.GenerateSlug(x.Name)
                }).OrderBy(p => Guid.NewGuid()).Skip(0).Take(6).ToListAsync().ConfigureAwait(false);
                httpMessage.Body.Data = products;
                return Json(httpMessage);
            }
        }
        private async Task<ProductDto> GetItem(int id)
        {
            using (var db = new ShopDbContext())
            {
                var product = await db.Products.Include(x => x.Images).Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);

                ProductDto result = new()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    PriceDiscount = product.PriceDiscount,
                    Description = product.Description,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category.Name,
                    ImageUrl = product.Images.Select(x => x.ImageUrl).ToList(),
                };
                result.CountInStock = await db.ProductVariants.AsNoTracking().Where(x => x.ProductId == product.Id).SumAsync(x => x.Quantity).ConfigureAwait(false);
                result.Attributes = await db.ProductAttributes.AsNoTracking().Where(x => x.ProductId == product.Id).Select(x => new AttributeDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ProductAttrs = x.Values.Select(y => new ProductAttrDto { Id = y.Id, Value = y.Value }).ToList()
                }).ToListAsync().ConfigureAwait(false);

                return result;
            }
        }

        private async Task<List<ProductDto>> GetList(string search, decimal? maxprice, string sort)
        {
            using (var db = new ShopDbContext())
            {
                var query = db.Products.Include(x => x.Images).Include(x => x.Variants).AsNoTracking().Where(x => x.Visible != false);
                if (search != null)
                    query = query.Where(x => x.SKU.Contains(search) || x.Name.Contains(search));
                if (maxprice != null)
                    query = query.Where(x => (x.PriceDiscount.HasValue && x.PriceDiscount.Value < maxprice) || x.Price < maxprice);

                var items = await query.Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    PriceDiscount = x.PriceDiscount,
                    Description = x.Description,
                    CategoryId = x.CategoryId,
                    ImageUrl = x.Images.Select(x => x.ImageUrl).ToList(),
                    CountInStock = x.Variants.Sum(v => v.Quantity)
                }).ToListAsync().ConfigureAwait(false);

                switch (sort)
                {
                    case "price-asc":
                        items = items.OrderBy(x => x.PriceDiscount ?? x.Price).ToList();
                        break;
                    case "price-desc":
                        items = items.OrderByDescending(x => x.PriceDiscount ?? x.Price).ToList();
                        break;
                    case "name-asc":
                        items = items.OrderBy(x => x.Name).ToList();
                        break;
                    case "name-desc":
                        items = items.OrderByDescending(x => x.Name).ToList();
                        break;
                }

                return items;
            }
        }
    }
}
