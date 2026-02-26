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
            ViewBag.ListSPHot = await GetListSPHot();
            return View();
        }

        private async Task<List<ProductDto>> GetListSPNew()
        {
            using (var db = new ShopDbContext())
            {
                var products = await db.Products.Include(x => x.Images).Include(x => x.Variants).AsNoTracking().Where(x => x.Visible != false && x.IsNew == true).Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    PriceDiscount = x.PriceDiscount,
                    ImageUrl = x.Images.Select(x => x.ImageUrl).ToList(),
                    CountInStock = x.Variants.Sum(v => v.Quantity)
                }).OrderByDescending(p => p.Id).Skip(0).Take(10).ToListAsync().ConfigureAwait(false);
                return products;
            }
        }

        private async Task<List<ProductDto>> GetListSPBanChay()
        {
            using (var db = new ShopDbContext())
            {
                var products = await db.Products.Include(x => x.Images).Include(x => x.Variants).AsNoTracking().Where(x => x.Visible != false && x.IsBestSeller == true).OrderByDescending(x => x.Id).Skip(0).Take(4).Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    PriceDiscount = x.PriceDiscount,
                    ImageUrl = x.Images.Select(x => x.ImageUrl).ToList(),
                    CountInStock = x.Variants.Sum(v => v.Quantity)
                }).ToListAsync().ConfigureAwait(false);
                return products;
            }
        }
        private async Task<List<ProductDto>> GetListSPHot()
        {
            using (var db = new ShopDbContext())
            {
                var products = await db.Products.Include(x => x.Images).Include(x => x.Variants).AsNoTracking().Where(x => x.Visible != false && x.IsHot == true).Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    PriceDiscount = x.PriceDiscount,
                    ImageUrl = x.Images.Select(x => x.ImageUrl).ToList(),
                    CountInStock = x.Variants.Sum(v => v.Quantity)
                }).ToListAsync().ConfigureAwait(false);
                return products;
            }
        }
    }
}
