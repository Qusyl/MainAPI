using Application.Dto;
using Application.Interface;
using Application.Interface.Repository;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Rules
{
    public class TooManyDeclinesRule : IFraudRule
    {
        public string RuleName => "TOO_MANY_DECLINES_RULE";

        private readonly IConnectionMultiplexer _redis;
        public TooManyDeclinesRule(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }
        public async Task<FraudCheckResult> CheckAsync(TransactionDto transactionDto)
        {

            var db =  _redis.GetDatabase();

            var key = $"fraud:user:{transactionDto.UserId}:declines";

            var count = await db.StringGetAsync(key);

           int declineCount = count.HasValue ? (int)count : 0 ;

            var status = declineCount switch
            {
                < 3 => new FraudCheckResult(Domain.value.FraudDecision.Allow, RuleName),
                >= 3 and < 5 => new FraudCheckResult(Domain.value.FraudDecision.Suspicious, RuleName, "Needs further verification"),
                >= 5 => new FraudCheckResult(Domain.value.FraudDecision.Deny, "Too many declines")
            };

            return status;

        }
    }
}
