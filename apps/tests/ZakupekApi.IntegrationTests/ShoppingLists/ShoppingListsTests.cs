using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using ZakupekApi.ShoppingLists.Endpoints;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Response;

namespace ZakupekApi.IntegrationTests.ShoppingLists;

[Collection(nameof(ZakupekCollection))]
public class ShoppingListsTests(IntegrationApp app) : TestBase<IntegrationApp>
{
    [Fact, Priority(1)]
    public void GetShoppingLists_WithNoToken_ShouldReturnUnauthenticated()
    {
        // Act
        var response = 
            app.Client.GETAsync<GetShoppingListsEndpoint, ShoppingListsResponse>().Result;

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
