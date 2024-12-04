using SV21T1080050.DomainModels;

namespace SV21T1080050.Web.Models
{
    public class SupplierSearchResult : PaginationSearchResult
    {
        public required List<Supplier> Data { get; set; }
    }
}
