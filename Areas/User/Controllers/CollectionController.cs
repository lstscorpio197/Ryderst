using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAdmin.Models;

namespace ShopAdmin.Areas.User.Controllers
{
    [Area("User")]
    public class CollectionController : Controller
    {
        public async Task<IActionResult> Index()
        {
            using (var db = new ShopDbContext())
            {
                var item = await db.Collections.AsNoTracking().ToListAsync().ConfigureAwait(false) ?? new List<Collection>();
                return View(item);
            }
        }

        public async Task<IActionResult> ViewDetail(string slug, int id)
        {
            using (var db = new ShopDbContext())
            {
                var item = await db.Collections
                    .Include(x => x.Products)
                    .ThenInclude(p => p.Images) // Include Images related to Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id && x.Slug == slug)
                    .ConfigureAwait(false) ?? new Collection();
                return View(item);
            }
        }
    }
}
