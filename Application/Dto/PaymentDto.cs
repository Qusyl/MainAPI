using Domain.value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class PaymentDto
    {
        public decimal Amount { get; set; }

        public string Currency {  get; set; }

        public string Provider { get; set; }

        public string IdempotencyKey {get; set; }

        public PaymentDto(decimal amount, string currency, string provider, string idempotencyKey)
        {
            Amount = amount;
            Currency = currency;
            Provider = provider;
            IdempotencyKey = idempotencyKey;
        }
            
            
    }
}
