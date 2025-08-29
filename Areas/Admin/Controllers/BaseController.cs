using Microsoft.AspNetCore.Mvc;
using ShopAdmin.Authorize;
using ShopAdmin.Common;
using ShopAdmin.Helper;
using ShopAdmin.Models;

namespace ShopAdmin.Areas.Admin.Controllers
{
    [AuthorizeAccessRole]
    public class BaseController : Controller
    {
        // GET: Base
        public ShopDbContext db = new ShopDbContext();
        public USER us => HttpContext.Session.GetObject<USER>(AppConst.UserSession);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Download(string fileGuid, string fileName)
        {
            //var dataCache = System.Web.HttpContext.Current.Cache.Get(fileGuid);
            //if (dataCache != null)
            //{
            //    byte[] data = dataCache as byte[];
            //    System.Web.HttpContext.Current.Cache.Remove(fileGuid);

            //    return File(data, "application/octet-stream", fileName);
            //}
            //else
            //{
            return new EmptyResult();
            //}
        }
    }
}