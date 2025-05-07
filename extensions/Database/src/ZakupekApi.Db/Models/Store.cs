namespace ZakupekApi.Db.Models;

public class Store
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<ShoppingList> ShoppingLists { get; set; } = new List<ShoppingList>();
}