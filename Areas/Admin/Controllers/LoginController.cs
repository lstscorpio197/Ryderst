using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAdmin.Common;
using ShopAdmin.Helper;
using ShopAdmin.Models;

namespace ShopAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            USER us = HttpContext.Session.GetObject<USER>(AppConst.UserSession);
            if (us == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult Index(string username, string password)
        {
            using (var db = new ShopDbContext())
            {
                try
                {
                    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    {
                        return View();
                    }

                    var us = db.SUsers.AsNoTracking().FirstOrDefault(x => x.Username == username);
                    if (us == null)
                    {
                        ViewBag.Error = "Tài khoản không tồn tại. Vui lòng kiểm tra lại";
                        return View();
                    }
                    bool isVerify = BCrypt.Net.BCrypt.Verify(password, us.Password);
                    if (!isVerify)
                    {
                        ViewBag.Error = "Tài khoản hoặc mật khẩu không chính xác";
                        return View();
                    }
                    if (us.IsActived != 1)
                    {
                        ViewBag.Error = "Tài khoản này đã bị khóa";
                        return View();
                    }
                    USER user = new USER(us);
                    HttpContext.Session.SetObject(AppConst.UserSession, user);
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                    return View();
                }
            }
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Remove(AppConst.UserSession);
            return View("Index");
        }
    }
}