namespace WebApplication1.DTOS.Response_DTOs
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? AppliedCouponCode { get; set; }
        public decimal DiscountAmount { get; set; } 
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }
        public string? CheckoutUrl { get; set; }
        public List<OrderItemResponseDto> Items { get; set; }
    }
}
