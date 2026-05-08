using Domain.Entity;
using Domain.value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events.Payment
{
    public class PaymentCancelledEvent : IDomainEvent
    {
        public string EventType => "Payment.cancelled";
        public int Version => 1;
        public DateTime OccurredOn { get;}
        public List<PaymentAttempt> Attempts { get; }

        public string ProviderName { get; }

        public string Message { get; }  
        public Guid PaymentId { get;}
        public PaymentCancelledEvent(DateTime occurredOn, List<PaymentAttempt> attempts, Guid paymentId)
        {
            OccurredOn = occurredOn;
            Attempts = attempts;
            PaymentId = paymentId;
        }
    }
}
