using Application.Dto;
using Application.Interface;
using Application.Interface.Services;
using Domain.Entity;
using Domain.Events.Payment;
using Microsoft.AspNetCore.Mvc;

namespace MainAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IRoutingService _routingService;
        private readonly ILogger<PaymentController> _logger;
        private readonly IHandler<PaymentCreateEvent> _createHandler;
        public PaymentController(IRoutingService routingService, ILogger<PaymentController> logger, IHandler<PaymentCreateEvent> createHandler)
        {
            _routingService = routingService;
            _logger = logger;
            _createHandler = createHandler;
        }
        [HttpPost("send")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentDto paymentDto)
        {
            _logger.LogInformation("Обращение к контроллеру Payment!");
            var payment = Payment.Create(paymentDto.Amount, paymentDto.Currency, paymentDto.Provider);
            if (!payment.IsSuccess)
            {
                return StatusCode(
                       500,
                       new
                       {
                           Error = "Failed to create payment",
                           Message = payment.Error
                       }
                   );
            }
           
           var res = await _routingService.SendAsync(payment.Value);
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
            
            return Ok(new
            {
                PaymentId = res.Value.Id,
                Status = res.Value.Status.ToString(),
                OccuredOn = DateTime.UtcNow
            });   
        }
    }
}
