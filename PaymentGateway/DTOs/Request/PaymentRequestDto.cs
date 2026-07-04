namespace WebApplication1.PaymentGateway.External.Request
{
    public class PaymentRequestDto
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string CurrencyCode { get; set; }
        public string CustomerName { get; set; }

        public required string CustomerEmail { get; set; }

    }
}
