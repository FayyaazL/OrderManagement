using Microsoft.AspNetCore.Mvc;

namespace OrderManagement.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (KeyNotFoundException ex)
        {
            await WriteError(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteError(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (ArgumentException ex)
        {
            await WriteError(context, StatusCodes.Status400BadRequest, ex.Message);
        }
    }

    private static async Task WriteError(
        HttpContext context,
        int statusCode,
        string message)
    {
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode switch
            {
                400 => "Validation error",
                404 => "Resource not found",
                409 => "Conflict",
                _ => "Request failed"
            },
            Detail = message
        });
    }
}