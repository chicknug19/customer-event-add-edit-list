using JPP.Models.Account.Requests;
using JPP.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace JPP.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login(string? status = null)
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("objUser")))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ShowForgotPassword = string.Equals(status, "ForgetPassword", StringComparison.OrdinalIgnoreCase);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestLoginOtp(RequestLoginOtpRequest request)
        {
            var result = await _accountService.RequestLoginOtpAsync(request);
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithOtp(LoginWithOtpRequest request)
        {
            request.IpAddress = GetClientIpAddress();
            request.Browser = GetBrowserName();
            request.BrowserVersion = GetBrowserVersion();

            var result = await _accountService.LoginWithOtpAsync(request);

            if (result.StatusCode == 200 && result.User != null)
            {
                HttpContext.Session.SetInt32("CurrentUserID", result.User.Id);
                HttpContext.Session.SetString("objUser", JsonSerializer.Serialize(result.User));
                HttpContext.Session.SetString("isMobileBrowser", IsMobileBrowser().ToString());

                result.RedirectUrl = Url.Action("Index", "Home");
            }

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            var result = await _accountService.ForgotPasswordAsync(request);
            return Json(result);
        }
    }
}
