using FastEndpoints;
using FluentValidation;
using ZakupekApi.Wrapper.Contract.Auth.Request;

namespace ZakupekApi.Wrapper.Contract.Auth.Validation;

public class UpdateUserProfileRequestValidator : Validator<UpdateUserProfileRequest>
{
    public UpdateUserProfileRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username is required");

        RuleFor(x => x.HouseholdSize)
            .GreaterThanOrEqualTo(1)
            .When(x => x.HouseholdSize.HasValue)
            .WithMessage("Household size must be at least 1");

        RuleFor(x => x.Ages)
            .Must((request, ages) => ages == null || ages.Count == request.HouseholdSize)
            .When(x => x.HouseholdSize.HasValue && x.Ages != null)
            .WithMessage("Number of ages must match household size");

        RuleForEach(x => x.Ages)
            .GreaterThan(0)
            .When(x => x.Ages != null)
            .WithMessage("Age must be greater than 0");

        RuleForEach(x => x.DietaryPreferences)
            .NotEmpty()
            .When(x => x.DietaryPreferences != null)
            .WithMessage("Dietary preference cannot be empty");
    }
}
