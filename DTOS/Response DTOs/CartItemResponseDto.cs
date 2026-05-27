namespace WebApplication1.DTOS.Response_DTOs
{
    public class CartItemResponseDto
    {
        public required int VariantId { get; set; }
        public required string ProductName { get; set; }
        public required int Quantity { get; set; }
        public required string Color { get; set; }

        public required string Size { get; set; }
        public required decimal Price { get; set; }
        public required decimal Discount { get; set; }
        public required decimal SubTotal { get; set; }
    }
}
