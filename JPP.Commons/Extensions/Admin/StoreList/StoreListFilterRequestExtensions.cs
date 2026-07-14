using JPP.Models.Admin.StoreList.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Commons.Extensions
{
    public static class StoreListFilterRequestExtensions
    {
        private static readonly string[] AllowedSortColumns =
        {
            "Id",
            "Code",
            "StoreName",
            "Phone",
            "InChargeName",
            "Inactive"
        };

        public static void NormalizeFilter(this StoreListFilterRequest filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            filter.Keyword = filter.Keyword.NormalizeNullableText();
            filter.Status = filter.Status.NormalizeTextOrDefault("All");

            filter.NormalizePagingAndSorting(
                defaultSortColumn: "StoreName",
                defaultSortDirection: "ASC",
                allowedSortColumns: AllowedSortColumns,
                defaultPageSize: 20,
                maxPageSize: 100);
        }
    }
}
