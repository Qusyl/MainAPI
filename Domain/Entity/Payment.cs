


using Domain.Events;
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
        public int Attempts { get; private set; }   
        
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
            Attempts = 0;
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

            payment._events.Add(new CreatePaymentEvent(DateTime.UtcNow));

            return Result<Payment, EntityError>.Success(payment);
        }

        public void RegisterAttempt(Provider provider)
        {
            CurrentProvider = provider;
            Attempts++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAccepted()
        {
            Status = PaymentStatus.Accepted;
        }

        public void MarkCancelled()
        {
            Status = PaymentStatus.Cancelled;
        }
        public void MarkUnknown()
        {
            Status = PaymentStatus.Unknown;
        }
        public void ClearEvents()
        {
            _events.Clear();
        }
    }
}
