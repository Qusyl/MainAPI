using Application.Dto;
using Application.Interface;
using Application.Interface.Services;
using Domain;
using Domain.Entity;
using Domain.Events.Payment;
using Domain.value;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net.Http.Json;


namespace Application.Service
{
    public class RoutingService : IRoutingService
    {
        private readonly IPaymentRepository _repository;
        private readonly IPaymentAttemptRepository _attemptRepository;
        private readonly IHandler<PaymentCancelledEvent> _paymentCancelHandler;
        private readonly IHandler<PaymentCompleteEvent> _paymentCompleteHandler;
        private readonly IHandler<PaymentUnknownEvent> _paymentUnknownHandler;

        private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> _lock = new();
        private readonly ILogger<RoutingService> _logger;

        private readonly IUnitOfWork _unitOfWork;

        private readonly HttpClient _httpClient;

        private readonly List<Provider> _providers = new List<Provider>
{
    new Provider("A", new Uri("http://providera:8080/api/ProviderA/call")),
    new Provider("B", new Uri("http://providerb:8080/api/ProviderB/call")),
    new Provider("C", new Uri("http://providerc:8080/api/ProviderC/call"))
};

        public RoutingService(IPaymentRepository repository, IHandler<PaymentCancelledEvent> paymentCancelHandler, IHandler<PaymentCompleteEvent> paymentCompleteHandler, IHandler<PaymentUnknownEvent> paymentUnknownHandler, IPaymentAttemptRepository attemptRepos, HttpClient httpClient, ILogger<RoutingService> logger, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _attemptRepository = attemptRepos;
            _paymentCancelHandler = paymentCancelHandler;
            _paymentCompleteHandler = paymentCompleteHandler;
            _paymentUnknownHandler = paymentUnknownHandler;
            _logger = logger;
            _httpClient = httpClient;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Payment,ApplicationError>> SendAsync(Payment payment)
        {
            var semaphore = _lock.GetOrAdd(payment.Id, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();
            try
            {
                var currentProvider = _providers[0];
                var attemptNum = 0;

                while (currentProvider != null)
                {
                    attemptNum++;
                    var attemp = await CreateAttemptAsync(payment, currentProvider, attemptNum);

                    var ProviderResponse = await SendToProviderAsync(currentProvider, payment);
                    var decision = await HandleAsync(payment, attemp, ProviderResponse);

                    await _unitOfWork.SaveChangesAsync();
                    var decisionStatus = decision.Response;

                    switch (decisionStatus)
                    {
                        case ResponseError.Complete: await _paymentCompleteHandler.HandleAsync(new PaymentCompleteEvent(DateTime.UtcNow, payment.Attempts, payment.Id, payment.CurrentProvider)); return Result<Payment, ApplicationError>.Success(payment);
                        case ResponseError.WaitForStatusCheck: return Result<Payment, ApplicationError>.Success(payment);
                        case ResponseError.RetryForNextProvider: currentProvider = ResolveNextProvider(currentProvider, ProviderResponse); if (currentProvider == null) { payment.MarkCancelled(); return Result<Payment, ApplicationError>.Failure(ApplicationError.PaymentCancelled); } break;
                    }
                }
                if (payment.Status == "Unknown")
                {
                    await _paymentUnknownHandler.HandleAsync(new PaymentUnknownEvent(DateTime.UtcNow, payment.Attempts, payment.Id));
                }
                else
                {
                    await _paymentCancelHandler.HandleAsync(new PaymentCancelledEvent(DateTime.UtcNow, payment.Attempts, payment.Id));
                }
                    return Result<Payment, ApplicationError>.Failure(ApplicationError.BadAttemptError);
            }
            finally
            {
                semaphore.Release();
                _lock.TryRemove(payment.Id, out _);
            }
            
        }
        private async Task<RoutingDecision> HandleAsync(Payment payment,PaymentAttempt attempt ,ProviderResponse response)
        {

            switch (response.Status) {
                case ProviderStatus.Pending: return new RoutingDecision(ResponseError.WaitForStatusCheck);
                case ProviderStatus.Accepted: payment.MarkAccepted(); attempt.MarkAccepted(payment.CurrentProvider); return new RoutingDecision(ResponseError.Complete);
                case ProviderStatus.Failed: attempt.MarkFailed("Attempt is failed");  return new RoutingDecision(ResponseError.RetryForNextProvider);
                case ProviderStatus.Timeout: payment.MarkUnknown(); attempt.MarkTimeOut(); return new RoutingDecision(ResponseError.RetryForNextProvider);
                case ProviderStatus.Unknown:payment.MarkUnknown(); return new RoutingDecision(ResponseError.RetryForNextProvider);
                default: return new RoutingDecision(ResponseError.RetryForNextProvider);
            }
        }
        private async Task<PaymentAttempt> CreateAttemptAsync(Payment payment, Provider provider, int attemptNum)
        {
           
            var attempt =  new PaymentAttempt(
                payment.Id, 
                provider.Name,
                attemptNum,
                AttemptStatus.Started.ToString(),
                default, provider.Name,
                DateTime.UtcNow, 
                default);
            payment.RegisterAttempt(provider.Name, attempt);

            await _attemptRepository.AddAsync(attempt);

            await _unitOfWork.SaveChangesAsync();

            return attempt;
        }
        private async Task<ProviderResponse> SendToProviderAsync(Provider provider, Payment payment)
        {
            try
            {
                var paymentDto = new PaymentDto(payment.Amount, payment.Currency, payment.CurrentProvider);
                var response = await _httpClient.PostAsJsonAsync(provider.Uri, paymentDto);

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
                _logger.LogWarning(ex, "Timeout  provider {Provider}", provider.Name);
                return new ProviderResponse(
                    ProviderStatus.Timeout,
                    "Timeout"
                    );
            }catch(HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP provider {Provider}", provider.Name);
                return new ProviderResponse(
                    ProviderStatus.Failed,
                    $"{ex.Message}"
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error  provider {Provider}", provider.Name);
                return new ProviderResponse(ProviderStatus.Failed, "Unexpected error");
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
            var currentIndex = _providers.FindIndex(p => p.Name == current.Name);
            return currentIndex < _providers.Count - 1 ? _providers[currentIndex + 1] : null;
        }
    }
}
