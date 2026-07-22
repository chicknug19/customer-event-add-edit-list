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
        private readonly IStoreDropdownService _storeService;

        public CustomerAddController(ICustomerAddService customerService, IEventDropdownService eventService, IStoreDropdownService storeService)
        {
            _customerService = customerService;
            _eventService = eventService;
            _storeService = storeService;
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
            var stores = await _storeService.GetStoreDropdownListAsync();

            var model = new CustomerDetailViewModel
            {
                Form = new CustomerRequest(),
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

            return View("CustomerAddPage", model);
        }

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
                return View("CustomerAddPage", model);
            }

            var result = await _customerService.AddCustomerAsync(form);

            if (result.StatusCode == 200)
            {
                TempData["SuccessMessage"] = "Customer successfully saved!";

                if (SubmitMode == "SaveAndClose")
                {
                    return RedirectToAction("Index", "CustomerList", new { area = "Customer" });
                }

                return RedirectToAction("CustomerAddPage");
            }
            else
            {
                TempData["ErrorMessage"] = result.StatusMessage;

                // JIKA GAGAL SIMPAN, AMBIL ULANG JUGA KEDUA DATANYA
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
                return View("CustomerAddPage", model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetPreviewAccountNumber(int storeId)
        {
            var accNo = await _customerService.GetPreviewAccountNumberAsync(storeId);
            return Json(new { accountNumber = accNo });
        }


    }
}