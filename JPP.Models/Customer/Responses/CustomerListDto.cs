namespace JPP.Models.Customer.Responses
{
    public class CustomerListDto
        {
            public int CustomerID { get; set; }
            public string? AccNo { get; set; }
            public string FullName { get; set; } = string.Empty;  // Combine FirstName + LastName
            public string? PhoneNumber { get; set; }
            public int? Age { get; set; }
            public string? Address1 { get; set; }
            public string Kecamatan { get; set; } = string.Empty;
            public int? EventId { get; set; }
            public string? EventName { get; set; }
            public string Skin { get; set; } = "No";
            public string Dental { get; set; } = "No";
        }
}