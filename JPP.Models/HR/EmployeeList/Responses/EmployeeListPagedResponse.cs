using JPP.Models.Shared.Responses;

namespace JPP.Models.HR.EmployeeList.Responses
{
    public class EmployeeListPagedResponse
    {
        public EmployeeListSummaryDto Summary { get; set; } = new();

        public PagedResult<EmployeeListItemDto> Employees { get; set; } = new();
    }
}
