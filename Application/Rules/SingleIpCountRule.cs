using Application.Dto;
using Application.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using Domain.value;

namespace Application.Rules
{
    public class SingleIpCountRule : IFraudRule
    {
        public string RuleName => "SINGLE_IP_COUNT_RULE";
        private readonly IConnectionMultiplexer _redis;

        public SingleIpCountRule(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<FraudCheckResult> CheckAsync(TransactionDto transactionDto)
        {
            var db = _redis.GetDatabase();
            var key = $"fraud:ip:{transactionDto.IP}";

            var currentIncrement = await db.StringIncrementAsync(key);

            if(currentIncrement == 1)
            {
                await db.KeyExpireAsync(key, TimeSpan.FromMinutes(1));
            }

            var status = currentIncrement switch
            {
                < 5 => new FraudCheckResult(FraudDecision.Allow, RuleName),

                >= 5 and < 10 =>new FraudCheckResult(FraudDecision.Suspicious, RuleName, "Needs further verification"),
                >= 10 => new FraudCheckResult(FraudDecision.Deny, RuleName, $"Too many attempts from IP:{transactionDto.IP}")
            };

            return status;
        }
    }
}
