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
    
    public List<int> Ages { get; set; } = new List<int>();
    
    public List<string> DietaryPreferences { get; set; } = new List<string>();
}
