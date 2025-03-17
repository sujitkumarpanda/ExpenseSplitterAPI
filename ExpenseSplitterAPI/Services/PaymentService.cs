using ExpenseSplitterAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseSplitterAPI.Services
{
    public class PaymentService
    {
        private readonly ExpenseSplitterDbContext _context;

        public PaymentService(ExpenseSplitterDbContext context)
        {
            _context = context;
        }

        // Get all payments for a group
        public async Task<List<Payment>> GetPaymentsByGroupIdAsync(int groupId)
        {
            return await _context.Payments
                .Where(p => p.GroupId == groupId)
                .Include(p => p.FromUser)
                .Include(p => p.ToUser)
                .ToListAsync();
        }

        // Create a new payment
        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        // Get a single payment by ID
        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.FromUser)
                .Include(p => p.ToUser)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Delete a payment
        public async Task<bool> DeletePaymentAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return false;

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
