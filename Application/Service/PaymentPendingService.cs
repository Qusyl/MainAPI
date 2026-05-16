using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Services;
using Domain;
using Domain.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class PaymentPendingService : IPaymentPendingService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentPendingService> _logger;
        /// <summary>
        /// По умолчанию пусть будет 15, но если чтт можно поставить больше/меньше
        /// </summary>
        private const int MaxAttempts = 15;

        public PaymentPendingService( IUnitOfWork unitOfWork, ILogger<PaymentPendingService> logger)
        {
          
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<string, ApplicationError>> HandleAsync(Payment payment, CancellationToken cts = default)
        {
            _logger.LogInformation(
             "Processing payment {PaymentId} attempt {Attempt}",
            payment.Id,
            payment.ProcessingAttempts);
            if (payment.Status != PaymentStatus.Pending.ToString())
            {
                return Result<string, ApplicationError>.Success(payment.Status);
            }
            if (DateTime.UtcNow - payment.CreatedAt > TimeSpan.FromMinutes(5))
            {
                _logger.LogInformation("Payment processing time limit reached for payment [{payment}]", payment.Id);
                payment.MarkCancelled();

                await _unitOfWork.SaveChangesAsync(cts);

                return Result<string, ApplicationError>.Success(payment.Status);
            }
            var result = Random.Shared.Next(1, 101);

            payment.IncrementProcessingAttempts();
     
            if (result <= 30)
            {
                payment.MarkAccepted();
            }
            else if(payment.ProcessingAttempts >= MaxAttempts)
            {
                _logger.LogInformation("Payment processing count limit reached for payment [{payment}]", payment.Id);
                payment.MarkCancelled();
            }
            await _unitOfWork.SaveChangesAsync(cts);
            return Result<string, ApplicationError>.Success(payment.Status);
        }
    }
}
