using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment);

        Task<List<Payment>?> GetByStatusAsync(PaymentStatus status);

        Task<Payment?> GetAsync(Guid Id);

    }
}
