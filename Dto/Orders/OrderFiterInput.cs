namespace ShopAdmin.Dto.Orders
{
    public class OrderFiterInput : SearchDto
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public int? TrangThai { get; set; }
        public int? TrangThaiThanhToan { get; set; }
    }
}
