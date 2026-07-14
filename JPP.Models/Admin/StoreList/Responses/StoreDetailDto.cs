using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Admin.StoreList.Responses
{
    public class StoreDetailDto
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string StoreName { get; set; } = string.Empty;

        public bool IsPublished { get; set; }

        public int InChargeId { get; set; }

        public string InChargeName { get; set; } = string.Empty;

        public string BizRegNo { get; set; } = string.Empty;

        public string BlockHouseNo { get; set; } = string.Empty;

        public string UnitNo { get; set; } = string.Empty;

        public string Address1 { get; set; } = string.Empty;

        public string Address2 { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string PostalCode { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Fax { get; set; } = string.Empty;

        public string IpAddress { get; set; } = string.Empty;

        public string PortNo { get; set; } = string.Empty;

        public string DbName { get; set; } = string.Empty;

        public string DbLogin { get; set; } = string.Empty;

        public string DbPassword { get; set; } = string.Empty;

        public string LastUpdatedText { get; set; } = string.Empty;
    }
}
