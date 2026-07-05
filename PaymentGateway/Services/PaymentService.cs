using Azure;
using Microsoft.Extensions.Logging;
using WebApplication1.Exceptions;
using WebApplication1.PaymentGateway.DTOs.Request;
using WebApplication1.PaymentGateway.DTOs.Response;
using WebApplication1.PaymentGateway.External.Request;
using WebApplication1.PaymentGateway.External.Response;

namespace WebApplication1.PaymentGateway.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IHttpClientFactory httpClientFactory, ILogger<PaymentService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<PaymentResponseDto> InitializePayment(PaymentRequestDto paymentRequest)
        {
            var client = _httpClientFactory.CreateClient();
            string TransactionId = "transction complete";
            try
            {
                var response = await client.PostAsJsonAsync("https://webhook.site/e95736fa-8f94-450a-ab6c-7becfcf454d5", paymentRequest);
                if (response.IsSuccessStatusCode == false)
                {
                    _logger.LogError("Payment initialization failed for Order {OrderId}. External API returned StatusCode: {StatusCode}", paymentRequest.OrderId, (int)response.StatusCode);
                    return new PaymentResponseDto
                    {
                        IsSuccess = response.IsSuccessStatusCode,
                        StatusCode = (int)response.StatusCode,
                        Exception = "لرابط الخارجي غير صحيح أو السيرفر غير متاح"
                    };
                }

                var paymentToken = Guid.NewGuid().ToString();
                var checkoutUrl = $"https://localhost:5001/checkout?orderId={paymentRequest.OrderId}&token={paymentToken}";

                _logger.LogInformation("Payment successfully initialized for Order {OrderId}. TransactionId: {TransactionId}", paymentRequest.OrderId, TransactionId);

                return new PaymentResponseDto
                {
                    IsSuccess = response.IsSuccessStatusCode,
                    StatusCode = (int)response.StatusCode,
                    TransactionId = TransactionId,
                    CheckoutUrl = checkoutUrl,
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while contacting payment gateway for Order {OrderId}.", paymentRequest.OrderId);
                return new PaymentResponseDto
                {
                    IsSuccess = false,
                    Exception = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred during payment initialization for Order {OrderId}.", paymentRequest.OrderId);
                return new PaymentResponseDto
                {
                    Exception = ex.Message,
                };
            }
        }

        public async Task<CallbackResponse> ProcessCallback(CallbackRequestDTO callbackRequest)
        {
            await Task.CompletedTask;
            try
            {
                if (callbackRequest.IsSuccess == true)
                {
                    _logger.LogInformation("Payment callback received: SUCCESS for Order {OrderId}. TransactionId: {TransactionId}", callbackRequest.OrderId, callbackRequest.TransactionId);
                    return new CallbackResponse
                    {
                        IsSuccess = true,
                        TransactionId = callbackRequest.TransactionId,
                        OrderId = callbackRequest.OrderId,
                    };
                }
                else
                {
                    _logger.LogWarning("Payment callback received: FAILED for Transaction {TransactionId}. OrderId: {OrderId}", callbackRequest.TransactionId, callbackRequest.OrderId);
                    return new CallbackResponse
                    {
                        IsSuccess = false,
                        Exception = null,
                        ProcessedAt = DateTime.Now,
                        TransactionId = callbackRequest.TransactionId,
                    };
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing payment callback for Transaction {TransactionId}.", callbackRequest.TransactionId);
                return new CallbackResponse
                {
                    IsSuccess = false,
                    Exception = $"حدث خطأ: {exception.Message}"
                };
            }
        }
    }
}