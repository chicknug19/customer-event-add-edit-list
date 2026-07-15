using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using JPP.Web.Controllers;
using JPP.Services.Interfaces;

namespace JPP.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CustomerListController : BaseController
    {
        protected override bool RequireLogin => true;

        private readonly ICustomerListService _customerListService;

        public CustomerListController(ICustomerListService customerListService)
        {
            _customerListService = customerListService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerList()
        {
            try
            {
                var result = await _customerListService.GetCustomerListAsync();

                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new 
                    { 
                        Success = false, 
                        Message = $"Failed to load customer data. {ex.Message}",
                        StatusCode = StatusCodes.Status500InternalServerError
                    });
            }
        }
    }
}