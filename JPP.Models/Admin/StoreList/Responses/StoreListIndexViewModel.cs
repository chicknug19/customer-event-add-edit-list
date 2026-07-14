using JPP.Models.Admin.StoreList.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Admin.StoreList.Responses
{
    public class StoreListIndexViewModel
    {
        public StoreListFilterRequest Filter { get; set; } = new();

        public string Keyword => Filter.Keyword ?? string.Empty;

        public string Status => string.IsNullOrWhiteSpace(Filter.Status)
            ? "All"
            : Filter.Status;

        public string SortColumn => string.IsNullOrWhiteSpace(Filter.SortColumn)
            ? "StoreName"
            : Filter.SortColumn;

        public string SortDirection => string.IsNullOrWhiteSpace(Filter.SortDirection)
            ? "ASC"
            : Filter.SortDirection;

        public int PageSize => Filter.PageSize > 0
            ? Filter.PageSize
            : 20;
    }
}
