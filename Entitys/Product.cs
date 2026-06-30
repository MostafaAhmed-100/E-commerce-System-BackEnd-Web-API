
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Entitys;
namespace WebApplication1.Entities
{


    public class Product
    {
        public int ProductId { get; set; }
        public required int SellerId { get; set; }
        public required string ProductName { get; set; }
        public  required string ProductDescription { get; set; }
        public  string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public required int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public Seller Seller { get; set; }
        public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

    }
}