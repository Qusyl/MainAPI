using Application.Interface.Repository;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class AntiFraudTrackingService : IAntiFraudTrackingService
    {
        private readonly IConnectionMultiplexer _redis;

        public AntiFraudTrackingService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }
        public async Task RegisterTransactionAttemptAsync(Guid userId)
        {
            var db = _redis.GetDatabase();
            var key = $"fraud:user:{userId}:transactions";
            var count = await db.StringIncrementAsync(key);
            if(count == 1)
            {
                await db.KeyExpireAsync(key, TimeSpan.FromMinutes(1));
            }
        }

        public async Task RegisterTransactionDeclineAsync(Guid userId)
        {
            var db = _redis.GetDatabase();

            var key = $"fraud:user:{userId}:declines";

            var count = await db.StringIncrementAsync(key);
            if (count == 1)
            {
                await db.KeyExpireAsync(key, TimeSpan.FromMinutes(10));
            }
        }
    }
}
