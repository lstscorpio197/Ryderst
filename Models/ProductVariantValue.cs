using System.ComponentModel.DataAnnotations.Schema;

namespace ShopAdmin.Models
{
    [Table("ProductVariantValues")]
    public class ProductVariantValue
    {
        public int Id { get; set; }

        public int ProductVariantId { get; set; }
        public int ProductAttributeValueId { get; set; }

        public ProductVariant ProductVariant { get; set; }
        public ProductAttributeValue ProductAttributeValue { get; set; }
    }
}
