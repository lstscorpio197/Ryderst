using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAdmin.Authorize;
using ShopAdmin.Dto.Orders;
using ShopAdmin.WHttpMessage;

namespace ShopAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAccessRole]
    public class OrderController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetTable(OrderFiterInput itemSearch)
        {
            HttpMessage httpMessage = new HttpMessage(true);
            try
            {
                var query = db.Orders.AsNoTracking().Where(x => true);
                if (!string.IsNullOrEmpty(itemSearch.Ten))
                {
                    query = query.Where(x => x.FullName.Contains(itemSearch.Ten) || x.Phone.Contains(itemSearch.Ten));
                }
                if (itemSearch.TuNgay.HasValue)
                {
                    DateTime tuNgay = itemSearch.TuNgay.Value.AddMilliseconds(-1);
                    query = query.Where(x => x.CreatedTime > tuNgay);
                }
                if (itemSearch.DenNgay.HasValue)
                {
                    DateTime denNgay = itemSearch.TuNgay.Value.AddDays(1);
                    query = query.Where(x => x.CreatedTime < denNgay);
                }
                if (itemSearch.TrangThai.HasValue)
                {
                    query = query.Where(x => x.Status == itemSearch.TrangThai);
                }
                if (itemSearch.TrangThaiThanhToan.HasValue)
                {
                    query = query.Where(x => x.PaymentStatus == itemSearch.TrangThaiThanhToan);
                }
                var result = await query.Select(x => new OrderDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Phone = x.Phone,
                    Address = x.Address,
                    TotalAmout = x.TotalAmout,
                    Status = x.Status,
                    PaymentStatus = x.PaymentStatus,
                    CreatedTime = x.CreatedTime,
                }).OrderByDescending(x => x.Id).Skip(itemSearch.Skip).Take(itemSearch.PageSize).ToListAsync().ConfigureAwait(false);
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
                var item = await db.Orders.Include(x => x.Items).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
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
    }
}
