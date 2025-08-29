namespace ShopAdmin.Dto.Categories
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string ParentId { get; set; }
        public string ParentName { get; set; }
    }
}
