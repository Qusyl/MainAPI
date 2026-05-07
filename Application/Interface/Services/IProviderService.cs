using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Services
{
    public interface IProviderService
    {
        Task<ProviderApiResponse> SendAsync(PaymentDto paymentDto);
    }
}
