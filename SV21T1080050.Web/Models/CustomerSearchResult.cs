using SV21T1080050.DomainModels;

namespace SV21T1080050.Web.Models
{
    public class CustomerSearchResult : PaginationSearchResult
    {
        public required List<Customer> Data { get; set; }
    }
}
