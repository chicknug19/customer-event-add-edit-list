using JPP.Models.Event.Request;
using JPP.Models.Event.Responses;
using JPP.Services.Interfaces;
using JPP.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JPP.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CustomerEventAddController : BaseController
    {
        protected override bool RequireLogin => true;

        private readonly IEventAddService _eventAddService;

        public CustomerEventAddController(IEventAddService eventAddService)
        {
            _eventAddService = eventAddService;
        }

        [HttpGet]
        public IActionResult CustomerEventAddPage()
        {
            var model = new EventDetailViewModel
            {
                Form = new EventRequest(),
                IsReadOnly = false
            };

            return View("CustomerEventAddPage", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(EventRequest form, string SubmitMode)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Mohon lengkapi data wajib.";
                return View("CustomerEventAddPage", new EventDetailViewModel { Form = form, IsReadOnly = false });
            }

            var result = await _eventAddService.AddEventAsync(form);

            if (result.StatusCode == 200)
            {
                TempData["SuccessMessage"] = "Event berhasil disimpan!";

                if (SubmitMode == "SaveAndClose")
                {
                    return RedirectToAction("Index", "CustomerEventList", new { area = "Customer" });
                }

                return RedirectToAction("CustomerEventAddPage");
            }
            else
            {
                TempData["ErrorMessage"] = result.StatusMessage;
                return View("CustomerEventAddPage", new EventDetailViewModel { Form = form, IsReadOnly = false });
            }
        }
    }
}
