using System.ComponentModel.DataAnnotations;

namespace ZakupekApi.Wrapper.Contract.Commands.Products;

public class UpdateProductCommand
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
