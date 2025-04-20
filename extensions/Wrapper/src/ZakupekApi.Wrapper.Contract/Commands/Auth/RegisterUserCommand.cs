using System.ComponentModel.DataAnnotations;
using MediatR;
using ZakupekApi.Wrapper.Contract.Dto.Auth;

namespace ZakupekApi.Wrapper.Contract.Commands.Auth;

/// <summary>
/// Command for registering a new user
/// </summary>
public class RegisterUserCommand : IRequest<AuthenticationResultDto>
{
    /// <summary>
    /// Email address that will be used as the username
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    
    /// <summary>
    /// User password (will be stored as a hash)
    /// </summary>
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = null!;
    
    /// <summary>
    /// Number of people in the household
    /// </summary>
    public int HouseholdSize { get; set; }
    
    /// <summary>
    /// Ages of household members
    /// </summary>
    public List<int> Ages { get; set; } = new();
    
    /// <summary>
    /// Dietary preferences (comma-separated string)
    /// </summary>
    public string DietaryPreferences { get; set; } = null!;
}
