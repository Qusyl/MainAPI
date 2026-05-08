using Domain.Entity;
using Domain.value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events.Payment
{
    public class PaymentCompleteEvent : IDomainEvent
    {
        public string EventType => "Payment.complete";

        public int Version => 1;

        public DateTime OccurredOn { get; }

        public List<PaymentAttempt> Attempts { get;  }
        public Guid PaymentId { get; }

        public string ProviderName { get; }

        public PaymentCompleteEvent(DateTime occurredOn, List<PaymentAttempt> attempts, Guid paymentId, string providerName)
        {
            OccurredOn = occurredOn;
            Attempts = attempts;
            PaymentId = paymentId;
            ProviderName = providerName;
        }
    }
}
