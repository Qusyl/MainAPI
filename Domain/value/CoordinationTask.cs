using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.value
{
    public class CoordinationTask
    {
        public Guid PaymentId { get; set; }
        public string ProviderName { get; set; }
        public DateTime RetryExecutionDate { get; set; }

        public CoordinationTask(Guid paymentId, string providerName, DateTime retryExecutionDate)
        {
            PaymentId = paymentId;
            ProviderName = providerName;
            RetryExecutionDate = retryExecutionDate;
        }
    }
}
