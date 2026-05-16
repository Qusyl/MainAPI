using Application.Dto;
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
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        public async Task<Payment?> GetAsync(Guid Id)
        {
           return await _context.Payments.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<Payment?> GetByIdempotencyAsync(string key)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.IdempotencyKey == key);
        }

        public async Task<List<Payment>?> GetByStatusAsync(PaymentStatus status)
        {
            return await _context.Payments.Where(p => p.Status == status.ToString()).ToListAsync();
        }

        public async Task<string> GetStatusAsync(Guid Id)
        {
         var payment =  await _context.Payments.FirstOrDefaultAsync(p => p.Id == Id);

            return payment.Status;
        }

        public async Task UpdateAsync(Guid id, Payment newPayment)
        {
           var existing = await _context.Payments.FindAsync(id);
            if (existing == null) { return; }

            _context.Entry(existing).CurrentValues.SetValues(newPayment);

        }
    }
}
