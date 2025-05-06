using System.Security.Claims;
using ErrorOr;
using ZakupekApi.Wrapper.Contract.Auth.Response;

namespace ZakupekApi.Wrapper.Abstraction.Users;

public interface IUserService
{
    Task<ErrorOr<UserProfileResponse>> GetUserProfile(int userId);
}
