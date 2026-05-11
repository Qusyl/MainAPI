using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Services;
using Domain;
using Domain.Entity;
using Domain.value;
using Microsoft.Extensions.Logging;


namespace Application.Service
{
    public class ManualErrorFixAuditService : IAuditService
    {
        private readonly ILogger<ManualErrorFixAuditService> _logger;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IAuditRepository _auditRepository;

        private readonly IPaymentRepository _paymentRepository;

        public ManualErrorFixAuditService(ILogger<ManualErrorFixAuditService> logger, IUnitOfWork unitOfWork, IAuditRepository auditRepository, IPaymentRepository paymentRepository)
        {
            _logger = logger;
            _auditRepository = auditRepository;
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ApplicationError>> SendAsync(CoordinationTask task)
        {
            var payment = await _paymentRepository.GetAsync(task.PaymentId);
            if (payment == null)
            {
                _logger.LogError($"Возникла не предвиденная ошибка - Audit [ ID = {task.PaymentId} ]");
                return Result<ApplicationError>.Failure(ApplicationError.EntityError);
            }
            var audit = ErrorAudit.Create(task.PaymentId, payment.Status, DateTime.UtcNow);
            if (!audit.IsSuccess)
            {
                _logger.LogError($"Возникла не предвиденная ошибка, при добавлении сущности - Audit [ ID = {task.PaymentId} ]");
                return Result<ApplicationError>.Failure(ApplicationError.EntityError);
            }
            await _auditRepository.AddAsync(audit.Value);

            var save =await _unitOfWork.SaveChangesAsync();

            if (!save.IsSuccess)
            {
                _logger.LogError($"Возникла не предвиденная ошибка, при сохранении сущности - Audit [ ID = {task.PaymentId} ]");
                return Result<ApplicationError>.Failure(ApplicationError.ConcurrencyError);
            }
            _logger.LogError($"Ошибка успешно сохранена в аудит [ {audit.Value.Id} ]");

            return Result<ApplicationError>.Success;
        }
    }
}
