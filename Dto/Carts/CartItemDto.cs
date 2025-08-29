﻿using ShopAdmin.Common;

namespace ShopAdmin.Dto.Carts
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
        public string ImageUrl { get; set; }
        public string PriceTxt => Price.ToStringNumber();
    }
}
