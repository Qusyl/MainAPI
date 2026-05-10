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
        private readonly ILogger<PaymentCreateHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentCreateHandler(IPaymentRepository paymentRepository, ILogger<PaymentCreateHandler> logger, IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid,ApplicationError>> HandleAsync(PaymentCreateEvent @event, CancellationToken cts = default)
        {
            var exists = await _paymentRepository.GetByIdempotencyAsync(@event.IdempotencyKey);
            if (exists != null)
            {
                if (exists.Status == PaymentStatus.Accepted.ToString())
                    return Result<Guid, ApplicationError>.Success(exists.Id);

                if (exists.Status == PaymentStatus.Pending.ToString())
                    return Result<Guid, ApplicationError>.Success(exists.Id);

                if (exists.Status == PaymentStatus.Unknown.ToString())
                    return Result<Guid, ApplicationError>.Success(exists.Id);
            }
            var payment = Payment.Create(@event.Amount, @event.Currency, @event.Provider, @event.IdempotencyKey);
            if (!payment.IsSuccess)
            {
                return Result<Guid,ApplicationError>.Failure(ApplicationError.EntityError);
            }
            await _paymentRepository.AddAsync(payment.Value);
            var save = await _unitOfWork.SaveChangesAsync(cts);
            if (!save.IsSuccess) 
            {
                return Result<Guid, ApplicationError>.Failure(ApplicationError.ConcurrencyError);
            }
            return Result<Guid, ApplicationError>.Success(payment.Value.Id);
        }
    }
}
