namespace WebApplication1.DTOS.Response_DTOs
{
    public class CartItemResponseDto
    {
        public int VariantId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; }
               
        public string Size { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal SubTotal { get; set; }
    }
}
