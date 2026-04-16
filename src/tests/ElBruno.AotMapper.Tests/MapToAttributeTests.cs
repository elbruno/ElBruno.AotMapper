using Xunit;

namespace ElBruno.AotMapper.Tests;

public class MapToAttributeTests
{
    [Fact]
    public void Constructor_SetsDestinationType()
    {
        var attr = new MapToAttribute(typeof(string));
        Assert.Equal(typeof(string), attr.DestinationType);
    }

    [Fact]
    public void AllowsMultiple()
    {
        var usage = typeof(MapToAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .Single();
        Assert.True(usage.AllowMultiple);
    }

    [Fact]
    public void TargetsClassAndStruct()
    {
        var usage = typeof(MapToAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .Single();
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Struct));
    }
}
