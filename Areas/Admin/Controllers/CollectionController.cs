using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShopAdmin.Authorize;
using ShopAdmin.Dto;
using ShopAdmin.Dto.Collections;
using ShopAdmin.Models;
using ShopAdmin.WHttpMessage;

namespace ShopAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAccessRole]
    public class CollectionController : BaseController
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
                var query = db.Collections.AsNoTracking().Where(x => true);
                if (!string.IsNullOrEmpty(itemSearch.Ten))
                {
                    query = query.Where(x => x.Name.Contains(itemSearch.Ten) || x.Slug.Contains(itemSearch.Ten));
                }
                var result = await query.Select(x => new CollectionDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Slug = x.Slug
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
                var item = await db.Collections.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);

                if (item == null)
                {
                    httpMessage.IsOk = false;
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("400", null, "Không tìm thấy thông tin");
                    return Json(httpMessage);
                }

                item.Products = await db.Products.AsNoTracking().Where(x => x.CollectionId == item.Id).ToListAsync().ConfigureAwait(false);

                httpMessage.Body.Data = item;
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
        [AuthorizeAccessRole(typeHandle: "delete")]
        public async Task<JsonResult> Delete(int id)
        {
            HttpMessage httpMessage = new HttpMessage(true);
            try
            {
                var item = await db.Collections.FindAsync(id).ConfigureAwait(false);
                if (item == null)
                {
                    httpMessage.IsOk = false;
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("400", null, "Không tìm thấy thông tin");
                    return Json(httpMessage);
                }
                db.Collections.Remove(item);
                await db.SaveChangesAsync().ConfigureAwait(false);
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
        [AuthorizeAccessRole(typeHandle: "create")]
        public async Task<JsonResult> Create([FromForm] IFormFile file)
        {
            HttpMessage httpMessage = new HttpMessage(true);
            try
            {
                string dataInput = HttpContext.Request.Form["data"].ToString();
                CreateOrUpdateCollectionDto input = JsonConvert.DeserializeObject<CreateOrUpdateCollectionDto>(dataInput);

                string stringIds = HttpContext.Request.Form["productIds"].ToString();
                List<long> productIds = JsonConvert.DeserializeObject<List<long>>(stringIds);

                httpMessage = await CheckValid(input);
                if (!httpMessage.IsOk)
                {
                    return Json(httpMessage);
                }
                Collection item = new Collection
                {
                    Id = input.Id,
                    Name = input.Name,
                    Slug = StringConverter.ConvertToSlug(input.Name),
                    Thumbnail = UploadImage(file),
                    Description = input.Description
                };

                var products = await db.Products.Where(x => productIds.Contains(x.Id)).ToListAsync().ConfigureAwait(false);
                item.Products = products;

                await db.Collections.AddAsync(item);
                await db.SaveChangesAsync().ConfigureAwait(false);
                httpMessage.Body.MsgNoti = new HttpMessageNoti("200", null, "Thêm mới thành công");
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
        [AuthorizeAccessRole(typeHandle: "update")]
        public async Task<JsonResult> Update([FromForm] IFormFile file)
        {
            HttpMessage httpMessage = new HttpMessage(true);
            try
            {
                string dataInput = HttpContext.Request.Form["data"].ToString();
                CreateOrUpdateCollectionDto input = JsonConvert.DeserializeObject<CreateOrUpdateCollectionDto>(dataInput);

                string stringIds = HttpContext.Request.Form["productIds"].ToString();
                List<long> productIds = JsonConvert.DeserializeObject<List<long>>(stringIds);

                httpMessage = await CheckValid(input);
                if (!httpMessage.IsOk)
                {
                    return Json(httpMessage);
                }

                var exist = await db.Collections.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == input.Id).ConfigureAwait(false);
                if (exist == null)
                {
                    httpMessage.IsOk = false;
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("400", null, "Không tìm thấy thông tin");
                    return Json(httpMessage);
                }

                exist.Name = input.Name;
                exist.Slug = StringConverter.ConvertToSlug(input.Name);
                exist.Description = input.Description;
                exist.Thumbnail = string.IsNullOrEmpty(input.Thumbnail) ? UploadImage(file) : input.Thumbnail;

                var products = await db.Products.Where(x => productIds.Contains(x.Id)).ToListAsync().ConfigureAwait(false);
                exist.Products = products;

                db.Entry(exist).State = EntityState.Modified;
                await db.SaveChangesAsync().ConfigureAwait(false);
                httpMessage.Body.MsgNoti = new HttpMessageNoti("200", null, "Cập nhập thông tin thành công");
                return Json(httpMessage);
            }
            catch (Exception ex)
            {
                httpMessage.IsOk = false;
                httpMessage.Body.MsgNoti = new HttpMessageNoti("500", null, ex.Message);
                return Json(httpMessage);
            }
        }

        public async Task<JsonResult> GetListProduct()
        {
            HttpMessage httpMessage = new HttpMessage(true);
            try
            {
                var products = await db.Products.AsNoTracking().Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.SKU
                }).ToListAsync().ConfigureAwait(false);
                httpMessage.Body.Data = products;
                return Json(httpMessage);
            }
            catch (Exception ex)
            {
                httpMessage.IsOk = false;
                httpMessage.Body.MsgNoti = new HttpMessageNoti("500", null, ex.Message);
                return Json(httpMessage);
            }
        }

        private string UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Trả về URL để Summernote chèn vào content
            var url = "/uploads/" + fileName;
            return url;
        }
        private async Task<HttpMessage> CheckValid(CreateOrUpdateCollectionDto item)
        {
            HttpMessage httpMessage = new HttpMessage(false);
            try
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("400", null, "Vui lòng nhập tên danh mục");
                    return httpMessage;
                }

                var exist = await db.Collections.AsNoTracking().FirstOrDefaultAsync(x => x.Id != item.Id && x.Name == item.Name).ConfigureAwait(false);
                if (exist != null)
                {
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("400", null, "Tên đăng nhập hoặc đường dẫn này đã tồn tại");
                    return httpMessage;
                }
                httpMessage.IsOk = true;
                return httpMessage;
            }
            catch (Exception ex)
            {
                httpMessage.IsOk = false;
                httpMessage.Body.MsgNoti = new HttpMessageNoti("500", null, ex.Message);
                return httpMessage;
            }
        }
    }
}
