using System.Text.Json.Serialization;

namespace ZakupekApi.Wrapper.Contract.ShoppingLists.OpenRouter;

public class OpenRouterRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;
    
    [JsonPropertyName("messages")]
    public ChatMessage[] Messages { get; set; } = [];
}
