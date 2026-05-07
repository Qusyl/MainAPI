using Application.Dto;
using Application.Interface.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class ResponseGenerateService : IProviderService
    {
        public async Task<ProviderApiResponse> SendAsync(PaymentDto paymentDto)
        {
            var status = GenerateStatus();
            var error = GenerateError(status);
            var response = new ProviderApiResponse
            {
                Status = status,
                ProviderTransactionId = Guid.NewGuid().ToString(),
                ErrorCode = error
            };
            return response;
        }
        private string GenerateStatus()
        {
            var choice = new Random().Next(1,10);

            if (choice % 3 == 0) return "Accept";
            else if (choice % 4 == 0) return "Pending";
            else return "Failed";
        }
        private string? GenerateError(string status) =>
           status switch
           {
               "Accept" => default,
               "Pending" => "Waiting for handling",
               "Failed" => "Payment is failed"
           };
        
    }
}
