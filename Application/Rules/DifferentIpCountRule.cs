using Application.Dto;
using Application.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Rules
{
    public class DifferentIpCountRule : IFraudRule
    {
        public string RuleName => "DIFFERENT_IP_COUNT_RULE";
        private readonly IConnectionMultiplexer _redis;

        public DifferentIpCountRule(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<FraudCheckResult> CheckAsync(TransactionDto transactionDto)
        {
            var db = _redis.GetDatabase();
            var key = $"fraud:user_ips:{transactionDto.UserId}";

            await db.SetAddAsync(key, transactionDto.IP);

            var count = await db.StringIncrementAsync(key);
            if(count == 1)
            {
                await db.KeyExpireAsync(key, TimeSpan.FromMinutes(5));
            }
          

            var uniques = await db.SetLengthAsync(key);

            var status = uniques switch
            {
                1 => new FraudCheckResult(Domain.value.FraudDecision.Allow, RuleName),
                > 1 and < 5 => new FraudCheckResult(Domain.value.FraudDecision.Suspicious, RuleName, "Needs further verification"),
                > 5 => new FraudCheckResult(Domain.value.FraudDecision.Deny, RuleName, "Too many IPs"),
            };

            return status;
        }
    }
}
