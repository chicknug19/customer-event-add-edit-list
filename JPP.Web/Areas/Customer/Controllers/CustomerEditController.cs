using JPP.Models.Customer.Request;
using JPP.Models.Customer.Responses;
using JPP.Models.Shared.Responses;
using JPP.Services.Interfaces;
using JPP.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JPP.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CustomerEditController : BaseController
    {
        protected override bool RequireLogin => true;

        private readonly ICustomerEditService _customerEditService;
        private readonly IEventDropdownService _eventService;
        private readonly IStoreDropdownService _storeService;


        public CustomerEditController(ICustomerEditService customerEditService, IEventDropdownService eventService, IStoreDropdownService storeService)
        {
            _customerEditService = customerEditService;
            _eventService = eventService;
            _storeService = storeService;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Customer ID is invalid.";
                return RedirectToAction(nameof(Index));
            }

            var model = await _customerEditService.BuildEditViewModelAsync(id);

            if (model == null)
            {
                TempData["ErrorMessage"] = "Customer data was not found.";
                return RedirectToAction(nameof(Index));
            }

            var events = await _eventService.GetDropdownListAsync();
            var stores = await _storeService.GetStoreDropdownListAsync();

            model.EventOptions = events.Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = $"{e.Code} - {e.Name}"
            });

            model.StoreOptions = stores.Select(s => new SelectListItem
            {
                Value = s.ID.ToString(),
                Text = s.StoreName
            });

            return View("CustomerEditPage", model);
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
        public async Task<IActionResult> Save(CustomerRequest form, string SubmitMode)
        {
            if (!ModelState.IsValid)
            {
                var events = await _eventService.GetDropdownListAsync();
                var stores = await _storeService.GetStoreDropdownListAsync();

                var model = new CustomerDetailViewModel
                {
                    Form = form,
                    IsReadOnly = false,
                    EventName = form.EventName ?? string.Empty,
                    EventOptions = events.Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = $"{e.Code} - {e.Name}"
                    }),
                    StoreOptions = stores.Select(s => new SelectListItem
                    {
                        Value = s.ID.ToString(),
                        Text = s.StoreName
                    })
                };
                return View("CustomerEditPage", model);
            }

            var result = await _customerEditService.SaveCustomerAsync(form);

            if (result.StatusCode == 200)
            {
                TempData["SuccessMessage"] = "Customer successfully saved!";

                if (SubmitMode == "SaveAndClose")
                {
                    return RedirectToAction("Index", "CustomerList", new { area = "Customer" });
                }

                return RedirectToAction("Edit", new { id = result.Data });
            }
            else
            {
                TempData["ErrorMessage"] = result.StatusMessage;

                var events = await _eventService.GetDropdownListAsync();
                var stores = await _storeService.GetStoreDropdownListAsync();

                var model = new CustomerDetailViewModel
                {
                    Form = form,
                    IsReadOnly = false,
                    EventOptions = events.Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = $"{e.Code} - {e.Name}"
                    }),
                    StoreOptions = stores.Select(s => new SelectListItem
                    {
                        Value = s.ID.ToString(),
                        Text = s.StoreName
                    })
                };
                return View("CustomerEditPage", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAjax(CustomerRequest request)
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
                var result = await _customerEditService.SaveCustomerAsync(request);

                if (result.StatusCode != 200)
                {
                    return BadRequest(BaseResult.Fail(
                        statusMessage: result.StatusMessage,
                        statusCode: result.StatusCode));
                }

                return Json(BaseResult<object>.Ok(
                    data: new { id = result.Data, redirectUrl = Url.Action(nameof(Index)) },
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