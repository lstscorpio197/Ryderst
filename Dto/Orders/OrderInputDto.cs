namespace ShopAdmin.Dto.Orders
{
    public class OrderInputDto
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }

        public string CouponCode { get; set; }
    }
}
