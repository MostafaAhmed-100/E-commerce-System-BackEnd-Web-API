using Microsoft.AspNetCore.Mvc;
using WebApplication1.PaymentGateway.DTOs.Request;
using WebApplication1.PaymentGateway.External.Request;
using WebApplication1.PaymentGateway.Services;
using WebApplication1.Services.OrderService;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;

        public PaymentController(IPaymentService paymentService, IOrderService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
        }

        [HttpPost("InitializePayment - FORTEST")]
        public async Task<IActionResult> InitializePayment([FromBody] PaymentRequestDto request)
        {
            var initialize = await _paymentService.InitializePayment(request);

            if (initialize.IsSuccess)
            {
                return StatusCode(initialize.StatusCode ?? 200, initialize);
            }
            return Problem(
                detail: initialize.Exception ?? "فشل في إنشاء معاملة الدفع.",
                statusCode: initialize.StatusCode ?? 500,
                title: "Initialize Payment Failed"
            );
        }

        [HttpPost("callback - FORTEST")]
        public async Task<IActionResult> Callback([FromBody] CallbackRequestDTO request)
        {
            var result = await _paymentService.ProcessCallback(request);

            if (result.IsSuccess)
            {
                await _orderService.HandlePaymentCallbackAsync(result.OrderId, true);
                return Ok(result);
            }
            else
            {
                await _orderService.HandlePaymentCallbackAsync(result.OrderId, false);
                return Problem(
                    detail: result.Exception ?? "تم رفض معاملة الدفع.",
                    statusCode: 400,
                    title: "Callback Processing Failed"
                );
            }
        }
    }
}