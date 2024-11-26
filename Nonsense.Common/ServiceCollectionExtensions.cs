using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Nonsense.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppSettings<T>(this IServiceCollection services, ConfigurationManager configuration)
        where T : class
    {
        services.AddOptions<T>()
            .Bind(configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    /// <summary>
    /// Must be called after <see cref="AddAppSettings{T}"/>
    /// </summary>
    public static IServiceCollection AddDataContext<TContext, TSettings>(
        this IServiceCollection services, 
        Func<TSettings, string> connectionStringResolver)
            where TContext: DbContext
            where TSettings: class
    {
        using var provider = services.BuildServiceProvider();
        var appSettings = provider.GetRequiredService<IOptions<TSettings>>();
        var connectionString = connectionStringResolver(appSettings.Value);
        
        services.AddDbContext<TContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        return services;
    }
}