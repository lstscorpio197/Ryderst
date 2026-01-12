namespace ShopAdmin.Dto.PosAPI
{
    public class PancakeApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
    }
}
