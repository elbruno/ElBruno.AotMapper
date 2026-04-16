using Xunit;

namespace ElBruno.AotMapper.Tests;

public class MapIgnoreAttributeTests
{
    [Fact]
    public void AttributeExists()
    {
        var attr = new MapIgnoreAttribute();
        Assert.NotNull(attr);
    }

    [Fact]
    public void DoesNotAllowMultiple()
    {
        var usage = typeof(MapIgnoreAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .Single();
        Assert.False(usage.AllowMultiple);
    }

    [Fact]
    public void TargetsPropertyAndParameter()
    {
        var usage = typeof(MapIgnoreAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .Single();
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Property));
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Parameter));
    }
}
