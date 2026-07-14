using JPP.Models.Account.Responses;
using JPP.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.ServiceModel;
using CellboxSmsc;

namespace JPP.Services.Services
{
    public class CellboxOtpService : IOtpService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CellboxOtpService> _logger;

        public CellboxOtpService(
            IConfiguration configuration,
            ILogger<CellboxOtpService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<OtpServiceResult> RequestOtpByEmailAsync(string emailAddress)
        {
            if (IsBypassEnabled())
            {
                return OtpServiceResult.Ok(
                    statusCode: 200,
                    statusMessage: "OTP is sent to your email. Please enter the OTP.",
                    otpRefId: "BYPASS");
            }

            try
            {
                using var clientWrapper = CreateClient();

                var client = clientWrapper.Client;

                var response = await client.GetOTPByEmailAsync(
                    GetOtpAccount(),
                    GetOtpSenderName(),
                    emailAddress
                );

                var otpRefId = GetStringProperty(response, "OTPRefID");

                if (string.IsNullOrWhiteSpace(otpRefId))
                {
                    otpRefId = GetNestedStringProperty(response, "GetOTPByEmailResult", "OTPRefID");
                }

                if (otpRefId == "0" || string.IsNullOrWhiteSpace(otpRefId))
                {
                    return OtpServiceResult.Fail(
                        statusCode: 500,
                        statusMessage: "Failure sending OTP. Please contact Cell Box.");
                }

                return OtpServiceResult.Ok(
                    statusCode: 200,
                    statusMessage: "OTP is sent to your email. Please enter the OTP.",
                    otpRefId: otpRefId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to request OTP for {EmailAddress}", emailAddress);

                return OtpServiceResult.Fail(
                    statusCode: 500,
                    statusMessage: "Failure sending OTP. Please contact Cell Box.");
            }
        }

        public async Task<OtpServiceResult> SubmitOtpAsync(string emailAddress, string otp)
        {
            if (IsBypassEnabled())
            {
                var bypassCode = _configuration["LegacyLogin:OtpBypassCode"] ?? "123456";

                return otp == bypassCode
                    ? OtpServiceResult.Ok(statusCode: 200, statusMessage: "OTP success")
                    : OtpServiceResult.Fail(statusCode: 401, statusMessage: "Invalid OTP");
            }

            try
            {
                using var clientWrapper = CreateClient();

                var client = clientWrapper.Client;

                var response = await client.SubmitOTPAsync(
                    GetOtpAccount(),
                    GetOtpSenderName(),
                    emailAddress,
                    otp.Trim()
                );

                var status = GetStringProperty(response, "Status");

                if (string.IsNullOrWhiteSpace(status))
                {
                    status = GetNestedStringProperty(response, "SubmitOTPResult", "Status");
                }

                return string.Equals(status, "SUCCESS", StringComparison.OrdinalIgnoreCase)
                    ? OtpServiceResult.Ok(statusCode: 200, statusMessage: "OTP success")
                    : OtpServiceResult.Fail(statusCode: 401, statusMessage: "Invalid OTP");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to submit OTP for {EmailAddress}", emailAddress);

                return OtpServiceResult.Fail(
                    statusCode: 500,
                    statusMessage: "Invalid OTP");
            }
        }

        private CellboxClientWrapper CreateClient()
        {
            var endpoint = _configuration["CellboxSmsc:Endpoint"];

            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new InvalidOperationException("Missing configuration: CellboxSmsc:Endpoint");
            }

            var binding = CreateBinding(endpoint);
            var endpointAddress = new EndpointAddress(endpoint);

            // Kalau nama client di Reference.cs berbeda, ganti _2FASoapClient di sini.
            var client = new _2FASoapClient(binding, endpointAddress);

            return new CellboxClientWrapper(client);
        }

        private static BasicHttpBinding CreateBinding(string endpoint)
        {
            var isHttps = endpoint.StartsWith("https", StringComparison.OrdinalIgnoreCase);

            var binding = new BasicHttpBinding(
                isHttps ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.None
            );

            binding.MaxReceivedMessageSize = 10 * 1024 * 1024;
            binding.ReaderQuotas.MaxStringContentLength = 10 * 1024 * 1024;

            return binding;
        }

        private bool IsBypassEnabled()
        {
            return bool.TryParse(_configuration["LegacyLogin:OtpBypassEnabled"], out var enabled) && enabled;
        }

        private string GetOtpAccount()
        {
            return _configuration["LegacyLogin:OtpAccount"] ?? "9087654321";
        }

        private string GetOtpSenderName()
        {
            return _configuration["LegacyLogin:OtpSenderName"] ?? "JPP SKIN LASER CLINIC";
        }

        private static string GetStringProperty(object? source, string propertyName)
        {
            if (source == null)
            {
                return string.Empty;
            }

            var property = source.GetType().GetProperty(propertyName);
            return Convert.ToString(property?.GetValue(source)) ?? string.Empty;
        }

        private static string GetNestedStringProperty(object? source, string objectPropertyName, string valuePropertyName)
        {
            if (source == null)
            {
                return string.Empty;
            }

            var objectProperty = source.GetType().GetProperty(objectPropertyName);
            var nestedObject = objectProperty?.GetValue(source);

            return GetStringProperty(nestedObject, valuePropertyName);
        }

        private sealed class CellboxClientWrapper : IDisposable
        {
            public _2FASoapClient Client { get; }

            public CellboxClientWrapper(_2FASoapClient client)
            {
                Client = client;
            }

            public void Dispose()
            {
                try
                {
                    if (Client.State == CommunicationState.Faulted)
                    {
                        Client.Abort();
                    }
                    else
                    {
                        Client.Close();
                    }
                }
                catch
                {
                    Client.Abort();
                }
            }
        }
    }
}