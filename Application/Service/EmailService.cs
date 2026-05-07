using Application.Interface.Services;
using Domain.value;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    /// <summary>
    /// Класс-имитация сервиса для отправки чека при успешном платеже
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger) => _logger = logger;
        
        public async Task SendAsync(PaymentConfirmation confirmation)
        {
            _logger.LogInformation(
                $"{confirmation.PaymentId} - ID платежа\n{confirmation.ProviderName} - провайдер\n{confirmation.ProcessedAt} - дата обработки\n"
                );
        }


    }
}
