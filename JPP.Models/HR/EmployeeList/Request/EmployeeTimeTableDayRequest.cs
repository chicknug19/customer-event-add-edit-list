using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JPP.Models.HR.EmployeeList.Request
{
    public class EmployeeTimeTableDayRequest : IValidatableObject
    {
        public string DayName { get; set; } = string.Empty;

        public bool Working { get; set; }

        public int LocationId { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Working)
            {
                yield break;
            }

            if (LocationId <= 0)
            {
                yield return new ValidationResult(
                    $"Please select work location for {DayName}.",
                    new[] { nameof(LocationId) });
            }

            if (!StartTime.HasValue)
            {
                yield return new ValidationResult(
                    $"Please select start time for {DayName}.",
                    new[] { nameof(StartTime) });
            }

            if (!EndTime.HasValue)
            {
                yield return new ValidationResult(
                    $"Please select end time for {DayName}.",
                    new[] { nameof(EndTime) });
            }

            if (StartTime.HasValue && EndTime.HasValue && StartTime.Value >= EndTime.Value)
            {
                yield return new ValidationResult(
                    $"End time must be greater than start time for {DayName}.",
                    new[] { nameof(EndTime) });
            }
        }
    }
}
