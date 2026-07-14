using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Admin.StoreList.Responses
{
    public class StoreListItemDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string InChargeName { get; set; } = string.Empty;
        public bool Inactive { get; set; }
    }
}
