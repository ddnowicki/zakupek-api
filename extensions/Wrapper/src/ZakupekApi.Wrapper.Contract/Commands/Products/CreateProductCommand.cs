using System.ComponentModel.DataAnnotations;

namespace ZakupekApi.Wrapper.Contract.Commands.Products;

public class CreateProductCommand
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
