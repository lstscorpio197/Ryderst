using System.ComponentModel.DataAnnotations.Schema;

namespace ShopAdmin.Models
{
    [Table("SCoupons")]
    public class SCoupon
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? Ratio { get; set; }
        public decimal? MaximumValue { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int? TotalQuantity { get; set; }
        public int? Remaining { get; set; }
        public decimal? MinPrice { get; set; }
    }
}
