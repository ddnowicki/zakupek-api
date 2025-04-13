namespace ZakupekApi.Wrapper.Contract.Dto.Commons;

public class PaginatedResultDto<T>
{
    public List<T> Data { get; set; } = new List<T>();
    
    public PaginationDto Pagination { get; set; } = new PaginationDto();
}

public class PaginationDto
{
    public int Page { get; set; }
    
    public int PageSize { get; set; }
    
    public int Total { get; set; }
}
