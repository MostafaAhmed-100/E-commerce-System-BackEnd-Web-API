using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entitys
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public required string CategoryName { get; set; }
        
        public int? ParentCategoryId { get; set; }

        public Category? ParentCategory { get; set; }

        public ICollection<Category> SubCategories { get; set; } = new List<Category>();

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}