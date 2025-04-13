using System.ComponentModel.DataAnnotations;

namespace ZakupekApi.Wrapper.Contract.Commands.Products;

public class AddProductCommand
{
    [Required]
    public string Name { get; set; } = null!;
    
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    public int StatusId { get; set; }
}
