
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Entitys;
namespace WebApplication1.Entities
{
    public class Cart
    {
        public int CartId { get; set; }
        public required int BuyerId { get; set; }
        public Buyer Buyer { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}