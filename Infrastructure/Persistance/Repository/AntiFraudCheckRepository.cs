using Application.Interface.Repository;
using Domain.Entity;

namespace Infrastructure.Persistance.Repository
{
    public class AntiFraudCheckRepository : IAntiFraudCheckRepository
    {
        private readonly AppDbContext _appDbContext;

        public AntiFraudCheckRepository(AppDbContext context)
        {
            _appDbContext = context;
        }
        public async Task AddAsync(FraudCheck check)
        {
           await _appDbContext.Checks.AddAsync(check);
        }
    }
}
