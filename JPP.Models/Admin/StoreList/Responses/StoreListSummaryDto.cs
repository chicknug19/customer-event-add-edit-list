using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Admin.StoreList.Responses
{
    public class StoreListSummaryDto
    {
        public int TotalStore { get; set; }
        public int ActiveStore { get; set; }
        public int DeletedStore { get; set; }
    }
}
