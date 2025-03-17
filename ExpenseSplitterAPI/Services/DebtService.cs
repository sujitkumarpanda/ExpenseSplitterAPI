using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ExpenseSplitterAPI.APIModels;
using ExpenseSplitterAPI.Model;

namespace ExpenseSplitterAPI.Services
{
    public class DebtService : IDebtService
    {
        private readonly ExpenseSplitterDbContext _context;

        public DebtService(ExpenseSplitterDbContext context)
        {
            _context = context;
        }

        public async Task<List<DebtResponseModel>> GetUserDebts(int userId)
        {
            var debts = await _context.Debts
                .Where(d => d.OwedByUserId == userId || d.OwedToUserId == userId)
                .ToListAsync();

            return debts.Select(d => new DebtResponseModel
            {
                Id = d.Id,
                GroupId = d.GroupId,
                ExpenseId = d.ExpenseId,
                OwedByUserId = d.OwedByUserId,
                OwedToUserId = d.OwedToUserId,
                Amount = d.Amount
            }).ToList();
        }
    }
}
