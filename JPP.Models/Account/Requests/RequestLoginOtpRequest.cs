using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Account.Requests
{
    public class RequestLoginOtpRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
