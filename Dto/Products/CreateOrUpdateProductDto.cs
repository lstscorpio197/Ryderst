using ShopAdmin.Models;

namespace ShopAdmin.Dto.Products
{
    public class CreateOrUpdateProductDto
    {
        public Product Product { get; set; }
        public List<ProductAttributeValues> ListAttribute { get; set; }

    }

    public class ProductAttributeValues
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public List<string> Values => string.IsNullOrEmpty(Value) ? new List<string>() : Value.Replace(",", ";").Split(';').ToList();
    }
}
