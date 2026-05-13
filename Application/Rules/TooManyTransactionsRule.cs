using Application.Dto;
using Application.Interface;
using Application.Interface.Repository;
using Domain.value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Rules
{
    public class TooManyTransactionsRule : IFraudRule
    {
        public string RuleName => "AMOUNT_OF_TRANSACTION_RULE";
        private readonly IPaymentAttemptRepository _attempts;

        public TooManyTransactionsRule(IPaymentAttemptRepository attempts)
        {
            _attempts = attempts;
        }
        public async Task<FraudCheckResult> CheckAsync(TransactionDto transactionDto)
        {
           var userAttempts = await _attempts.GetByUserIdAsync(transactionDto.UserId);

            if (userAttempts == null) {
                return new FraudCheckResult(FraudDecision.Allow, RuleName);
            }
            DateTime currentTime = DateTime.UtcNow;
            var offsetOfTimeLimit = currentTime.AddMinutes(-5);
            var check = userAttempts.Where(a => a.StartedAt >= offsetOfTimeLimit).Select(a => a.StartedAt).ToList() ;
            if(check.Count > 5)
            {
                return new FraudCheckResult(FraudDecision.Deny, RuleName, "Transactions limit has been exceeded");
            }
            return new FraudCheckResult(FraudDecision.Allow, RuleName);
        }
    }
}
