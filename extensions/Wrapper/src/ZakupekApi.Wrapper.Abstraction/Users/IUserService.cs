using ErrorOr;
using ZakupekApi.Wrapper.Contract.Auth.Request;
using ZakupekApi.Wrapper.Contract.Auth.Response;

namespace ZakupekApi.Wrapper.Abstraction.Users;

public interface IUserService
{
    Task<ErrorOr<UserProfileResponse>> GetUserProfile(int userId);
    Task<ErrorOr<UserProfileResponse>> UpdateProfile(int userId, UpdateUserProfileRequest request);
}
