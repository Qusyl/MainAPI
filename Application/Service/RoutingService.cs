using Application.Dto;
using Application.Interface;
using Domain;
using Domain.Entity;
using Domain.value;
using Microsoft.Extensions.Logging;

using System.Net.Http.Json;


namespace Application.Service
{
    public class RoutingService : IRoutingService
    {
        private readonly IPaymentRepository _repository;

        private readonly ILogger<RoutingService> _logger;

        private readonly IUnitOfWork _unitOfWork;

        private readonly HttpClient _httpClient;

        private readonly List<Provider> _providers = new List<Provider> { new Provider("A", new Uri("https://localhost:7078/api/provider-a")), new Provider("B", new Uri("https://localhost:7078/api/provider-b")), new Provider("C", new Uri("https://localhost:7078/api/provider-c")) };

        public RoutingService(IPaymentRepository repository,HttpClient httpClient, ILogger<RoutingService> logger, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _logger = logger;
            _httpClient = httpClient;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ApplicationError>> SendAsync(Payment payment)
        {
            var currentProvider = _providers[0];

            while(currentProvider != null)
            {
                var attemp = await CreateAttemptAsync(payment, currentProvider);

                var ProviderResponse = await SendToProviderAsync(currentProvider, payment);
                var decision = await HandleAsync(payment,attemp, ProviderResponse);

                await _unitOfWork.SaveChangesAsync();
                var decisionStatus = decision.Response;

                switch(decisionStatus){
                    case ResponseError.Complete: return Result<ApplicationError>.Success;
                    case ResponseError.WaitForStatusCheck: return Result<ApplicationError>.Success;
                    case ResponseError.RetryForNextProvider:currentProvider = ResolveNextProvider(currentProvider, ProviderResponse);if (currentProvider == null) { payment.MarkCancelled(); return Result<ApplicationError>.Failure(ApplicationError.PaymentCancelled); } break;
                }
            }
            
            return Result<ApplicationError>.Success;
        }
        private async Task<RoutingDecision> HandleAsync(Payment payment,PaymentAttempt attempt ,ProviderResponse response)
        {

            switch (response.Status) {
                case ProviderStatus.Pending: return new RoutingDecision(ResponseError.WaitForStatusCheck);
                case ProviderStatus.Accepted: payment.MarkAccepted(); attempt.MarkAccepted(payment.CurrentProvider.Name); return new RoutingDecision(ResponseError.Complete);
                case ProviderStatus.Failed: attempt.MarkFailed("Attempt is failed");  return new RoutingDecision(ResponseError.RetryForNextProvider);
                case ProviderStatus.Timeout: payment.MarkUnknown(); attempt.MarkTimeOut(); return new RoutingDecision(ResponseError.RetryForNextProvider);
                case ProviderStatus.Unknown:payment.MarkUnknown(); return new RoutingDecision(ResponseError.RetryForNextProvider);
                default: return new RoutingDecision(ResponseError.RetryForNextProvider);
            }
        }
        private async Task<PaymentAttempt> CreateAttemptAsync(Payment payment, Provider provider)
        {
            payment.RegisterAttempt(provider);
            var attempt =  new PaymentAttempt(
                payment.Id, 
                provider,
                payment.Attempts, 
                AttemptStatus.Started,
                default, provider.Name,
                DateTime.UtcNow, 
                default);

            return attempt;
        }
        private async Task<ProviderResponse> SendToProviderAsync(Provider provider, Payment payment)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(provider.Uri, payment);

                if (!response.IsSuccessStatusCode)
                {
                    return new ProviderResponse
                    (ProviderStatus.Failed,
                        $"HTTP {(int)response.StatusCode}");
                }
                var apiResponse = await response.Content.ReadFromJsonAsync<ProviderApiResponse>();
                return new ProviderResponse(MapStatus(apiResponse.Status), apiResponse.ErrorCode);

            }catch(TaskCanceledException ex)
            {
                return new ProviderResponse(
                    ProviderStatus.Timeout,
                    "Timeout"
                    );
            }catch(HttpRequestException ex)
            {
                return new ProviderResponse(
                    ProviderStatus.Failed,
                    $"{ex.Message}"
                    );
            }
        }

        private ProviderStatus MapStatus(string status)
        {
            switch (status)
            {
                case "Accept": return ProviderStatus.Accepted;
                case "Pending": return ProviderStatus.Pending;
                case "Failed": return ProviderStatus.Failed;
                default: return ProviderStatus.Unknown;
            }
        }
        private Provider? ResolveNextProvider(Provider current, ProviderResponse response)
        {
            if(current.Name == _providers[0].Name && (response.Status == ProviderStatus.Timeout || response.Status == ProviderStatus.Failed))
            {
                return _providers[1];
            }
            if (current.Name == _providers[1].Name && (response.Status == ProviderStatus.Timeout || response.Status == ProviderStatus.Failed))
            {
                return _providers[2];
            }
            if(current.Name == _providers[2].Name && (response.Status == ProviderStatus.Timeout || response.Status == ProviderStatus.Failed))
            {
                return null;
            }

            return null;
        }
    }
}
