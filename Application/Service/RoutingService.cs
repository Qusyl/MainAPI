using Application.Interface;
using Domain.Entity;
using Domain.value;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class RoutingService : IRoutingService
    {
        private readonly IPaymentRepository _repository;

        private readonly ILogger<RoutingService> _logger;

        private readonly HttpClient _httpClient;

        public RoutingService(IPaymentRepository repository,HttpClient httpClient, ILogger<RoutingService> logger)
        {
            _repository = repository;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task SendAsync(Payment payment)
        {
            var currentProvider = new Provider("A", new Uri("https://localhost:7078/api/provider-a")); // пока что заглушка, не забыть убрать!

            while(currentProvider != null)
            {
                var attemp = await CreateAttemptAsync(payment, currentProvider);
                //проверку бахнуть

               var ProviderResponse = await SendToProviderAsync(currentProvider, payment);
                var decision = await HandleAsync(payment, ProviderResponse);
            }
        }
        private async Task<RoutingDecision> HandleAsync(Payment payment, ProviderResponse response)
        {

        }
        private async Task<PaymentAttempt> CreateAttemptAsync(Payment payment, Provider provider)
        {

        }
        private async Task<ProviderResponse> SendToProviderAsync(Provider provider, Payment payment)
        {
            var response = await _httpClient.PostAsJsonAsync(provider.Uri, payment);
            return new ProviderResponse(response);
        }
        private Provider? ResolveNextProvider()
        {

        }
    }
}
