using Domain.value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Services
{
    public interface IAlertService
    {
        Task SendAsync(PaymentAlert alert, SecurityStatus status);
    }
}
