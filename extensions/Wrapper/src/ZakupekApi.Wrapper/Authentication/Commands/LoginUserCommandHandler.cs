using MediatR;
using ZakupekApi.Wrapper.Abstraction.Authentication;
using ZakupekApi.Wrapper.Contract.Commands.Auth;
using ZakupekApi.Wrapper.Contract.Dto.Auth;

namespace ZakupekApi.Wrapper.Authentication.Commands;

/// <summary>
/// Handles user login requests
/// </summary>
public class LoginUserCommandHandler(IAuthenticationService authenticationService) 
    : IRequestHandler<LoginUserCommand, AuthenticationResultDto>
{
    /// <summary>
    /// Handles the user login process
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result with tokens and user information</returns>
    public async Task<AuthenticationResultDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        return await authenticationService.LoginAsync(request);
    }
}
