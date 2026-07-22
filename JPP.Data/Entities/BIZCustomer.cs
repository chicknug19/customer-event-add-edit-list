
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
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Address1 { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}
