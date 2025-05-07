namespace ZakupekApi.Db.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string HashedPassword { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public int? HouseholdSize { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public ICollection<ShoppingList> ShoppingLists { get; set; } = new List<ShoppingList>();
    public ICollection<UserDietaryPreference> DietaryPreferences { get; set; } = new List<UserDietaryPreference>();
    public ICollection<UserAge> Ages { get; set; } = new List<UserAge>();
    public ICollection<Store> Stores { get; set; } = new List<Store>();
}
