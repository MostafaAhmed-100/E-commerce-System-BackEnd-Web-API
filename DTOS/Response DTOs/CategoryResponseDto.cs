namespace WebApplication1.DTOS.Response_DTOs
{
    public class CategoryResponseDto
    {
        public required int CategoryId { get; set; }

        public required string CategoryName { get; set; }

        public int? ParentCategoryId { get; set; }

        public List<CategoryResponseDto>? SubCategories { get; set; }
    }
}
