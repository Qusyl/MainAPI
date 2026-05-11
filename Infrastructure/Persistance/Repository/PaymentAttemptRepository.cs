using Application.Interface.Repository;
using Domain.Entity;
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
    }
}
