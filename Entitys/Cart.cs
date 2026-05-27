
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Entitys;
namespace WebApplication1.Entities
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        public required decimal Size { get; set; }

        [Required]
        public required int Quantity { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}