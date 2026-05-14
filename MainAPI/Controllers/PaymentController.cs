using Application.Dto;
using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Services;
using Domain.Entity;
using Domain.Events.Payment;
using Domain.value;
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
        private readonly IAntiFraudTrackingService _antiFraudTrackingService;
        private readonly IAntiFraudCheckService _antiFraudService;
        private readonly ILogger<PaymentController> _logger;
        private readonly IHandler<PaymentCreateEvent> _createHandler;
        private readonly IHttpClientFactory _httpClientFactory;
        /// <summary>
        /// Не совсем красиво и корректно, но в данной реализации было удобно и достаточно использовать IPaymentRepository в контроллере, вместо созданий отдельного IQuery + Query<GetById>
        /// </summary>
        private readonly IPaymentRepository _paymentRepository;
        
        public PaymentController(IRoutingService routingService, IPaymentRepository paymentRepository, ILogger<PaymentController> logger, IHandler<PaymentCreateEvent> createHandler,IAntiFraudTrackingService antiFraudTrackingService, IHttpClientFactory httpClientFactory, IAntiFraudCheckService atifraudService)
        {
            _routingService = routingService;
            _logger = logger;
            _createHandler = createHandler;
            _paymentRepository = paymentRepository;
            _antiFraudTrackingService = antiFraudTrackingService;
            _httpClientFactory = httpClientFactory;
            _antiFraudService = atifraudService;
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
            ///<summary>
            ///Т.К Тестирование программы проходит на Docker для ClientIp поставлена заглушка, потому что не будет реального отображения страны! 
            ///МОЖНО УБРАТ ВООБЩЕМ
            ///</summary>
            //var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString(); - если нужен реальный кейс
            var clientIp = "8.8.8.8";
            var clientCountry = "Unknow";
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetFromJsonAsync<IpWhoIsResponseDto>($"https://ipwho.is/{clientIp}");
                if (response != null && response.IsSuccess)
                {
                    clientCountry = response.Country;
                }
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to get response from IPWhoIs");
            }
            var transaction = new TransactionDto(userId, paymentDto.Amount, clientCountry, clientIp ?? "Unknown", Request.Headers["User-Agent"].ToString());
            var antiFraudDecision = await _antiFraudService.CheckAsync(transaction);
            if (!antiFraudDecision.IsSuccess)
            {
                return StatusCode(500,

                    new {
                    Error = antiFraudDecision.Error,
                    Message = "Failed to get antifraud decision"
                    }
                    );
            }
            if(antiFraudDecision.Value == FraudDecision.Suspicious)
            {
                return StatusCode(409,
                    new
                    {
                        Error = "Transaction need further verification",
                        Decision = "Suspicious"
                    });
            }
            if(antiFraudDecision.Value == FraudDecision.Deny)
            {
                return StatusCode(403,
 
                    new {
                    Error = "Fraud detected",
                    Decision = "Deny"
                    }
                    );
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
           var res = await _routingService.SendAsync(payment, userId);
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
            await _antiFraudTrackingService.RegisterTransactionAttemptAsync(userId);
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
