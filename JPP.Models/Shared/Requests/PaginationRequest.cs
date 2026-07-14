using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Shared.Requests
{
    public class PaginationRequest
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public string SortColumn { get; set; } = string.Empty;

        public string SortDirection { get; set; } = "ASC";

        public int Skip => Page <= 1 ? 0 : (Page - 1) * PageSize;
    }
}
