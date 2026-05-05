using Domain;
using Domain.Entity;


namespace Application.Interface
{
    public interface IRoutingService
    {
        Task<Result<Payment,ApplicationError>> SendAsync(Payment payment);
    }
}
