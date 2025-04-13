namespace ZakupekApi.Wrapper.Contract.Dto.Users;

public class UserProfileDto : UserDto
{
    public int HouseholdSize { get; set; }
    
    public List<int> Ages { get; set; } = new List<int>();
    
    public List<string> DietaryPreferences { get; set; } = new List<string>();
}
