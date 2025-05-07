namespace ZakupekApi.Wrapper.Contract.ShoppingLists.Response;

public record PaginationMetadata(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages
);

public record ShoppingListsResponse(
    IEnumerable<ShoppingListResponse> Data,
    PaginationMetadata Pagination
);