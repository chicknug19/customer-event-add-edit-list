using JPP.Models.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Account.Responses
{
    public class AccountServiceResult : BaseResult
    {
        public string? RedirectUrl { get; set; }
        public AccountLoginUserDto? User { get; set; }

        public new static AccountServiceResult Ok(
            string statusMessage = "Success",
            int statusCode = 200,
            string? redirectUrl = null,
            AccountLoginUserDto? user = null)
        {
            return new AccountServiceResult
            {
                StatusCode = statusCode,
                StatusMessage = statusMessage,
                RedirectUrl = redirectUrl,
                User = user
            };
        }

        public new static AccountServiceResult Fail(string statusMessage, int statusCode = 400)
        {
            return new AccountServiceResult
            {
                StatusCode = statusCode,
                StatusMessage = statusMessage
            };
        }
    }
}
