namespace WebApplication1.DTOS.Response_DTOs
{
    public class CartResponseDto
    {
        public required int CartId { get; set; }
        public required List<CartItemResponseDto> Items { get; set; }
        public required decimal TotalPrice { get; set; }
    }
}
