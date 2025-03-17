using ExpenseSplitterAPI.APIModels;
using ExpenseSplitterAPI.Model;
using ExpenseSplitterAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseSplitterAPI.Controllers
{
    [Route("api/payments")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // Get payments by group ID
        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<List<PaymentResponseModel>>> GetPaymentsByGroupId(int groupId)
        {
            var payments = await _paymentService.GetPaymentsByGroupIdAsync(groupId);
            return Ok(payments);
        }

        // Get payment by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentResponseModel>> GetPaymentById(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        // Create a new payment
        [HttpPost]
        public async Task<ActionResult<PaymentResponseModel>> CreatePayment([FromBody] PaymentRequestModel model)
        {
            var payment = new Payment
            {
                GroupId = model.GroupId,
                FromUserId = model.FromUserId,
                ToUserId = model.ToUserId,
                Amount = model.Amount
            };

            var createdPayment = await _paymentService.CreatePaymentAsync(payment);
            return CreatedAtAction(nameof(GetPaymentById), new { id = createdPayment.Id }, createdPayment);
        }

        // Delete a payment
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var success = await _paymentService.DeletePaymentAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
