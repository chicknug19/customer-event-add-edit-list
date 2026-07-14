using JPP.Commons.Extensions;
using JPP.Models.Account.Responses;
using JPP.Models.Admin.StoreList.Request;
using JPP.Models.Admin.StoreList.Responses;
using JPP.Models.Shared.Responses;
using JPP.Services.Interfaces;
using JPP.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace JPP.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StoreListController : BaseController
    {
        protected override bool RequireLogin => true;

        private readonly IStoreListService _storeListService;

        public StoreListController(IStoreListService storeListService)
        {
            _storeListService = storeListService;
        }

        [HttpGet]
        public IActionResult Index(StoreListFilterRequest filter)
        {
            filter.NormalizeFilter();

            return View(new StoreListIndexViewModel
            {
                Filter = filter
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetStoreList(StoreListFilterRequest filter)
        {
            try
            {
                var result = await _storeListService.GetPagedAsync(filter);

                return Json(BaseResult<StoreListPagedResponse>.Ok(
                    data: result,
                    statusMessage: "Store data loaded successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    BaseResult<StoreListPagedResponse>.Fail(
                        statusMessage: $"Failed to load store data. {ex.Message}",
                        statusCode: StatusCodes.Status500InternalServerError));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var model = await _storeListService.BuildEditViewModelAsync(
                id,
                canChangeInCharge: CanChangeInCharge());

            if (model == null)
            {
                TempData["ErrorMessage"] = "Store data was not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = await _storeListService.BuildAddViewModelAsync(
                canChangeInCharge: CanChangeInCharge());

            return View("Form", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _storeListService.BuildEditViewModelAsync(
                id,
                canChangeInCharge: CanChangeInCharge());

            if (model == null)
            {
                TempData["ErrorMessage"] = "Store data was not found.";
                return RedirectToAction(nameof(Index));
            }

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StoreDetailRequest request)
        {
            if (request.Id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid store ID.";
                return RedirectToAction(nameof(Index));
            }

            var inChargeAllowed = await ApplyInChargePermissionAsync(request);

            if (!inChargeAllowed)
            {
                TempData["ErrorMessage"] = "Store data was not found.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                var invalidModel = await _storeListService.BuildEditViewModelAsync(
                    request.Id,
                    canChangeInCharge: CanChangeInCharge());

                if (invalidModel == null)
                {
                    TempData["ErrorMessage"] = "Store data was not found.";
                    return RedirectToAction(nameof(Index));
                }

                invalidModel.Form = request;

                return View("Form", invalidModel);
            }

            try
            {
                var result = await _storeListService.UpdateStoreAsync(request);

                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update store. Please try again.");

                    var invalidModel = await _storeListService.BuildEditViewModelAsync(
                        request.Id,
                        canChangeInCharge: CanChangeInCharge());

                    if (invalidModel == null)
                    {
                        TempData["ErrorMessage"] = "Store data was not found.";
                        return RedirectToAction(nameof(Index));
                    }

                    invalidModel.Form = request;

                    return View("Form", invalidModel);
                }

                TempData["SuccessMessage"] = "Store has been updated successfully.";

                if (IsSaveAndClose(request))
                {
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Edit), new { id = request.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Failed to update store. {ex.Message}");

                var invalidModel = await _storeListService.BuildEditViewModelAsync(
                    request.Id,
                    canChangeInCharge: CanChangeInCharge());

                if (invalidModel == null)
                {
                    TempData["ErrorMessage"] = "Store data was not found.";
                    return RedirectToAction(nameof(Index));
                }

                invalidModel.Form = request;

                return View("Form", invalidModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            try
            {
               
                var result = await _storeListService.DeleteStoreAsync(id);

                if (!result)
                {
                    return BadRequest(BaseResult.Fail(
                        statusMessage: "Failed to delete store.",
                        statusCode: StatusCodes.Status400BadRequest));
                }

                return Json(BaseResult.Ok(
                    statusMessage: "Store has been deleted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    BaseResult.Fail(
                        statusMessage: $"Failed to delete store. {ex.Message}",
                        statusCode: StatusCodes.Status500InternalServerError));
            }
        }

        [HttpGet]
        public IActionResult ExportPdf()
        {
            TempData["InfoMessage"] = "Export to PDF has not been implemented yet.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ExportExcel()
        {
            TempData["InfoMessage"] = "Export to Excel has not been implemented yet.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAjax(StoreDetailRequest request)
        {
            var inChargeAllowed = await ApplyInChargePermissionAsync(request);

            if (!inChargeAllowed)
            {
                return BadRequest(BaseResult.Fail(
                    statusMessage: "Store data was not found.",
                    statusCode: StatusCodes.Status400BadRequest));
            }

            ModelState.Remove(nameof(StoreDetailRequest.InChargeId));
            ModelState.Remove(nameof(StoreDetailRequest.InChargeName));

            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResult.Fail(
                    statusMessage: GetModelStateErrorMessage(),
                    statusCode: StatusCodes.Status400BadRequest));
            }

            try
            {
                if (request.Id <= 0)
                {
                    var currentUser = GetCurrentUser();
                    var currentUserId = currentUser!.Id;
                    var hqId = currentUser!.HqId;

                    if (hqId <= 0)
                    {
                        return BadRequest(BaseResult.Fail(
                            statusMessage: "HQ information was not found. Please login again.",
                            statusCode: StatusCodes.Status400BadRequest));
                    }

                    var newStoreId = await _storeListService.CreateStoreAsync(
                        request,
                        hqId,
                        currentUserId);

                    if (newStoreId <= 0)
                    {
                        return BadRequest(BaseResult.Fail(
                            statusMessage: "Failed to save store. Please try again.",
                            statusCode: StatusCodes.Status400BadRequest));
                    }

                    var redirectUrl = IsSaveAndClose(request)
                        ? Url.Action(nameof(Index), "StoreList", new { area = "Admin" })
                        : Url.Action(nameof(Edit), "StoreList", new { area = "Admin", id = newStoreId });

                    return Json(BaseResult<object>.Ok(
                        data: new
                        {
                            id = newStoreId,
                            redirectUrl
                        },
                        statusMessage: "Store has been saved successfully."));
                }

                var result = await _storeListService.UpdateStoreAsync(request);

                if (!result)
                {
                    return BadRequest(BaseResult.Fail(
                        statusMessage: "Failed to update store. Please try again.",
                        statusCode: StatusCodes.Status400BadRequest));
                }

                var editRedirectUrl = IsSaveAndClose(request)
                    ? Url.Action(nameof(Index), "StoreList", new { area = "Admin" })
                    : Url.Action(nameof(Edit), "StoreList", new { area = "Admin", id = request.Id });

                return Json(BaseResult<object>.Ok(
                    data: new
                    {
                        id = request.Id,
                        redirectUrl = editRedirectUrl
                    },
                    statusMessage: "Store has been updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    BaseResult.Fail(
                        statusMessage: $"Failed to save store. {ex.Message}",
                        statusCode: StatusCodes.Status500InternalServerError));
            }
        }

        private string GetModelStateErrorMessage()
        {
            var errors = ModelState.Values
                .SelectMany(value => value.Errors)
                .Select(error => error.ErrorMessage)
                .Where(message => !string.IsNullOrWhiteSpace(message))
                .ToList();

            return errors.Count > 0
                ? string.Join(" ", errors)
                : "Please complete all required fields.";
        }

        private bool CanChangeInCharge()
        {
            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return false;
            }

            return currentUser.RoleId <= 1
                   || string.Equals(
                       currentUser.RoleName,
                       "MANAGER",
                       StringComparison.OrdinalIgnoreCase);
        }

        private async Task<bool> ApplyInChargePermissionAsync(StoreDetailRequest request)
        {
            if (CanChangeInCharge())
            {
                return true;
            }

            if (request.Id <= 0)
            {
                request.InChargeId = 0;
                request.InChargeName = "- None -";
                return true;
            }

            var existingModel = await _storeListService.BuildEditViewModelAsync(
                request.Id,
                canChangeInCharge: false);

            if (existingModel == null)
            {
                return false;
            }

            request.InChargeId = existingModel.Form.InChargeId;
            request.InChargeName = existingModel.Form.InChargeName;

            return true;
        }

        private static bool IsSaveAndClose(StoreDetailRequest request)
        {
            return string.Equals(
                request.SubmitMode,
                "SaveAndClose",
                StringComparison.OrdinalIgnoreCase);
        }
    }
}