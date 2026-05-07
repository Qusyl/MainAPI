using Application.Dto;
using Application.Interface.Services;
using Microsoft.AspNetCore.Mvc;

namespace ProviderA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProviderAController : ControllerBase
    {
        private readonly ILogger<ProviderAController> _logger;
        private readonly IProviderService _providerService;


        public ProviderAController(ILogger<ProviderAController> logger, IProviderService projectService)
        {
            _logger = logger;
            _providerService = projectService;
        }

        [HttpGet("call")]
        public async Task<ActionResult<ProviderApiResponse>> GetResponseAsync([FromBody] PaymentDto paymentDto)
        {
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
            return Ok(response);
        }
    }
}
