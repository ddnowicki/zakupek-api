using ZakupekApi.Wrapper.Contract.Auth.Response;
using ErrorOr;

namespace ZakupekApi.Wrapper.Abstraction.Auth;

public interface IAuthService
{
    public Task<ErrorOr<AuthResponse>> Login(string email, string password);
    public Task<ErrorOr<AuthResponse>> Register(string email, string password);
}
