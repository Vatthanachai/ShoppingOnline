using Autofac;
using Autofac.Extensions.DependencyInjection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

using ShoppingOnline.API.Utilities;
using ShoppingOnline.Component.Abstractions.Emails;
using ShoppingOnline.Component.Abstractions.Securities;
using ShoppingOnline.Database;
using ShoppingOnline.Database.Context;
using ShoppingOnline.Handler;
using ShoppingOnline.Service;

namespace ShoppingOnline.API.Extensions;

public static class AutofacExtension
{
    public static IHostBuilder AddAutofacRegister(this IHostBuilder builder)
    {
        builder.UseServiceProviderFactory(new AutofacServiceProviderFactory());

        builder.ConfigureContainer<ContainerBuilder>((context, cb) =>
        {
            cb.AddDatabaseRegister();
            cb.AddUnitOfWorkRegister();
            cb.AddServiceRegister();
            cb.AddUtilitiesRegister();
            cb.AddHandlerRegister();
        });

        return builder;
    }


    private static ContainerBuilder AddDatabaseRegister(this ContainerBuilder builder)
    {
        builder.Register(context =>
        {
            var configuration = context.Resolve<IConfiguration>();
            var strConnection = configuration.GetConnectionString("shoppingDb");

            return new DbContextOptionsBuilder<ShoppingDbContext>()
                .UseNpgsql(strConnection, option =>
                {
                    option.EnableRetryOnFailure(5);
                    option.MinBatchSize(5);
                    option.MaxBatchSize(10);
                    option.CommandTimeout(60);
                }).Options;
        });

        builder.Register(context =>
            {
                var dbContextOptions = context.Resolve<DbContextOptions<ShoppingDbContext>>();

                return dbContextOptions == null
                    ? throw new ArgumentException("The DbContext is not initialized.")
                    : new ShoppingDbContext(dbContextOptions);
            }).AsSelf()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();


        return builder;
    }

    private static ContainerBuilder AddUnitOfWorkRegister(this ContainerBuilder builder)
    {
        var currentServiceAssembly = DatabaseAssembly.Instance.Assembly;

        builder.RegisterAssemblyTypes(currentServiceAssembly)
            .Where(t => t.Name.ToLower().EndsWith("unitofwork") && !t.IsInterface &&
                        t.GetInterfaces().Any(i => i.Name.ToLower().StartsWith("i")))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        return builder;
    }

    private static ContainerBuilder AddServiceRegister(this ContainerBuilder builder)
    {
        var currentServiceAssembly = ServiceAssembly.Instance.Assembly;

        builder.RegisterAssemblyTypes(currentServiceAssembly)
            .Where(t => t.Name.ToLower().EndsWith("service") && !t.IsInterface &&
                        t.GetInterfaces().Any(i => i.Name.ToLower().StartsWith("i")))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        return builder;
    }

    private static ContainerBuilder AddHandlerRegister(this ContainerBuilder builder)
    {
        var currentServiceAssembly = HandlerAssembly.Instance.Assembly;

        builder.RegisterAssemblyTypes(currentServiceAssembly)
            .Where(t => t.Name.ToLower().EndsWith("handler") && !t.IsInterface &&
                        t.GetInterfaces().Any(i => i.Name.ToLower().StartsWith("i")))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        return builder;
    }

    private static ContainerBuilder AddUtilitiesRegister(this ContainerBuilder builder)
    {
        builder.RegisterType<EncryptionService>()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterType<PasetoTokenService>()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterType<SmtpEmailService>()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterType<DataMockupService>()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        return builder;
    }
}