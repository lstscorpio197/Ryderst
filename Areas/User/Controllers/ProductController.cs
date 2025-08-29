using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAdmin.Dto.Products;
using ShopAdmin.Models;
using ShopAdmin.WHttpMessage;

namespace ShopAdmin.Areas.User.Controllers
{
    [Area("User")]
    public class ProductController : Controller
    {
        public async Task<IActionResult> Index(string name, int id)
        {
            return View(await GetItem(id));
        }

        public async Task<JsonResult> GetRelatedItems(int id, int cateId)
        {
            using (var db = new ShopDbContext())
            {
                HttpMessage httpMessage = new HttpMessage(true);

                var products = await db.Products.Include(x => x.Images).AsNoTracking().Where(x => x.CategoryId == cateId && x.Id != id).Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    ImageUrl = x.Images.Select(x => x.ImageUrl).FirstOrDefault()
                }).OrderBy(p => Guid.NewGuid()).Skip(0).Take(6).ToListAsync().ConfigureAwait(false);
                httpMessage.Body.Data = products;
                return Json(httpMessage);
            }
        }
        private async Task<ProductDto> GetItem(int id)
        {
            using (var db = new ShopDbContext())
            {
                var product = await db.Products.Include(x => x.Images).Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);

                ProductDto result = new()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category.Name,
                    ImageUrl = product.Images.Select(x => x.ImageUrl).ToList()
                };

                result.Attributes = await db.ProductAttributes.AsNoTracking().Where(x => x.ProductId == product.Id).Select(x => new AttributeDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ProductAttrs = x.Values.Select(y => new ProductAttrDto { Id = y.Id, Value = y.Value }).ToList()
                }).ToListAsync().ConfigureAwait(false);

                return result;
            }
        }
    }
}
