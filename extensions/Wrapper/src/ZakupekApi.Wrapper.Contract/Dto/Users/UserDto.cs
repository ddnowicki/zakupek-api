namespace ZakupekApi.Wrapper.Contract.Dto.Users;

public class UserDto
{
    public int Id { get; set; }
    
    public string Email { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
}
