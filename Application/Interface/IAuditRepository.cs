using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IAuditRepository
    {
        Task AddAsync(Domain.Entity.ErrorAudit errorAudit);

        Task<Domain.Entity.ErrorAudit> GetAsync(Guid Id);
        
    }
}
