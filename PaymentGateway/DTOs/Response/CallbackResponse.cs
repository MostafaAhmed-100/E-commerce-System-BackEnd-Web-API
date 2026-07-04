namespace WebApplication1.PaymentGateway.DTOs.Response
{
    public class CallbackResponse
    {
        public int OrderId { get; set; }
        public string TransactionId { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
        public string? Exception { get; set; }
    }
}
