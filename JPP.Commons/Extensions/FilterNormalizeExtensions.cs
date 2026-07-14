using JPP.Models.Shared.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Commons.Extensions
{
    public static class FilterNormalizeExtensions
    {
        public static string? NormalizeNullableText(this string? value)
        {
            var normalizedValue = value?.Trim();

            return string.IsNullOrWhiteSpace(normalizedValue)
                ? null
                : normalizedValue;
        }

        public static string NormalizeTextOrDefault(this string? value, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(value)
                ? defaultValue
                : value.Trim();
        }

        public static void NormalizePagination(
            this PaginationRequest filter,
            int defaultPageSize = 20,
            int maxPageSize = 100)
        {
            ArgumentNullException.ThrowIfNull(filter);

            if (filter.Page < 1)
            {
                filter.Page = 1;
            }

            if (filter.PageSize < 1)
            {
                filter.PageSize = defaultPageSize;
            }

            if (filter.PageSize > maxPageSize)
            {
                filter.PageSize = maxPageSize;
            }
        }

        public static void NormalizeSorting(
            this PaginationRequest filter,
            string defaultSortColumn,
            string defaultSortDirection = "ASC",
            string[]? allowedSortColumns = null)
        {
            ArgumentNullException.ThrowIfNull(filter);

            filter.SortColumn = string.IsNullOrWhiteSpace(filter.SortColumn)
                ? defaultSortColumn
                : filter.SortColumn.Trim();

            filter.SortDirection = string.IsNullOrWhiteSpace(filter.SortDirection)
                ? defaultSortDirection
                : filter.SortDirection.Trim().ToUpperInvariant();

            if (filter.SortDirection != "ASC" && filter.SortDirection != "DESC")
            {
                filter.SortDirection = defaultSortDirection;
            }

            if (allowedSortColumns != null &&
                allowedSortColumns.Length > 0 &&
                !allowedSortColumns.Contains(filter.SortColumn, StringComparer.OrdinalIgnoreCase))
            {
                filter.SortColumn = defaultSortColumn;
            }
        }

        public static void NormalizePagingAndSorting(
            this PaginationRequest filter,
            string defaultSortColumn,
            string defaultSortDirection = "ASC",
            string[]? allowedSortColumns = null,
            int defaultPageSize = 20,
            int maxPageSize = 100)
        {
            ArgumentNullException.ThrowIfNull(filter);

            filter.NormalizePagination(
                defaultPageSize: defaultPageSize,
                maxPageSize: maxPageSize);

            filter.NormalizeSorting(
                defaultSortColumn: defaultSortColumn,
                defaultSortDirection: defaultSortDirection,
                allowedSortColumns: allowedSortColumns);
        }
    }
}
