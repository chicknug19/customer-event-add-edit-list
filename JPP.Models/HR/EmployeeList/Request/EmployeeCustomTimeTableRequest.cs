using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JPP.Models.HR.EmployeeList.Request
{
    public class EmployeeCustomTimeTableRequest : IValidatableObject
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Custom date is required.")]
        public DateTime? CustomDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Location is required.")]
        public int LocationId { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        public TimeSpan? StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        public TimeSpan? EndTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EmployeeId <= 0)
            {
                yield return new ValidationResult("Please save employee before adding custom time table.", new[] { nameof(EmployeeId) });
            }

            if (StartTime.HasValue && EndTime.HasValue && StartTime.Value >= EndTime.Value)
            {
                yield return new ValidationResult("Custom end time must be greater than start time.", new[] { nameof(EndTime) });
            }
        }
    }
}
