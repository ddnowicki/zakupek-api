using ErrorOr;
using Microsoft.EntityFrameworkCore;
using ZakupekApi.Db.Data;
using ZakupekApi.Db.Models;
using ZakupekApi.Wrapper.Abstraction.ShoppingList;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Request;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Response;

namespace ZakupekApi.Wrapper.ShoppingList;

public class ShoppingListService(AppDbContext dbContext) : IShoppingListService
{
    public async Task<ErrorOr<ShoppingListsResponse>> GetShoppingListsAsync(
        int userId, int page = 1, int pageSize = 10, string sort = "newest")
    {
        var query = dbContext.ShoppingLists
            .Include(sl => sl.Source)
            .Include(sl => sl.Store)
            .Where(sl => sl.UserId == userId);

        query = ApplySorting(query, sort);

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

        static IQueryable<Db.Models.ShoppingList> ApplySorting(IQueryable<Db.Models.ShoppingList> query, string sort) =>
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
}
