using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace LegacyEcom.Api.Filters;

/// <summary>
/// Minimal-API endpoint filter that validates complex-type arguments with
/// data annotations (including <see cref="IValidatableObject"/>) and returns
/// a 400 validation problem on failure. .NET 8 has no built-in minimal-API
/// validation, so this replaces MVC's automatic ModelState validation.
/// </summary>
public class ValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var errors = new Dictionary<string, string[]>();

        foreach (var argument in context.Arguments)
        {
            if (argument is null) continue;
            var type = argument.GetType();
            if (!IsDto(type)) continue;

            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(argument);
            Validator.TryValidateObject(argument, validationContext, results, validateAllProperties: true);

            foreach (var result in results)
            {
                var key = result.MemberNames.FirstOrDefault() ?? string.Empty;
                if (!errors.TryGetValue(key, out var existing))
                    errors[key] = new[] { result.ErrorMessage ?? "Invalid value." };
                else
                    errors[key] = existing.Append(result.ErrorMessage ?? "Invalid value.").ToArray();
            }
        }

        if (errors.Count > 0)
            return Results.ValidationProblem(errors);

        return await next(context);
    }

    private static bool IsDto(Type type)
    {
        if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal) ||
            type == typeof(DateTime) || type == typeof(Guid))
            return false;
        if (type.Namespace is null) return false;
        return type.Namespace.StartsWith("LegacyEcom.Application.DTOs", StringComparison.Ordinal);
    }
}

/// <summary>
/// Marks a minimal-API endpoint as requiring a valid anti-forgery token
/// (X-XSRF-TOKEN header), the API equivalent of MVC's [ValidateAntiForgeryToken].
/// </summary>
public static class AntiforgeryEndpointExtensions
{
    public static TBuilder RequireAntiforgeryToken<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.AddEndpointFilter(async (context, next) =>
        {
            var antiforgery = context.HttpContext.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
            try
            {
                await antiforgery.ValidateRequestAsync(context.HttpContext);
            }
            catch (Microsoft.AspNetCore.Antiforgery.AntiforgeryValidationException)
            {
                return Results.Problem(
                    title: "Invalid anti-forgery token.",
                    detail: "Send the XSRF-TOKEN cookie value in the X-XSRF-TOKEN header. " +
                            "Get a token from GET /api/auth/xsrf-token.",
                    statusCode: StatusCodes.Status400BadRequest);
            }
            return await next(context);
        });
        return builder;
    }
}
