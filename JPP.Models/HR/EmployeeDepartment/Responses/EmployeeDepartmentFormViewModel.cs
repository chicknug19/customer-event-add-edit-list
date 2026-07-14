using JPP.Models.HR.EmployeeDepartment.Request;

namespace JPP.Models.HR.EmployeeDepartment.Responses
{
    public class EmployeeDepartmentFormViewModel
    {
        public EmployeeDepartmentDetailRequest Form { get; set; } = new();
        public bool IsEdit => Form.Id > 0;
    }
}
