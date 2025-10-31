namespace ShopAdmin.Dto.Products
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceDiscount { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<string> ImageUrl { get; set; }
        public List<string> Tags { get; set; }

        public List<AttributeDto> Attributes { get; set; }
    }


    public class AttributeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<ProductAttrDto> ProductAttrs { get; set; }
    }

    public class ProductAttrDto
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
