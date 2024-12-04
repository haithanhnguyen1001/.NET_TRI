using SV21T1080050.DomainModels;

namespace SV21T1080050.Web.Models
{
    public class CategorySearchResult : PaginationSearchResult
    {
        public required List<Category> Data { get; set; }
    }

}
