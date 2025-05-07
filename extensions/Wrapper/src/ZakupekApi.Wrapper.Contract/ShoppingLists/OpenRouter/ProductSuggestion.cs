using System.Text.Json.Serialization;

namespace ZakupekApi.Wrapper.Contract.ShoppingLists.OpenRouter;

public class ProductSuggestion
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}
