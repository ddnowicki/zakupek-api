namespace ZakupekApi.Db.Models;

public class UserAge
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int Age { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}