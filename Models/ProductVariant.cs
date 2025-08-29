using System.ComponentModel.DataAnnotations.Schema;

namespace ShopAdmin.Models
{
    [Table("ProductVariants")]
    public class ProductVariant
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Sku { get; set; } // SKU duy nhất cho biến thể
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Stock { get; set; }

        public Product Product { get; set; }
        public ICollection<ProductVariantValue> VariantValues { get; set; }
    }
}
