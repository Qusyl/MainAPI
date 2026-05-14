using Application.Dto;
using Application.Interface;
using Domain.value;
using StackExchange.Redis;


namespace Application.Rules
{
    public class DifferentCountriesRule : IFraudRule
    {
        public string RuleName => "DIFFERENT_COUNTRIES_RULE";

        private readonly IConnectionMultiplexer _redis;

        public DifferentCountriesRule(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<FraudCheckResult> CheckAsync(TransactionDto transactionDto)
        {
            var db = _redis.GetDatabase();

            var key = $"fraud:user_countries:{transactionDto.UserId}";

            var added = await db.SetAddAsync(key, transactionDto.Country);
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

                1 => new FraudCheckResult(FraudDecision.Allow, RuleName),
                2 => new FraudCheckResult(FraudDecision.Suspicious, RuleName, "Needs further verification"),
                >= 3 => new FraudCheckResult(FraudDecision.Deny, RuleName, "Too many countries detected")
            };

            return status;
        }
    }
}
