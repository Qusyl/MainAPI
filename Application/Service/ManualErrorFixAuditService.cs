using Domain.value;
using Microsoft.Extensions.Logging;


namespace Application.Service
{
    public class ManualErrorFixAuditService
    {
        private readonly ILogger _logger;

        public ManualErrorFixAuditService(ILogger logger) => _logger = logger;

        public async Task SendAsync(CoordinationTask task)
        {
            _logger.LogInformation(
                $"{task.PaymentId} - ID платежа\n{task.ProviderName} - провайдер\n{task.RetryExecutionDate} - дата повторной обработки\n"
                );
        }
    }
}
