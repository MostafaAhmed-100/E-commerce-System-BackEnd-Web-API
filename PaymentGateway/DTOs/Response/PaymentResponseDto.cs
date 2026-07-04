namespace WebApplication1.PaymentGateway.External.Response
{
    public class PaymentResponseDto
    {
        public bool IsSuccess { get; set; }
        public string? TransactionId { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
        public int? StatusCode { get; set; } = 200;
        public string? CheckoutUrl { get; set; }

        public string? Exception {  get; set; }

    }
}