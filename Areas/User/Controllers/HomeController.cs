using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAdmin.Dto.Products;
using ShopAdmin.Models;

namespace ShopAdmin.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.ListSPNew = await GetListSPNew();
            ViewBag.ListSPBanChay = await GetListSPBanChay();
            return View();
        }

        private async Task<List<ProductDto>> GetListSPNew()
        {
            using (var db = new ShopDbContext())
            {
                var products = await db.Products.Include(x => x.Images).AsNoTracking().Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    PriceDiscount = x.PriceDiscount,
                    ImageUrl = new List<string> { x.Images.Select(x => x.ImageUrl).FirstOrDefault() }
                }).OrderByDescending(p => p.Id).Skip(0).Take(12).ToListAsync().ConfigureAwait(false);
                return products;
            }
        }

        private async Task<List<ProductDto>> GetListSPBanChay()
        {
            using (var db = new ShopDbContext())
            {
                var products = await db.Products.Include(x => x.Images).AsNoTracking().OrderByDescending(p => p.Sales).Skip(0).Take(12).Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    PriceDiscount = x.PriceDiscount,
                    ImageUrl = new List<string> { x.Images.Select(x => x.ImageUrl).FirstOrDefault() }
                }).ToListAsync().ConfigureAwait(false);
                return products;
            }
        }
    }
}
