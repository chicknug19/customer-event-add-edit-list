using JPP.Models.HR.EmployeeList.Request;

namespace JPP.Models.HR.EmployeeList.Responses
{
    public class EmployeeListIndexViewModel
    {
        public EmployeeListFilterRequest Filter { get; set; } = new();

        public string Keyword => Filter.Keyword ?? string.Empty;

        public string Status => string.IsNullOrWhiteSpace(Filter.Status)
            ? "All"
            : Filter.Status;

        public string SortColumn => string.IsNullOrWhiteSpace(Filter.SortColumn)
            ? "DisplayName"
            : Filter.SortColumn;

        public string SortDirection => string.IsNullOrWhiteSpace(Filter.SortDirection)
            ? "ASC"
            : Filter.SortDirection;

        public int PageSize => Filter.PageSize > 0
            ? Filter.PageSize
            : 20;
    }
}
