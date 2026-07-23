using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using JPP.Web.Controllers;
using JPP.Services.Interfaces;
using JPP.Models.Customer.Request;

namespace JPP.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CustomerListController : BaseController
    {
        protected override bool RequireLogin => true;

        private readonly ICustomerListService _customerListService;
        private readonly ICustomerStoreDropdownService _storeDropdownService;
        private readonly IEventDropdownService _eventDropdownService;
        private readonly ICustomerDiagnosticService _customerDiagnosticService;

        public CustomerListController(
            ICustomerListService customerListService,
            ICustomerStoreDropdownService storeDropdownService,
            IEventDropdownService eventDropdownService,
            ICustomerDiagnosticService customerDiagnosticService)
        {
            _customerListService = customerListService;
            _storeDropdownService = storeDropdownService;
            _eventDropdownService = eventDropdownService;
            _customerDiagnosticService = customerDiagnosticService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerList([FromQuery] CustomerListFilterRequest filter)
        {
            try
            {
                var result = await _customerListService.GetCustomerListAsync(filter);
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

        [HttpGet]
        public async Task<IActionResult> GetStoreOptions()
        {
            var stores = await _storeDropdownService.GetDropdownListAsync();
            return Json(stores);
        }

        [HttpGet]
        public async Task<IActionResult> GetEventOptions()
        {
            var events = await _eventDropdownService.GetDropdownListAsync();
            return Json(events);
        }

        [HttpGet]
        public async Task<IActionResult> GetDiagnosticList([FromQuery] int customerId)
        {
            try
            {
                var result = await _customerDiagnosticService.GetCustomerDiagnosticAsync(customerId);
                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        Success = false,
                        Message = $"Failed to load diagnostic history. {ex.Message}",
                        StatusCode = StatusCodes.Status500InternalServerError
                    });
            }
        }
    }
}