using Application.Interface;
using Application.Service;
using Domain;
using Domain.Events.Payment;
using Microsoft.Extensions.Logging;


namespace Application.Handler
{
    public class PaymentCancelledHandler : IHandler<PaymentCancelledEvent>
    {
        private readonly ILogger<PaymentCancelledHandler> _logger;

        private readonly AlertService _alertService;

        public PaymentCancelledHandler(ILogger<PaymentCancelledHandler> logger, AlertService alert)
        {
            _logger = logger;
            _alertService = alert;
        }
        public async Task<Result<ApplicationError>> HandleAsync(PaymentCancelledEvent @event, CancellationToken cts = default)
        {
            await _alertService.SendAsync(new Domain.value.PaymentAlert(@event.PaymentId, @event.Message), Domain.value.SecurityStatus.high);
            _logger.LogInformation("Отчёт о проблеме был отправлен");
            return Result<ApplicationError>.Success;
        }
    }
}
