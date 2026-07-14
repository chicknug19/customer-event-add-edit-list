using JPP.Models.Admin.StoreList.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Admin.StoreList.Responses
{
    public class StoreDetailViewModel
    {
        public StoreDetailRequest Form { get; set; } = new();

        public List<StoreEmployeeOptionDto> Employees { get; set; } = new();

        public bool IsEditMode => Form.Id > 0;

        public bool CanChangeInCharge { get; set; } = true;

        public string LastUpdatedText { get; set; } = string.Empty;

        public string PageTitle => IsEditMode ? "Edit Store" : "Add Store";

        public string SubmitAction => IsEditMode ? "Edit" : "Add";
    }
}
