namespace ZakupekApi.Wrapper.Contract.Auth.Response;

public sealed record UserProfileResponse
(
    int Id,
    string Email,
    string UserName,
    int? HouseholdSize,
    List<int>? Ages,
    List<string>? DietaryPreferences,
    DateTime CreatedAt,
    int ListsCount
);