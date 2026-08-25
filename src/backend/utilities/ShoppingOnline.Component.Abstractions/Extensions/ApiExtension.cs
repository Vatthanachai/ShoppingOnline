using Microsoft.Extensions.DependencyInjection;

using ShoppingOnline.Component.Abstractions.Health;
using ShoppingOnline.Component.Abstractions.Health.HealthChecks;
using ShoppingOnline.Component.Data.Context;

namespace ShoppingOnline.Component.Abstractions.Extensions;

public static class ApiExtension
{
    public static IHealthChecksBuilder HealthCheckRegister(this IHealthChecksBuilder builder)
    {
        // "self" is already registered by ServiceDefault's AddDefaultHealthChecks() (called via
        // builder.AddServiceDefaults() in Program.cs) - registering it again throws at startup
        // ("Duplicate health checks were registered with the name(s): self").
        builder.AddCheck<VersionHealthCheck>("version", tags: ["version"]);
        builder.AddDbContextHealthCheck([typeof(IBaseDbContext)]);


        return builder;
    }

    public static IHealthChecksBuilder AddDbContextHealthCheck(this IHealthChecksBuilder builder,
        params Type[] dbContextTypes)
        => builder.AddTypeActivatedCheck<DbContextHealthCheck>("dbcontexts", HealthStatus.Healthy
            , tags: ["dbcontexts"]
            , args: [dbContextTypes]);
}