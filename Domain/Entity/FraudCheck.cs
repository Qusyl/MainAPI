using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class FraudCheck : IAppEntity
    {
        private readonly List<IDomainEvent> _events = new List<IDomainEvent>();
        public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public string RuleName { get; private set; }

        public string RuleDecision { get; private set; }

        public string? Reason { get; private set; }  

        public DateTime CreatedAt { get; private set; }

        private FraudCheck() { }

        private FraudCheck(Guid userId, string ruleName, string ruleDecision, string? reason )
        {
            Id = Guid.NewGuid();
            RuleName = ruleName;
            RuleDecision = ruleDecision;
            Reason = reason;
            CreatedAt = DateTime.UtcNow;
        }
        public static Result<FraudCheck, EntityError> Create(Guid userId, string ruleName, string ruleDecision, string? reason)
        {
            if(userId == Guid.Empty)
            {
                return Result<FraudCheck, EntityError>.Failure(EntityError.InvalidId);
            }
            if (string.IsNullOrEmpty(ruleName)) {
                return Result<FraudCheck, EntityError>.Failure(EntityError.InvalidName);
            }
            if (string.IsNullOrEmpty(ruleDecision)) {
                return Result<FraudCheck, EntityError>.Failure(EntityError.InvalidDecision);
            }

            var check = new FraudCheck(userId, ruleName, ruleDecision, reason);

            return Result<FraudCheck, EntityError>.Success(check);
        }


        public void ClearEvents()
        {
            _events.Clear();
        }
    }
}
