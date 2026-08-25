using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Scalar.AspNetCore;

using ShoppingOnline.API.Utilities;
using ShoppingOnline.Database.Context;

namespace ShoppingOnline.API.Extensions;

public static class WebApplicationExtension
{
    public static WebApplication UseApplicationSetting(this WebApplication app)
    {
        app.UseRouting();
        app.MapDefaultEndpoints();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "docs/{documentName}.json";
            });

            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    string MapStatus(HealthStatus status) => status switch
                    {
                        HealthStatus.Healthy => "Health",
                        HealthStatus.Degraded => "Degraded",
                        HealthStatus.Unhealthy => "Unhealthy",
                        _ => "Internal Error"
                    };

                    var entries = report.Entries.ToDictionary(
                        kvp => kvp.Key,
                        kvp => new
                        {
                            data = kvp.Value.Data ?? new Dictionary<string, object>(),
                            description = kvp.Value.Description,
                            duration = kvp.Value.Duration.ToString(),
                            status = MapStatus(kvp.Value.Status),
                            tags = kvp.Value.Tags
                        });

                    var response = new
                    {
                        is_success = report.Status == HealthStatus.Healthy,
                        data = new
                        {
                            entries,
                            status = MapStatus(report.Status),
                            total_duration = report.TotalDuration.ToString()
                        }
                    };

                    var json = JsonConvert.SerializeObject(
                        response,
                        Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            ContractResolver =
                                new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() }
                        });

                    await context.Response.WriteAsync(json);
                }
            });

            app.MapScalarApiReference("/docs", options =>
            {
                options.WithTitle("Todo Identity Api")
                    .WithTheme(ScalarTheme.Saturn)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.RestSharp);

                // Scalar defaults to "/openapi/{documentName}.json" (the ASP.NET Core native
                // OpenApi route from MapOpenApi(), which isn't registered here). The actual
                // document - versioned and filtered - is served by Swashbuckle at the custom
                // route configured above via UseSwagger(options.RouteTemplate). Hardcoded to
                // the single "v1.0" document registered via SwaggerSettings:Versions below,
                // rather than relying on Scalar's undocumented default {documentName} guess.
                options.OpenApiRoutePattern = "/docs/v1.0.json";
                options.Servers = new List<ScalarServer>();
            });
        }

        app.UseForwardedHeaders();
        app.UseExceptionHandler("/error");
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseStaticFiles();

        app.MapControllers();
        app.MapStaticAssets().ShortCircuit();

        app.UseDatabaseMigration();
        return app;
    }

    private static void UseDatabaseMigration(this WebApplication app)
    {
        using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IShoppingDbContext>();
        var dataMockup = scope.ServiceProvider.GetRequiredService<IDataMockupService>();

        dbContext.Database.Migrate();
        dataMockup.InitializeData();
    }
}