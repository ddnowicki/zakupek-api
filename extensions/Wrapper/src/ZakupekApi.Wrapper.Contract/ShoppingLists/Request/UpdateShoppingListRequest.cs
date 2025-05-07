namespace ZakupekApi.Wrapper.Contract.ShoppingLists.Request;

using System;
using System.Collections.Generic;

public record UpdateProductRequest(int? Id, string Name, int Quantity);

public record UpdateShoppingListRequest(
    string? Title,
    IEnumerable<UpdateProductRequest>? Products,
    DateTime? PlannedShoppingDate,
    string? StoreName
);
