using ExpenseSplitterAPI.APIModels;
using ExpenseSplitterAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseSplitterAPI.Controllers
{
    [Route("api/expenses")]
    [ApiController]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly ExpenseSplitterDbContext _context;

        public ExpenseController(ExpenseSplitterDbContext context)
        {
            _context = context;
        }


        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<ExpenseResponseModel>>> GetExpensesByGroup(int groupId)
        {
            var expenses = await _context.Expenses
                .Where(e => e.GroupId == groupId)
                .Include(e => e.Payer)
                .ToListAsync();

            var response = expenses.Select(e => new ExpenseResponseModel
            {
                Id = e.Id,
                GroupId = e.GroupId,
                Description = e.Description,
                Amount = e.Amount,
                PayerId = e.PayerId,
                PayerName = e.Payer.Name
            }).ToList();

            return Ok(response);
        }


        [HttpPost]
        public async Task<ActionResult<ExpenseResponseModel>> CreateExpense([FromBody] ExpenseCreateModel model)
        {
            if (model == null)
                return BadRequest("Invalid expense data.");

            var payer = await _context.Users.FindAsync(model.PayerId);
            if (payer == null)
                return NotFound("Payer not found.");

            var groupMembers = await _context.GroupMembers
                                             .Where(gm => gm.GroupId == model.GroupId)
                                             .Select(gm => gm.UserId)
                                             .ToListAsync();

            if (!groupMembers.Contains(model.PayerId))
                return BadRequest("Payer is not a member of the group.");

            if (groupMembers.Count <= 1)
                return BadRequest("Cannot split expenses in a group with only one member.");

            var expense = new Expense
            {
                GroupId = model.GroupId,
                Description = model.Description,
                Amount = model.Amount,
                PayerId = model.PayerId
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            // Split Expense Equally Among Group Members (excluding payer)
            decimal splitAmount = model.Amount / groupMembers.Count;

            var debts = groupMembers
                .Where(userId => userId != model.PayerId)  // Exclude payer
                .Select(userId => new Debt
                {
                    ExpenseId = expense.Id,
                    GroupId = model.GroupId,
                    OwedByUserId = userId,
                    OwedToUserId = model.PayerId,
                    Amount = splitAmount
                }).ToList();

            _context.Debts.AddRange(debts);
            await _context.SaveChangesAsync();

            var response = new ExpenseResponseModel
            {
                Id = expense.Id,
                GroupId = expense.GroupId,
                Description = expense.Description,
                Amount = expense.Amount,
                PayerId = expense.PayerId,
                PayerName = payer.Name
            };

            return CreatedAtAction(nameof(GetExpensesByGroup), new { groupId = expense.GroupId }, response);
        }
        [HttpGet("group/{groupId}/balances")]
        public async Task<ActionResult<IEnumerable<object>>> GetGroupBalances(int groupId)
        {
            var balances = await _context.Debts
                .Where(d => d.GroupId == groupId)
                .GroupBy(d => new { d.OwedByUserId, d.OwedToUserId })
                .Select(g => new
                {
                    OwedByUserId = g.Key.OwedByUserId,
                    OwedToUserId = g.Key.OwedToUserId,
                    TotalAmount = g.Sum(d => d.Amount)
                })
                .ToListAsync();

            return Ok(balances);
        }

        [HttpGet("summary/{groupId}")]
        public async Task<ActionResult<object>> GetExpenseSummary(int groupId)
        {
            var totalExpense = await _context.Expenses
                .Where(e => e.GroupId == groupId)
                .SumAsync(e => e.Amount);

            return Ok(new { GroupId = groupId, TotalExpense = totalExpense });
        }

        [HttpGet("recent/{groupId}")]
        public async Task<ActionResult<IEnumerable<ExpenseResponseModel>>> GetRecentExpenses(int groupId)
        {
            var expenses = await _context.Expenses
                .Where(e => e.GroupId == groupId)
                .Include(e => e.Payer)
                .OrderByDescending(e => e.Id)
                .Take(5)
                .ToListAsync();

            var response = expenses.Select(e => new ExpenseResponseModel
            {
                Id = e.Id,
                GroupId = e.GroupId,
                Description = e.Description,
                Amount = e.Amount,
                PayerId = e.PayerId,
                PayerName = e.Payer.Name
            }).ToList();

            return Ok(response);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                return NotFound("Expense not found.");

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
