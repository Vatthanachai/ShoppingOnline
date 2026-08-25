using System.Net;

using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using ShoppingOnline.Component.Abstractions.Emails.Options;
using ShoppingOnline.Component.Abstractions.Extensions;
using ShoppingOnline.Component.Abstractions.Filters;
using ShoppingOnline.Component.Abstractions.Health.HealthChecks.Settings;
using ShoppingOnline.Component.Abstractions.Middlewares;
using ShoppingOnline.Component.Abstractions.Middlewares.Validations;
using ShoppingOnline.Component.Abstractions.Securities;
using ShoppingOnline.Component.Abstractions.Securities.Options;
using ShoppingOnline.Component.Abstractions.Swaggers;
using ShoppingOnline.Model;

using IPNetwork = System.Net.IPNetwork;

namespace ShoppingOnline.API.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddServiceRegister(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment env)
    {
        services.SettingRegister(configuration);
        services.AddMemoryCache();

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SnakeCaseParameterTransformer()));
                options.Filters.Add<JsonExceptionFilter>();

                // Aspire runs services over plain HTTP locally (no TLS termination in front),
                // so enforcing HTTPS here would reject every request in dev. RequireHttpsAttribute
                // (unlike classic ASP.NET MVC) has no built-in localhost exemption.
                if (!env.IsDevelopment())
                {
                    options.Filters.Add<RequireHttpsOrCloseAttribute>();
                }
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver =
                    new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };
                options.SerializerSettings.Converters.Add(new StringEnumConverter());

                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

                options.SerializerSettings.Formatting = Formatting.None;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd'T'HH:mm:ssZ";
            });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.ConfigureSnakeCaseValidationResponse();
        });

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto;
            if (env.IsDevelopment())
            {
                options.KnownIPNetworks.Clear();
                options.KnownProxies.Clear();
            }
            else
            {
                options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
                //Ingress Controller IP/Traefik/CIDR
                /*
                options.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(
                    IPAddress.Parse("10.42.0.0"), 16));
                */
                options.KnownIPNetworks.Add(new IPNetwork(IPAddress.Parse("127.0.0.1"), 16));
            }
        });

        services.AddAuthentication(PasetoAuthenticationDefaults.AuthenticationScheme)
            .AddScheme<PasetoAuthenticationOptions, PasetoAuthenticationHandler>(
                PasetoAuthenticationDefaults.AuthenticationScheme, null);
        services.AddAuthorization();

        services.AddHealthChecks()
            .HealthCheckRegister();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerDocument(ApiAssembly.Instance.Assembly, ModelAssembly.Instance.Assembly);
        return services;
    }


    private static IServiceCollection SettingRegister(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterAppSettings<ReleaseSettings>(configuration, nameof(ReleaseSettings));
        services.RegisterAppSettings<HashSetting>(configuration, nameof(HashSetting));
        services.RegisterAppSettings<PasetoSetting>(configuration, nameof(PasetoSetting));
        services.RegisterAppSettings<SmtpSetting>(configuration, nameof(SmtpSetting));

        return services;
    }
}