using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class UserRegisterDto
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public UserRegisterDto(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
