using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Models.Event.Request
{
    public class EventRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Event Code is required.")]
        [StringLength(20, ErrorMessage = "Event Code cannot exceed 20 characters.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event Name is required.")]
        [StringLength(50, ErrorMessage = "Event Name cannot exceed 50 characters.")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Event Name is required.")]
        [StringLength(150, ErrorMessage = "Description cannot exceed 150 characters.")]
        public string Description { get; set; } = string.Empty;
    }
}