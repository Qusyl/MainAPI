using Application.Dto;
using Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Rules
{
    public class TooMuchAmountRule : IFraudRule
    {
        public string RuleName => "AMOUNT_RULE";

        public TooMuchAmountRule() { }

        public async Task<FraudCheckResult> CheckAsync(TransactionDto transactionDto)
        {
            FraudCheckResult result = transactionDto.Amount switch
            {
                < 10000 => new FraudCheckResult(Domain.value.FraudDecision.Allow, RuleName),
                >= 10000 and < 300000 => new FraudCheckResult(Domain.value.FraudDecision.Suspicious, RuleName, "Needs further verification"),
                >= 300000 => new FraudCheckResult(Domain.value.FraudDecision.Deny, RuleName, "Transaction amount limit")
            };

            return result;
        }
    }
}
