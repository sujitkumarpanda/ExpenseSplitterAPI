﻿using ExpenseSplitterAPI.APIModels;
using ExpenseSplitterAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ExpenseSplitterAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ExpenseSplitterDbContext _context;

        public PaymentService(ExpenseSplitterDbContext context)
        {
            _context = context;
        }

     
        public async Task<bool> SettlePayment(PaymentRequestModel request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Check if users exist in the group
                var group = await _context.Groups
                    .Include(g => g.GroupMembers)
                    .FirstOrDefaultAsync(g => g.Id == request.GroupId);

                if (group == null ||
                    !group.GroupMembers.Any(m => m.UserId == request.FromUserId) ||
                    !group.GroupMembers.Any(m => m.UserId == request.ToUserId))
                {
                    return false; // One of the users is not in the group
                }

                // Create a new payment record
                var payment = new Payment
                {
                    GroupId = request.GroupId,
                    FromUserId = request.FromUserId,
                    ToUserId = request.ToUserId,
                    Amount = request.Amount
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // ✅ Deduct the settled amount from the payer's expenses
                var remainingAmount = request.Amount;

                var expenses = await _context.Expenses
                    .Where(e => e.GroupId == request.GroupId && e.PayerId == request.FromUserId)
                    .OrderBy(e => e.Id) // Ensuring we deduct from oldest expense first
                    .ToListAsync();

                foreach (var expense in expenses)
                {
                    if (remainingAmount <= 0) break; // Stop if fully settled

                    if (expense.Amount >= remainingAmount)
                    {
                        expense.Amount -= remainingAmount;
                        remainingAmount = 0;
                    }
                    else
                    {
                        remainingAmount -= expense.Amount;
                        expense.Amount = 0;
                    }
                }

                await _context.SaveChangesAsync(); // ✅ Save updated expense amounts

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
