using CSharpFunctionalExtensions;
using EventBooking.Domain.Errors;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking.Api;

public sealed record ApiError(string Code, string Message);

public sealed record ApiResponse(bool Success, object? Data, ApiError? Error);

public static class EndpointResponse
{
    public static IActionResult From<T>(ControllerBase controller, Result<T, Error> result)
    {
        return result.IsSuccess
            ? controller.Ok(new ApiResponse(true, result.Value, null))
            : FromError(controller, result.Error);
    }

    public static IActionResult From(ControllerBase controller, UnitResult<Error> result)
    {
        return result.IsSuccess
            ? controller.Ok(new ApiResponse(true, null, null))
            : FromError(controller, result.Error);
    }

    private static IActionResult FromError(ControllerBase controller, Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return controller.StatusCode(
            statusCode,
            new ApiResponse(false, null, new ApiError(error.Code, error.Message)));
    }
}
