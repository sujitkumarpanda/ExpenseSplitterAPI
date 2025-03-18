using ExpenseSplitterAPI.APIModels;
using ExpenseSplitterAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ExpenseSplitterAPI.Controllers
{
    [Route("api/payments")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

       
        [HttpPost("settle")]
        public async Task<IActionResult> SettlePayment([FromBody] PaymentRequestModel request)
        {
            if (request == null || request.GroupId <= 0 || request.FromUserId <= 0 || request.ToUserId <= 0 || request.Amount <= 0)
            {
                return BadRequest("Invalid payment details.");
            }

            var result = await _paymentService.SettlePayment(request);
            if (!result)
            {
                return BadRequest("Failed to settle payment.");
            }

            return Ok(new { message = "Payment settled successfully." });
        }
    }
}
