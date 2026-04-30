using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.value
{
    public class AttemptInfo
    {
        public string ProviderName { get; set; }

        public string Status { get; set; }

        public string? Error { get; set; }

        public AttemptInfo(string providerName, string status, string error) { ProviderName = providerName; Status = status; Error = error; }   
    }
}
