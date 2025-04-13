namespace ZakupekApi.Db.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string HashedPassword { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public ICollection<ShoppingList> ShoppingLists { get; set; } = new List<ShoppingList>();
}
