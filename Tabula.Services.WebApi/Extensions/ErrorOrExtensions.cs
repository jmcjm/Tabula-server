using ErrorOr;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Tabula.Services.WebApi.Extensions;

public static class ErrorOrExtensions
{
    public static IResult ToHttpResult<T>(this ErrorOr<T> errorOr)
    {
        if (errorOr.IsError)
        {
            return HandleErrors(errorOr.Errors);
        }

        // Jeśli to jest typ Result (Success, Created, etc.), zwracamy odpowiedni status
        if (typeof(T).IsAssignableTo(typeof(IResult)))
        {
            return (IResult)(object)errorOr.Value!;
        }

        // W przeciwnym razie zwracamy wartość jako JSON
        return TypedResults.Ok(errorOr.Value);
    }

    private static IResult ToHttpResult<T>(this ErrorOr<T> errorOr, Func<T, IResult> onSuccess)
    {
        return errorOr.IsError ? HandleErrors(errorOr.Errors) : onSuccess(errorOr.Value);
    }

    public static async Task<IResult> ToHttpResultAsync<T>(this Task<ErrorOr<T>> errorOrTask)
    {
        var errorOr = await errorOrTask;
        return errorOr.ToHttpResult();
    }

    public static async Task<IResult> ToHttpResultAsync<T>(this Task<ErrorOr<T>> errorOrTask, Func<T, IResult> onSuccess)
    {
        var errorOr = await errorOrTask;
        return errorOr.ToHttpResult(onSuccess);
    }

    private static IResult HandleErrors(List<Error> errors)
    {
        var firstError = errors[0];

        return firstError.Type switch
        {
            ErrorType.NotFound => TypedResults.NotFound(new 
            {
                error = firstError.Description,
                code = firstError.Code
            }),
            ErrorType.Validation => TypedResults.BadRequest(new 
            {
                error = firstError.Description,
                code = firstError.Code,
                errors = errors.Count > 1 ? errors.Select(e => new { e.Code, e.Description }).ToArray() : null
            }),
            ErrorType.Conflict => TypedResults.Conflict(new 
            {
                error = firstError.Description,
                code = firstError.Code
            }),
            ErrorType.Forbidden => TypedResults.Forbid(),
            ErrorType.Unauthorized => TypedResults.Unauthorized(),
            _ => TypedResults.Problem(
                title: "An error occurred",
                detail: firstError.Description,
                statusCode: 500)
        };
    }
} 