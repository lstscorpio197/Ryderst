using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAdmin.Common;
using ShopAdmin.Dto.Carts;
using ShopAdmin.Dto.Orders;
using ShopAdmin.Enums;
using ShopAdmin.Helper;
using ShopAdmin.Models;
using ShopAdmin.WHttpMessage;

namespace ShopAdmin.Areas.User.Controllers
{
    [Area("User")]
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            List<CartItemDto> cartItems = HttpContext.Session.GetObject<List<CartItemDto>>(AppConst.CartSession) ?? new List<CartItemDto>();
            return View(cartItems);
        }

        public async Task<JsonResult> ApplyCoupon(string couponCode)
        {
            using (var db = new ShopDbContext())
            {
                HttpMessage httpMessage = new HttpMessage(false);

                List<CartItemDto> cartItems = HttpContext.Session.GetObject<List<CartItemDto>>(AppConst.CartSession) ?? new List<CartItemDto>();
                var totalPrice = cartItems.Sum(x => x.Price * x.Quantity) ?? 0;
                if (totalPrice == 0)
                {
                    httpMessage.Body.Description = "Vui lòng chọn sản phẩm vào giỏ hàng trước khi thanh toán";
                    return Json(httpMessage);
                }

                var coupon = await db.SCoupons.AsNoTracking().FirstOrDefaultAsync(x => x.Code == couponCode);
                if (coupon == null || coupon.StartDate > DateTime.Today || coupon.EndDate < DateTime.Today || (coupon.Remaining.HasValue && coupon.Remaining < 1))
                {
                    httpMessage.Body.Description = "Mã giảm giá không tồn tại hoặc đã hết hạn. Vui lòng kiểm tra lại";
                    return Json(httpMessage);
                }


                if (totalPrice < coupon.MinPrice)
                {
                    httpMessage.Body.Description = "Đơn hàng của bạn chưa đạt giá trị tối thiểu để áp dụng mã giảm giá này";
                    return Json(httpMessage);
                }

                decimal shippingFee = 30000;

                if (coupon.Ratio.HasValue && coupon.Ratio > 0)
                {
                    var discoutValue = totalPrice * coupon.Ratio / 100;

                    if (coupon.MaximumValue.HasValue && discoutValue > coupon.MaximumValue)
                        discoutValue = coupon.MaximumValue;

                    httpMessage.IsOk = true;
                    httpMessage.Body.Data = new { DiscoutValue = discoutValue.ToStringNumber(), TotalValue = (totalPrice - discoutValue + shippingFee).ToStringNumber() };
                    return Json(httpMessage);
                }

                if (coupon.DiscountPrice.HasValue)
                {
                    httpMessage.IsOk = true;
                    httpMessage.Body.Data = new { DiscoutValue = coupon.DiscountPrice.ToStringNumber(), TotalValue = (totalPrice - coupon.DiscountPrice + shippingFee).ToStringNumber() };
                    return Json(httpMessage);
                }

                httpMessage.IsOk = true;
                httpMessage.Body.Data = new { DiscoutValue = "0", TotalValue = (totalPrice + shippingFee).ToStringNumber() };
                return Json(httpMessage);
            }
        }

        public async Task<JsonResult> Checkout(OrderInputDto input)
        {
            using (var db = new ShopDbContext())
            {
                using (var trans = await db.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    HttpMessage httpMessage = new HttpMessage(false);
                    try
                    {

                        List<CartItemDto> cartItems = HttpContext.Session.GetObject<List<CartItemDto>>(AppConst.CartSession) ?? new List<CartItemDto>();
                        var totalPrice = cartItems.Sum(x => x.Price * x.Quantity) ?? 0;

                        if (totalPrice == 0)
                        {
                            httpMessage.Body.Description = "Vui lòng chọn sản phẩm vào giỏ hàng trước khi thanh toán";
                            return Json(httpMessage);
                        }

                        decimal shippingFee = 30000;

                        var order = new Order
                        {
                            FullName = input.FullName,
                            Address = input.Address,
                            Phone = input.Phone,
                            Note = input.Note,
                            CouponCode = input.CouponCode,
                            CreatedTime = DateTime.Now,
                            Status = (int)OrderStatus.DaDatHang,
                            PaymentStatus = (int)PaymentStatus.ChuaThanhToan,
                            PaymentType = (int)PaymentType.COD,
                            ShippingFee = shippingFee
                        };

                        order.CouponDiscount = await GetCouponDiscount(input.CouponCode, totalPrice, db);
                        order.TotalDiscount = order.CouponDiscount ?? 0 + order.LoyaltyDiscount ?? 0;

                        order.TotalAmout = totalPrice - order.TotalDiscount ?? 0 + shippingFee;

                        order.Items = cartItems.Select(x => new OrderItem
                        {
                            Price = x.Price ?? 0,
                            Quantity = x.Quantity,
                            ProductId = x.ProductId,
                            ProductName = x.ProductName,
                            ProductVariantId = x.ProductVariantId,
                        }).ToList();

                        await db.Orders.AddAsync(order).ConfigureAwait(false);
                        await db.SaveChangesAsync().ConfigureAwait(false);

                        await UpdateQuantityProduct(db, cartItems);
                        await trans.CommitAsync().ConfigureAwait(false);

                        HttpContext.Session.SetObject(AppConst.CartSession, new List<CartItemDto>());
                        httpMessage.IsOk = true;
                        httpMessage.Body.Description = "Thanh toán thành công. Chúng tôi sẽ gọi điện cho bạn sớm để xác nhận đơn hàng";
                        return Json(httpMessage);
                    }
                    catch (Exception ex)
                    {
                        await trans.RollbackAsync().ConfigureAwait(false);
                        httpMessage.IsOk = false;
                        httpMessage.Body.Description = "Đã xảy ra lỗi trong quá trình thanh toán. Vui lòng thử lại sau";
                        return Json(httpMessage);
                    }
                }
            }
        }

        private async Task<decimal?> GetCouponDiscount(string couponCode, decimal totalPrice, ShopDbContext db)
        {
            var coupon = await db.SCoupons.FirstOrDefaultAsync(x => x.Code == couponCode).ConfigureAwait(false);
            if (coupon == null || coupon.StartDate > DateTime.Today || coupon.EndDate < DateTime.Today || (coupon.Remaining.HasValue && coupon.Remaining < 1))
                return null;

            if (totalPrice < coupon.MinPrice)
            {
                return null;
            }

            if (coupon.Ratio.HasValue && coupon.Ratio > 0)
            {
                var discoutValue = totalPrice * coupon.Ratio / 100;

                if (coupon.MaximumValue.HasValue && discoutValue > coupon.MaximumValue)
                    discoutValue = coupon.MaximumValue;

                if (coupon.Remaining.HasValue)
                {
                    coupon.Remaining--;
                    db.Entry(coupon).State = EntityState.Modified;
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }

                return discoutValue;
            }

            if (coupon.DiscountPrice.HasValue)
            {
                if (coupon.Remaining.HasValue)
                {
                    coupon.Remaining--;
                    db.Entry(coupon).State = EntityState.Modified;
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
                return coupon.DiscountPrice;
            }
            return null;
        }

        private async Task UpdateQuantityProduct(ShopDbContext db, List<CartItemDto> items)
        {
            var productIds = items.Select(x => x.ProductId).Distinct().ToList();
            var products_quantity = items.GroupBy(x => x.ProductId).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));
            var products = await db.Products.Where(x => productIds.Contains(x.Id)).ToListAsync().ConfigureAwait(false);
            products.ForEach(x =>
            {
                if (products_quantity.TryGetValue(x.Id, out int quantity))
                    x.Quantity = x.Quantity - quantity;
                //x.Sales = x.Sales + products_quantity[x.Id];
            });

            var variantIds = items.Select(x => x.ProductVariantId).Distinct().ToList();
            var variant_quantity = items.GroupBy(x => x.ProductVariantId).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));
            var variants = await db.ProductVariants.Where(x => variantIds.Contains(x.Id)).ToListAsync().ConfigureAwait(false);
            variants.ForEach(x =>
            {
                if (products_quantity.TryGetValue(x.Id, out int quantity))
                    x.Quantity = x.Quantity - quantity;
            });

            await db.SaveChangesAsync();
        }
    }
}
