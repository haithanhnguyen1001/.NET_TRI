using SV21T1080050.DomainModels;

namespace SV21T1080050.Web.Models
{
    public class ShipperSearchResult : PaginationSearchResult
    {
        public required List<Shipper> Data { get; set; }
    }
}
