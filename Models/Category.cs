using System.ComponentModel.DataAnnotations.Schema;

namespace ShopAdmin.Models
{
    [Table("Categories")]
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }

        public int? ParentId { get; set; }

        public Category Parent { get; set; }

        public ICollection<Category> ChildCategories { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
