using Application.Dto;
using Application.Interface.Repository;
using Application.Rules;
using Domain;
using Domain.Entity;
using Domain.value;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Services
{
    public class AntiFraudCheckService : IAntiFraudCheckService
    {
       
        private readonly List<IFraudRule> _rules = new List<IFraudRule>();
        private readonly IAntiFraudCheckRepository _repos;


        public IReadOnlyCollection<IFraudRule> Rules => _rules.AsReadOnly();



        public AntiFraudCheckService( IEnumerable<IFraudRule> rules, IAntiFraudCheckRepository repos)
        {
            _rules.AddRange(rules);
            _repos = repos;
        }

        public async Task<Result<FraudDecision, ApplicationError>> CheckAsync(TransactionDto transactionDto)
        {
            foreach (var rule in _rules)
            {
                var result = await rule.CheckAsync(transactionDto);

                var fraudCheck = FraudCheck.Create(transactionDto.UserId, rule.RuleName, result.Decision.ToString(), result.Message);

                if (!fraudCheck.IsSuccess)
                {
                    return Result<FraudDecision, ApplicationError>.Failure(ApplicationError.EntityError);
                }

                if (result.Decision == FraudDecision.Deny)
                {
                    return Result<FraudDecision, ApplicationError>.Success(FraudDecision.Deny);
                }
                else if (result.Decision == FraudDecision.Suspicious)
                {
                    return Result<FraudDecision, ApplicationError>.Success(FraudDecision.Suspicious);
                }
            }

            return  Result<FraudDecision, ApplicationError>.Success(FraudDecision.Allow);
        }
    }
}
