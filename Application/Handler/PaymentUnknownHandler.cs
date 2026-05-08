using Application.Interface;
using Application.Interface.Services;
using Application.Service;
using Domain;
using Domain.Events.Payment;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handler
{
    public class PaymentUnknownHandler : IHandler<PaymentUnknownEvent>
    {
        private readonly ILogger<PaymentUnknownHandler> _logger;

        private readonly IAuditService _audit;

        public PaymentUnknownHandler(ILogger<PaymentUnknownHandler> logger, IAuditService audit)
        {
            _logger = logger;
            _audit = audit;
        }
      public async  Task<Result<Guid,ApplicationError>> HandleAsync(PaymentUnknownEvent @event, CancellationToken cts = default)
        {
            await _audit.SendAsync(new Domain.value.CoordinationTask(@event.PaymentId, @event.ProviderName, DateTime.FromOADate(1).AddHours(5)));
            _logger.LogInformation($"Ошибка была перенаправлена в аудит для решения в ручном режиме");
            return Result<Guid,ApplicationError>.Success(@event.PaymentId);
        }
    }
}
