namespace WebApplication1.DTOS.Response_DTOs
{
    public class OrderItemResponseDto
    {
        public required string ProductNameSnapshot { get; set; }

        public required int Quantity { get; set; }
        public required decimal UnitPrice { get; set; }
        public required decimal SubTotal { get; set; }
    }
}
