using Application.Interface;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistance
{
    /// <summary>
    /// Аналог MediatR - оповещение ( notification ) для событий
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger<EventPublisher> _logger;
        public EventPublisher(IServiceProvider serviceProvider, ILogger<EventPublisher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task PublishAsync(IDomainEvent @event, CancellationToken cts = default)
        {
           var eventType = @event.GetType();

           if(eventType == null)
            {
                _logger.LogError("Event type is not found", eventType);
                return;
            }

           var handlerType = typeof(IHandler<>).MakeGenericType(eventType);
            if (handlerType == null)
            {
                _logger.LogError("Handler type is not found", handlerType);
                return;
            }

            var handlers = _serviceProvider.GetServices(handlerType);
            var tasks = handlers.Cast<IHandler<IDomainEvent>>().Select(h => h.HandleAsync(@event, cts));

            await Task.WhenAll(tasks);

        }
    }
}
