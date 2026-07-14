using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Shared.Responses
{
    public class BaseResult
    {
        public int StatusCode { get; set; }

        public string StatusMessage { get; set; } = string.Empty;

        public static BaseResult Ok(string statusMessage = "Success", int statusCode = 200)
        {
            return new BaseResult
            {
                StatusCode = statusCode,
                StatusMessage = statusMessage
            };
        }

        public static BaseResult Fail(string statusMessage, int statusCode = 400)
        {
            return new BaseResult
            {
                StatusCode = statusCode,
                StatusMessage = statusMessage
            };
        }
    }

    public class BaseResult<T> : BaseResult
    {
        public T? Data { get; set; }

        public static BaseResult<T> Ok(
            T data,
            string statusMessage = "Success",
            int statusCode = 200)
        {
            return new BaseResult<T>
            {
                StatusCode = statusCode,
                StatusMessage = statusMessage,
                Data = data
            };
        }

        public new static BaseResult<T> Fail(
            string statusMessage,
            int statusCode = 400)
        {
            return new BaseResult<T>
            {
                StatusCode = statusCode,
                StatusMessage = statusMessage,
                Data = default
            };
        }
    }
}
