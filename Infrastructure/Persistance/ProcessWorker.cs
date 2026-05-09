using System.Text.Json;
using Application.Interface;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistance
{
    public class ProcessWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger<ProcessWorker> _logger;
        public ProcessWorker(IServiceProvider serviceProvider, ILogger<ProcessWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessOutboxMessage(stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);   
            }
        }

        private async Task ProcessOutboxMessage(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
            var count = await context.OutBoxMessages.CountAsync(m => m.ProcessedOn == null);
            _logger.LogInformation("Found {Count} unprocessed messages", count);
            var messages = await context.OutBoxMessages
                .Where(m => m.ProcessedOn == null)
                .OrderBy(m => m.OccurredOn)
                .Take(20)
                .ToListAsync();
            _logger.LogInformation($"BackroundServiceError: checking the messages...");
            foreach (var message in messages)
            {
                try
                {
                    var type = Type.GetType(message.Type);
                    if (type == null)
                    {

                        throw new Exception("Type is not found");
                    }
                    var domainEvent = (IDomainEvent)JsonSerializer.Deserialize(message.Payload, type)!;
                    await publisher.PublishAsync(domainEvent, stoppingToken);
                   
                    message.MarkProcessed(DateTime.UtcNow);
                    _logger.LogInformation($"BackroundServiceError: message {message.Id} was processed ");
                }
                catch (Exception ex) {
                    message.SetError(ex.Message);
                    _logger.LogError($"BackroundServiceError: {ex.Message} - {message.Id}");
                }
                await context.SaveChangesAsync(stoppingToken);
            }
        }
    }
}
