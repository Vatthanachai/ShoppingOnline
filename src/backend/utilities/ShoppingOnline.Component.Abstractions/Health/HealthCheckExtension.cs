using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using ShoppingOnline.Component.Abstractions.Health.HealthChecks;

namespace ShoppingOnline.Component.Abstractions.Health;

/// <summary>
/// Extension methods for adding health checks.
/// </summary>
public static class HealthCheckExtension
{
    /// <summary>
    /// Adds a health check for validating the database context.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHealthChecksBuilder AddVersionHealthCheck(this IHealthChecksBuilder builder)
        => builder.AddCheck<VersionHealthCheck>("version", tags: ["version"]);
}