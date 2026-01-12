namespace ShopAdmin.Dto.PosAPI
{
    public class PancakeProductVariantResponse
    {
        public string Display_id { get; set; }
        public int Remain_quantity { get; set; }
        public int Retail_price { get; set; }
        public int Retail_price_after_discount { get; set; }
        public string Id { get; set; }
        public string Product_id { get; set; }
        public List<string> Images { get; set; }

        public PancakeProductResponse Product { get; set; }
    }

    public class PancakeProductResponse
    {
        public string Display_id { get; set; }
        public string Name { get; set; }
        public string Note_product { get; set; }
        public List<PancakeCategory> Categories { get; set; }
        public List<PancakeAttribute> Product_attributes { get; set; }

    }

    public class PancakeCategory
    {
        public string Name { get; set; }
    }
    public class PancakeAttribute
    {
        public string Name { get; set; }
        public List<string> Values { get; set; }
    }

    public class PancakeProduct
    {
        public string Display_id { get; set; }
        public string Name { get; set; }
        public string Note_product { get; set; }
        public string Product_id { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Images { get; set; }
        public List<PancakeAttribute> Product_attributes { get; set; }
        public List<PancakeProductVariantResponse> Product_variant { get; set; }
    }
}
