using FastEndpoints;
using FluentValidation;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Request;

namespace ZakupekApi.Wrapper.Contract.ShoppingLists.Validation;

public class GenerateShoppingListValidator : Validator<GenerateShoppingListRequest>
{
    public GenerateShoppingListValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(100)
            .WithMessage("Title cannot exceed 100 characters");
    }
}
