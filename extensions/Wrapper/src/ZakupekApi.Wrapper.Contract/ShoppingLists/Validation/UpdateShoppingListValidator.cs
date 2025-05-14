using FastEndpoints;
using FluentValidation;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Request;

namespace ZakupekApi.Wrapper.Contract.ShoppingLists.Validation;

public class UpdateShoppingListValidator : Validator<UpdateShoppingListRequest>
{
    public UpdateShoppingListValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(100)
            .WithMessage("Title cannot exceed 100 characters");

        RuleForEach(x => x.Products)
            .SetValidator(new UpdateProductRequestValidator());
    }
}

public class UpdateProductRequestValidator : Validator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");
    }
}
