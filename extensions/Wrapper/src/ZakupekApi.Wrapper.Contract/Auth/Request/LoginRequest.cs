namespace ZakupekApi.Wrapper.Contract.Auth.Request;

public record LoginRequest(
    string Email,
    string Password
);
