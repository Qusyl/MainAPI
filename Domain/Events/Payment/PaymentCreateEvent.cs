using Domain.value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events.Payment
{
    public class PaymentCreateEvent : IDomainEvent
    {
        public string EventType => "payment.create";

        public int Version => 1;

        public DateTime OccurredOn { get; set; }

        public List<AttemptInfo> Attempts => new List<AttemptInfo>(0);

        public decimal Amount { get; set; }

        public string Currency {  get; set; }

        public string Provider { get; set; }

        public PaymentCreateEvent(DateTime occurredOn) { 
            OccurredOn=  occurredOn;
        }
    }
}
