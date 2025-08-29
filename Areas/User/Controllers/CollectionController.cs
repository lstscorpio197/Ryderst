using Microsoft.AspNetCore.Mvc;

namespace ShopAdmin.Areas.User.Controllers
{
    [Area("User")]
    public class CollectionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
