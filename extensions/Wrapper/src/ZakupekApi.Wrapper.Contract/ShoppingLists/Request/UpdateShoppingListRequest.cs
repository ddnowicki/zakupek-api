namespace ZakupekApi.Wrapper.Contract.ShoppingLists.Request;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public record UpdateProductRequest(
    int? Id,
    [Required(ErrorMessage = "Product name is required")] string Name,
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")] int Quantity
);

public record UpdateShoppingListRequest(
    string? Title,
    IEnumerable<UpdateProductRequest>? Products,
    DateTime? PlannedShoppingDate,
    string? StoreName
);
