using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JPP.Data.Entities
{
    [Table("BIZ_Stores")]
    public class Store
    {
        [Key]
        public int ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
    }
}