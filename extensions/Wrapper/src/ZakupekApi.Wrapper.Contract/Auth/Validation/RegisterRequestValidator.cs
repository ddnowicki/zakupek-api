using FastEndpoints;
using FluentValidation;
using ZakupekApi.Wrapper.Contract.Auth.Request;

namespace ZakupekApi.Wrapper.Contract.Auth.Validation;

public class RegisterRequestValidator : Validator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required");

        RuleFor(x => x.HouseholdSize)
            .GreaterThanOrEqualTo(1).WithMessage("Household size must be at least 1");

        When(x => x.Ages != null, () =>
        {
            RuleFor(x => x.Ages)
                .Must(ages => ages!.All(age => age > 0))
                .WithMessage("All ages must be greater than 0")
                .Must((request, ages) => ages!.Count == request.HouseholdSize)
                .WithMessage("The number of ages must match the household size");
        });
    }
}
