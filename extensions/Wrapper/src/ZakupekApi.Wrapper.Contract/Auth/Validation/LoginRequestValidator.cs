using FastEndpoints;
using FluentValidation;
using ZakupekApi.Wrapper.Contract.Auth.Request;

namespace ZakupekApi.Wrapper.Contract.Auth.Validation;

public class LoginRequestValidator : Validator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}
