using ZakupekApi.Wrapper.Contract.Commands.Products;

namespace ZakupekApi.Wrapper.Contract.Commands.ShoppingLists;

public class CreateShoppingListCommand
{
    public string? Title { get; set; }
    public List<CreateProductCommand> Products { get; set; } = new();
}
