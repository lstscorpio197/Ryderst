using Microsoft.AspNetCore.Mvc;

namespace ShopAdmin.Areas.User.Controllers
{
    [Area("User")]
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ChinhSachBaoMat()
        {
            return View("ChinhSachBaoMat");
        }
        public IActionResult ChinhSachVanChuyen()
        {
            return View("ChinhSachVanChuyen");
        }

        public IActionResult HinhThucThanhToan()
        {
            return View("HinhThucThanhToan");
        }

        public IActionResult ChinhSachDoiTra()
        {
            return View("ChinhSachDoiTra");
        }
    }
}
