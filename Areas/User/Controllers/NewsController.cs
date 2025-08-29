using Microsoft.AspNetCore.Mvc;

namespace ShopAdmin.Areas.User.Controllers
{
    [Area("User")]
    public class NewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
