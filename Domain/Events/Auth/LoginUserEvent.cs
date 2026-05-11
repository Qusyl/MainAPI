using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events.Auth
{
    public class LoginUserEvent : IDomainEvent
    {
        public string EventType => "Auth.login";

        public int Version => 1;

        public DateTime OccurredOn { get; set; }

        public List<PaymentAttempt> Attempts => new List<PaymentAttempt>();

        public Guid UserId {  get; set; }
        public string Email { get; set; }

        public string Password { get; set; }

        public LoginUserEvent(string email, string password)
        {
           
            Email = email;
            Password = password;
        }
    }
}
