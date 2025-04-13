namespace ZakupekApi.Db.Models;

public class Product
{
    public int Id { get; set; }
    public int ShoppingListId { get; set; }
    public string Name { get; set; } = null!;
    public int Quantity { get; set; }
    public int StatusId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public ShoppingList ShoppingList { get; set; } = null!;
    public ProductStatus Status { get; set; } = null!;
}
