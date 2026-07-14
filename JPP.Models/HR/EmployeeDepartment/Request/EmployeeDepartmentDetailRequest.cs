using System.ComponentModel.DataAnnotations;

namespace JPP.Models.HR.EmployeeDepartment.Request
{
    public class EmployeeDepartmentDetailRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        public bool Inactive { get; set; }
        public string? SubmitAction { get; set; }
    }
}
