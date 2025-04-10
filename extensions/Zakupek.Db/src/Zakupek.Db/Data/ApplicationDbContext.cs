using Microsoft.EntityFrameworkCore;

namespace Zakupek.Db.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
}
