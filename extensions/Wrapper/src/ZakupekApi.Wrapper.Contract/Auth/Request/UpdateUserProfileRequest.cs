namespace ZakupekApi.Wrapper.Contract.Auth.Request;

public sealed record UpdateUserProfileRequest(
    string UserName,
    int? HouseholdSize,
    List<int>? Ages,
    List<string>? DietaryPreferences
);