using Application.Dto;
using Application.Interface;
using Application.Interface.Repository;
using Domain.value;
using StackExchange.Redis;
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
        private readonly IConnectionMultiplexer _redis;


        public TooManyTransactionsRule(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }
        public async Task<FraudCheckResult> CheckAsync(TransactionDto transactionDto)
        {
          var db = _redis.GetDatabase();

            var key = $"fraud:user:{transactionDto.UserId}:transactions";

            var count = await db.StringGetAsync(key);

            var transactionCount = count.HasValue ? (int)count : 0 ;

            var status = transactionCount switch
            {
                < 3 => new FraudCheckResult(FraudDecision.Allow, RuleName),
                >= 3 and < 5 => new FraudCheckResult(FraudDecision.Suspicious, RuleName, "Needs further verification"),
                >= 5 => new FraudCheckResult(FraudDecision.Deny, RuleName, "Too many transactions detected")
            };

            return status;
        }
    }
}
