using System;

namespace JPP.Models.HR.EmployeeList.Responses
{
    public class EmployeeCustomTimeTableDto
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int LocationId { get; set; }

        public string LocationName { get; set; } = string.Empty;

        public DateTime CustomDate { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string CustomDateText => CustomDate == default
            ? string.Empty
            : CustomDate.ToString("dd MMM yyyy");

        public string StartTimeText => StartTime.HasValue
            ? StartTime.Value.ToString("HH:mm")
            : string.Empty;

        public string EndTimeText => EndTime.HasValue
            ? EndTime.Value.ToString("HH:mm")
            : string.Empty;
    }
}
