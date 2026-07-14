namespace JPP.Models.HR.EmployeeDepartment.Responses
{
    public class EmployeeDepartmentItemDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool Inactive { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
