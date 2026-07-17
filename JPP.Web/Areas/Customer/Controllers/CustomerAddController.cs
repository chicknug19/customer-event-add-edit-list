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
    public class CustomerAddController : BaseController
    {

        protected override bool RequireLogin => true;

        private readonly ICustomerAddService _customerService;
        private readonly IEventDropdownService _eventService;

        public CustomerAddController(ICustomerAddService customerService, IEventDropdownService eventService)
        {
            _customerService = customerService;
            _eventService = eventService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CustomerAddPage() 
        {
            var events = await _eventService.GetDropdownListAsync();

            var model = new CustomerDetailViewModel
            {
                Form = new CustomerRequest(),
                IsReadOnly = false,
                EventOptions = events.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = $"{e.Code} - {e.Name}"
                })
            };

            return View("CustomerAddPage", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(CustomerRequest form, string SubmitMode)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Mohon periksa kembali kelengkapan data Anda.";

                var events = await _eventService.GetDropdownListAsync();

                var model = new CustomerDetailViewModel
                {
                    Form = form,
                    IsReadOnly = false,

                    EventOptions = events.Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = $"{e.Code} - {e.Name}"
                    })
                };
                return View("CustomerAddPage", model);
            }

            var result = await _customerService.AddCustomerAsync(form);

            if (result.StatusCode == 200)
            {
                TempData["SuccessMessage"] = "Customer berhasil disimpan!";

                if (SubmitMode == "SaveAndClose")
                {
                    return RedirectToAction("Index", "CustomerList", new { area = "Customer" });
                }

                return RedirectToAction("CustomerAddPage");
            }
            else
            {
                TempData["ErrorMessage"] = result.StatusMessage;

                var events = await _eventService.GetDropdownListAsync();

                var model = new CustomerDetailViewModel
                {
                    Form = form,
                    IsReadOnly = false,
                    EventOptions = events.Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = $"{e.Code} - {e.Name}"
                    })
                };
                return View("CustomerAddPage", model);
            }
        }



    }
}