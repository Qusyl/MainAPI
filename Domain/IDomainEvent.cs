using Domain.value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IDomainEvent
    {
        string EventType { get;}

        int Version { get;}

        DateTime OccurredOn { get;  }

        List<AttemptInfo> Attempts { get;  }
    }
}
