using JPP.Models.Customer.Request;
using JPP.Models.Customer.Responses;
using JPP.Models.Shared.Responses;
using JPP.Services.Interfaces;
using JPP.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JPP.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CustomerAddController : BaseController
    {
        protected override bool RequireLogin => true;

        private readonly ICustomerService _customerService;

        public CustomerAddController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CustomerAddPage()
        {
            // 1. Kita buat model kosong untuk halaman Add
            var model = new CustomerDetailViewModel
            {
                Form = new CustomerRequest(), // Form kosong untuk diisi user
                IsReadOnly = false
            };

            // 2. Kita kirim model tersebut ke View
            return View("CustomerAddPage", model);
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
                var result = await _customerService.AddCustomerAsync(request);

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