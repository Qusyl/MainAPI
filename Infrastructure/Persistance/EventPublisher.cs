using Application.Interface;
using Domain;


namespace Infrastructure.Persistance
{
    public class EventPublisher : IEventPublisher
    {
        public Task PublishAsync(IDomainEvent @event, CancellationToken cts = default)
        {
           //Mediatr сделать

        }
    }
}
