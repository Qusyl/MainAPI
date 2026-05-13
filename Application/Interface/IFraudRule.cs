using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IFraudRule
    {
        string RuleName { get; }
        Task<FraudCheckResult> CheckAsync(TransactionDto transactionDto);
    }
}
