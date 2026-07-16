using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Customer.Responses.CustomerDto
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string? PhoneNumber2 { get; set; }
        public string? EmailAddress { get; set; }
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
    }
}
