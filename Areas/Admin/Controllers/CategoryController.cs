using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAdmin.Authorize;
using ShopAdmin.Dto;
using ShopAdmin.Dto.Categories;
using ShopAdmin.Models;
using ShopAdmin.WHttpMessage;

namespace ShopAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAccessRole]
    public class CategoryController : BaseController
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
                var query = db.Categories.AsNoTracking().Where(x => true);
                if (!string.IsNullOrEmpty(itemSearch.Ten))
                {
                    query = query.Where(x => x.Name.Contains(itemSearch.Ten) || x.Slug.Contains(itemSearch.Ten));
                }
                var result = await query.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Slug = x.Slug,
                    ParentName = x.Parent.Name
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
                var item = await db.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
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
                var item = await db.Categories.FindAsync(id).ConfigureAwait(false);
                if (item == null)
                {
                    httpMessage.IsOk = false;
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("400", null, "Không tìm thấy thông tin");
                    return Json(httpMessage);
                }
                db.Categories.Remove(item);
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
        public async Task<JsonResult> Create(CreateOrUpdateCategoryDto input)
        {
            HttpMessage httpMessage = new HttpMessage(true);
            try
            {
                httpMessage = await CheckValid(input);
                if (!httpMessage.IsOk)
                {
                    return Json(httpMessage);
                }
                Category item = new Category
                {
                    Id = input.Id,
                    Name = input.Name,
                    ParentId = input.ParentId,
                    Slug = StringConverter.ConvertToSlug(input.Name)
                };
                await db.Categories.AddAsync(item);
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
        public async Task<JsonResult> Update(CreateOrUpdateCategoryDto input)
        {
            HttpMessage httpMessage = new HttpMessage(true);
            try
            {
                httpMessage = await CheckValid(input);
                if (!httpMessage.IsOk)
                {
                    return Json(httpMessage);
                }

                var exist = await db.Categories.FindAsync(input.Id).ConfigureAwait(false);
                if (exist == null)
                {
                    httpMessage.IsOk = false;
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("400", null, "Không tìm thấy thông tin");
                    return Json(httpMessage);
                }

                exist.Name = input.Name;
                exist.Slug = input.Slug;
                exist.ParentId = input.ParentId;
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

        private async Task<HttpMessage> CheckValid(CreateOrUpdateCategoryDto item)
        {
            HttpMessage httpMessage = new HttpMessage(false);
            try
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("400", null, "Vui lòng nhập tên danh mục");
                    return httpMessage;
                }

                var exist = await db.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id != item.Id && x.Name == item.Name).ConfigureAwait(false);
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
