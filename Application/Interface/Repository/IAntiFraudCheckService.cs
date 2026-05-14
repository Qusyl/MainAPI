using Application.Dto;
using Domain;
using Domain.value;


namespace Application.Interface.Repository
{
    public interface IAntiFraudCheckService
    {
        IReadOnlyCollection<IFraudRule> Rules { get; }
        Task<Result<FraudDecision, ApplicationError>> CheckAsync(TransactionDto transactionDto);
    }
}
