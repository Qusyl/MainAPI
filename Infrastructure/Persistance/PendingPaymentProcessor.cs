using Application.Interface.Services;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance
{
    public class PendingPaymentProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PendingPaymentProcessor> _logger;
        public PendingPaymentProcessor(IServiceProvider serviceProvider, ILogger<PendingPaymentProcessor> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) {

                await ProcessPayments(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task ProcessPayments(CancellationToken cts = default)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var pendingService = scope.ServiceProvider.GetRequiredService<IPaymentPendingService>();

            var payments = await context.Payments.Where(p => p.Status == PaymentStatus.Pending.ToString())
                .OrderBy(p => p.CreatedAt)
                .Take(20)
                .ToListAsync();
            foreach (var payment in payments) {
                try
                {
                    var pendingResult = await pendingService.HandleAsync(payment, cts);

                    if (!pendingResult.IsSuccess)
                    {

                        throw new Exception($"{pendingResult.Error}");
                    }
                    _logger.LogInformation($"Payment[{payment.Id}] processed successfully");
                }
                catch (Exception ex) {
                    _logger.LogError(ex, $"{ex.Message}");
                }
            }
            

        }
    }
}
