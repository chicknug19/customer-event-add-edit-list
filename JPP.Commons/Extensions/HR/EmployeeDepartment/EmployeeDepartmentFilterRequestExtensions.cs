using JPP.Models.HR.EmployeeDepartment.Request;

namespace JPP.Commons.Extensions
{
    public static class EmployeeDepartmentFilterRequestExtensions
    {
        public static void NormalizeFilter(this EmployeeDepartmentFilterRequest filter)
        {
            filter.Keyword = filter.Keyword?.Trim();
            filter.Status = filter.Status is "Active" or "Inactive" ? filter.Status : "All";
            filter.Page = filter.Page <= 0 ? 1 : filter.Page;
            filter.PageSize = filter.PageSize is < 1 or > 100 ? 20 : filter.PageSize;
            filter.SortColumn = filter.SortColumn is "Id" or "Code" or "Name" or "Inactive" or "LastUpdated"
                ? filter.SortColumn
                : "Name";
            filter.SortDirection = string.Equals(filter.SortDirection, "DESC", StringComparison.OrdinalIgnoreCase)
                ? "DESC"
                : "ASC";
        }
    }
}
