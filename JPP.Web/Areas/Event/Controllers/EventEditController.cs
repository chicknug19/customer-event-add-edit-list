using JPP.Models.Event.Request;
using JPP.Models.Event.Responses;
using JPP.Models.Shared.Responses;
using JPP.Services.Interfaces;
using JPP.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(StoreDetailRequest request)
        // {
        //     if (request.Id <= 0)
        //     {
        //         TempData["ErrorMessage"] = "Invalid store ID.";
        //         return RedirectToAction(nameof(Index));
        //     }

        //     var inChargeAllowed = await ApplyInChargePermissionAsync(request);

        //     if (!inChargeAllowed)
        //     {
        //         TempData["ErrorMessage"] = "Store data was not found.";
        //         return RedirectToAction(nameof(Index));
        //     }

        //     if (!ModelState.IsValid)
        //     {
        //         var invalidModel = await _storeListService.BuildEditViewModelAsync(
        //             request.Id,
        //             canChangeInCharge: CanChangeInCharge());

        //         if (invalidModel == null)
        //         {
        //             TempData["ErrorMessage"] = "Store data was not found.";
        //             return RedirectToAction(nameof(Index));
        //         }

        //         invalidModel.Form = request;

        //         return View("Form", invalidModel);
        //     }

        //     try
        //     {
        //         var result = await _storeListService.UpdateStoreAsync(request);

        //         if (!result)
        //         {
        //             ModelState.AddModelError(string.Empty, "Failed to update store. Please try again.");

        //             var invalidModel = await _storeListService.BuildEditViewModelAsync(
        //                 request.Id,
        //                 canChangeInCharge: CanChangeInCharge());

        //             if (invalidModel == null)
        //             {
        //                 TempData["ErrorMessage"] = "Store data was not found.";
        //                 return RedirectToAction(nameof(Index));
        //             }

        //             invalidModel.Form = request;

        //             return View("Form", invalidModel);
        //         }

        //         TempData["SuccessMessage"] = "Store has been updated successfully.";

        //         if (IsSaveAndClose(request))
        //         {
        //             return RedirectToAction(nameof(Index));
        //         }

        //         return RedirectToAction(nameof(Edit), new { id = request.Id });
        //     }
        //     catch (Exception ex)
        //     {
        //         ModelState.AddModelError(string.Empty, $"Failed to update store. {ex.Message}");

        //         var invalidModel = await _storeListService.BuildEditViewModelAsync(
        //             request.Id,
        //             canChangeInCharge: CanChangeInCharge());

        //         if (invalidModel == null)
        //         {
        //             TempData["ErrorMessage"] = "Store data was not found.";
        //             return RedirectToAction(nameof(Index));
        //         }

        //         invalidModel.Form = request;

        //         return View("Form", invalidModel);
        //     }
        // }

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
                    IsReadOnly = false
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
                        IsReadOnly = false
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
                    IsReadOnly = false
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