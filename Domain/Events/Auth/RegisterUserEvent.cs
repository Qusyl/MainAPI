using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events.Auth
{
    public class RegisterUserEvent : IDomainEvent
    {
        public string EventType => "Auth.registration";

        public int Version => 1;

        public DateTime OccurredOn  {get; set; }

        public List<PaymentAttempt> Attempts => new List<PaymentAttempt>();

        public string Email { get ; set; }

        public string Password { get; set; }

        public RegisterUserEvent( string email, string password)
        {
            
            Email = email;
            Password = password;
        }
    }
}
