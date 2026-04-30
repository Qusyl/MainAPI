using Application.Interface;
using Application.Service;
using Domain;
using Domain.Events.Payment;
using Microsoft.Extensions.Logging;


namespace Application.Handler
{
    public class PaymentCompleteHandler : IHandler<PaymentCompleteEvent>
    {
        private readonly ILogger<PaymentCompleteHandler> _logger;

        private readonly EmailService _emailService;

        public PaymentCompleteHandler(ILogger<PaymentCompleteHandler> logger, EmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }
        public async Task HandleAsync(PaymentCompleteEvent @event, CancellationToken cts = default)
        {
            await _emailService.SendAsync(new Domain.value.PaymentConfirmation(@event.PaymentId, @event.ProviderName));

            _logger.LogInformation("Чек отправлен на почту");
        }
    }
}
