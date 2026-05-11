using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class PaymentResponseDto
    {
        public Guid PaymentId { get; set; }
        public string Status { get; set; }
        public string Provider { get; set; }
        public DateTime OccuredOn { get; set; }
    }
}
