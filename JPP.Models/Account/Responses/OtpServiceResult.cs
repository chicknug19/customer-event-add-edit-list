using JPP.Models.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Account.Responses
{
    public class OtpServiceResult : BaseResult
    {
        public string? OtpRefId { get; set; }

        public new static OtpServiceResult Ok(string statusMessage = "Success", int statusCode = 200, string? otpRefId = null)
        {
            return new OtpServiceResult
            {
                StatusCode = statusCode,
                StatusMessage = statusMessage,
                OtpRefId = otpRefId
            };
        }

        public new static OtpServiceResult Fail(string statusMessage, int statusCode = 400)
        {
            return new OtpServiceResult
            {
                StatusCode = statusCode,
                StatusMessage = statusMessage
            };
        }
    }
}
