using Domain.value;

namespace Domain.Entity
{
    public enum AttemptStatus
    {
        Started,
        Accepted,
        Failed,
        Timeout,
        Pending
    }
    public class PaymentAttempt : IAppEntity
    {
        public Guid Id { get;private set; }

        public Guid PaymentId { get;private set; }

        public Provider Provider { get; private set; }

        public int AttemptNumber { get; private set; }

        public AttemptStatus AttemptStatus { get; private set; }

        public string? ErrorMessage { get; private set; }

        public string ProviderTransactionId { get; private set; }   

        public DateTime StartedAt { get; private set; }

        public DateTime? CompletedAt { get; private set; }

        private List<IDomainEvent> _events => new();

        public IReadOnlyCollection<IDomainEvent> Events => _events;

        public PaymentAttempt(Guid paymentId, Provider provider, int attemptNumber, AttemptStatus attemptStatus, string? errorMessage, string providerTransactionId, DateTime startedAt, DateTime? completedAt)
        {
            Id = Guid.NewGuid();
            PaymentId = paymentId;
            Provider = provider;
            AttemptNumber = attemptNumber;
            AttemptStatus = attemptStatus;
            ErrorMessage = errorMessage;
            ProviderTransactionId = providerTransactionId;
            StartedAt = startedAt;
            CompletedAt = completedAt;
        }

        public void MarkTimeOut()
        {
            AttemptStatus = AttemptStatus.Timeout;
            ErrorMessage = "TimeOut";
        }
        public void MarkAccepted(string transactionId)
        {
            AttemptStatus = AttemptStatus.Accepted;
            ProviderTransactionId = transactionId;
        }
        public void MarkFailed(string error)
        {
            AttemptStatus = AttemptStatus.Failed;
            ErrorMessage = error;
        }

        public void ClearEvents()
        {
            _events.Clear();
        }
    }
    
}
