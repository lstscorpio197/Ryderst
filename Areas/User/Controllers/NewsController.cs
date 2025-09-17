using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAdmin.Models;

namespace ShopAdmin.Areas.User.Controllers
{
    [Area("User")]
    public class NewsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await GetNews());
        }

        public async Task<IActionResult> ViewDetail(string title, int id)
        {
            using (var db = new ShopDbContext())
            {
                var item = await db.SNews.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false) ?? new SNew();
                return View(item);
            }

        }

        private async Task<List<SNew>> GetNews()
        {
            using (var db = new ShopDbContext())
            {
                return await db.SNews.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync().ConfigureAwait(false) ?? new List<SNew>();
            }
        }
    }
}
