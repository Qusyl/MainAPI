using Domain.value;

namespace Domain.Entity
{
    public enum AttemptStatus
    {
        Started,
        Accepted,
        Failed,
        Timeout,
        Pending,
        Unknown
    }
    public class PaymentAttempt : IAppEntity
    {
        public Guid Id { get;private set; }

        public Guid PaymentId { get;private set; }

        public string Provider { get; private set; }

        public int AttemptNumber { get; private set; }

        public string CurrentAttemptStatus { get; private set; }

        public string? ErrorMessage { get; private set; }

        public string ProviderTransactionId { get; private set; }   

        public DateTime StartedAt { get; private set; }

        public DateTime? CompletedAt { get; private set; }

        private List<IDomainEvent> _events => new();

        public IReadOnlyCollection<IDomainEvent> Events => _events;
        private PaymentAttempt()
        {
        }
        public PaymentAttempt(Guid paymentId, string provider, int attemptNumber, string attemptStatus, string? errorMessage, string providerTransactionId, DateTime startedAt, DateTime? completedAt)
        {
            Id = Guid.NewGuid();
            PaymentId = paymentId;
            Provider = provider;
            AttemptNumber = attemptNumber;
            CurrentAttemptStatus = attemptStatus;
            ErrorMessage = errorMessage;
            ProviderTransactionId = providerTransactionId;
            StartedAt = startedAt;
            CompletedAt = completedAt;
        }

        public void MarkTimeOut()
        {
            CurrentAttemptStatus = AttemptStatus.Timeout.ToString();
            ErrorMessage = "TimeOut";
        }
        public void MarkAccepted(string transactionId)
        {
            CurrentAttemptStatus = AttemptStatus.Accepted.ToString(); ;
            ProviderTransactionId = transactionId;
        }
        public void MarkFailed(string error)
        {
            CurrentAttemptStatus = AttemptStatus.Failed.ToString(); ;
            ErrorMessage = error;
        }
        public void MarkUnknown(string error)
        {
            CurrentAttemptStatus = AttemptStatus.Unknown.ToString(); ;
            ErrorMessage = error;
        }

        public void ClearEvents()
        {
            _events.Clear();
        }
    }
    
}
