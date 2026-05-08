using Application.Interface;
using Application.Interface.Services;
using Application.Service;
using Domain;
using Domain.Events.Payment;
using Microsoft.Extensions.Logging;


namespace Application.Handler
{
    public class PaymentCancelledHandler : IHandler<PaymentCancelledEvent>
    {
        private readonly ILogger<PaymentCancelledHandler> _logger;

        private readonly IAlertService _alertService;

        public PaymentCancelledHandler(ILogger<PaymentCancelledHandler> logger, IAlertService alert)
        {
            _logger = logger;
            _alertService = alert;
        }
        public async Task<Result<Guid,ApplicationError>> HandleAsync(PaymentCancelledEvent @event, CancellationToken cts = default)
        {
            await _alertService.SendAsync(new Domain.value.PaymentAlert(@event.PaymentId, @event.Message), Domain.value.SecurityStatus.high);
            _logger.LogInformation("Отчёт о проблеме был отправлен");
            return Result<Guid,ApplicationError>.Success(@event.PaymentId);
        }
    }
}
