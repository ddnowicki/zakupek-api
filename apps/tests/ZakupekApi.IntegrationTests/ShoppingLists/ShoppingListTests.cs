using System.Net;
using ErrorOr;
using FakeItEasy;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using ZakupekApi.ShoppingLists.Endpoints;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Request;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Response;

namespace ZakupekApi.IntegrationTests.ShoppingLists;

public class ShoppingListTests(IntegrationApp app) : TestBase<IntegrationApp>
{
    private const string TEST_EMAIL = "shopper@example.com";
    private const string TEST_PASSWORD = "Shopping123!";
    private const string TEST_USERNAME = "Shopper User";
    private const string TEST_LIST_TITLE = "Test Shopping List";
    private int createdListId;

    protected override async ValueTask SetupAsync()
    {
        await app.ResetDatabaseAsync();
        // Authenticate the user before running shopping list tests
        await app.AuthenticateAsUser(TEST_EMAIL, TEST_PASSWORD);
    }

    [Fact, Priority(1)]
    public async Task ShoppingListTests_WhenCreatingWithValidData_ShouldCreateList()
    {
        // Arrange
        var request = new CreateShoppingListRequest(
            Title: TEST_LIST_TITLE,
            Products: [new("Milk", 2), new("Bread", 1)],
            PlannedShoppingDate: DateTime.UtcNow.AddDays(1),
            StoreName: "Lidl"
        );

        // Act
        var response = await app.Customer.POSTAsync<CreateShoppingListEndpoint, CreateShoppingListRequest, ShoppingListDetailResponse>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Result.ShouldNotBeNull();
        response.Result.Title.ShouldBe(TEST_LIST_TITLE);
        response.Result.Products.Count().ShouldBe(2);
        response.Result.StoreName.ShouldBe("Lidl");
        
        // Save the created list ID for subsequent tests
        createdListId = response.Result.Id;
    }

    [Fact, Priority(2)]
    public async Task ShoppingListTests_WhenCreatingWithoutTitle_ShouldCreateListWithNoTitle()
    {
        // Arrange
        var request = new CreateShoppingListRequest(
            Title: null,
            Products: [new("Cheese", 1)],
            PlannedShoppingDate: null,
            StoreName: null
        );

        // Act
        var response = await app.Customer.POSTAsync<CreateShoppingListEndpoint, CreateShoppingListRequest, ShoppingListDetailResponse>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Result.ShouldNotBeNull();
        response.Result.Title.ShouldBeNull();
        response.Result.Products.Count().ShouldBe(1);
    }

    [Fact, Priority(3)]
    public async Task ShoppingListTests_WhenCreatingWithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateShoppingListRequest(
            Title: "", // Empty title should be rejected
            Products: [new("", -1)], // Invalid product
            PlannedShoppingDate: null,
            StoreName: null
        );

        // Act
        var response = await app.Customer.POSTAsync<CreateShoppingListEndpoint, CreateShoppingListRequest, ErrorOr<ShoppingListDetailResponse>>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact, Priority(4)]
    public async Task ShoppingListTests_WhenGettingLists_ShouldReturnUserLists()
    {
        // Arrange - Create a list first
        await ShoppingListTests_WhenCreatingWithValidData_ShouldCreateList();

        // Act
        var response = await app.Customer.GETAsync<GetShoppingListsEndpoint, EmptyRequest, ShoppingListsResponse>(new EmptyRequest());

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Result.ShouldNotBeNull();
        response.Result.Data.ShouldNotBeEmpty();
        response.Result.Data.Count().ShouldBeGreaterThanOrEqualTo(1);
        response.Result.Pagination.TotalItems.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact, Priority(5)]
    public async Task ShoppingListTests_WhenGettingListWithValidId_ShouldReturnList()
    {
        // Arrange - Create a list first
        await ShoppingListTests_WhenCreatingWithValidData_ShouldCreateList();

        // Create request with property assignment
        var request = new GetShoppingListByIdRequest { Id = createdListId };

        // Act
        var response = await app.Customer.GETAsync<GetShoppingListByIdEndpoint, GetShoppingListByIdRequest, ShoppingListDetailResponse>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Result.ShouldNotBeNull();
        response.Result.Id.ShouldBe(createdListId);
        response.Result.Title.ShouldBe(TEST_LIST_TITLE);
        response.Result.Products.ShouldNotBeEmpty();
    }

    [Fact, Priority(6)]
    public async Task ShoppingListTests_WhenGettingListWithInvalidId_ShouldReturnNotFound()
    {
        // Create request with property assignment
        var request = new GetShoppingListByIdRequest { Id = 9999 }; // Non-existent ID

        // Act
        var response = await app.Customer.GETAsync<GetShoppingListByIdEndpoint, GetShoppingListByIdRequest, ErrorOr<ShoppingListDetailResponse>>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact, Priority(7)]
    public async Task ShoppingListTests_WhenGettingListsAsGuest_ShouldReturnUnauthorized()
    {
        // Act - Use non-authenticated client
        var response = await app.Client.GETAsync<GetShoppingListsEndpoint, EmptyRequest, ErrorOr<ShoppingListsResponse>>(new EmptyRequest());

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact, Priority(8)]
    public async Task ShoppingListTests_WhenUpdatingList_ShouldUpdateSuccessfully()
    {
        // Arrange - Create a list first
        await ShoppingListTests_WhenCreatingWithValidData_ShouldCreateList();
        
        var updateRequest = new UpdateShoppingListRequest(
            Title: "Updated Shopping List",
            Products: [
                new(Id: null, Name: "Milk", Quantity: 3), // Updated quantity
                new(Id: null, Name: "Eggs", Quantity: 12) // New item
            ],
            PlannedShoppingDate: DateTime.UtcNow.AddDays(2),
            StoreName: "Auchan"
        );

        // Create endpoint request with the required ID and Body parameters
        var endpointRequest = new UpdateShoppingListEndpointRequest(
            Id: createdListId,
            Body: updateRequest
        );

        // Act
        var response = await app.Customer.PUTAsync<UpdateShoppingListEndpoint, UpdateShoppingListEndpointRequest, bool>(endpointRequest);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Result.ShouldBeTrue();

        // Verify the update worked by getting the list
        var getRequest = new GetShoppingListByIdRequest { Id = createdListId };
        var getResponse = await app.Customer.GETAsync<GetShoppingListByIdEndpoint, GetShoppingListByIdRequest, ShoppingListDetailResponse>(getRequest);
        
        getResponse.Result.Title.ShouldBe("Updated Shopping List");
        getResponse.Result.StoreName.ShouldBe("Auchan");
        getResponse.Result.Products.Count().ShouldBe(2); // Original bread was removed
    }

    [Fact, Priority(10)]
    public async Task ShoppingListTests_WhenUpdatingNonExistentList_ShouldReturnNotFound()
    {
        var updateRequest = new UpdateShoppingListRequest(
            Title: "Non-existent List",
            Products: [new(Id: null, Name: "Milk", Quantity: 1)],
            PlannedShoppingDate: null,
            StoreName: null
        );

        // Create endpoint request with a non-existent ID
        var endpointRequest = new UpdateShoppingListEndpointRequest(
            Id: 9999, // Non-existent ID
            Body: updateRequest
        );

        // Act
        var response = await app.Customer.PUTAsync<UpdateShoppingListEndpoint, UpdateShoppingListEndpointRequest, ErrorOr<bool>>(endpointRequest);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact, Priority(11)]
    public async Task ShoppingListTests_WhenDeletingList_ShouldDeleteSuccessfully()
    {
        // Arrange - Create a list first
        await ShoppingListTests_WhenCreatingWithValidData_ShouldCreateList();

        // Create delete request with property assignment
        var deleteRequest = new DeleteShoppingListRequest { Id = createdListId };

        // Act
        var response = await app.Customer.DELETEAsync<DeleteShoppingListEndpoint, DeleteShoppingListRequest, bool>(deleteRequest);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Result.ShouldBeTrue();

        // Verify the list was deleted
        var getRequest = new GetShoppingListByIdRequest { Id = createdListId };
        var getResponse = await app.Customer.GETAsync<GetShoppingListByIdEndpoint, GetShoppingListByIdRequest, ErrorOr<ShoppingListDetailResponse>>(getRequest);
        
        getResponse.Response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact, Priority(12)]
    public async Task ShoppingListTests_WhenDeletingNonExistentList_ShouldReturnNotFound()
    {
        // Create delete request with property assignment for non-existent ID
        var deleteRequest = new DeleteShoppingListRequest { Id = 9999 };

        // Act
        var response = await app.Customer.DELETEAsync<DeleteShoppingListEndpoint, DeleteShoppingListRequest, ErrorOr<bool>>(deleteRequest);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact, Priority(13)]
    public async Task ShoppingListTests_WhenGeneratingList_ShouldCreateAIGeneratedList()
    {
        // Arrange
        var request = new GenerateShoppingListRequest(
            Title: "AI Generated List",
            PlannedShoppingDate: DateTime.UtcNow.AddDays(3),
            StoreName: "Lidl"
        );
        
        A.CallTo(app.FakeHandler)
            .WithReturnType<Task<HttpResponseMessage>>()
            .Where(call => call.Method.Name == "SendAsync")
            .ReturnsLazily(() => Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("FakeItEasy is fun")
            }));

        // Act
        var response = await app.Customer.POSTAsync<GenerateShoppingListEndpoint, GenerateShoppingListRequest, ShoppingListDetailResponse>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Result.ShouldNotBeNull();
        response.Result.Title.ShouldBe("AI Generated List");
        response.Result.StoreName.ShouldBe("Lidl");
        response.Result.Source.ShouldBe("AI generated");
        response.Result.Products.ShouldNotBeEmpty();
    }

    [Fact, Priority(14)]
    public async Task ShoppingListTests_WhenGeneratingListWithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new GenerateShoppingListRequest(
            Title: "AI Generated List",
            PlannedShoppingDate: DateTime.UtcNow.AddDays(3),
            StoreName: "Lidl"
        );

        // Act - Use non-authenticated client
        var response = await app.Client.POSTAsync<GenerateShoppingListEndpoint, GenerateShoppingListRequest, ErrorOr<ShoppingListDetailResponse>>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
