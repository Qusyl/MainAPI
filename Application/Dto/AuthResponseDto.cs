using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class AuthResponseDto
    {
        public string Token { get; set; }

        public DateTime ExpiresAt { get; set; }

        public AuthResponseDto(string token, DateTime expiresAt)
        {
            Token = token;
            ExpiresAt = expiresAt;
        }
    }
}
