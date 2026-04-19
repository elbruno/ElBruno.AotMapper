using Microsoft.Extensions.DependencyInjection;
using System;

namespace ElBruno.AotMapper.AspNetCore;

/// <summary>
/// Extension methods for registering AotMapper services with the DI container.
/// </summary>
public static class AotMapperServiceCollectionExtensions
{
    /// <summary>
    /// Registers AotMapper services with default configuration.
    /// Generated mappers are static extension methods, so no runtime registration is needed.
    /// This method provides a consistent API and future extensibility.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAotMapper(this IServiceCollection services)
    {
        return services.AddAotMapper(_ => { });
    }

    /// <summary>
    /// Registers AotMapper services with custom configuration.
    /// Generated mappers are static extension methods, so no runtime registration is needed.
    /// This method provides a consistent API and future extensibility.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration action for AotMapper options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAotMapper(this IServiceCollection services, Action<AotMapperOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new AotMapperOptions();
        configure(options);

        // Future: register options or services based on configuration
        // For now, this is a placeholder for v1 API stability

        return services;
    }
}

