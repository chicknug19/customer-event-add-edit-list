namespace JPP.Models.Customer.Responses
{
    public class CustomerDiagnosticDto
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }
            public int EventId { get; set; }
            public string Type { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public DateTime LogDate {get; set; }
        }
    
    public class NewCustomerDiagnosticDto
    {
        public int HQID { get; set; }
        public int CustomerId { get; set; }
        public int EventId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }

    public class LatestCustomerEventDto
    {
        public int HQID { get; set; }
        public int EventId { get; set; }
    }
}