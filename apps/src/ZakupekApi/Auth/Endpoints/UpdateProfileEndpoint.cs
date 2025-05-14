using ErrorOr;
using FastEndpoints;
using ZakupekApi.Wrapper.Abstraction.Users;
using ZakupekApi.Wrapper.Contract.Auth.Request;
using ZakupekApi.Wrapper.Contract.Auth.Response;
using ZakupekApi.Wrapper.Users;

namespace ZakupekApi.Auth.Endpoints;

public class UpdateProfileEndpoint : Endpoint<UpdateUserProfileRequest, ErrorOr<UserProfileResponse>>
{
    public IUserService UserService { get; set; } = default!;

    public override void Configure()
    {
        Put("/api/users/profile");
    }

    public override async Task<ErrorOr<UserProfileResponse>> ExecuteAsync(UpdateUserProfileRequest request, CancellationToken ct)
        => await UserService.UpdateProfile(User.GetUserId(), request);
}
