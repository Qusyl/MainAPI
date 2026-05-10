using Application.Dto;
using Application.Interface.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace ProviderA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProviderAController : ControllerBase
    {
        private readonly ILogger<ProviderAController> _logger;
        private readonly IProviderService _providerService;
        private readonly ConcurrentDictionary<string, ProviderApiResponse> _idempotencyDictionary = new();
        public ProviderAController(ILogger<ProviderAController> logger, IProviderService projectService)
        {
            _logger = logger;
            _providerService = projectService;
        }
        [HttpGet("check")]
        public IActionResult Check() {
            return Ok(new {
               Provider = "A",
               Status = "Active"
            });
        }
        [HttpPost("call")]
        public async Task<ActionResult<ProviderApiResponse>> GetResponseAsync([FromBody] PaymentDto paymentDto)
        {
            _logger.LogInformation("ProviderA: Попытка восстановления платежа...");

            if(_idempotencyDictionary.TryGetValue(paymentDto.IdempotencyKey, out var existing)){
                _logger.LogInformation($"ProviderA: Платеж {paymentDto.IdempotencyKey} уже обрабатывается!");
                return Ok(existing);
            }

            _logger.LogInformation("ProviderA: Получение запроса...");
            var response = await _providerService.SendAsync(paymentDto);

          
            if (response == null)
            {
                _logger.LogInformation("ProviderA: Не удалось получить ответ");
                return StatusCode(500,
                    new
                    {
                        Error = "Response is not unreachable"
                    }
                    );
            }

            _logger.LogInformation("ProviderA: Ответ получен!");

            _logger.LogInformation("ProviderA: Кэширование...");
            _idempotencyDictionary[paymentDto.IdempotencyKey] = response;
            _logger.LogInformation("ProviderA: Сохранение завершено!");
            return Ok(response);
        }
    }
}
