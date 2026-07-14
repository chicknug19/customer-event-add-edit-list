using JPP.Models.Account.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace JPP.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected virtual bool RequireLogin => false;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (RequireLogin && !IsLoggedIn())
            {
                var loginUrl = BuildLoginUrl(context.HttpContext.Request);

                if (IsAjaxRequest(context.HttpContext.Request))
                {
                    context.Result = new JsonResult(new
                    {
                        statusCode = StatusCodes.Status401Unauthorized,
                        statusMessage = "Your session has expired. Please login again.",
                        redirectUrl = loginUrl
                    })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };

                    return;
                }

                context.Result = new RedirectResult(loginUrl);
                return;
            }

            base.OnActionExecuting(context);
        }

        protected bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("objUser"));
        }

        private static string BuildLoginUrl(HttpRequest request)
        {
            var returnUrl = $"{request.PathBase}{request.Path}{request.QueryString}";

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return "/Account/Login";
            }

            return $"/Account/Login?returnUrl={Uri.EscapeDataString(returnUrl)}";
        }

        private static bool IsAjaxRequest(HttpRequest request)
        {
            var requestedWith = request.Headers["X-Requested-With"].ToString();
            var accept = request.Headers["Accept"].ToString();

            return string.Equals(requestedWith, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase)
                   || accept.Contains("application/json", StringComparison.OrdinalIgnoreCase);
        }

        protected string GetClientIpAddress()
        {
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        }

        protected string GetBrowserName()
        {
            var userAgent = Request.Headers["User-Agent"].ToString();

            if (string.IsNullOrWhiteSpace(userAgent))
            {
                return "Unknown";
            }

            if (userAgent.Contains("Edg/", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("EdgA/", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("EdgiOS/", StringComparison.OrdinalIgnoreCase))
            {
                return "Microsoft Edge";
            }

            if (userAgent.Contains("OPR/", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("Opera", StringComparison.OrdinalIgnoreCase))
            {
                return "Opera";
            }

            if (userAgent.Contains("SamsungBrowser/", StringComparison.OrdinalIgnoreCase))
            {
                return "Samsung Internet";
            }

            if (userAgent.Contains("Firefox/", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("FxiOS/", StringComparison.OrdinalIgnoreCase))
            {
                return "Mozilla Firefox";
            }

            if (userAgent.Contains("CriOS/", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("Chrome/", StringComparison.OrdinalIgnoreCase))
            {
                return "Google Chrome";
            }

            if (userAgent.Contains("Safari/", StringComparison.OrdinalIgnoreCase))
            {
                return "Safari";
            }

            if (userAgent.Contains("MSIE", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("Trident/", StringComparison.OrdinalIgnoreCase))
            {
                return "Internet Explorer";
            }

            return "Unknown";
        }

        protected string GetBrowserVersion()
        {
            var userAgent = Request.Headers["User-Agent"].ToString();

            if (string.IsNullOrWhiteSpace(userAgent))
            {
                return "Unknown";
            }

            var match = Regex.Match(userAgent, @"Edg[A-Za-z]*\/([\d\.]+)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            match = Regex.Match(userAgent, @"OPR\/([\d\.]+)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            match = Regex.Match(userAgent, @"SamsungBrowser\/([\d\.]+)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            match = Regex.Match(userAgent, @"Firefox\/([\d\.]+)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            match = Regex.Match(userAgent, @"Chrome\/([\d\.]+)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            match = Regex.Match(userAgent, @"Version\/([\d\.]+).*Safari");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            match = Regex.Match(userAgent, @"MSIE ([\d\.]+)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            match = Regex.Match(userAgent, @"rv:([\d\.]+)");
            if (match.Success && userAgent.Contains("Trident", StringComparison.OrdinalIgnoreCase))
            {
                return match.Groups[1].Value;
            }

            return "Unknown";
        }

        protected bool IsMobileBrowser()
        {
            var userAgent = Request.Headers["User-Agent"].ToString().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(userAgent))
            {
                return false;
            }

            var mobileKeywords = new[]
            {
                "midp", "cldc", "iphone", "avant", "nokia", "pda", "moto",
                "windows ce", "hand", "mobi", "htc", "sony", "panasonic",
                "blackberry", "240x320", "voda", "android", "ipad"
            };

            return mobileKeywords.Any(userAgent.Contains);
        }

        protected AccountLoginUserDto? GetCurrentUser()
        {
            var userJson = HttpContext.Session.GetString("objUser");

            if (string.IsNullOrWhiteSpace(userJson))
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<AccountLoginUserDto>(userJson);
            }
            catch
            {
                return null;
            }
        }

        protected int GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("CurrentUserID") ?? 0;
        }

    }


}
