using MediatR;
using ZakupekApi.Wrapper.Abstraction.Authentication;
using ZakupekApi.Wrapper.Contract.Commands.Auth;
using ZakupekApi.Wrapper.Contract.Dto.Auth;

namespace ZakupekApi.Wrapper.Authentication.Commands;

/// <summary>
/// Handles the registration of new users
/// </summary>
public class RegisterUserCommandHandler(IAuthenticationService authenticationService) : IRequestHandler<RegisterUserCommand, AuthenticationResultDto>
{

    /// <summary>
    /// Handles the user registration process
    /// </summary>
    /// <param name="request">Registration data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result with tokens and user information</returns>
    public async Task<AuthenticationResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await authenticationService.RegisterUserAsync(request);
    }
}
