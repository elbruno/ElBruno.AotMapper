using Xunit;

namespace ElBruno.AotMapper.Tests;

public class MapFromAttributeTests
{
    [Fact]
    public void Constructor_SetsSourceType()
    {
        var attr = new MapFromAttribute(typeof(string));
        Assert.Equal(typeof(string), attr.SourceType);
    }

    [Fact]
    public void Strict_DefaultsToFalse()
    {
        var attr = new MapFromAttribute(typeof(string));
        Assert.False(attr.Strict);
    }

    [Fact]
    public void Strict_CanBeSetToTrue()
    {
        var attr = new MapFromAttribute(typeof(string)) { Strict = true };
        Assert.True(attr.Strict);
    }

    [Fact]
    public void AllowsMultiple()
    {
        var usage = typeof(MapFromAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .Single();
        Assert.True(usage.AllowMultiple);
    }

    [Fact]
    public void TargetsClassAndStruct()
    {
        var usage = typeof(MapFromAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .Single();
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Struct));
    }
}
