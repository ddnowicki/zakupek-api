using ZakupekApi.Wrapper.Contract.Commands.Products;

namespace ZakupekApi.Wrapper.Contract.Commands.ShoppingLists;

public class UpdateShoppingListCommand
{
    public string? Title { get; set; }
    public List<UpdateProductCommand> Products { get; set; } = new();
}
