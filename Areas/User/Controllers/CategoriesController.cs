using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAdmin.Dto.Categories;
using ShopAdmin.Dto.Products;
using ShopAdmin.Models;

namespace ShopAdmin.Areas.User.Controllers
{
    [Area("User")]
    public class CategoriesController : Controller
    {
        public async Task<IActionResult> Index(string name, int id, int page = 1, string sort = "default")
        {
            return View(await GetCategoryProduct(id, page, sort));
        }

        private async Task<CategoryProductDto> GetCategoryProduct(int id, int pageNum = 1, string sort = "default")
        {
            using (var db = new ShopDbContext())
            {
                var cate = await db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id).ConfigureAwait(false) ?? new Category();
                var result = new CategoryProductDto
                {
                    Id = id,
                    Name = cate.Name
                };
                var productsQuery = db.Products.Include(x => x.Images).AsNoTracking().Where(x => x.CategoryId == id).Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    PriceDiscount = x.PriceDiscount,
                    ImageUrl = new List<string> { x.Images.Select(x => x.ImageUrl).FirstOrDefault() }
                });
                switch (sort)
                {
                    case "price-low":
                        productsQuery = productsQuery.OrderBy(x => x.Price);
                        break;
                    case "price-high":
                        productsQuery = productsQuery.OrderByDescending(x => x.Price);
                        break;
                    case "newest":
                        productsQuery = productsQuery.OrderByDescending(x => x.Id);
                        break;
                    default:
                        productsQuery = productsQuery.OrderByDescending(x => x.Id);
                        break;
                }

                //result.Products = await productsQuery.Skip((pageNum - 1) * 20).Take(20).ToListAsync().ConfigureAwait(false);
                result.Products = await productsQuery.ToListAsync().ConfigureAwait(false);

                return result;
            }
        }
    }
}
