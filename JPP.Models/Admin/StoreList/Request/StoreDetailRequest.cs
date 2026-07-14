using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Admin.StoreList.Request
{
    public class StoreDetailRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Code is required.")]
        [StringLength(4, ErrorMessage = "Code maximum length is 4 characters.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Store Name is required.")]
        [StringLength(255, ErrorMessage = "Store Name maximum length is 255 characters.")]
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

        public string SubmitMode { get; set; } = "Save";
    }
}
