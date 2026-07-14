using JPP.Models.Shared.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Admin.StoreList.Request
{
    public class StoreListFilterRequest : PaginationRequest
    {
        public string? Keyword { get; set; }

        public string Status { get; set; } = "All";
    }
}
