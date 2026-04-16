using Xunit;

namespace ElBruno.AotMapper.Tests;

public class MapPropertyAttributeTests
{
    [Fact]
    public void Constructor_SetsSourceAndDestinationProperty()
    {
        var attr = new MapPropertyAttribute("SourceProp", "DestProp");
        Assert.Equal("SourceProp", attr.SourceProperty);
        Assert.Equal("DestProp", attr.DestinationProperty);
    }

    [Fact]
    public void AllowsMultiple()
    {
        var usage = typeof(MapPropertyAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .Single();
        Assert.True(usage.AllowMultiple);
    }

    [Fact]
    public void TargetsClassAndStruct()
    {
        var usage = typeof(MapPropertyAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .Single();
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Struct));
    }
}
