using Application.Interface.Repository;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Repository
{
    public class ErrorAuditRepository : IAuditRepository
    {
        private readonly AppDbContext _appDbContext;

        public ErrorAuditRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task AddAsync(ErrorAudit errorAudit)
        {
           await _appDbContext.Audits.AddAsync(errorAudit);
        }

        public async Task<ErrorAudit?> GetAsync(Guid Id)
        {
            return await _appDbContext.Audits.FirstOrDefaultAsync(a => a.Id == Id);
        }
    }
}
