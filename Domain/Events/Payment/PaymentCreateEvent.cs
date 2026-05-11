using Domain.Entity;


namespace Domain.Events.Payment
{
    public class PaymentCreateEvent : IDomainEvent
    {
        public string EventType => "payment.create";

        public int Version => 1;

        public DateTime OccurredOn { get; set; }

        public List<PaymentAttempt> Attempts => new List<PaymentAttempt>(0);

        public decimal Amount { get; set; }

        public string Currency {  get; set; }

        public string Provider { get; set; }

        public string IdempotencyKey { get; set; }

        public Guid UserId { get; set; }    

        public PaymentCreateEvent(DateTime occurredOn, decimal amount, string currency, string provider, string idempotencyKey, Guid userId) { 
            OccurredOn=  occurredOn;
            Amount= amount;
            UserId = userId;
            Currency= currency;
            Provider= provider;
            IdempotencyKey= idempotencyKey;
        }
    }
}
