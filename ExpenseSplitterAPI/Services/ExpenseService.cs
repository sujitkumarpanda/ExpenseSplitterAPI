using ExpenseSplitterAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseSplitterAPI.Services
{
    public class ExpenseService
    {
        private readonly ExpenseSplitterDbContext _context;

        public ExpenseService(ExpenseSplitterDbContext context)
        {
            _context = context;
        }

     
        public async Task<Expense> CreateExpense(int groupId, string description, decimal amount, int payerId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            var payer = await _context.Users.FindAsync(payerId);

            if (group == null || payer == null)
                return null;

            var expense = new Expense
            {
                GroupId = groupId,
                Description = description,
                Amount = amount,
                PayerId = payerId
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return expense;
        }

    
        public async Task<List<Expense>> GetAllExpenses(int groupId)
        {
            return await _context.Expenses
                                 .Where(e => e.GroupId == groupId)
                                 .Include(e => e.Payer)  
                                 .Include(e => e.Group)  
                                 .ToListAsync();
        }

        public async Task<decimal> GetExpenseSummary(int groupId)
        {
            return await _context.Expenses
                .Where(e => e.GroupId == groupId)
                .SumAsync(e => e.Amount);
        }

        public async Task<List<Expense>> GetRecentExpenses(int groupId)
        {
            return await _context.Expenses
                .Where(e => e.GroupId == groupId)
                .Include(e => e.Payer)
                .OrderByDescending(e => e.Id)
                .Take(5)
                .ToListAsync();
        }



        public async Task<Expense> GetExpenseById(int expenseId)
        {
            return await _context.Expenses
                                 .Include(e => e.Payer)
                                 .Include(e => e.Group)
                                 .FirstOrDefaultAsync(e => e.Id == expenseId);
        }

        public async Task<bool> DeleteExpense(int expenseId)
        {
            var expense = await _context.Expenses.FindAsync(expenseId);
            if (expense == null)
                return false;

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
