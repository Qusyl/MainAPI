using Application.Dto;
using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Services;
using Domain.Entity;
using Domain.Events.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MainAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IRoutingService _routingService;
        private readonly ILogger<PaymentController> _logger;
        private readonly IHandler<PaymentCreateEvent> _createHandler;
        /// <summary>
        /// Не совсем красиво и корректно, но в данной реализации было удобно и достаточно использовать IPaymentRepository в контроллере, вместо созданий отдельного IQuery + Query<GetById>
        /// </summary>
        private readonly IPaymentRepository _paymentRepository;
        public PaymentController(IRoutingService routingService, IPaymentRepository paymentRepository, ILogger<PaymentController> logger, IHandler<PaymentCreateEvent> createHandler)
        {
            _routingService = routingService;
            _logger = logger;
            _createHandler = createHandler;
            _paymentRepository = paymentRepository;
        }
        [HttpPost("send")]
        public async Task<ActionResult<PaymentResponseDto>> CreatePayment([FromBody] PaymentDto paymentDto)
        {
            _logger.LogInformation("Обращение к контроллеру Payment!");
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var existing = await _paymentRepository.GetByIdempotencyAsync(paymentDto.IdempotencyKey);
            if (existing != null) {
                return Ok(new PaymentResponseDto
                {
                    PaymentId = existing.Id,
                    Status = existing.Status.ToString(),
                    Provider = existing.CurrentProvider,
                    OccuredOn = DateTime.UtcNow
                });
            }
            var handle = await _createHandler.HandleAsync(new PaymentCreateEvent(DateTime.UtcNow, paymentDto.Amount, paymentDto.Currency, paymentDto.Provider, paymentDto.IdempotencyKey, userId));
            if (!handle.IsSuccess)
            {
             
                return StatusCode(
                        500,
                        new
                        {
                            Error = $"{handle.Error}",
                            Message = "Failed to handle payment"
                        }
                    );
            }
            var payment = await _paymentRepository.GetAsync(handle.Value);
            if(payment == null)
            {
                return StatusCode(
                       500,
                       new
                       {
                           Error = $"Payment create failure",
                           Message = "Payment wasn't been created"
                       }
                   );
            }
           var res = await _routingService.SendAsync(payment);
            if (!res.IsSuccess) {
                _logger.LogError($"{res.Error}");
                return StatusCode(
                        500,
                        new
                        {
                            Error = "Failed to send payment",
                            Message = res.Error
                        }
                    );
            }
            _logger.LogInformation("Запрос к провайдеру создан, идёт обработка...");

            return Ok(new PaymentResponseDto
            {
                PaymentId = res.Value.Id,
                Status = res.Value.Status.ToString(),
                Provider = res.Value.CurrentProvider,
                OccuredOn = DateTime.UtcNow
            });
        }
    }
}
