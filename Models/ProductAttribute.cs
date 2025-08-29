using System.ComponentModel.DataAnnotations.Schema;

namespace ShopAdmin.Models
{
    [Table("ProductAttributes")]
    public class ProductAttribute
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; } // Ví dụ: Size, Color

        public Product Product { get; set; }
        public ICollection<ProductAttributeValue> Values { get; set; }
    }
}
