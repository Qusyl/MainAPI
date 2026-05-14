using Application.Dto;
using Application.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Rules
{
    public class DifferentDevicesRule : IFraudRule
    {
        public string RuleName => "DIFFERENT_DEVICE_RULE";
        private readonly IConnectionMultiplexer _redis;

        public DifferentDevicesRule(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }
        public async Task<FraudCheckResult> CheckAsync(TransactionDto transactionDto)
        {

            var db = _redis.GetDatabase();

            var key = $"fraud:user_devices:{transactionDto.UserId}";

            var added = await db.SetAddAsync(key, transactionDto.Device);

            if (added)
            {
                var ttl = await db.KeyTimeToLiveAsync(key);
                if(ttl == null)
                {
                    await db.KeyExpireAsync(key, TimeSpan.FromMinutes(10));
                }
            }
           
            var uniques = await db.SetLengthAsync(key);

            var status = uniques switch
            {
                < 3 => new FraudCheckResult(Domain.value.FraudDecision.Allow, RuleName),
                >= 3 and < 5 => new FraudCheckResult(Domain.value.FraudDecision.Suspicious, RuleName, "Needs further verification"),
                >= 5 => new FraudCheckResult(Domain.value.FraudDecision.Deny, RuleName, "Too many devices detected")
            };

            return status;
        }
    }
}
