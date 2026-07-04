namespace WebApplication1.DTOS.Response_DTOs
{
    public class OrderResponseDto
    {
        public required int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? AppliedCouponCode { get; set; }
        public required decimal DiscountAmount { get; set; } 
        public required decimal TotalAmount { get; set; }
        public required string Status { get; set; }
        public required string ShippingAddress { get; set; }
        public string? CheckoutUrl { get; set; }
        public required List<OrderItemResponseDto> Items { get; set; }
    }
}
