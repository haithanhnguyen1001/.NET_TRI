using SV21T1080050.DomainModels;

namespace SV21T1080050.Web.Models
{
    public class EmployeeSearchResult : PaginationSearchResult
    {
        public required List<Employee> Data { get; set; }
    }

}
