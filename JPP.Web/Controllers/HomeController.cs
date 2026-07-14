using Microsoft.AspNetCore.Mvc;

namespace JPP.Web.Controllers
{
    public class HomeController : BaseController
    {
        protected override bool RequireLogin => true;

        public IActionResult Index()
        {
            return View();
        }
    }
}
