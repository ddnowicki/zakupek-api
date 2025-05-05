using ErrorOr;
using FastEndpoints;
using ZakupekApi.Wrapper.Abstraction.Auth;
using ZakupekApi.Wrapper.Contract.Auth.Request;
using ZakupekApi.Wrapper.Contract.Auth.Response;

namespace ZakupekApi.Auth.Endpoints;

public class RegisterEndpoint : Endpoint<RegisterRequest, ErrorOr<AuthResponse>>
{
    public IAuthService AuthService { get; set; } = default!;

    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
    }

    public override async Task<ErrorOr<AuthResponse>> ExecuteAsync(RegisterRequest registerRequest, CancellationToken ct)
        => await AuthService.Register(registerRequest.Email, registerRequest.Password);
}
