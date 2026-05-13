using Application.Dto;
using Application.Interface.Repository;
using Domain.value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Services
{
    public class AntiFraudCheckService : IAntiFraudCheckService
    {
        public Task<FraudDecision> CheckAsync(TransactionDto transactionDto)
        {
            
        }
    }
}
