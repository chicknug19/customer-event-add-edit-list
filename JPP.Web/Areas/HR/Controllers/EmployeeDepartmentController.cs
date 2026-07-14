using JPP.Commons.Extensions;
using JPP.Models.HR.EmployeeDepartment.Request;
using JPP.Models.HR.EmployeeDepartment.Responses;
using JPP.Models.Shared.Responses;
using JPP.Services.Interfaces;
using JPP.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace JPP.Web.Areas.HR.Controllers
{
    [Area("HR")]
    public class EmployeeDepartmentController : BaseController
    {
        protected override bool RequireLogin => true;
        private readonly IEmployeeDepartmentService _service;

        public EmployeeDepartmentController(IEmployeeDepartmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Index(EmployeeDepartmentFilterRequest filter)
        {
            filter.NormalizeFilter();
            return View(new EmployeeDepartmentIndexViewModel { Filter = filter });
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeDepartments(EmployeeDepartmentFilterRequest filter)
        {
            try
            {
                var result = await _service.GetPagedAsync(filter);
                return Json(BaseResult<EmployeeDepartmentPagedResponse>.Ok(result, "Department data loaded successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, BaseResult<EmployeeDepartmentPagedResponse>.Fail($"Failed to load department data. {ex.Message}", 500));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Add() => View("Form", await _service.BuildAddViewModelAsync());

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _service.BuildEditViewModelAsync(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Department data was not found.";
                return RedirectToAction(nameof(Index));
            }
            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(EmployeeDepartmentFormViewModel model)
        {
            var request = model.Form;
            if (!string.IsNullOrWhiteSpace(request.Code) && await _service.CodeExistsAsync(request.Code, request.Id))
                ModelState.AddModelError("Form.Code", "Department code already exists.");

            if (!ModelState.IsValid)
                return View("Form", new EmployeeDepartmentFormViewModel { Form = request });

            try
            {
                var user = GetCurrentUser();
                if (request.Id <= 0)
                {
                    var id = await _service.CreateAsync(request, GetCurrentUserId(), user?.DisplayName ?? "", user?.HqId ?? 0, user?.StoreId ?? 0);
                    TempData["SuccessMessage"] = "Department has been saved successfully.";
                    return string.Equals(request.SubmitAction, "SaveAndClose", StringComparison.OrdinalIgnoreCase)
                        ? RedirectToAction(nameof(Index))
                        : RedirectToAction(nameof(Edit), new { id });
                }

                var result = await _service.UpdateAsync(request, GetCurrentUserId(), user?.DisplayName ?? "", user?.HqId ?? 0, user?.StoreId ?? 0);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update department.");
                    return View("Form", new EmployeeDepartmentFormViewModel { Form = request });
                }
                TempData["SuccessMessage"] = "Department has been updated successfully.";
                return string.Equals(request.SubmitAction, "SaveAndClose", StringComparison.OrdinalIgnoreCase)
                    ? RedirectToAction(nameof(Index))
                    : RedirectToAction(nameof(Edit), new { id = request.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Form", new EmployeeDepartmentFormViewModel { Form = request });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            try
            {
                var user = GetCurrentUser();
                var result = await _service.DeleteAsync(id, GetCurrentUserId(), user?.DisplayName ?? "", user?.HqId ?? 0, user?.StoreId ?? 0);
                return result
                    ? Json(BaseResult.Ok("Department has been deleted successfully."))
                    : BadRequest(BaseResult.Fail("Failed to delete department.", 400));
            }
            catch (Exception ex)
            {
                return StatusCode(500, BaseResult.Fail($"Failed to delete department. {ex.Message}", 500));
            }
        }
    }
}
