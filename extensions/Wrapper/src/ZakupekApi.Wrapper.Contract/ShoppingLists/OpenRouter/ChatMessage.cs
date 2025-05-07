using System.Text.Json.Serialization;

namespace ZakupekApi.Wrapper.Contract.ShoppingLists.OpenRouter;

public class ChatMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;
    
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}
