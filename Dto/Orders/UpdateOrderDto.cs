namespace ShopAdmin.Dto.Orders
{
    public class UpdateOrderDto
    {
        public long Id { get; set; }
        public int Status { get; set; }
        public int? PaymentStatus { get; set; }
    }
}
