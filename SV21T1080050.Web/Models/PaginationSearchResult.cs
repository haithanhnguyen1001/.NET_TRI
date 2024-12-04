using SV21T1080050.DomainModels;

namespace SV21T1080050.Web.Models
{
    public abstract class PaginationSearchResult
    {
        /// <summary>
        /// Cho biết trang cần hiển thị
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Số dòng được hiển thị trên mỗi trang
        /// </summary>
        public int PageSize {  get; set; }
        /// <summary>
        /// Chuỗi giá trị cần tìm kiếm
        /// </summary>
        public string SearchValue { get; set; } = "";
        /// <summary>
        /// Cho biết tổng số dòng dữ liệu mà chúng ta truy vấn được
        /// </summary>
        public int RowCount {  get; set; }
        /// <summary>
        /// Cho biết số trang mà chúng ta tìm kiếm được
        /// </summary>
        public int PageCount
        {
            get
            {
                if (PageSize == 0)//nếu = 0 tức là k thực hiệc việc phân trang và trả về 1 trang
                    return 1;
                int c = RowCount / PageSize;
                if (RowCount % PageSize > 0)
                    c += 1;
                return c;

            }
        }
    }
}
