using Domain;
using Domain.Entity;


namespace Application.Interface.Services
{
    public interface IRoutingService
    {
        Task<Result<Payment, ApplicationError>> SendAsync(Payment payment, Guid userId);
    }
}
