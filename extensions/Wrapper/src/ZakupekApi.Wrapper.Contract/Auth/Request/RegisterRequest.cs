namespace ZakupekApi.Wrapper.Contract.Auth.Request;

public record RegisterRequest(
    string Email,
    string Password,
    string UserName,
    int HouseholdSize,
    List<int>? Ages = null,
    List<string>? DietaryPreferences = null
);
