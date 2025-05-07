using ErrorOr;
using FastEndpoints;
using ZakupekApi.Wrapper.Abstraction.Users;
using ZakupekApi.Wrapper.Contract.Auth.Response;
using ZakupekApi.Wrapper.Users;

namespace ZakupekApi.Auth.Endpoints;

public class GetProfileEndpoint : EndpointWithoutRequest<ErrorOr<UserProfileResponse>>
{
    public IUserService UserService { get; set; } = default!;

    public override void Configure()
    {
        Get("/api/users/profile");
    }

    public override async Task<ErrorOr<UserProfileResponse>> ExecuteAsync(CancellationToken ct)
        => await UserService.GetUserProfile(User.GetUserId());
}
