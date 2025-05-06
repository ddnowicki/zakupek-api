namespace ZakupekApi.Wrapper.Contract.Auth.Response;

public sealed record AuthResponse
(
    int UserId,
    string UserName,
    string AccessToken,
    DateTime ExpiresAt
);
