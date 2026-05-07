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
    public class AlertService : IAlertService
    {
        private readonly ILogger _logger;

        public AlertService(ILogger logger) => _logger = logger;

        public async Task SendAsync(PaymentAlert alert, SecurityStatus status)
        {
            _logger.LogInformation(
                $"{alert.PaymentId} - ID платежа\n{status} - статус безопасности\n{alert.Message} - сообщение об ошибке\n"
                );
        }
    }
}
