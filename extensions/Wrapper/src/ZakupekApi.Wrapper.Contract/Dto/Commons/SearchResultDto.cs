namespace ZakupekApi.Wrapper.Contract.Dto.Commons;

public class SearchResultDto<T>
{
    public List<T> Data { get; set; } = new List<T>();
}
