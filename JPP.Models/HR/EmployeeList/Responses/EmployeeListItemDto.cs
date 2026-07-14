namespace JPP.Models.HR.EmployeeList.Responses
{
    public class EmployeeListItemDto
    {
        public int Id { get; set; }

        public string Number { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;

        public string HPNum { get; set; } = string.Empty;

        public string SupervisorDisplayName { get; set; } = string.Empty;

        public string HRCategory { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        public bool Inactive { get; set; }
    }
}
