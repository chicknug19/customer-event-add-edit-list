using JPP.Models.Customer.Request;
using System;
using System.Collections.Generic;

namespace JPP.Models.Customer.Responses
{
    public class CustomerDetailViewModel
    {
        public CustomerRequest Form { get; set; } = new();

        public bool IsReadOnly { get; set; }

        public bool IsEditMode => Form.ID > 0;

        public string PageTitle
        {
            get
            {
                if (IsReadOnly)
                {
                    return "Customer Detail";
                }

                return IsEditMode ? "Edit Customer" : "Add Customer";
            }
        }

        public string PageSubtitle
        {
            get
            {
                if (IsReadOnly)
                {
                    return "View customer master data";
                }

                return IsEditMode ? "Update customer master data" : "Create new customer master data";
            }
        }

        public string DisplayName => string.Join(" ", new[] { Form.FirstName, Form.MiddleName, Form.LastName }
            .Where(x => !string.IsNullOrWhiteSpace(x))).Trim();
    }
}
