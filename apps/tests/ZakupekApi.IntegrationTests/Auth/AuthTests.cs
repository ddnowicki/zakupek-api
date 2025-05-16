using System.Net;
using System.Net.Http.Json;
using ErrorOr;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using Xunit;
using ZakupekApi.Auth.Endpoints;
using ZakupekApi.Wrapper.Contract.Auth.Request;
using ZakupekApi.Wrapper.Contract.Auth.Response;

namespace ZakupekApi.IntegrationTests.Auth;

public class AuthTests(IntegrationApp app) : TestBase<IntegrationApp>
{
    private const string TEST_EMAIL = "user@example.com";
    private const string TEST_PASSWORD = "Password123!";
    private const string TEST_USERNAME = "Test User";

    protected override async ValueTask SetupAsync()
    {
        await app.ResetDatabaseAsync();
    }
    
    [Fact, Priority(1)]
    public async Task Register_With_Valid_Data_Creates_User()
    {
        // Arrange
        var request = new RegisterRequest(
            Email: TEST_EMAIL,
            Password: TEST_PASSWORD,
            UserName: TEST_USERNAME,
            HouseholdSize: 3,
            Ages: new List<int> { 30, 28, 5 },
            DietaryPreferences: new List<string> { "Vegetarian", "Gluten-free" }
        );

        // Act
        var response = await app.Client.POSTAsync<RegisterEndpoint, RegisterRequest, AuthResponse>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Result.AccessToken.ShouldNotBeNullOrEmpty();
        response.Result.UserName.ShouldBe(TEST_USERNAME);
        response.Result.UserId.ShouldNotBe(0);
        response.Result.ExpiresAt.ShouldNotBe(DateTime.MinValue);
    }

    [Fact, Priority(2)]
    public async Task Register_With_Invalid_Data_Returns_BadRequest()
    {
        // Arrange
        var request = new RegisterRequest(
            Email: "invalid-email",
            Password: "short",
            UserName: "",
            HouseholdSize: -1
        );

        // Act
        var response = await app.Client.POSTAsync<RegisterEndpoint, RegisterRequest, ErrorOr<AuthResponse>>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact, Priority(3)]
    public async Task Login_With_Valid_Credentials_Returns_Token()
    {
        // Arrange - Ensure a user exists
        await Register_With_Valid_Data_Creates_User();
        var request = new LoginRequest(TEST_EMAIL, TEST_PASSWORD);

        // Act
        var response = await app.Client.POSTAsync<LoginEndpoint, LoginRequest, AuthResponse>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = response.Result;
        result.ShouldNotBeNull();
        result.AccessToken.ShouldNotBeEmpty();
        result.UserName.ShouldBe(TEST_USERNAME);
    }

    [Fact, Priority(4)]
    public async Task Login_With_Invalid_Credentials_Returns_Unauthorized()
    {
        // Arrange
        var request = new LoginRequest("wrong@example.com", "WrongPassword123!");

        // Act
        var response = await app.Client.POSTAsync<LoginEndpoint, LoginRequest, ErrorOr<AuthResponse>>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact, Priority(5)]
    public async Task Get_Profile_Returns_Unauthorized_For_Guest()
    {
        // Act
        var response = await app.Client.GETAsync<GetProfileEndpoint, EmptyRequest, ErrorOr<UserProfileResponse>>(new EmptyRequest());

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact, Priority(6)]
    public async Task Get_Profile_Returns_Profile_For_Authenticated_User()
    {
        // Arrange
        await app.AuthenticateAsUser(TEST_EMAIL, TEST_PASSWORD);
    
        // Act
        var response = await app.Customer.GETAsync<GetProfileEndpoint, EmptyRequest, UserProfileResponse>(new EmptyRequest());
    
        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = response.Result;
        result.ShouldNotBeNull();
        result.Email.ShouldBe(TEST_EMAIL);
        result.UserName.ShouldBe(TEST_USERNAME);
    }

    [Fact, Priority(7)]
    public async Task Update_Profile_Returns_Unauthorized_For_Guest()
    {
        // Arrange
        const string UPDATED_USERNAME = "Updated User";
        var request = new UpdateUserProfileRequest(
            UserName: UPDATED_USERNAME,
            HouseholdSize: 4,
            Ages: new List<int> { 31, 29, 6, 1 },
            DietaryPreferences: new List<string> { "Vegan" }
        );

        // Act
        var response = await app.Client.PUTAsync<UpdateProfileEndpoint, UpdateUserProfileRequest, ErrorOr<UserProfileResponse>>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact, Priority(8)]
    public async Task Update_Profile_Updates_User_Profile()
    {
        // Arrange
        await app.AuthenticateAsUser(TEST_EMAIL, TEST_PASSWORD);
        const string UPDATED_USERNAME = "Updated User";
        var request = new UpdateUserProfileRequest(
            UserName: UPDATED_USERNAME,
            HouseholdSize: 4,
            Ages: new List<int> { 31, 29, 6, 1 },
            DietaryPreferences: new List<string> { "Vegan" }
        );
    
        // Act
        var response = await app.Customer.PUTAsync<UpdateProfileEndpoint, UpdateUserProfileRequest, UserProfileResponse>(request);
    
        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = response.Result;
        result.ShouldNotBeNull();
        result.UserName.ShouldBe(UPDATED_USERNAME);
        result.HouseholdSize.ShouldBe(4);
        result.Ages?.Count.ShouldBe(4);
        result.DietaryPreferences?.Count.ShouldBe(1);
        result.DietaryPreferences?.ShouldContain("Vegan");
    }
}
