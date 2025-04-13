namespace ZakupekApi.Wrapper.Contract.Dto.Auth;

public class AuthResultDto
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime Expiration { get; set; }
}
