namespace ZakupekApi.Db.Models;

public enum ShoppingListSource
{
    Manual,
    AiGenerated,
    PartiallyAiGenerated
}

public class ShoppingList
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Title { get; set; }
    public ShoppingListSource Source { get; set; } = ShoppingListSource.Manual;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
