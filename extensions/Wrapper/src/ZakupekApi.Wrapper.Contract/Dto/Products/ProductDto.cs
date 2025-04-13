namespace ZakupekApi.Wrapper.Contract.Dto.Products;

public class ProductDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public int Quantity { get; set; }
    
    public string Status { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
}
