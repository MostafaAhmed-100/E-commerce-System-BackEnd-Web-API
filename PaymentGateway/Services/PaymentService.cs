using Azure;
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

        public PaymentService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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
                    return new PaymentResponseDto
                    {
                        IsSuccess = response.IsSuccessStatusCode,
                        StatusCode = (int)response.StatusCode,
                        Exception = "لرابط الخارجي غير صحيح أو السيرفر غير متاح"

                    };
                }
                var paymentToken = Guid.NewGuid().ToString();
                var checkoutUrl = $"https://localhost:5001/checkout?orderId={paymentRequest.OrderId}&token={paymentToken}";
                
                return new PaymentResponseDto
                {
                    IsSuccess = response.IsSuccessStatusCode,
                    StatusCode = (int)response.StatusCode,
                    TransactionId = TransactionId,
                    CheckoutUrl = checkoutUrl,

                };
            }
            catch(HttpRequestException ex)
            {
                return new PaymentResponseDto
                {
                    IsSuccess = false,
                    Exception = ex.Message
                };
            }
            catch (Exception ex)
            {
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
                    return new CallbackResponse
                    {
                        IsSuccess = true,
                        TransactionId = callbackRequest.TransactionId,
                        OrderId = callbackRequest.OrderId,

                    };

                }
                else
                {
                    return new CallbackResponse
                    {
                        IsSuccess = false,
                        Exception = null,
                        ProcessedAt = DateTime.Now,
                        TransactionId = callbackRequest.TransactionId,
                    };
                }
            }
            catch(Exception exception)
            {
                return new CallbackResponse
                {
                    IsSuccess = false,
                    Exception = $"حدث خطأ: {exception.Message}"
                };
            }
        }
    }
}