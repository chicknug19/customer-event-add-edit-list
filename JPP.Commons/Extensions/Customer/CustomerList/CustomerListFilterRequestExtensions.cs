using JPP.Models.Customer.Request;
using JPP.Models.Shared.Requests;

namespace JPP.Commons.Extensions
{
    public static class CustomerListFilterRequestExtensions
    {
        private static readonly string[] AllowedSortColumns =
        {
            "CustomerID",
            "FullName",
            "Address1",
            "PhoneNumber"
        };

        public static void NormalizeFilter(this CustomerListFilterRequest filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            filter.Keyword = filter.Keyword.NormalizeNullableText();

            filter.NormalizePagingAndSorting(
                defaultSortColumn: "FullName",
                defaultSortDirection: "ASC",
                allowedSortColumns: AllowedSortColumns,
                defaultPageSize: 20,
                maxPageSize: 100);
        }
    }
}