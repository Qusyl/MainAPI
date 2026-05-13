using Application.Interface.Repository;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Repository
{
    public class PaymentAttemptRepository : IPaymentAttemptRepository
    {
        private readonly AppDbContext _context;

        public PaymentAttemptRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PaymentAttempt attempt)
        {
            await _context.Attempts.AddAsync(attempt);

        }

        public async Task<List<PaymentAttempt>?> GetByUserIdAsync(Guid userId)
        {
           return await _context.Attempts.Where(a => a.UserId == userId).ToListAsync();
        }
    }
}
