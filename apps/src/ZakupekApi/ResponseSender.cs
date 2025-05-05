using System.Collections.Concurrent;
using System.Linq.Expressions;
using ErrorOr;
using FastEndpoints;
using FluentValidation.Results;

namespace ZakupekApi;

sealed class ResponseSender : IGlobalPostProcessor
{
    public Task PostProcessAsync(IPostProcessorContext ctx, CancellationToken ct)
    {
        if (ctx.HttpContext.ResponseStarted() || ctx.Response is not IErrorOr errorOr)
            return Task.CompletedTask;

        if (!errorOr.IsError)
            return ctx.HttpContext.Response.SendAsync(GetValueFromErrorOr(errorOr), cancellation: ct);

        if (errorOr.Errors?.All(e => e.Type == ErrorType.Validation) is true)
        {
            return ctx.HttpContext.Response.SendErrorsAsync(
                failures: [..errorOr.Errors.Select(e => new ValidationFailure(e.Code, e.Description))],
                cancellation: ct);
        }

        var problem = errorOr.Errors?.FirstOrDefault(e => e.Type != ErrorType.Validation);

        switch (problem?.Type)
        {
            case ErrorType.Conflict:
                return ctx.HttpContext.Response.SendAsync("Duplicate submission!", 409, cancellation: ct);
            case ErrorType.NotFound:
                return ctx.HttpContext.Response.SendNotFoundAsync(ct);
            case ErrorType.Unauthorized:
                return ctx.HttpContext.Response.SendUnauthorizedAsync(ct);
            case ErrorType.Forbidden:
                return ctx.HttpContext.Response.SendForbiddenAsync(ct);
            case null:
                throw new InvalidOperationException();
        }

        return Task.CompletedTask;
    }

    //cached compiled expressions for reading ErrorOr<T>.Value property
    static readonly ConcurrentDictionary<Type, Func<object, object>> _valueAccessors = new();

    static object GetValueFromErrorOr(object errorOr)
    {
        ArgumentNullException.ThrowIfNull(errorOr);
        var tErrorOr = errorOr.GetType();

        if (!tErrorOr.IsGenericType || tErrorOr.GetGenericTypeDefinition() != typeof(ErrorOr<>))
            throw new InvalidOperationException("The provided object is not an instance of ErrorOr<>.");

        return _valueAccessors.GetOrAdd(tErrorOr, CreateValueAccessor)(errorOr);

        static Func<object, object> CreateValueAccessor(Type errorOrType)
        {
            var parameter = Expression.Parameter(typeof(object), "errorOr");

            return Expression.Lambda<Func<object, object>>(
                                 Expression.Convert(
                                     Expression.Property(
                                         Expression.Convert(parameter, errorOrType),
                                         "Value"),
                                     typeof(object)),
                                 parameter)
                             .Compile();
        }
    }
}
