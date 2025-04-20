using System.ComponentModel.DataAnnotations;
using MediatR;
using ZakupekApi.Wrapper.Contract.Dto.Auth;

namespace ZakupekApi.Wrapper.Contract.Commands.Auth;

/// <summary>
/// Command for user login
/// </summary>
public class LoginUserCommand : IRequest<AuthenticationResultDto>
{
    /// <summary>
    /// Email address used for authentication
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    
    /// <summary>
    /// User password
    /// </summary>
    [Required]
    public string Password { get; set; } = null!;
}
