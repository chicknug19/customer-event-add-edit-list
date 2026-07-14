namespace JPP.Models.HR.EmployeeList.Responses
{
    public class EmployeeListSummaryDto
    {
        public int TotalEmployee { get; set; }

        public int ActiveEmployee { get; set; }

        public int InactiveEmployee { get; set; }
    }
}
