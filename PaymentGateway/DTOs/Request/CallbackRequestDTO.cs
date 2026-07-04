namespace WebApplication1.PaymentGateway.DTOs.Request
{
    public class CallbackRequestDTO
    {
        public int OrderId { get; set; }
        public string TransactionId { get; set; }
        public bool IsSuccess { get; set; }
    }
}
