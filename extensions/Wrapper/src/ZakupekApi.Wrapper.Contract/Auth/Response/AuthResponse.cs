namespace ZakupekApi.Wrapper.Contract.Auth.Response;

public sealed record AuthResponse
(
    int UserId,
    string Token
);
