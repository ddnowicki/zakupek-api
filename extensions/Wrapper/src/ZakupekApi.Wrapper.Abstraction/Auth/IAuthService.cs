using ZakupekApi.Wrapper.Contract.Auth.Response;
using ZakupekApi.Wrapper.Contract.Auth.Request;
using ErrorOr;

namespace ZakupekApi.Wrapper.Abstraction.Auth;

public interface IAuthService
{
    Task<ErrorOr<AuthResponse>> Login(LoginRequest request);
    Task<ErrorOr<AuthResponse>> Register(RegisterRequest request);
    Task<ErrorOr<UserProfileResponse>> GetProfile(int userId);
}
