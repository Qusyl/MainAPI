using Application.Interface;
using Domain;
using Domain.Entity;
using Domain.Events.Payment;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handler
{
    public class PaymentCreateHandler : IHandler<PaymentCreateEvent>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentCompleteHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentCreateHandler(IPaymentRepository paymentRepository, ILogger<PaymentCompleteHandler> logger, IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ApplicationError>> HandleAsync(PaymentCreateEvent @event, CancellationToken cts = default)
        {
            var payment = Payment.Create(@event.Amount, @event.Currency, @event.Provider);
            if (!payment.IsSuccess)
            {
                return Result<ApplicationError>.Failure(ApplicationError.EntityError);
            }
            await _paymentRepository.AddAsync(payment.Value);
            var save = await _unitOfWork.SaveChangesAsync(cts);
            if (!save.IsSuccess)
            {
                return Result<ApplicationError>.Failure(ApplicationError.ConcurrencyError);
            }
            return Result<ApplicationError>.Success;
        }
    }
}
