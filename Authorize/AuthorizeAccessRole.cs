using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ShopAdmin.Common;
using ShopAdmin.Helper;
using ShopAdmin.Models;
using ShopAdmin.WHttpMessage;
using System.Security.Claims;

namespace ShopAdmin.Authorize
{
    public class AuthorizeAccessRoleAttribute : TypeFilterAttribute
    {
        public AuthorizeAccessRoleAttribute(int level = UserLevel.NhanVien, string typeHandle = "") : base(typeof(AuthorizeAccessRoleFilter))
        {
            Arguments = new object[] { new Claim(level.ToString(), typeHandle) };
        }
    }
    public class AuthorizeAccessRoleFilter : AuthorizeAttribute, IAuthorizationFilter
    {
        public int Level = UserLevel.NhanVien;
        public string TypeHandle = string.Empty;

        public AuthorizeAccessRoleFilter(Claim claim)
        {
            Level = int.Parse(claim.Type);
            TypeHandle = claim.Value;
        }


        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            USER us = filterContext.HttpContext.Session.GetObject<USER>(AppConst.UserSession);
            if (us == null)
            {
                filterContext.Result = new RedirectResult("/admin/dang-nhap");
                return;
            }
            using (var db = new ShopDbContext())
            {
                var user = db.SUsers.AsNoTracking().FirstOrDefault(x => x.Id == us.Id);
                if (user == null)
                {
                    filterContext.Result = new RedirectResult("/admin/dang-nhap");
                    return;
                }
            }
            if (us.ChucVu == UserLevel.Admin) return;
            if (this.Level > UserLevel.NhanVien || (this.Level != UserLevel.NhanVien && us.ChucVu > this.Level))
            {
                filterContext.Result = new RedirectResult("/admin/dang-nhap");
                return;
            }

            var routeValues = filterContext.ActionDescriptor.RouteValues;
            string controller = routeValues["controller"].ToString();
            if (controller == "Home" || string.IsNullOrEmpty(TypeHandle))
            {
                return;
            }
            if (!us.LstPermission.Where(x => x.Controller == controller && x.Action == TypeHandle).Any())
            {
                if (TypeHandle == "view")
                {
                    filterContext.Result = new RedirectResult("/admin/dang-nhap");
                    return;
                }
                HttpMessage httpMessage = new HttpMessage(false);
                httpMessage.Body.MsgNoti = new HttpMessageNoti("403", null, "Bạn không có quyền thực hiện chức năng này. Vui lòng liên hệ quản trị viên nếu muốn phân quyền.");
                filterContext.Result = new JsonResult(httpMessage);
                filterContext.HttpContext.Response.StatusCode = 200;
                return;
            }
            return;
        }

    }
}