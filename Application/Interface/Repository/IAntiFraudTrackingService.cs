using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Repository
{
    public interface IAntiFraudTrackingService
    {
        Task RegisterTransactionAttemptAsync(Guid userId);
        Task RegisterTransactionDeclineAsync(Guid userId);
    }
}
