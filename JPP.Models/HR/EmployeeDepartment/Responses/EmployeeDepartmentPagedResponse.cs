using JPP.Models.Shared.Responses;

namespace JPP.Models.HR.EmployeeDepartment.Responses
{
    public class EmployeeDepartmentPagedResponse
    {
        public EmployeeDepartmentSummaryDto Summary { get; set; } = new();
        public PagedResult<EmployeeDepartmentItemDto> Departments { get; set; } = new();
    }
}
