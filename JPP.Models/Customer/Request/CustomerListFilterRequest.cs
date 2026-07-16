using JPP.Models.Shared.Requests;

namespace JPP.Models.Customer.Request
{
    public class CustomerListFilterRequest : PaginationRequest
    {
        public string? Keyword { get; set; }

    }
}