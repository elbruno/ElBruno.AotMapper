using Xunit;

namespace ElBruno.AotMapper.Tests;

public class MapConverterAttributeTests
{
    [Fact]
    public void Constructor_SetsConverterType()
    {
        var attr = new MapConverterAttribute(typeof(string));
        Assert.Equal(typeof(string), attr.ConverterType);
    }

    [Fact]
    public void DoesNotAllowMultiple()
    {
        var usage = typeof(MapConverterAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .Single();
        Assert.False(usage.AllowMultiple);
    }

    [Fact]
    public void TargetsPropertyAndParameter()
    {
        var usage = typeof(MapConverterAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .Single();
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Property));
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Parameter));
    }
}
