using Application.Interface;
using Application.Interface.Services;
using Application.Service;
using Domain;
using Domain.Events.Payment;
using Microsoft.Extensions.Logging;


namespace Application.Handler.Payment
{
    public class PaymentCompleteHandler : IHandler<PaymentCompleteEvent>
    {
        private readonly ILogger<PaymentCompleteHandler> _logger;

        private readonly IEmailService _emailService;

        public PaymentCompleteHandler(ILogger<PaymentCompleteHandler> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }
        public async Task<Result<Guid,ApplicationError>> HandleAsync(PaymentCompleteEvent @event, CancellationToken cts = default)
        {
            await _emailService.SendAsync(new Domain.value.PaymentConfirmation(@event.PaymentId, @event.ProviderName));

            _logger.LogInformation("Чек отправлен на почту");
            return Result<Guid,ApplicationError>.Success(@event.PaymentId);
        }
    }
}
