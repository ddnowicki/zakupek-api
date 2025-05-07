using System.Text.Json.Serialization;

namespace ZakupekApi.Wrapper.Contract.ShoppingLists.OpenRouter;

public class OpenRouterResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;
    
    [JsonPropertyName("created")]
    public long Created { get; set; }
    
    [JsonPropertyName("choices")]
    public List<Choice> Choices { get; set; } = new();
    
    [JsonPropertyName("usage")]
    public Usage Usage { get; set; } = new();
}
