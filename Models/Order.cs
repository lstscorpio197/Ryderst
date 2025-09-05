using System.ComponentModel.DataAnnotations.Schema;

namespace ShopAdmin.Models
{
    [Table("Orders")]
    public class Order
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string CouponCode { get; set; }
        public decimal? CouponDiscount { get; set; }
        public int? LoyaltyPoint { get; set; }
        public decimal? LoyaltyDiscount { get; set; }
        public decimal? TotalDiscount { get; set; }
        public decimal TotalAmout { get; set; }
        public DateTime CreatedTime { get; set; }
        public int Status { get; set; }
        public int? PaymentStatus { get; set; }
        public int PaymentType { get; set; }
        public decimal ShippingFee { get; set; }
        public ICollection<OrderItem> Items { get; set; }

        public DateTime? LastModifiedTime { get; set; }
        public int? LastModifiedUser { get; set; }
    }
}
