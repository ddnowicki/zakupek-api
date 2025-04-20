using ZakupekApi.Wrapper.Contract.Commands.Auth;
using ZakupekApi.Wrapper.Contract.Dto.Auth;

namespace ZakupekApi.Wrapper.Abstraction.Authentication;

/// <summary>
/// Service for handling user authentication operations
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Registers a new user in the system
    /// </summary>
    /// <param name="command">Registration data</param>
    /// <returns>Authentication result with tokens and user information</returns>
    Task<AuthenticationResultDto> RegisterUserAsync(RegisterUserCommand command);
    
    /// <summary>
    /// Authenticates a user with provided credentials
    /// </summary>
    /// <param name="command">Login credentials</param>
    /// <returns>Authentication result with tokens and user information</returns>
    Task<AuthenticationResultDto> LoginAsync(LoginUserCommand command);
}