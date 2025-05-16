using System.Net;
using System.Net.Http.Headers;
using FakeItEasy;
using FastEndpoints;
using FastEndpoints.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ZakupekApi.Auth.Endpoints;
using ZakupekApi.Db.Data;
using ZakupekApi.Wrapper.Contract.Auth.Request;
using ZakupekApi.Wrapper.Contract.Auth.Response;

namespace ZakupekApi.IntegrationTests;

[DisableWafCache]
public class IntegrationApp : AppFixture<Program>
{
    public HttpClient Customer { get; private set; } = null!;
    public HttpMessageHandler FakeHandler { get; private set; } = null!;
   
    public IntegrationApp()
    {
        FakeHandler = A.Fake<HttpMessageHandler>();
    }

    protected override async ValueTask SetupAsync()
    {
        Customer = CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-token");
        });

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
        
        var fakeHttpClient = new HttpClient(FakeHandler);
        
        services.RemoveAll<HttpClient>();
        services.AddSingleton(fakeHttpClient);
    }

    protected override ValueTask TearDownAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureDeleted();

        return ValueTask.CompletedTask;
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public async Task<AuthResponse> AuthenticateAsUser(string email = "user@example.com", string password = "Password123!")
    {
        var registerRequest = new RegisterRequest(
            Email: email,
            Password: password,
            UserName: "Test User",
            HouseholdSize: 1,
            Ages: [30],
            DietaryPreferences: ["Vegetarian"]
        );

        // Register a new user
        var response = await Customer.POSTAsync<RegisterEndpoint, RegisterRequest, AuthResponse>(registerRequest);

        if (response.Response.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception("Failed to register user");
        }

        Customer.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.Result.AccessToken);

        return response.Result;
    }
}
