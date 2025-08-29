using Microsoft.AspNetCore.Mvc;

namespace ShopAdmin.Areas.User.Controllers
{
    [Area("User")]
    public class AlbumController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
