using ExpenseSplitterAPI.APIModels;
using ExpenseSplitterAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseSplitterAPI.Controllers
{
    [Route("api/debts")]
    [ApiController]
    [Authorize]
    public class DebtController : ControllerBase
    {
        private readonly IDebtService _debtService; 

        public DebtController(IDebtService debtService) 
        {
            _debtService = debtService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<DebtResponseModel>>> GetUserDebts(int userId)
        {
            var debts = await _debtService.GetUserDebts(userId);
            return Ok(debts);
        }
    }
}
