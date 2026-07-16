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
        public async Task<IActionResult> Save(CustomerRequest form, string SubmitMode)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Mohon periksa kembali kelengkapan data Anda.";

                var model = new CustomerDetailViewModel
                {
                    Form = form,
                    IsReadOnly = false
                };
                return View("CustomerAddPage", model);
            }

            // Panggil Service untuk menyimpan ke database
            var result = await _customerService.AddCustomerAsync(form);

            if (result.StatusCode == 200)
            {
                TempData["SuccessMessage"] = "Customer berhasil disimpan!";

                // Jika user menekan tombol "Save And Close", kembali ke halaman List
                if (SubmitMode == "SaveAndClose")
                {
                    return RedirectToAction("Index", "CustomerList", new { area = "Customer" });
                }

                // Jika hanya menekan "Save", tetap di halaman ini dan reset form (atau bisa redirect ke mode Edit nantinya)
                return RedirectToAction("CustomerAddPage");
            }
            else
            {
                // Jika gagal simpan (misal KTP duplikat)
                TempData["ErrorMessage"] = result.StatusMessage;

                var model = new CustomerDetailViewModel
                {
                    Form = form,
                    IsReadOnly = false
                };
                return View("CustomerAddPage", model);
            }
        }



    }
}