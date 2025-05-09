namespace ZakupekApi.Db.Models;

public class Status
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    // Navigation property
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
