using System.ComponentModel.DataAnnotations.Schema;

namespace ShopAdmin.Models
{
    [Table("Customers")]
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime? Birthday { get; set; }
        public string Sex { get; set; }
        public int IsActive { get; set; }
        public int? RPoint { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
