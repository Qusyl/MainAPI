using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class TransactionDto
    {
        public Guid UserId { get; set; }

        public decimal Amount { get; set; } 

        public string Country { get; set; }

        public string IP {  get; set; }

        public string Device {  get; set; } 


        public TransactionDto(Guid userId, decimal amount, string country, string iP, string device)
        {
            UserId = userId;
            Amount = amount;
            Country = country;
            IP = iP;
            Device = device;
        }
    }
}
