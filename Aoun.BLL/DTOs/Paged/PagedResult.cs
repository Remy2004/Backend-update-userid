namespace Aoun.BLL.DTOs.Paged
{
    public class PagedResult <T>         //Response Pagination DTO
    {
        
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int TotalCount { get; set; }
            public List<T> Data { get; set; }
        

    }
}
