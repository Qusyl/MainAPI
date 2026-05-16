using Domain;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Services
{
    public interface IPaymentPendingService
    {
        Task<Result<string, ApplicationError>> HandleAsync(Payment payment, CancellationToken cts = default);
    }
}
