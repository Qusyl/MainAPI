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
            var choice = new Random().Next(1,11);

            if (choice % 4 == 0) return "Pending";
            else if (choice % 5 == 0) return "Failed";
            else if (choice % 10 == 0 || choice % 9 == 0) return "Unknown";
            else return "Accept";
        }
        private string? GenerateError(string status) =>
           status switch
           {
               "Accept" => default,
               "Pending" => "Waiting for handling",
               "Failed" => "Payment is failed",
               "Unknown" => "Unknown error"
           };
        
    }
}
