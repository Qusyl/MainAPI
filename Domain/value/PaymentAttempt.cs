using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.value
{
    public record PaymentAttempt(Payment Payment, bool IsSucces);
    
}
