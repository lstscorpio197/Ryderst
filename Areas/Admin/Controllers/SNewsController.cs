using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAdmin.Authorize;
using ShopAdmin.Dto;
using ShopAdmin.Models;
using ShopAdmin.WHttpMessage;

namespace ShopAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAccessRole]
    public class SNewsController : BaseController
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
                var query = db.SNews.AsNoTracking().Where(x => true);

                var result = await query.Select(x => new
                {
                    x.Id,
                    x.Title,
                    NgayTao = x.CreatedTime.ToString("dd/MM/yyyy HH:mm:ss"),
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
                var item = await db.SNews.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
                if (item == null)
                {
                    httpMessage.IsOk = false;
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("400", null, "Không tìm thấy thông tin");
                    return Json(httpMessage);
                }
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
                var item = await db.SNews.FindAsync(id).ConfigureAwait(false);
                if (item == null)
                {
                    httpMessage.IsOk = false;
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("400", null, "Không tìm thấy thông tin");
                    return Json(httpMessage);
                }
                db.SNews.Remove(item);
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
        public async Task<JsonResult> Create(SNew input)
        {
            HttpMessage httpMessage = new HttpMessage(true);
            try
            {
                SNew item = new SNew
                {
                    Id = input.Id,
                    Title = input.Title,
                    ContentNews = input.ContentNews,
                    CreatedTime = DateTime.Now,
                };
                await db.SNews.AddAsync(item);
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
        public async Task<JsonResult> Update(SNew input)
        {
            HttpMessage httpMessage = new HttpMessage(true);
            try
            {
                var exist = await db.SNews.FindAsync(input.Id).ConfigureAwait(false);
                if (exist == null)
                {
                    httpMessage.IsOk = false;
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("400", null, "Không tìm thấy thông tin");
                    return Json(httpMessage);
                }

                exist.Title = input.Title;
                exist.ContentNews = input.ContentNews;
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

        [HttpPost]
        public IActionResult UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

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
            return Json(url);
        }
    }
}
