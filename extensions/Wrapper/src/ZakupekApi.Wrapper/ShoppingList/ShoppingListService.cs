using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ZakupekApi.Db.Data;
using ZakupekApi.Db.Models;
using ZakupekApi.Wrapper.Abstraction.ShoppingList;
using ZakupekApi.Wrapper.Contract.ShoppingLists.OpenRouter;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Request;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Response;

namespace ZakupekApi.Wrapper.ShoppingList;

public class ShoppingListService(AppDbContext dbContext, HttpClient httpClient, IOptions<OpenRouterSettings> options) : IShoppingListService
{
    private readonly string apiKey = options.Value.ApiKey;
    private readonly string baseUrl = options.Value.BaseUrl;
    private readonly string model = options.Value.Model;

    public async Task<ErrorOr<ShoppingListsResponse>> GetShoppingListsAsync(
        int userId, int page = 1, int pageSize = 10, string sort = "newest")
    {
        var query = dbContext.ShoppingLists
            .Include(sl => sl.Source)
            .Include(sl => sl.Store)
            .Where(sl => sl.UserId == userId);

        query = applySorting(query, sort);

        var lists = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(sl => new ShoppingListResponse(
                sl.Id,
                sl.Title,
                sl.Products.Count,
                sl.PlannedShoppingDate,
                sl.CreatedAt,
                sl.Source.Name,
                sl.Store != null ? sl.Store.Name : null
            ))
            .ToListAsync();

        var totalItems = await GetTotalShoppingListsCountAsync(userId);

        return new ShoppingListsResponse(
            lists,
            new(
                Page: page,
                PageSize: pageSize,
                TotalItems: totalItems,
                TotalPages: (int)Math.Ceiling((double)totalItems / pageSize)
            )
        );

        static IQueryable<Db.Models.ShoppingList> applySorting(IQueryable<Db.Models.ShoppingList> query, string sort) =>
            sort switch
            {
                "oldest" => query.OrderBy(sl => sl.CreatedAt),
                "name" => query.OrderBy(sl => sl.Title),
                _ => query.OrderByDescending(sl => sl.CreatedAt)
            };
    }

    public async Task<int> GetTotalShoppingListsCountAsync(int userId)
    {
        return await dbContext.ShoppingLists
            .Where(sl => sl.UserId == userId)
            .CountAsync();
    }

    public async Task<ErrorOr<ShoppingListDetailResponse>> GetShoppingListByIdAsync(int listId, int userId)
    {
        var shoppingList = await dbContext.ShoppingLists
            .Include(sl => sl.Products)
            .ThenInclude(p => p.Status)
            .Include(shoppingList => shoppingList.Source)
            .Include(shoppingList => shoppingList.Store)
            .FirstOrDefaultAsync(sl => sl.Id == listId && sl.UserId == userId);

        if (shoppingList == null)
        {
            return Error.NotFound("Shopping list not found");
        }

        var productResponses = shoppingList.Products.Select(p => new ProductInListResponse(
            p.Id,
            p.Name,
            p.Quantity,
            p.StatusId,
            p.Status.Name,
            p.CreatedAt
        ));

        var response = new ShoppingListDetailResponse(
            shoppingList.Id,
            shoppingList.Title,
            shoppingList.Store?.Name,
            shoppingList.PlannedShoppingDate,
            shoppingList.CreatedAt,
            shoppingList.UpdatedAt,
            shoppingList.Source.Name,
            shoppingList.Store?.Name,
            productResponses
        );

        return response;
    }

    public async Task<ErrorOr<ShoppingListDetailResponse>> CreateShoppingListAsync(
        int userId, CreateShoppingListRequest request)
    {
        const int MANUALLY_ADDED_STATUS_ID = 3;

        Store? store = null;
        if (!string.IsNullOrWhiteSpace(request.StoreName))
        {
            store = await dbContext.Stores
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Name.ToLower() == request.StoreName.ToLower());

            if (store == null)
            {
                store = new()
                {
                    UserId = userId, Name = request.StoreName, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
                };
                dbContext.Stores.Add(store);
                await dbContext.SaveChangesAsync();
            }
        }

        var shoppingList = new Db.Models.ShoppingList
        {
            UserId = userId,
            Title = request.Title,
            SourceId = MANUALLY_ADDED_STATUS_ID,
            StoreId = store?.Id,
            PlannedShoppingDate = request.PlannedShoppingDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.ShoppingLists.Add(shoppingList);
        await dbContext.SaveChangesAsync();

        if (request.Products != null && request.Products.Any())
        {

            var products = request.Products.Select(p => new Product
            {
                ShoppingListId = shoppingList.Id,
                Name = p.Name,
                Quantity = p.Quantity,
                StatusId = MANUALLY_ADDED_STATUS_ID,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();

            return await GetShoppingListByIdAsync(shoppingList.Id, userId);
        }

        var response = new ShoppingListDetailResponse(
            shoppingList.Id,
            shoppingList.Title,
            shoppingList.Store?.Name,
            shoppingList.PlannedShoppingDate,
            shoppingList.CreatedAt,
            shoppingList.UpdatedAt,
            shoppingList.Source.Name,
            store?.Name,
            []
        );

        return response;
    }

    public async Task<ErrorOr<ShoppingListDetailResponse>> UpdateShoppingListAsync(
        int listId, int userId, UpdateShoppingListRequest request)
    {
        var shoppingList = await dbContext.ShoppingLists
            .FirstOrDefaultAsync(sl => sl.Id == listId && sl.UserId == userId);

        if (shoppingList == null)
        {
            return Error.NotFound("Shopping list not found");
        }

        shoppingList.SourceId = shoppingList.SourceId == 1
            ? 2
            : shoppingList.SourceId;
        shoppingList.Title = request.Title;
        shoppingList.PlannedShoppingDate = request.PlannedShoppingDate;

        if (!string.IsNullOrWhiteSpace(request.StoreName))
        {
            var store = await dbContext.Stores
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Name.ToLower() == request.StoreName.ToLower());

            if (store == null)
            {
                store = new()
                {
                    UserId = userId, Name = request.StoreName, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
                };
                dbContext.Stores.Add(store);
                await dbContext.SaveChangesAsync();
            }

            shoppingList.StoreId = store.Id;
        }

        shoppingList.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return await GetShoppingListByIdAsync(listId, userId);
    }

    public async Task<ErrorOr<bool>> DeleteShoppingListAsync(int listId, int userId)
    {
        var shoppingList = await dbContext.ShoppingLists
            .FirstOrDefaultAsync(sl => sl.Id == listId && sl.UserId == userId);

        if (shoppingList == null)
        {
            return Error.NotFound("Shopping list not found");
        }

        dbContext.ShoppingLists.Remove(shoppingList);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<ErrorOr<ShoppingListDetailResponse>> GenerateShoppingListAsync(
        int userId, GenerateShoppingListRequest request)
    {
        const int AI_GENERATED_STATUS_ID = 1;

        try
        {
            var user = await dbContext.Users
                .Include(u => u.Ages)
                .Include(u => u.DietaryPreferences)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Error.NotFound("User not found");
            }
            
            var recentLists = await dbContext.ShoppingLists
                .Include(sl => sl.Products)
                .Where(sl => sl.UserId == userId)
                .OrderByDescending(sl => sl.CreatedAt)
                .Take(5)
                .ToListAsync();

            var prompt = new StringBuilder();
            
            prompt.AppendLine("You are an AI assistant that helps create shopping lists based on user preferences and shopping history.");
            prompt.AppendLine("Generate a shopping list with common grocery items appropriate for the provided user profile and history.");
            prompt.AppendLine("For each product, suggest a reasonable quantity based on household size and dietary preferences.");
            prompt.AppendLine("Format the response as a JSON array with products in the format: [{\"name\": \"Product name\", \"quantity\": number}]");
            prompt.AppendLine("Only return the JSON array, nothing else.");
            prompt.AppendLine("Product names should be in Polish.");
            
            prompt.AppendLine("\nUser profile:");
            prompt.AppendLine($"- Household size: {user.HouseholdSize ?? 1}");
            
            if (user.Ages.Any())
            {
                prompt.AppendLine($"- Ages: {string.Join(", ", user.Ages.Select(a => a.Age))}");
            }
            
            if (user.DietaryPreferences.Any())
            {
                prompt.AppendLine($"- Dietary preferences: {string.Join(", ", user.DietaryPreferences.Select(dp => dp.Preference))}");
            }
            
            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                prompt.AppendLine($"\nShopping list title: {request.Title}");
            }
            
            if (request.PlannedShoppingDate.HasValue)
            {
                prompt.AppendLine($"Planned shopping date: {request.PlannedShoppingDate.Value:yyyy-MM-dd}");
            }
            
            if (!string.IsNullOrWhiteSpace(request.StoreName))
            {
                prompt.AppendLine($"Store: {request.StoreName}");
            }
            
            if (recentLists.Any())
            {
                prompt.AppendLine("\nRecent shopping history:");
                foreach (var list in recentLists)
                {
                    prompt.AppendLine($"- {list.Title ?? "Untitled list"} ({list.PlannedShoppingDate:yyyy-MM-dd}):");
                    foreach (var product in list.Products)
                    {
                        prompt.AppendLine($"  * {product.Name}: {product.Quantity}");
                    }
                }
            }

            var messages = new[]
            {
                new ChatMessage { Role = "system", Content = "You are a helpful shopping list assistant." },
                new ChatMessage { Role = "user", Content = prompt.ToString() }
            };
            
            var openRouterRequest = new OpenRouterRequest
            {
                Model = model,
                Messages = messages
            };
            
            string chatCompletionsEndpoint = baseUrl.EndsWith("/") 
                ? "chat/completions" 
                : "/chat/completions";
                
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}{chatCompletionsEndpoint}");
            httpRequest.Headers.Authorization = new("Bearer", apiKey);
            httpRequest.Content = new StringContent(
                JsonSerializer.Serialize(openRouterRequest),
                Encoding.UTF8,
                "application/json");
            
            var httpResponse = await httpClient.SendAsync(httpRequest);
            
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                return Error.Failure("AIServiceError", $"Failed to generate shopping list: {errorContent}");
            }
            
            var openRouterResponse = await httpResponse.Content.ReadFromJsonAsync<OpenRouterResponse>();
            if (openRouterResponse == null)
            {
                return Error.Failure("AIServiceError", "Failed to parse AI response");
            }
            
            string aiResponseContent = openRouterResponse.Choices[0].Message.Content;
            
            string jsonContent = extractJsonArray(aiResponseContent);
            var productSuggestions = JsonSerializer.Deserialize<List<ProductSuggestion>>(jsonContent);
            
            if (productSuggestions == null || !productSuggestions.Any())
            {
                return Error.Failure("AIServiceError", "AI generated an empty shopping list");
            }
            
            Store? store = null;
            if (!string.IsNullOrWhiteSpace(request.StoreName))
            {
                store = await dbContext.Stores
                    .FirstOrDefaultAsync(s => s.UserId == userId && s.Name.ToLower() == request.StoreName.ToLower());

                if (store == null)
                {
                    store = new()
                    {
                        UserId = userId, 
                        Name = request.StoreName, 
                        CreatedAt = DateTime.UtcNow, 
                        UpdatedAt = DateTime.UtcNow
                    };
                    dbContext.Stores.Add(store);
                    await dbContext.SaveChangesAsync();
                }
            }
            
            var shoppingList = new Db.Models.ShoppingList
            {
                UserId = userId,
                Title = request.Title ?? "AI Generated Shopping List",
                SourceId = AI_GENERATED_STATUS_ID,
                StoreId = store?.Id,
                PlannedShoppingDate = request.PlannedShoppingDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            dbContext.ShoppingLists.Add(shoppingList);
            await dbContext.SaveChangesAsync();
            
            var products = productSuggestions.Select(p => new Product
            {
                ShoppingListId = shoppingList.Id,
                Name = p.Name,
                Quantity = p.Quantity,
                StatusId = AI_GENERATED_STATUS_ID,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();
            
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
            
            return await GetShoppingListByIdAsync(shoppingList.Id, userId);
        }
        catch (Exception ex)
        {
            return Error.Failure("AIServiceError", $"Error generating shopping list: {ex.Message}");
        }
    }
    
    private string extractJsonArray(string content)
    {
        var startIndex = content.IndexOf('[');
        var endIndex = content.LastIndexOf(']');
        
        if (startIndex >= 0 && endIndex > startIndex)
        {
            return content.Substring(startIndex, endIndex - startIndex + 1);
        }
        
        return "[]";
    }
}
