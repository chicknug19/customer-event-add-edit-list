using JPP.Data.Entities;
using JPP.Data.Interfaces;
using JPP.Models.Account.Requests;
using JPP.Models.Account.Responses;
using JPP.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOtpService _otpService;
        private readonly IAccountEmailService _accountEmailService;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            IAccountRepository accountRepository,
            IOtpService otpService,
            IAccountEmailService accountEmailService,
            ILogger<AccountService> logger)
        {
            _accountRepository = accountRepository;
            _otpService = otpService;
            _accountEmailService = accountEmailService;
            _logger = logger;
        }

        public async Task<AccountServiceResult> RequestLoginOtpAsync(RequestLoginOtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return AccountServiceResult.Fail(
                    statusCode: 400,
                    statusMessage: "Please fill in your username");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return AccountServiceResult.Fail(
                    statusCode: 400,
                    statusMessage: "Please fill in your password");
            }

            try
            {
                var currentUser = await _accountRepository.GetEmployeeByUsernameAsync(request.Username.Trim());

                if (currentUser == null)
                {
                    return AccountServiceResult.Fail(
                        statusCode: 401,
                        statusMessage: "Invalid username or password");
                }

                var security = await _accountRepository.GetEmployeeSecurityByEmployeeIdAsync(currentUser.Id);

                if (security == null)
                {
                    return AccountServiceResult.Fail(
                        statusCode: 403,
                        statusMessage: "Please contact IT Support for security verification.");
                }

                if (!security.ModHome)
                {
                    return AccountServiceResult.Fail(
                        statusCode: 403,
                        statusMessage: "Access is Prohibited");
                }

                if (!IsPasswordValid(currentUser, request.Password))
                {
                    return AccountServiceResult.Fail(
                        statusCode: 401,
                        statusMessage: "Invalid username or password");
                }

                var otpResult = await _otpService.RequestOtpByEmailAsync(currentUser.EmailAddress);

                if (otpResult.StatusCode != 200)
                {
                    return AccountServiceResult.Fail(
                        statusCode: otpResult.StatusCode,
                        statusMessage: otpResult.StatusMessage);
                }

                return AccountServiceResult.Ok(
                    statusCode: 200,
                    statusMessage: "OTP is sent to your email. Please enter the OTP.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RequestLoginOtp failed for username {Username}", request.Username);

                return AccountServiceResult.Fail(
                    statusCode: 500,
                    statusMessage: "Invalid username or password");
            }
        }

        public async Task<AccountServiceResult> LoginWithOtpAsync(LoginWithOtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return AccountServiceResult.Fail(
                    statusCode: 400,
                    statusMessage: "Please fill in your username");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return AccountServiceResult.Fail(
                    statusCode: 400,
                    statusMessage: "Please fill in your password");
            }

            if (string.IsNullOrWhiteSpace(request.Otp))
            {
                return AccountServiceResult.Fail(
                    statusCode: 400,
                    statusMessage: "Please fill in your OTP");
            }

            try
            {
                var currentUser = await _accountRepository.GetEmployeeByUsernameAsync(request.Username.Trim());

                if (currentUser == null)
                {
                    return AccountServiceResult.Fail(
                        statusCode: 401,
                        statusMessage: "Invalid username or password");
                }

                var otpResult = await _otpService.SubmitOtpAsync(currentUser.EmailAddress, request.Otp.Trim());

                if (otpResult.StatusCode != 200)
                {
                    return AccountServiceResult.Fail(
                        statusCode: otpResult.StatusCode,
                        statusMessage: otpResult.StatusMessage);
                }

                if (!IsPasswordValid(currentUser, request.Password))
                {
                    return AccountServiceResult.Fail(
                        statusCode: 401,
                        statusMessage: "Invalid username or password");
                }

                await _accountRepository.InsertUserSessionAsync(
                    currentUser.Id,
                    request.IpAddress,
                    request.Browser,
                    request.BrowserVersion);

                await _accountRepository.SaveLoginAuditAsync(currentUser);

                return AccountServiceResult.Ok(
                    statusCode: 200,
                    statusMessage: "Login success",
                    user: MapLoginUser(currentUser));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoginWithOtp failed for username {Username}", request.Username);

                return AccountServiceResult.Fail(
                    statusCode: 500,
                    statusMessage: "Invalid username or password");
            }
        }

        public async Task<AccountServiceResult> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return AccountServiceResult.Fail(
                    statusCode: 400,
                    statusMessage: "Please enter your registered email address.");
            }

            try
            {
                var email = request.Email.Trim();
                var isRegistered = await _accountRepository.EmailExistsAsync(email);

                if (!isRegistered)
                {
                    return AccountServiceResult.Fail(
                        statusCode: 404,
                        statusMessage: "Invalid Email Address.");
                }

                var newPassword = GenerateLegacyPassword();

                await _accountRepository.UpdatePasswordByEmailAsync(email, newPassword);
                await _accountEmailService.SendForgotPasswordEmailAsync(email, newPassword);

                return AccountServiceResult.Ok(
                    statusCode: 200,
                    statusMessage: $"A new password has been sent to {email}. You may change the password in the User Profile settings after your login.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ForgotPassword failed for email {Email}", request.Email);

                return AccountServiceResult.Fail(
                    statusCode: 500,
                    statusMessage: "Unable to reset password. Please contact IT Support.");
            }
        }

        private static bool IsPasswordValid(BIZEmployee currentUser, string? password)
        {
            return string.Equals(currentUser.Password, password, StringComparison.Ordinal);
        }

        private static string GenerateLegacyPassword()
        {
            var now = DateTime.Now;
            return $"{now.Year}{now.Month}{now.Hour}{now.Minute}{now.Second}";
        }

        private static AccountLoginUserDto MapLoginUser(BIZEmployee employee)
        {
            return new AccountLoginUserDto
            {
                Id = employee.Id,
                Username = employee.Username,
                EmailAddress = employee.EmailAddress,
                DisplayName = employee.DisplayName,
                HqId = employee.HqId,
                StoreId = employee.StoreId,
                RoleId = employee.RoleId,
                RoleName = employee.RoleName
            };
        }
    }
}
