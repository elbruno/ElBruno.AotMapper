using Microsoft.Extensions.DependencyInjection;

namespace ElBruno.AotMapper.AspNetCore;

/// <summary>
/// Extension methods for registering AotMapper services with the DI container.
/// </summary>
public static class AotMapperServiceCollectionExtensions
{
    /// <summary>
    /// Registers AotMapper services. Generated mappers are static extension methods,
    /// so this primarily sets up any future DI-based mapper scenarios.
    /// </summary>
    public static IServiceCollection AddAotMapper(this IServiceCollection services)
    {
        return services;
    }
}
