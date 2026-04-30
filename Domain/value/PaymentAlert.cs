using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.value
{
    public enum SecurityStatus
    {
        low,
        medium,
        high
    }
    public class PaymentAlert
    {
        public Guid PaymentId { get; set; }

    

        public string Message { get; set; } 

        public PaymentAlert(Guid paymentId, string message)
        {
            PaymentId = paymentId;
          
            Message = message;
        }
    }
}
