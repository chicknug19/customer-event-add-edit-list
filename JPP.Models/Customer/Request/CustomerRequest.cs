using JPP.Models.HR.EmployeeList.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Customer.Request
{
    public class CustomerRequest
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        public string? LastName { get; set; }

        [Required(ErrorMessage = "Phone Number cannot be empty.")]
        [RegularExpression(@"^8[0-9]{5,14}$", ErrorMessage = "The mobile number must start with the digit 8 (without a leading 0) and contain only digits.")]
        public string PhoneNumber { get; set; } = string.Empty;

        //public string? PhoneNumber2 { get; set; }
        [Required(ErrorMessage = "Email Address cannot be empty.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format. Example: name@domain.com")]
        public string? EmailAddress { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address1 { get; set; } = string.Empty;

        //public string? Address2 { get; set; }

        [Required(ErrorMessage = "Choosing an event is required")]
        public int? EventId { get; set; }

        public string? EventName { get; set; }

        [Required(ErrorMessage = "Choosing a store is required")]
        public int? StoreId { get; set; }

        public string? AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required")]
        public int? Age { get; set; }
    }
}