using Microsoft.AspNetCore.Mvc;
using JPP.Web.Controllers;
using JPP.Models.Customer.Responses;
using JPP.Models.Customer.Request;
using System;

namespace JPP.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerListController : BaseController
    {
        protected override bool RequireLogin => true;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = new CustomerDetailViewModel
            {
                Form = new CustomerRequest
                {
                    ID = 0,
                    Title = "Ms",
                    FirstName = "Jane",
                    MiddleName = "A.",
                    LastName = "Tester",
                    IdentityNo = "S7654321B",
                    DOB = DateTime.Today.AddYears(-28),
                    MaritalStatus = "Single",
                    Gender = "Female",
                    Nation = "SG",
                    Occupation = "Designer",
                    PhoneNumber = "91234567",
                    EmailAddress = "jane.tester@example.com",
                    BlockHouseNo = "12",
                    UnitNo = "01-02",
                    Address1 = "1 Example St",
                    City = "Singapore",
                    Country = "Singapore",
                    Zip = "123456",
                    CategoryID = 1,
                    StoreID = 1,
                    AcceptSMS = true,
                    AcceptMailEmail = false
                },
                IsReadOnly = false
            };

            return View("Add", model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var model = new CustomerDetailViewModel
            {
                Form = new CustomerRequest
                {
                    ID = id,
                    Title = "Mr",
                    FirstName = "John",
                    MiddleName = "",
                    LastName = "Doe",
                    IdentityNo = "S1234567A",
                    DOB = DateTime.Today.AddYears(-34),
                    MaritalStatus = "Married",
                    Gender = "Male",
                    Nation = "SG",
                    Occupation = "Developer",
                    PhoneNumber = "98765432",
                    EmailAddress = "john.doe@example.com",
                    BlockHouseNo = "99",
                    UnitNo = "10-11",
                    Address1 = "10 Demo Rd",
                    City = "Singapore",
                    Country = "Singapore",
                    Zip = "654321",
                    CategoryID = 2,
                    StoreID = 3,
                    AcceptSMS = false,
                    AcceptMailEmail = true
                },
                IsReadOnly = false
            };

            return View("Edit", model);
        }
    }
}