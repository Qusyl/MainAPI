using Application.Dto;
using Domain.value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Application.Interface.Repository
{
    public interface IAntiFraudCheckService
    {
        Task<FraudDecision> CheckAsync(TransactionDto transactionDto);
    }
}
