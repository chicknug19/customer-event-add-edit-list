using JPP.Models.HR.EmployeeList.Request;

namespace JPP.Commons.Extensions
{
    public static class EmployeeListFilterRequestExtensions
    {
        private static readonly string[] AllowedSortColumns =
        {
            "Id",
            "Number",
            "DisplayName",
            "Department",
            "RoleName",
            "HPNum",
            "SupervisorDisplayName",
            "HRCategory",
            "EmailAddress",
            "Inactive"
        };

        public static void NormalizeFilter(this EmployeeListFilterRequest filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            filter.Keyword = filter.Keyword.NormalizeNullableText();
            filter.Status = filter.Status.NormalizeTextOrDefault("All");

            filter.NormalizePagingAndSorting(
                defaultSortColumn: "DisplayName",
                defaultSortDirection: "ASC",
                allowedSortColumns: AllowedSortColumns,
                defaultPageSize: 20,
                maxPageSize: 100);
        }
    }
}
