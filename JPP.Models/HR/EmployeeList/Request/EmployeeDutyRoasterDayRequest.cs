using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JPP.Models.HR.EmployeeList.Request
{
    public class EmployeeDutyRoasterDayRequest : IValidatableObject
    {
        public int Id { get; set; }

        public DateTime WorkDate { get; set; }

        public bool OnDuty { get; set; } = true;

        public string Remarks { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!OnDuty && string.IsNullOrWhiteSpace(Remarks))
            {
                yield return new ValidationResult(
                    $"Please select a remark for {WorkDate:dd MMM yyyy}.",
                    new[] { nameof(Remarks) });
            }
        }
    }
}
