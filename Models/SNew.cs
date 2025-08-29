using System.ComponentModel.DataAnnotations.Schema;

namespace ShopAdmin.Models
{
    [Table("SNews")]
    public class SNew
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ContentNews { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
