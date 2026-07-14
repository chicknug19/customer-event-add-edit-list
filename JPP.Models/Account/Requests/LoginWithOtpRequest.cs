using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Account.Requests
{
    public class LoginWithOtpRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Otp { get; set; }

        // Filled by AccountController from HttpContext.
        public string? IpAddress { get; set; }
        public string? Browser { get; set; }
        public string? BrowserVersion { get; set; }
    }
}
