using System;

namespace ElBruno.AotMapper.Generator.Models;

internal readonly struct PropertyRename : IEquatable<PropertyRename>
{
    public string DestinationProperty { get; }
    public string SourceProperty { get; }

    public PropertyRename(string destinationProperty, string sourceProperty)
    {
        DestinationProperty = destinationProperty;
        SourceProperty = sourceProperty;
    }

    public bool Equals(PropertyRename other)
    {
        return DestinationProperty == other.DestinationProperty &&
               SourceProperty == other.SourceProperty;
    }

    public override bool Equals(object? obj)
    {
        return obj is PropertyRename other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((DestinationProperty?.GetHashCode() ?? 0) * 397) ^
                   (SourceProperty?.GetHashCode() ?? 0);
        }
    }
}
