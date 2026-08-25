using Serilog;

namespace ShoppingOnline.API.Extensions;

public static class HostBuilderExtension
{
    public static IHostBuilder AddHostSetting(this IHostBuilder builder)
    {
        builder.UseConfigurationSetting();
        builder.UseSeriLog();
        return builder;
    }

    private static IHostBuilder UseConfigurationSetting(this IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var environment = context.HostingEnvironment;


            config
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, true);

            config
                .AddJsonFile("serilogsettings.json", optional: true, true)
                .AddJsonFile($"serilogsettings.{environment.EnvironmentName}.json", optional: true, true);

            config.AddEnvironmentVariables();
        });

        return builder;
    }

    private static IHostBuilder UseSeriLog(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });
        return hostBuilder;
    }
}