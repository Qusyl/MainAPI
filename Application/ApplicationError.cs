using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class ApplicationError
    {
        public string Message { get; set; }

        private ApplicationError(string message) => Message = message;

        public static ApplicationError BadAttemptError => new ApplicationError("Attempt creating error"); 
        public static ApplicationError PaymentCancelled => new ApplicationError("Payment is cancelled"); 
        public static ApplicationError ConcurrencyError => new ApplicationError("Concurrency error"); 
    }
}
