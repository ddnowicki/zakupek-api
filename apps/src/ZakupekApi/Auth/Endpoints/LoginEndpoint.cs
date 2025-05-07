using FastEndpoints;
using ErrorOr;
using ZakupekApi.Wrapper.Abstraction.Auth;
using ZakupekApi.Wrapper.Contract.Auth.Request;
using ZakupekApi.Wrapper.Contract.Auth.Response;

namespace ZakupekApi.Auth.Endpoints;

public class LoginEndpoint : Endpoint<LoginRequest, ErrorOr<AuthResponse>>
{
    public IAuthService AuthService { get; set; } = default!;

    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
    }

    public override async Task<ErrorOr<AuthResponse>> ExecuteAsync(LoginRequest loginRequest, CancellationToken ct)
        => await AuthService.Login(loginRequest);
}
