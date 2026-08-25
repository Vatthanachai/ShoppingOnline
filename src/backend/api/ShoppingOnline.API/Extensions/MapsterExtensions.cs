using System.Reflection;

using Mapster;

using ShoppingOnline.Service;

namespace ShoppingOnline.API.Extensions;

public static class MapsterExtensions
{
    public static IServiceCollection MapsterRegister(this IServiceCollection services)
    {
        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly(), ServiceAssembly.Instance.Assembly);

        services.AddSingleton(TypeAdapterConfig.GlobalSettings);
        services.AddMapster();

        return services;
    }
}