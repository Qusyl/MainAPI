using Domain.value;


namespace Domain.Events.Payment
{
    public class PaymentUnknownEvent : IDomainEvent
    {
        public string EventType => "Payment.unknown";

        public int Version => 1;

        public DateTime OccurredOn { get;  }

        public List<AttemptInfo> Attempts { get; }
        public string ProviderName { get;  }
        public Guid PaymentId { get; }
        public PaymentUnknownEvent(DateTime occurredOn, List<AttemptInfo> attempts, Guid paymentId)
        {
            OccurredOn = occurredOn;
            Attempts = attempts;
            PaymentId = paymentId;
        }
    }
}
