namespace ZakupekApi.Db.Models;

public class UserDietaryPreference
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Preference { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}