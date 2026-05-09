using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class ErrorAudit : IAppEntity
    {
        private readonly List<IDomainEvent> _events = new();

        public Guid Id { get; private set; }

        public Guid PaymentId { get; private set; } 

        public DateTime OccuredOn { get; private set; }
        public string Status { get; private set; }

        public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();
        private ErrorAudit(Guid paymentId, string status, DateTime occuredOn)
        {
            Id = Guid.NewGuid();
            PaymentId = paymentId;
            Status = status;
            OccuredOn = occuredOn;
        }

        private ErrorAudit() { }

        public static Result<ErrorAudit,EntityError> Create(Guid paymentId, string status, DateTime occuredOn)
        {
            if(paymentId == Guid.Empty)
            {
                return Result<ErrorAudit, EntityError>.Failure(EntityError.InvalidId);
            }
            else if (string.IsNullOrEmpty(status))
            {
                return Result<ErrorAudit, EntityError>.Failure(EntityError.InvalidStatus);
            }
            var audit = new ErrorAudit(paymentId, status, occuredOn);

            return Result<ErrorAudit, EntityError>.Success(audit);
        }
        public void ClearEvents()
        {
         _events.Clear();   
        }
    }
}
