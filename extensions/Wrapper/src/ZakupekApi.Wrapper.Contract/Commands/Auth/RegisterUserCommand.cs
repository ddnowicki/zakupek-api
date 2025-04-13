using System.ComponentModel.DataAnnotations;

namespace ZakupekApi.Wrapper.Contract.Commands.Auth;

public class RegisterUserCommand
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = null!;
    
    public int HouseholdSize { get; set; }
    
    public List<int> Ages { get; set; } = new();
    
    public string DietaryPreferences { get; set; } = null!;
}
