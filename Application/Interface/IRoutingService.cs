using Domain;
using Domain.Entity;


namespace Application.Interface
{
    public interface IRoutingService
    {
        Task<Result<ApplicationError>> SendAsync(Payment payment);
    }
}
