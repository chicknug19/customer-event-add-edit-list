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
        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; } = string.Empty;
        public string? PhoneNumber2 { get; set; }
        public string? EmailAddress { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
    }
}
