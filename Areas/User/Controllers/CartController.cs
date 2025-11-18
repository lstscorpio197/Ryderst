using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAdmin.Common;
using ShopAdmin.Dto.Carts;
using ShopAdmin.Dto.Products;
using ShopAdmin.Helper;
using ShopAdmin.Models;
using ShopAdmin.WHttpMessage;

namespace ShopAdmin.Areas.User.Controllers
{
    [Area("User")]
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetCart()
        {
            HttpMessage httpMessage = new HttpMessage(true);
            List<CartItemDto> cartItems = HttpContext.Session.GetObject<List<CartItemDto>>(AppConst.CartSession) ?? new List<CartItemDto>();
            httpMessage.Body.Data = cartItems;
            httpMessage.Body.Description = cartItems.Sum(x => x.Quantity * x.Price).ToStringNumber();
            return Json(httpMessage);
        }

        public async Task<JsonResult> AddToCard(AddToCartInputDto input)
        {
            using (var db = new ShopDbContext())
            {
                HttpMessage httpMessage = new HttpMessage(true);

                var productVariants = await db.ProductVariants.Include(x => x.VariantValues).AsNoTracking().Where(x => x.ProductId == input.ProductId).ToListAsync().ConfigureAwait(false);
                var item = productVariants.FirstOrDefault(x => string.Join(',', x.VariantValues.Select(y => y.ProductAttributeValueId).OrderBy(y => y)) == string.Join(',', input.ProductAttributeValueIds.OrderBy(y => y))) ?? new ProductVariant();
                var productImg = await db.ProductImages.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == input.ProductId).ConfigureAwait(false);
                var itemResult = new CartItemDto { ProductVariantId = item.Id, ProductName = item.Sku, Price = item.Price, Quantity = input.Quantity, ImageUrl = productImg?.ImageUrl, ProductId = input.ProductId };


                List<CartItemDto> cartItems = HttpContext.Session.GetObject<List<CartItemDto>>(AppConst.CartSession) ?? new List<CartItemDto>();
                int index = cartItems.FindIndex(x => x.ProductVariantId == itemResult.ProductVariantId);
                if (index < 0)
                    cartItems.Add(itemResult);
                else
                    cartItems[index].Quantity += itemResult.Quantity;

                HttpContext.Session.SetObject(AppConst.CartSession, cartItems);
                httpMessage.Body.Data = cartItems;
                httpMessage.Body.Description = cartItems.Sum(x => x.Quantity * x.Price).ToStringNumber();
                return Json(httpMessage);
            }
        }

        public async Task<JsonResult> RemoveItem(int productVariantId)
        {
            HttpMessage httpMessage = new HttpMessage(true);

            List<CartItemDto> cartItems = HttpContext.Session.GetObject<List<CartItemDto>>(AppConst.CartSession) ?? new List<CartItemDto>();

            cartItems = cartItems.Where(x => x.ProductVariantId != productVariantId).ToList();

            HttpContext.Session.SetObject(AppConst.CartSession, cartItems);
            httpMessage.Body.Data = cartItems;
            httpMessage.Body.Description = cartItems.Sum(x => x.Quantity * x.Price).ToStringNumber();
            httpMessage.Body.Description2 = (cartItems.Sum(x => x.Quantity * x.Price) + AppConst.ShippingFee).ToStringNumber();
            return Json(httpMessage);
        }

        public async Task<JsonResult> ChangeQuantityItem(int productVariantId, int quantity)
        {
            HttpMessage httpMessage = new HttpMessage(true);

            List<CartItemDto> cartItems = HttpContext.Session.GetObject<List<CartItemDto>>(AppConst.CartSession) ?? new List<CartItemDto>();

            int index = cartItems.FindIndex(x => x.ProductVariantId == productVariantId);
            if (index < 0 || quantity == 0)
                cartItems = cartItems.Where(x => x.ProductVariantId != productVariantId).ToList();
            else
                cartItems[index].Quantity = quantity;

            HttpContext.Session.SetObject(AppConst.CartSession, cartItems);
            httpMessage.Body.Data = cartItems;
            httpMessage.Body.Description = cartItems.Sum(x => x.Quantity * x.Price).ToStringNumber();
            httpMessage.Body.Description2 = (cartItems.Sum(x => x.Quantity * x.Price) + AppConst.ShippingFee).ToStringNumber();
            return Json(httpMessage);
        }
    }
}
