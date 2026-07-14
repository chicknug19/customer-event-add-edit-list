using JPP.Commons.Extensions;
using JPP.Models.Account.Responses;
using JPP.Models.HR.EmployeeList.Request;
using JPP.Models.HR.EmployeeList.Responses;
using JPP.Models.Shared.Responses;
using JPP.Services.Interfaces;
using JPP.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace JPP.Web.Areas.HR.Controllers
{
    [Area("HR")]
    public class EmployeeListController : BaseController
    {
        protected override bool RequireLogin => true;

        private readonly IEmployeeListService _employeeListService;

        public EmployeeListController(IEmployeeListService employeeListService)
        {
            _employeeListService = employeeListService;
        }

        [HttpGet]
        public IActionResult Index(EmployeeListFilterRequest filter)
        {
            filter.NormalizeFilter();

            return View(new EmployeeListIndexViewModel
            {
                Filter = filter
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeList(EmployeeListFilterRequest filter)
        {
            try
            {
                var result = await _employeeListService.GetPagedAsync(filter);

                return Json(BaseResult<EmployeeListPagedResponse>.Ok(
                    data: result,
                    statusMessage: "Employee data loaded successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    BaseResult<EmployeeListPagedResponse>.Fail(
                        statusMessage: $"Failed to load employee data. {ex.Message}",
                        statusCode: StatusCodes.Status500InternalServerError));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id, int? roasterYear = null, int? roasterMonth = null)
        {
            var model = await _employeeListService.BuildEditViewModelAsync(
                id,
                isReadOnly: true,
                roasterYear,
                roasterMonth);

            if (model == null)
            {
                TempData["ErrorMessage"] = "Employee data was not found.";
                return RedirectToAction(nameof(Index));
            }

            return View("Form", model);
        }

        [HttpGet]
        public async Task<IActionResult> Add(int? roasterYear = null, int? roasterMonth = null)
        {
            var model = await _employeeListService.BuildAddViewModelAsync(roasterYear, roasterMonth);

            return View("Form", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int? roasterYear = null, int? roasterMonth = null)
        {
            var model = await _employeeListService.BuildEditViewModelAsync(
                id,
                isReadOnly: false,
                roasterYear,
                roasterMonth);

            if (model == null)
            {
                TempData["ErrorMessage"] = "Employee data was not found.";
                return RedirectToAction(nameof(Index));
            }

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(EmployeeDetailRequest request)
        {
            if (!ModelState.IsValid)
            {
                var invalidModel = await _employeeListService.RebuildViewModelAsync(request);
                return View("Form", invalidModel);
            }

            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUser = GetCurrentUser();
                var hqId = currentUser?.HqId ?? 0;
                var storeId = currentUser?.StoreId ?? 0;
                var currentUserName = currentUser?.DisplayName ?? string.Empty;

                if (request.Id <= 0)
                {
                    var newEmployeeId = await _employeeListService.CreateEmployeeAsync(
                        request,
                        hqId,
                        storeId,
                        currentUserId,
                        currentUserName);

                    if (newEmployeeId <= 0)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to save employee. Please try again.");
                        var invalidModel = await _employeeListService.RebuildViewModelAsync(request);
                        return View("Form", invalidModel);
                    }

                    TempData["SuccessMessage"] = "Employee has been saved successfully.";

                    if (IsSaveAndClose(request))
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    return RedirectToAction(nameof(Edit), new { id = newEmployeeId, roasterYear = request.RoasterYear, roasterMonth = request.RoasterMonth });
                }

                var result = await _employeeListService.UpdateEmployeeAsync(
                    request,
                    hqId,
                    storeId,
                    currentUserId,
                    currentUserName);

                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update employee. Please try again.");
                    var invalidModel = await _employeeListService.RebuildViewModelAsync(request);
                    return View("Form", invalidModel);
                }

                TempData["SuccessMessage"] = "Employee has been updated successfully.";

                if (IsSaveAndClose(request))
                {
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Edit), new { id = request.Id, roasterYear = request.RoasterYear, roasterMonth = request.RoasterMonth });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var invalidModel = await _employeeListService.RebuildViewModelAsync(request);
                return View("Form", invalidModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomTimeTables(int employeeId)
        {
            try
            {
                var result = await _employeeListService.GetCustomTimeTablesAsync(employeeId);

                return Json(BaseResult<List<EmployeeCustomTimeTableDto>>.Ok(
                    data: result,
                    statusMessage: "Custom time table data loaded successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    BaseResult<List<EmployeeCustomTimeTableDto>>.Fail(
                        statusMessage: $"Failed to load custom time table. {ex.Message}",
                        statusCode: StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCustomTimeTableAjax(EmployeeCustomTimeTableRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResult.Fail(
                    statusMessage: GetModelStateErrorMessage(),
                    statusCode: StatusCodes.Status400BadRequest));
            }

            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUser = GetCurrentUser();
                var hqId = currentUser?.HqId ?? 0;
                var storeId = currentUser?.StoreId ?? 0;
                var currentUserName = currentUser?.DisplayName ?? string.Empty;

                var id = await _employeeListService.AddCustomTimeTableAsync(
                    request,
                    hqId,
                    storeId,
                    currentUserId,
                    currentUserName);

                if (id <= 0)
                {
                    return BadRequest(BaseResult.Fail(
                        statusMessage: "Failed to add custom time table.",
                        statusCode: StatusCodes.Status400BadRequest));
                }

                return Json(BaseResult<object>.Ok(
                    data: new { id },
                    statusMessage: "Custom time table has been added successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    BaseResult.Fail(
                        statusMessage: $"Failed to add custom time table. {ex.Message}",
                        statusCode: StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCustomTimeTableAjax(int id)
        {
            try
            {
                var result = await _employeeListService.DeleteCustomTimeTableAsync(
                    id,
                    GetCurrentUserId(),
                    GetCurrentUser()?.DisplayName ?? string.Empty);

                if (!result)
                {
                    return BadRequest(BaseResult.Fail(
                        statusMessage: "Failed to delete custom time table.",
                        statusCode: StatusCodes.Status400BadRequest));
                }

                return Json(BaseResult.Ok(
                    statusMessage: "Custom time table has been deleted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    BaseResult.Fail(
                        statusMessage: $"Failed to delete custom time table. {ex.Message}",
                        statusCode: StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            try
            {
                var result = await _employeeListService.DeleteEmployeeAsync(id);

                if (!result)
                {
                    return BadRequest(BaseResult.Fail(
                        statusMessage: "Failed to delete employee.",
                        statusCode: StatusCodes.Status400BadRequest));
                }

                return Json(BaseResult.Ok(
                    statusMessage: "Employee has been deleted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    BaseResult.Fail(
                        statusMessage: $"Failed to delete employee. {ex.Message}",
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

        private int GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("CurrentUserID") ?? 0;
        }

        private AccountLoginUserDto? GetCurrentUser()
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

        private static bool IsSaveAndClose(EmployeeDetailRequest request)
        {
            return string.Equals(
                request.SubmitMode,
                "SaveAndClose",
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
