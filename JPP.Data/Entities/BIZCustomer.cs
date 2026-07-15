using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JPP.Data.Entities
{
    [Table("BIZ_Customer")]
    public class BIZCustomer
    {
        [Key]
        public int ID { get; set; }
        public Guid UID { get; set; } = Guid.NewGuid();
        public DateTime? DateCreated { get; set; }
        public DateTime? LastUpdated { get; set; }
        public bool Inactive { get; set; } 
        public string? Title { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string IdentityNo { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public string? MaritalStatus { get; set; }
        public string? Gender { get; set; }
        public string? Race { get; set; }
        public string? Nation { get; set; }
        public string? Occupation { get; set; }
        public string? PhoneNumber { get; set; }
        public string? HPNum { get; set; }
        public string? EmailAddress { get; set; }
        public string Password { get; set; } = string.Empty;
        public string? Password_WEB { get; set; }
        public string? BlockHouseNo { get; set; }
        public string? UnitNo { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Zip { get; set; }
        public int? CategoryID { get; set; }
        public int? StoreID { get; set; }
        public bool? AcceptSMS { get; set; }
        public bool? AcceptMailEmail { get; set; }
    }
}
