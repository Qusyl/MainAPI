using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class IpWhoIsResponseDto
    {
        public bool IsSuccess { get; set; }

        public string Country { get; set; } = string.Empty;

        public string CountryCode { get; set; } = string.Empty;

        public string PublicIp { get; set; } = string.Empty;

    }
}
