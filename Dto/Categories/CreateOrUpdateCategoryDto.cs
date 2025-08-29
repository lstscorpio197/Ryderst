namespace ShopAdmin.Dto.Categories
{
    public class CreateOrUpdateCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int? ParentId { get; set; }
    }
}
