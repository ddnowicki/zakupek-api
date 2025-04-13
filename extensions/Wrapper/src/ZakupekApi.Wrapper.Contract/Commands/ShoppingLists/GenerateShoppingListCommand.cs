namespace ZakupekApi.Wrapper.Contract.Commands.ShoppingLists;

public class GenerateShoppingListCommand
{
    public int HouseholdSize { get; set; }
    
    public List<string> DietaryPreferences { get; set; } = new List<string>();
    
    public List<int> RecentPurchases { get; set; } = new List<int>();
}
