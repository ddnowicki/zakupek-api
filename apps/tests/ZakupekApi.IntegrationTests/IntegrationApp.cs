using FastEndpoints;
using FastEndpoints.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ZakupekApi.Db.Data;

namespace ZakupekApi.IntegrationTests;

[DisableWafCache]
public class IntegrationApp : AppFixture<Program>
{
    public HttpClient Client { get; private set; }
    
    protected override async ValueTask SetupAsync()
    {
        // Configure test HttpClient
        Client = CreateClient();

        // Get the DbContext and ensure database is created
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureCreatedAsync();
    }

    protected override void ConfigureApp(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        // Remove MySql registration and add InMemory provider
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));
    }

    protected override ValueTask TearDownAsync()
    {
        // Clean up after each test
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureDeleted();
        
        return ValueTask.CompletedTask;
    }
}
