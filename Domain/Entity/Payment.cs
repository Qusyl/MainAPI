using Domain.Events.Payment;
using Domain.value;

namespace Domain.Entity
{
    public enum PaymentStatus
    {
        Accepted, 
        Pending, 
        Cancelled,
        Unknown
    }
    public class Payment : IAppEntity
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public decimal Amount { get; private set; }

        public string Currency { get; private set; }

        public string Status { get; private set; }

        public string CurrentProvider { get; private set; } 

        public string IdempotencyKey { get; private set; }  

        public uint Version { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public List<PaymentAttempt> Attempts { get; private set; } = new(); 
        
        public DateTime UpdatedAt { get; private set; }

        private List<IDomainEvent> _events = new();

        public IReadOnlyCollection<IDomainEvent> Events => _events;
        private Payment()
        {
        }
        private Payment(decimal amount, string currency, string currentProvider, string idempotencyKey, Guid userId)
        {
            Id = Guid.NewGuid();
            Amount = amount;
            Currency = currency;
            Status = PaymentStatus.Pending.ToString();
            IdempotencyKey = idempotencyKey;
            CurrentProvider = currentProvider;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            UserId = userId;
        }

        public static Result<Payment,EntityError> Create(decimal amount, string currency, string currencyProvider, string idempotencyKey, Guid userId)
        {
            if(amount <= 0)
            {
                return Result<Payment, EntityError>.Failure(EntityError.InvalidAmount);
            }
            if (string.IsNullOrEmpty(currency))
            {
                return Result<Payment, EntityError>.Failure(EntityError.InvalidCurrency);
            }

            var payment = new Payment(amount, currency, currencyProvider, idempotencyKey, userId);

            return Result<Payment, EntityError>.Success(payment);
        }

        public void RegisterAttempt(string provider, PaymentAttempt attempt)
        {
            CurrentProvider = provider;
            Attempts.Add(attempt);
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAccepted()
        {
            Status = PaymentStatus.Accepted.ToString();
            _events.Add(new PaymentCompleteEvent(DateTime.UtcNow, Attempts, Id, CurrentProvider));
        }

        public void MarkCancelled()
        {
            Status = PaymentStatus.Cancelled.ToString(); 
            _events.Add(new PaymentCancelledEvent(DateTime.UtcNow, Attempts, Id));
        }
        public void MarkUnknown()
        {
            Status = PaymentStatus.Unknown.ToString(); 
            _events.Add(new PaymentUnknownEvent(DateTime.UtcNow, Attempts, Id));
        }
        public void ClearEvents()
        {
            _events.Clear();
        }
    }
}
