using Application.Dto;
using Application.Interface.Services;
using Microsoft.AspNetCore.Mvc;

namespace ProviderC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProviderCController : ControllerBase
    {
        private readonly ILogger<ProviderCController> _logger;
        private readonly IProviderService _providerService;

        public ProviderCController(ILogger<ProviderCController> logger, IProviderService serviceProvider)
        {
            _logger = logger;
            _providerService = serviceProvider;
        }
        [HttpGet("check")]
        public IActionResult Check()
        {
            return Ok(new
            {
                Provider = "C",
                Status = "Active"
            });
        }
        [HttpPost("call")]
        public async Task<ActionResult<ProviderApiResponse>> GetResponseAsync([FromBody] PaymentDto paymentDto)
        {
            _logger.LogInformation("ProviderC: Получение запроса...");
            var response = await _providerService.SendAsync(paymentDto);

            if (response == null)
            {
                _logger.LogInformation("ProviderC: Не удалось получить ответ");
                return StatusCode(500,
                    new
                    {
                        Error = "Response is not unreachable"
                    }
                    );
            }
            _logger.LogInformation("ProviderC: Ответ получен!");
            return Ok(response);
        }
    }
}
