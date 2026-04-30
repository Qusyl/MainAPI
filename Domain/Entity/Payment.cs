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

        public decimal Amount { get; private set; }

        public string Currency { get; private set; }

        public PaymentStatus Status { get; private set; }

        public Provider CurrentProvider { get; private set; } 

        public string IdempotencyKey { get; private set; }  

        public uint Version { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public List<PaymentAttempt> Attempts { get; private set; } = new(); 
        
        public DateTime UpdatedAt { get; private set; }

        private List<IDomainEvent> _events = new();

        public IReadOnlyCollection<IDomainEvent> Events => _events;

        private Payment(decimal amount, string currency, Provider currentProvider)
        {
            Id = Guid.NewGuid();
            Amount = amount;
            Currency = currency;
            Status = PaymentStatus.Pending;
            CurrentProvider = currentProvider;
            CreatedAt = DateTime.UtcNow;
        }

        public static Result<Payment,EntityError> Create(decimal amount, string currency, Provider currencyProvider)
        {
            if(amount <= 0)
            {
                return Result<Payment, EntityError>.Failure(EntityError.InvalidAmount);
            }
            if (string.IsNullOrEmpty(currency))
            {
                return Result<Payment, EntityError>.Failure(EntityError.InvalidCurrency);
            }

            var payment = new Payment(amount, currency, currencyProvider);

            return Result<Payment, EntityError>.Success(payment);
        }

        public void RegisterAttempt(Provider provider, PaymentAttempt attempt)
        {
            CurrentProvider = provider;
            Attempts.Add(attempt);
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAccepted()
        {
            Status = PaymentStatus.Accepted;
            _events.Add(new PaymentCompleteEvent(DateTime.UtcNow, Attempts.Select(a => new AttemptInfo(a.Provider.Name, a.AttemptStatus.ToString(), a.ErrorMessage)).ToList(), Id, CurrentProvider.Name));
        }

        public void MarkCancelled()
        {
            Status = PaymentStatus.Cancelled;
            _events.Add(new PaymentCancelledEvent(DateTime.UtcNow, Attempts.Select(a => new AttemptInfo(a.Provider.Name, a.AttemptStatus.ToString(), a.ErrorMessage)).ToList(), Id));
        }
        public void MarkUnknown()
        {
            Status = PaymentStatus.Unknown;
            _events.Add(new PaymentUnknownEvent(DateTime.UtcNow, Attempts.Select(a => new AttemptInfo(a.Provider.Name, a.AttemptStatus.ToString(), a.ErrorMessage)).ToList(), Id));
        }
        public void ClearEvents()
        {
            _events.Clear();
        }
    }
}
