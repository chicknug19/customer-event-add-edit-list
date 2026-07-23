namespace JPP.Models.Event.Responses
{
    public class EventListDto
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string EventOrganizer { get; set; } = string.Empty;
        public DateTime? EventDateTime { get; set; }
        public decimal Duration { get; set; }
    }
}