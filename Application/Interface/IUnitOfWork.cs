using Domain;


namespace Application.Interface
{
    public interface  IUnitOfWork
    {
        Task<Result<ApplicationError>> SaveChangesAsync(CancellationToken cts = default); 
    }
}
