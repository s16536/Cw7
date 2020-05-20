using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace lab05.Services
{
    public class AuthenticationResult
    {
        public Claim[] Claims { get; set; }
        public string RefreshToken { get; set; }
    }
}
