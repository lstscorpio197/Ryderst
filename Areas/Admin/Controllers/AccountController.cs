

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAdmin.Common;
using ShopAdmin.Dto;
using ShopAdmin.Dto.User;
using ShopAdmin.Helper;
using ShopAdmin.Models;
using ShopAdmin.WHttpMessage;

namespace ShopAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : BaseController
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult ChangePassword(ChangePasswordDto itemPass)
        {
            HttpMessage httpMessage = new HttpMessage(false);
            try
            {
                var item = db.SUsers.Find(us.Id);
                if (item == null)
                {
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("400", null, "Không tìm thấy thông tin user");
                    return Json(httpMessage);
                }
                bool validPass = BCrypt.Net.BCrypt.Verify(itemPass.OldPassword, item.Password);
                if (!validPass)
                {
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("400", null, "Mật khẩu cũ không đúng");
                    return Json(httpMessage);
                }
                if (itemPass.NewPassword != itemPass.ConfirmPassword)
                {
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("400", null, "Mật khẩu cũ không đúng");
                    return Json(httpMessage);
                }
                item.Password = BCrypt.Net.BCrypt.HashPassword(itemPass.NewPassword);
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                httpMessage.IsOk = true;
                httpMessage.Body.MsgNoti = new HttpMessageNoti("200", null, "Thay đổi mật khẩu thành công");
                return Json(httpMessage);
            }
            catch (Exception ex)
            {
                httpMessage.IsOk = false;
                httpMessage.Body.MsgNoti = new HttpMessageNoti("500", null, ex.Message);
                return Json(httpMessage);
            }
        }

        public JsonResult UpdateUserInfo(CreateOrUpdateUserDto itemUpdate)
        {
            HttpMessage httpMessage = new HttpMessage(true);
            try
            {
                var item = db.SUsers.Find(us.Id);
                if (item == null)
                {
                    httpMessage.IsOk = false;
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("400", null, "Không tìm thấy thông tin user");
                    return Json(httpMessage);
                }
                item.HoTen = itemUpdate.HoTen;
                item.NgaySinh = itemUpdate.NgaySinh;
                item.Email = itemUpdate.Email;
                item.SDT = itemUpdate.SDT;

                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                USER user = new USER(item);
                HttpContext.Session.SetObject(AppConst.UserSession, user);
                return Json(httpMessage);
            }
            catch (Exception ex)
            {
                httpMessage.IsOk = false;
                httpMessage.Body.MsgNoti = new HttpMessageNoti("500", null, ex.Message);
                return Json(httpMessage);
            }
        }

        public JsonResult GetUserCurrent()
        {
            HttpMessage httpMessage = new HttpMessage(true);
            try
            {
                var item = db.SUsers.Find(us.Id);
                if (item == null)
                {
                    httpMessage.IsOk = false;
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("400", null, "Không tìm thấy thông tin user");
                    return Json(httpMessage);
                }
                httpMessage.Body.Data = new { item.Username, item.HoTen, item.NgaySinh, item.Email, item.SDT };
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