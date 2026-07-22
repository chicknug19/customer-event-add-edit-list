using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Event.Responses
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string DatabaseName { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public string EventOrganizer { get; set; } = string.Empty;

        public DateTime? EventDateTime { get; set; }

        public decimal? Duration { get; set; }

    }
}
