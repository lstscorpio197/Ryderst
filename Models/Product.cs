using System.ComponentModel.DataAnnotations.Schema;

namespace ShopAdmin.Models
{
    [Table("Products")]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; } // dùng cho URL
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? PriceDiscount { get; set; }
        public string SKU { get; set; } // mã sản phẩm
        public int Quantity { get; set; }
        public int Stock { get; set; }

        public int CategoryId { get; set; }
        public int? CollectionId { get; set; }

        public int? Sales { get; set; }
        public Category Category { get; set; }
        public Collection Collection { get; set; }

        public ICollection<ProductImage> Images { get; set; }
        public ICollection<ProductAttribute> Attributes { get; set; }
        public ICollection<ProductVariant> Variants { get; set; }
    }
}
