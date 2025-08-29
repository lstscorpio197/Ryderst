namespace ShopAdmin.Dto.Products
{
    public class AddToCartInputDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int[] ProductAttributeValueIds { get; set; }
    }
}
