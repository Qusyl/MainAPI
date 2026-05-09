using Application.Dto;
using Application.Interface.Services;
using Microsoft.AspNetCore.Mvc;

namespace ProviderB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProviderBController : ControllerBase
    {
        private readonly ILogger<ProviderBController> _logger;
        private readonly IProviderService _providerService;

        public ProviderBController(ILogger<ProviderBController> logger, IProviderService serviceProvider)
        {
            _logger = logger;
            _providerService = serviceProvider;
        }
        [HttpGet("check")]
        public IActionResult Check()
        {
            return Ok(new
            {
                Provider = "B",
                Status = "Active"
            });
        }
        [HttpPost("call")]
        public async Task<ActionResult<ProviderApiResponse>> GetResponseAsync([FromBody] PaymentDto paymentDto)
        {
            _logger.LogInformation("ProviderB: Получение запроса...");
            var response = await _providerService.SendAsync(paymentDto);

            if (response == null)
            {
                _logger.LogInformation("ProviderB: Не удалось получить ответ");
                return StatusCode(500,
                    new
                    {
                        Error = "Response is not unreachable"
                    }
                    );
            }
            _logger.LogInformation($"ProviderB: ответ {response.Status}");
            return Ok(response);
        }
    }
}
