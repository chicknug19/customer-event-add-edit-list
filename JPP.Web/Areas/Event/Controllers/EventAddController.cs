using JPP.Models.Event.Responses;
using JPP.Services.Interfaces;
using JPP.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JPP.Web.Areas.Customer.Controllers
{
    [Area("Event")]
    public class EventAddController : BaseController
    {
        protected override bool RequireLogin => true;

        private readonly IEventAddService _eventAddService;

        public EventAddController(IEventAddService eventAddService)
        {
            _eventAddService = eventAddService;
        }

        [HttpGet]
        public IActionResult EventAddPage()
        {
            var model = new EventDetailViewModel
            {
                Form = new EventDto(),
                IsReadOnly = false
            };

            return View("EventAddPage", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(EventDto form, string SubmitMode)
        {
            if (!ModelState.IsValid)
            {
                return View("EventAddPage", new EventDetailViewModel { Form = form, IsReadOnly = false });
            }

            var result = await _eventAddService.AddEventAsync(form);

            if (result.StatusCode == 200)
            {
                TempData["SuccessMessage"] = "Event berhasil disimpan!";

                if (SubmitMode == "SaveAndClose")
                {
                    return RedirectToAction("Index", "EventList", new { area = "Event" });
                }

                return RedirectToAction("EventAddPage");
            }
            else
            {
                TempData["ErrorMessage"] = result.StatusMessage;
                return View("EventAddPage", new EventDetailViewModel { Form = form, IsReadOnly = false });
            }
        }
    }
}
