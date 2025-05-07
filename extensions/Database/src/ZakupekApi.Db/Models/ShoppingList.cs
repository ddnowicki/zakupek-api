namespace ZakupekApi.Db.Models;

public class ShoppingList
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Title { get; set; }
    public int SourceId { get; set; }
    public int? StoreId { get; set; }
    public DateTime? PlannedShoppingDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Status Source { get; set; } = null!;
    public Store? Store { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
