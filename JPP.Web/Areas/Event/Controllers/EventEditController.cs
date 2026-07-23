using JPP.Models.Event.Request;
using JPP.Models.Event.Responses;
using JPP.Models.Shared.Responses;
using JPP.Services.Interfaces;
using JPP.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JPP.Web.Areas.Event.Controllers
{
    [Area("Event")]
    public class EventEditController : BaseController
    {
        protected override bool RequireLogin => true;

        private readonly IEventEditService _eventEditService;

        public EventEditController(IEventEditService eventEditService)
        {
            _eventEditService = eventEditService;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Event ID is invalid.";
                return RedirectToAction(nameof(Index));
            }

            var model = await _eventEditService.BuildEditViewModelAsync(id);

            if (model == null)
            {
                TempData["ErrorMessage"] = "Event data was not found.";
                return RedirectToAction(nameof(Index));
            }

            return View("EventEditPage", model);
        }

        private List<SelectListItem> GenerateDurationList()
        {
            var durationList = new List<SelectListItem>();
            for (decimal i = 0.5m; i <= 4.0m; i += 0.5m)
            {
                durationList.Add(new SelectListItem
                {
                    Value = i.ToString("0.0"),
                    Text = $"{i:0.0} hours"
                });
            }
            return durationList;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(EventRequestDto form, string SubmitMode)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Mohon periksa kembali kelengkapan data Anda.";

                var invalidModel = new EventDetailViewModel
                {
                    Form = form,
                    IsReadOnly = false,
                    DurationOptions = GenerateDurationList() // <- added
                };

                return View("EventEditPage", invalidModel);
            }

            try
            {
                var result = await _eventEditService.SaveEventAsync(form);

                if (result.StatusCode != 200)
                {
                    TempData["ErrorMessage"] = result.StatusMessage;

                    var invalidModel = new EventDetailViewModel
                    {
                        Form = form,
                        IsReadOnly = false,
                        DurationOptions = GenerateDurationList() // <- added
                    };

                    return View("EventEditPage", invalidModel);
                }

                TempData["SuccessMessage"] = "Event berhasil diperbarui.";

                if (SubmitMode == "SaveAndClose")
                {
                    return RedirectToAction("Index", "EventList", new { area = "Event" });
                }

                return RedirectToAction(nameof(Edit), new { id = form.Id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Terjadi kesalahan sistem: {ex.Message}";

                var invalidModel = new EventDetailViewModel
                {
                    Form = form,
                    IsReadOnly = false,
                    DurationOptions = GenerateDurationList() // <- added
                };
                return View("EventEditPage", invalidModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAjax(EventRequestDto form)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                return BadRequest(BaseResult.Fail(
                    statusMessage: string.IsNullOrWhiteSpace(errors) ? "Data tidak valid." : errors,
                    statusCode: 400));
            }

            try
            {
                var result = await _eventEditService.SaveEventAsync(form);

                if (result.StatusCode != 200)
                {
                    return BadRequest(BaseResult.Fail(
                        statusMessage: result.StatusMessage,
                        statusCode: result.StatusCode));
                }

                return Json(BaseResult<object>.Ok(
                    data: new { id = result.Data, redirectUrl = Url.Action(nameof(Edit), new { id = form.Id }) },
                    statusMessage: result.StatusMessage));
            }
            catch (Exception ex)
            {
                return StatusCode(500, BaseResult.Fail(
                    statusMessage: $"Terjadi kesalahan sistem: {ex.Message}",
                    statusCode: 500));
            }
        }



    }
}