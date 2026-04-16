namespace ElBruno.AotMapper;

/// <summary>
/// Interface for custom property converters.
/// </summary>
/// <typeparam name="TSource">Source property type</typeparam>
/// <typeparam name="TDestination">Destination property type</typeparam>
public interface IMapConverter<TSource, TDestination>
{
    TDestination Convert(TSource source);
}
