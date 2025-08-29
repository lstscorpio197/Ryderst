using ShopAdmin.Dto.Products;

namespace ShopAdmin.Dto.Categories
{
    public class CategoryProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductDto> Products { get; set; }
    }
}
