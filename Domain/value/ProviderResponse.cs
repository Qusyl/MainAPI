using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.value
{
    public enum ProviderStatus
    {
        Accepted,
        Pending, 
        Failed, 
        Timeout,
        Unknown
    }
    public record ProviderResponse(ProviderStatus Status, string ErrorCode);

}
