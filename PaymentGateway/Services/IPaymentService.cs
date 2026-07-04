using WebApplication1.PaymentGateway.DTOs.Request;
using WebApplication1.PaymentGateway.DTOs.Response;
using WebApplication1.PaymentGateway.External.Request;
using WebApplication1.PaymentGateway.External.Response;
namespace WebApplication1.PaymentGateway.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> InitializePayment(PaymentRequestDto paymentRequest);
        Task<CallbackResponse> ProcessCallback(CallbackRequestDTO callbackRequest);
    }
}
