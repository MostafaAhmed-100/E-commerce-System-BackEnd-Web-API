using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entitys
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public  int CartId { get; set; }
        public required int Quantity { get; set; }
        public required int ProductVariantId { get; set; }
        public Cart Cart { get; set; }
        public ProductVariant ProductVariant { get; set; }


    }
}
