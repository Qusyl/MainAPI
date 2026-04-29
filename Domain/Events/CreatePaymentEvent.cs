using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public class CreatePaymentEvent : IDomainEvent
    {
        public Guid Id { get;  }
        public string EventType => "Payment.created";

        public int Version => 1;

        public DateTime OccurredOn { get;  }

        public CreatePaymentEvent(DateTime occurredOn)
        {
            Id = Guid.NewGuid();
            OccurredOn = occurredOn;
        }
    }
}
