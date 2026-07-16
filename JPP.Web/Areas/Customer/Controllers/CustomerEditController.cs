using JPP.Models.Customer.Request;
using JPP.Models.Customer.Responses;
using JPP.Models.Shared.Responses;
using JPP.Models.Customer.Responses.CustomerDto;
using JPP.Services.Interfaces;
using JPP.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
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

        public CustomerEditController(ICustomerEditService customerEditService)
        {
            _customerEditService = customerEditService;
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
                TempData["ErrorMessage"] = "Mohon periksa kembali kelengkapan data Anda.";

                var invalidModel = new CustomerDetailViewModel
                {
                    Form = form,
                    IsReadOnly = false
                };

                return View("CustomerEditPage", invalidModel);
            }

            try
            {
                var result = await _customerEditService.SaveCustomerAsync(form);

                if (result.StatusCode != 200)
                {
                    TempData["ErrorMessage"] = result.StatusMessage;

                    var invalidModel = new CustomerDetailViewModel
                    {
                        Form = form,
                        IsReadOnly = false
                    };

                    return View("CustomerEditPage", invalidModel);
                }

                TempData["SuccessMessage"] = "Customer berhasil diperbarui.";

                if (SubmitMode == "SaveAndClose")
                {
                    return RedirectToAction("Index", "CustomerList", new { area = "Customer" });
                }

                return RedirectToAction(nameof(Edit), new { id = form.ID });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Terjadi kesalahan sistem: {ex.Message}";

                var invalidModel = new CustomerDetailViewModel
                {
                    Form = form,
                    IsReadOnly = false
                };

                return View("CustomerEditPage", invalidModel);
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