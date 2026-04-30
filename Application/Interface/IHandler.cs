using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IHandler<IDomainEvent>
    {
        Task HandleAsync(IDomainEvent @event, CancellationToken cts = default);
    }
}
