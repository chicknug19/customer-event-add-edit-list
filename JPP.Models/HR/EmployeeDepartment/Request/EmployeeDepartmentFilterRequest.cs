using JPP.Models.Shared.Requests;

namespace JPP.Models.HR.EmployeeDepartment.Request
{
    public class EmployeeDepartmentFilterRequest : PaginationRequest
    {
        public string? Keyword { get; set; }
        public string Status { get; set; } = "All";
    }
}
