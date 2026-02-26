using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAdmin.Common;
using ShopAdmin.Helper;
using ShopAdmin.Models;
using ShopAdmin.WHttpMessage;

namespace ShopAdmin.Areas.User.Controllers
{
    [Area("User")]
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            var account = HttpContext.Session.GetObject<CustomerSession>(AppConst.CustomerSession);
            if (account == null)
            {
                return Redirect("/signin");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signin(string username, string password)
        {
            using (var db = new ShopDbContext())
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return View();
                }

                var item = await db.Customers.FirstOrDefaultAsync(x => x.Phone == username || x.Email == username).ConfigureAwait(false);
                if (item == null)
                {
                    ViewBag.Message = "Tên đăng nhập hoặc mật khẩu không chính xác";
                    return View();
                }

                bool isVerify = BCrypt.Net.BCrypt.Verify(password, item.Password);
                if (!isVerify)
                {
                    ViewBag.Message = "Tài khoản hoặc mật khẩu không chính xác";
                    return View();
                }
                if (item.IsActive != 1)
                {
                    ViewBag.Message = "Tài khoản này đã bị khóa";
                    return View();
                }
                CustomerSession user = new CustomerSession(item);
                HttpContext.Session.SetObject(AppConst.CustomerSession, user);
                return Redirect("/");
            }
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Signup(string name, string password, string email, string phone, string address)
        {
            HttpMessage httpMessage = new HttpMessage(false);
            using (var db = new ShopDbContext())
            {
                try
                {
                    httpMessage = await CheckValid(0, name, password, email, phone, address, db);
                    if (!httpMessage.IsOk)
                    {
                        return Json(httpMessage);
                    }

                    Customer cus = new()
                    {
                        Password = BCrypt.Net.BCrypt.HashPassword(password),
                        Name = name,
                        Email = email,
                        Phone = phone,
                        Address = address,
                        IsActive = 1,
                        CreatedDate = DateTime.Now
                    };
                    await db.Customers.AddAsync(cus);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                    httpMessage.Body.Description = "Đăng ký mới thành công";
                    return Json(httpMessage);
                }
                catch (Exception ex)
                {
                    httpMessage.IsOk = false;
                    httpMessage.Body.Description = "Đã xảy ra lỗi";
                    return Json(httpMessage);
                }
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateInfo(string name, string password, string email, string phone, string address, string birthday, string sex)
        {
            HttpMessage httpMessage = new HttpMessage(false);
            using (var db = new ShopDbContext())
            {
                try
                {
                    var session = HttpContext.Session.GetObject<CustomerSession>(AppConst.CustomerSession);
                    if (session == null)
                    {
                        httpMessage.Body.Description = "Vui lòng đăng nhập lại";
                        return Json(httpMessage);
                    }

                    httpMessage = await CheckValid(session.Id, name, password, email, phone, address, db);
                    if (!httpMessage.IsOk)
                    {
                        return Json(httpMessage);
                    }

                    var exist = await db.Customers.FirstOrDefaultAsync(x => x.Id == session.Id).ConfigureAwait(false);
                    if (exist == null)
                    {
                        httpMessage.Body.Description = "Tài khoản không tồn tại";
                        return Json(httpMessage);
                    }
                    exist.Name = name;
                    exist.Email = email;
                    exist.Phone = phone;
                    exist.Address = address;
                    exist.Sex = sex;
                    db.Entry(exist).State = EntityState.Modified;
                    await db.SaveChangesAsync().ConfigureAwait(false);

                    HttpContext.Session.Remove(AppConst.CustomerSession);
                    CustomerSession user = new CustomerSession(exist);
                    HttpContext.Session.SetObject(AppConst.CustomerSession, user);

                    httpMessage.Body.Description = "Cập nhật thông tin cá nhân thành công";
                    return Json(httpMessage);
                }
                catch (Exception ex)
                {
                    httpMessage.IsOk = false;
                    httpMessage.Body.Description = "Đã xảy ra lỗi";
                    return Json(httpMessage);
                }
            }
        }

        public ActionResult Signout()
        {
            HttpContext.Session.Remove(AppConst.CustomerSession);
            return Redirect("/");
        }

        private async Task<HttpMessage> CheckValid(int id, string name, string password, string email, string phone, string address, ShopDbContext db)
        {
            HttpMessage httpMessage = new HttpMessage(false);
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    httpMessage.Body.Description = "Vui lòng nhập tên";
                    return httpMessage;
                }
                if (string.IsNullOrEmpty(password) && id == 0)
                {
                    httpMessage.Body.Description = "Vui lòng nhập password";
                    return httpMessage;
                }
                if (string.IsNullOrEmpty(email))
                {
                    httpMessage.Body.Description = "Vui lòng nhập email";
                    return httpMessage;
                }
                if (string.IsNullOrEmpty(phone))
                {
                    httpMessage.Body.Description = "Vui lòng nhập số điện thoại";
                    return httpMessage;
                }
                if (string.IsNullOrEmpty(address))
                {
                    httpMessage.Body.Description = "Vui lòng nhập địa chỉ";
                    return httpMessage;
                }
                var exist = await db.Customers.AsNoTracking().AnyAsync(x => x.Id != id && (x.Phone == phone || x.Email == email)).ConfigureAwait(false);
                if (exist)
                {
                    httpMessage.Body.Description = "Số điện thoại hoặc email này đã tồn tại";
                    return httpMessage;
                }
                httpMessage.IsOk = true;
                return httpMessage;
            }
            catch (Exception ex)
            {
                httpMessage.IsOk = false;
                httpMessage.Body.Description = "Đã xảy ra lỗi";
                return httpMessage;
            }

        }
    }
}
