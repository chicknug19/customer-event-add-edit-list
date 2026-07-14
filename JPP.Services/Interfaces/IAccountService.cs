using JPP.Models.Account.Requests;
using JPP.Models.Account.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountServiceResult> RequestLoginOtpAsync(RequestLoginOtpRequest request);
        Task<AccountServiceResult> LoginWithOtpAsync(LoginWithOtpRequest request);
        Task<AccountServiceResult> ForgotPasswordAsync(ForgotPasswordRequest request);
    }
}
