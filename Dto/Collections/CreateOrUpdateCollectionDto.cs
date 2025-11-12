namespace ShopAdmin.Dto.Collections
{
    public class CreateOrUpdateCollectionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
    }
}
