using JPP.Models.Account.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Services.Interfaces
{
    public interface IOtpService
    {
        Task<OtpServiceResult> RequestOtpByEmailAsync(string emailAddress);
        Task<OtpServiceResult> SubmitOtpAsync(string emailAddress, string otp);
    }
}
