using System.Net;
using ErrorOr;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
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
    public async Task AuthTests_WhenRegisteringWithValidData_ShouldCreateUser()
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
    public async Task AuthTests_WhenRegisteringWithInvalidData_ShouldReturnBadRequest()
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
    public async Task AuthTests_WhenLoginWithValidCredentials_ShouldReturnToken()
    {
        // Arrange - Ensure a user exists
        await AuthTests_WhenRegisteringWithValidData_ShouldCreateUser();
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
    public async Task AuthTests_WhenLoginWithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LoginRequest("wrong@example.com", "WrongPassword123!");

        // Act
        var response = await app.Client.POSTAsync<LoginEndpoint, LoginRequest, ErrorOr<AuthResponse>>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact, Priority(5)]
    public async Task AuthTests_WhenGuestAccessesProfile_ShouldReturnUnauthorized()
    {
        // Act
        var response = await app.Client.GETAsync<GetProfileEndpoint, EmptyRequest, ErrorOr<UserProfileResponse>>(new EmptyRequest());

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact, Priority(6)]
    public async Task AuthTests_WhenAuthenticatedUserAccessesProfile_ShouldReturnProfile()
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
    public async Task AuthTests_WhenGuestUpdatesProfile_ShouldReturnUnauthorized()
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
    public async Task AuthTests_WhenUpdatingProfile_ShouldUpdateUserProfile()
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
