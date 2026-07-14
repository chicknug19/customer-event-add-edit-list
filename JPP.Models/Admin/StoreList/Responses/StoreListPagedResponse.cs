using JPP.Models.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Admin.StoreList.Responses
{
    public class StoreListPagedResponse
    {
        public StoreListSummaryDto Summary { get; set; } = new();

        public PagedResult<StoreListItemDto> Stores { get; set; } = new();
    }
}
