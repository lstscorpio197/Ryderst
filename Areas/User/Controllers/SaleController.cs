using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAdmin.Dto.Products;
using ShopAdmin.Models;

namespace ShopAdmin.Areas.User.Controllers
{
    [Area("User")]
    public class SaleController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await GetAllProduct());
        }

        private async Task<List<ProductDto>> GetAllProduct()
        {
            using (var db = new ShopDbContext())
            {
                var productsQuery = db.Products.Include(x => x.Images).AsNoTracking().Where(x => x.PriceDiscount.HasValue && x.PriceDiscount > 0).Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    PriceDiscount = x.PriceDiscount,
                    ImageUrl = new List<string> { x.Images.Select(x => x.ImageUrl).FirstOrDefault() }
                });


                return await productsQuery.ToListAsync().ConfigureAwait(false);
            }
        }
    }
}
