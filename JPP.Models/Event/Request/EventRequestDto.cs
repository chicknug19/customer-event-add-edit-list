using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Event.Request
{
    public class EventRequestDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Event Code is required.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event Name is required.")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Event Description is required.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Database is required.")]
        public string DatabaseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Brand is required.")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event Organizer is required.")]
        public string EventOrganizer { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date and Time is required.")]
        public DateTime? EventDateTime { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        public decimal? Duration { get; set; }


    }
}