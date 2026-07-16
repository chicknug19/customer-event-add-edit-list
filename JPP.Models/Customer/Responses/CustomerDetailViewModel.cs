using JPP.Models.Customer.Request;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace JPP.Models.Customer.Responses
{
    public class CustomerDetailViewModel
    {
        public CustomerRequest Form { get; set; } = new();
        public bool IsReadOnly { get; set; }
        public bool IsEditMode => Form.ID > 0;
        public string DisplayName => $"{Form.FirstName} {Form.LastName}".Trim();

        public string PageTitle => IsEditMode ? "Edit Customer" : "Add Customer";
        public string PageSubtitle => IsEditMode ? "Update customer master data" : "Create new customer master data";

        // TAMBAHKAN INI UNTUK DROPDOWN
        public IEnumerable<SelectListItem> EventOptions { get; set; } = new List<SelectListItem>();
    }
}