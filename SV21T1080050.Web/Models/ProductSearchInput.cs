namespace SV21T1080050.Web.Models
{
    public class ProductSearchInput:PaginationSearchInput
    {
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
        //public int CustomerID { get; set; } = 0;
        //public string Province { get; set; } = "";
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;

    }
}
