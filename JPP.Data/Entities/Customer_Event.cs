using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JPP.Data.Entities
{
    [Table("Customer_Event")]
    public class CustomerEvent
    {
        [Key]
        public int ID { get; set; }

        public Guid UID { get; set; }

        public int HQID { get; set; }

        public int CustomerID { get; set; }

        public int EventID { get; set; }

        public bool InActive { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public bool IsUpdating { get; set; }

        public DateTime? LastHQSync { get; set; }
    }
}