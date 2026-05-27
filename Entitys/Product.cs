
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Entitys;
namespace WebApplication1.Entities
{


    public class Product
    {
        [Key]
        public required int ProductId { get; set; }
        [Required]
        public required int SellerId { get; set; }
        [Required]
        public required string ProductName { get; set; }

        [Required]
        public  required string ProductDescription { get; set; }
        public  string? ImagePath { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        public bool IsDeleted { get; set; } = false;
        [Required]
        public required int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public required Category Category { get; set; }
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        [Required]
        public required Seller Seller { get; set; }
        public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

    }
}