using JPP.Models.Event.Request;
using JPP.Models.Event.Responses;
using JPP.Services.Interfaces;
using JPP.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var durationList = new List<SelectListItem>();
            for (decimal i = 0.5m; i <= 4.0m; i += 0.5m)
            {
                durationList.Add(new SelectListItem
                {
                    Value = i.ToString("0.0"),
                    Text = $"{i.ToString("0.0")} hours"
                });
            }

            var model = new EventDetailViewModel
            {
                Form = new EventRequestDto(),
                IsReadOnly = false,
                DurationOptions = durationList
            };

            return View("EventAddPage", model); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(EventRequestDto form, string SubmitMode)
        {
            if (!ModelState.IsValid)
            {
                var model = new EventDetailViewModel
                {
                    Form = form,
                    IsReadOnly = false,
                    DurationOptions = GenerateDurationList()
                };
                return View("EventAddPage", model);
            }

            var result = await _eventAddService.AddEventAsync(form);

            if (result.StatusCode == 200)
            {
                TempData["SuccessMessage"] = "Event saved successfully!";

                if (SubmitMode == "SaveAndClose")
                {
                    return RedirectToAction("Index", "EventList", new { area = "Event" });
                }

                return RedirectToAction("EventAddPage");
            }
            else
            {
                TempData["ErrorMessage"] = result.StatusMessage;

                var model = new EventDetailViewModel
                {
                    Form = form,
                    IsReadOnly = false,
                    DurationOptions = GenerateDurationList()
                };
                return View("EventAddPage", model);
            }
        }


        private List<SelectListItem> GenerateDurationList()
        {
            var durationList = new List<SelectListItem>();
            for (decimal i = 0.5m; i <= 4.0m; i += 0.5m)
            {
                durationList.Add(new SelectListItem
                {
                    Value = i.ToString("0.0"),
                    Text = $"{i.ToString("0.0")} hours"
                });
            }
            return durationList;
        }


    }
}
