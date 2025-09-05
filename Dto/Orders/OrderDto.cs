using ShopAdmin.Common;

namespace ShopAdmin.Dto.Orders
{
    public class OrderDto
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public decimal TotalAmout { get; set; }
        public string TotalAmoutTxt => TotalAmout.ToStringNumber() + " đ";
        public int Status { get; set; }
        public int? PaymentStatus { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedTimeTxt => CreatedTime.ToString("HH:mm:ss dd/MM/yyyy");
    }
}
