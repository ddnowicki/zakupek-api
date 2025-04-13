using ZakupekApi.Wrapper.Contract.Dto.Products;

namespace ZakupekApi.Wrapper.Contract.Dto.ShoppingLists;

public class ShoppingListDetailDto : ShoppingListDto
{
    public List<ProductDto> Products { get; set; } = new List<ProductDto>();
}
