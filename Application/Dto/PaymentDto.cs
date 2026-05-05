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

        public Provider Provider { get; set; }

        public PaymentDto(decimal amount, string currency, Provider provider)
        {
            Amount = amount;
            Currency = currency;
            Provider = provider;
        }
            
            
    }
}
