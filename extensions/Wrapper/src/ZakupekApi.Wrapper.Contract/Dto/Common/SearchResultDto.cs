namespace ZakupekApi.Wrapper.Contract.Dto.Common;

public class SearchResultDto<T>
{
    public List<T> Data { get; set; } = new List<T>();
}
