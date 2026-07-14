using JPP.Models.HR.EmployeeList.Request;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JPP.Models.HR.EmployeeList.Responses
{
    public class EmployeeDetailViewModel
    {
        public EmployeeDetailRequest Form { get; set; } = new();

        public List<EmployeeOptionDto> Roles { get; set; } = new();

        public List<EmployeeOptionDto> Departments { get; set; } = new();

        public List<EmployeeOptionDto> Supervisors { get; set; } = new();

        public List<EmployeeOptionDto> TimeCategories { get; set; } = new();

        public List<EmployeeOptionDto> WorkLocations { get; set; } = new();

        public List<EmployeeCustomTimeTableDto> CustomTimeTables { get; set; } = new();

        public bool IsReadOnly { get; set; }

        public bool IsEditMode => Form.Id > 0;

        public string PageTitle
        {
            get
            {
                if (IsReadOnly)
                {
                    return "Employee Detail";
                }

                return IsEditMode ? "Edit Employee" : "Add Employee";
            }
        }

        public string PageSubtitle
        {
            get
            {
                if (IsReadOnly)
                {
                    return "View employee master data";
                }

                return IsEditMode ? "Update employee master data" : "Create new employee master data";
            }
        }

        public string DisplayName => string.Join(" ", new[] { Form.FirstName, Form.LastName }
            .Where(x => !string.IsNullOrWhiteSpace(x))).Trim();

        public IEnumerable<int> RoasterYears
        {
            get
            {
                var currentYear = DateTime.Today.Year;

                for (var year = currentYear + 1; year >= currentYear - 5; year--)
                {
                    yield return year;
                }
            }
        }
    }
}
