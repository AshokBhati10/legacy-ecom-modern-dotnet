using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace LegacyEcom.Api.Middleware;

/// <summary>
/// Last-resort exception handler. Converts unhandled exceptions into RFC 7807
/// problem details without leaking stack traces or internals to API clients.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadHttpRequestException ex)
        {
            // Malformed request / missing required parameter -> 400, not 500.
            _logger.LogDebug(ex, "Bad request for {Method} {Path}.",
                context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad request.",
                Detail = _env.IsDevelopment() ? ex.Message : "The request was invalid.",
                Instance = context.Request.Path
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing {Method} {Path}.",
                context.Request.Method, context.Request.Path);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "An unexpected error occurred.",
                Detail = _env.IsDevelopment() ? ex.Message : "Please try again later.",
                Instance = context.Request.Path
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
