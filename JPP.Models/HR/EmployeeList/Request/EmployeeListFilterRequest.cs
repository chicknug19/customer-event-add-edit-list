using JPP.Models.Shared.Requests;

namespace JPP.Models.HR.EmployeeList.Request
{
    public class EmployeeListFilterRequest : PaginationRequest
    {
        public string? Keyword { get; set; }

        public string Status { get; set; } = "All";
    }
}
