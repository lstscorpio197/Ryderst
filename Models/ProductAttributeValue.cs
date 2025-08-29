using System.ComponentModel.DataAnnotations.Schema;

namespace ShopAdmin.Models
{
    [Table("ProductAttributeValues")]
    public class ProductAttributeValue
    {
        public int Id { get; set; }
        public int ProductAttributeId { get; set; }
        public string Value { get; set; } // Ví dụ: S, M, L hoặc Đen, Trắng

        public ProductAttribute ProductAttribute { get; set; }
        public ICollection<ProductVariantValue> VariantValues { get; set; }
    }
}
