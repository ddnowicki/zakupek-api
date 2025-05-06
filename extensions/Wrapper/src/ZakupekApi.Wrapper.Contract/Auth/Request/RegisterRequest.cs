namespace ZakupekApi.Wrapper.Contract.Auth.Request;

public class RegisterRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string UserName { get; set; }
    public int HouseholdSize { get; set; }
    public List<int>? Ages { get; set; }
    public List<string>? DietaryPreferences { get; set; }
}
