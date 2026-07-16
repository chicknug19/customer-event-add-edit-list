namespace JPP.Models.Customer.Responses
{
    public class CustomerListDto
    {
        public int CustomerID { get; set; }
        public string FullName { get; set; } = string.Empty;  // Combine FirstName + LastName
        public string? Address1 { get; set; }
        public string? PhoneNumber { get; set; }
    }
}