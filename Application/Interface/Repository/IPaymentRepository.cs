using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Repository
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment);

        Task<List<Payment>?> GetByStatusAsync(PaymentStatus status);

        Task<Payment?> GetAsync(Guid Id);

        Task<string> GetStatusAsync(Guid Id);

        Task<Payment?> GetByIdempotencyAsync(string key);

    }
}
