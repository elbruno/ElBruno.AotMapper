# Known Limitations

This document lists known constraints and workarounds for ElBruno.AotMapper.

## Mapping Limitations

### Complex Type Converters

❌ **Not Supported:**
Custom converters that require runtime reflection, lambdas, or complex logic.

**Workaround:**
Implement a partial method to handle the custom logic:

```csharp
[MapFrom(typeof(Source))]
public sealed partial record Destination(string CustomField)
{
    partial void MapFromPartial_OnMapEnd(Source source);
}

public sealed partial record Destination
{
    partial void MapFromPartial_OnMapEnd(Source source)
    {
        // Custom logic here
        CustomField = TransformComplexValue(source.ComplexProperty);
    }

    private string TransformComplexValue(object value) => /* ... */;
}
```

### Circular Dependencies

❌ **Not Supported:**
Circular reference mapping (Type A references Type B references Type A) can cause infinite recursion.

**Workaround:**
Break the cycle with `[MapIgnore]` and use partial methods for manual handling:

```csharp
[MapFrom(typeof(Parent))]
public sealed partial record ParentDto(
    string Id,
    [MapIgnore] ChildDto? Child  // Break the cycle
);
```

### Dynamic Property Discovery

❌ **Not Supported:**
Dynamic properties, properties added at runtime via reflection, or property bags.

**Workaround:**
Use explicit DTOs for each shape you need to map. ElBruno.AotMapper assumes a static type system.

### Conditional Mapping

❌ **Not Supported:**
Mapping that depends on runtime conditions (e.g., map property X only if property Y > 0).

**Workaround:**
Implement custom logic in a partial method:

```csharp
[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(
    string Id,
    string Name,
    decimal? Discount
)
{
    partial void MapFromPartial_OnMapEnd(Customer source);
}

public sealed partial record CustomerDto
{
    partial void MapFromPartial_OnMapEnd(Customer source)
    {
        Discount = source.Score > 100 ? source.Discount : null;
    }
}
```

## Type System Limitations

### Unbound Generic Types

❌ **Not Fully Supported:**
Open generic types (e.g., `T` without constraints) require explicit instantiation.

**Workaround:**
Define concrete mappings for each type you need:

```csharp
// Instead of generic MapFrom, define mappings per type
[MapFrom(typeof(List<string>))]
public sealed partial record StringListDto(List<string> Items);

[MapFrom(typeof(List<int>))]
public sealed partial record IntListDto(List<int> Items);
```

### Reflection-Based Property Discovery

❌ **Not Supported:**
The generator uses static type information; no runtime reflection-based property discovery.

**Workaround:**
Explicitly define all mappings at compile time. This is by design for AOT safety.

## EF Core Query Translation

See [EF Integration Guide](ef-integration.md) for detailed limitations around query translation.

### What EF Cannot Translate

❌ **Not Translatable to SQL:**
- Conditional logic with complex C# expressions
- Custom CLR methods (unless they're SQL functions)
- Nested collection projections in some query patterns
- Circular references

**Workaround:**
Materialize the query before mapping:

```csharp
// ❌ May not translate
var dtos = context.Orders
    .ProjectToOrderDto()  // Complex projection fails
    .ToList();

// ✅ Workaround: materialize first
var orders = await context.Orders.ToListAsync();
var dtos = orders.Select(o => o.ToOrderDto()).ToList();
```

## Performance Considerations

### Large Object Graphs

⚠️ **Performance Note:**
Mapping large nested object hierarchies or collections generates larger IL code. Consider breaking large DTOs into smaller, focused DTOs.

**Mitigation:**
- Map only the properties you need
- Use `[MapIgnore]` to exclude unnecessary fields
- Consider separate DTOs for different use cases

### Collection Materialization

⚠️ **Performance Note:**
Collections are materialized in memory during mapping. Large collections will consume heap memory.

**Mitigation:**
- Filter collections before mapping (e.g., in LINQ where appropriate)
- Use `ProjectToDto()` on EF queries to filter at the database level
- Consider pagination for large result sets

## Compatibility Limitations

### Assembly Trimming

⚠️ **Caveat:**
While the generated mappers are AOT-safe, custom partial methods or converters you add may not be trimming-safe. Use `[DynamicallyAccessedMembers]` attributes if needed.

### NativeAOT

✅ **Supported:**
Generated mappers are fully compatible with NativeAOT. However:
- Custom partial methods must not use reflection
- All types used in mappings must be reachable in the call graph
- Serializers (if used with generated mappers) must be AOT-aware

### .NET Framework

❌ **Not Supported:**
ElBruno.AotMapper targets .NET Standard 2.0+ for the core and .NET 8+ for integration packages. .NET Framework is not supported.

## Build & Generation Limitations

### Incremental Build Cache

⚠️ **Known Issue:**
In rare cases, incremental build cache may become stale. If mappings aren't generated:

**Workaround:**
```bash
dotnet clean
dotnet build
```

### Large Solution Scalability

⚠️ **Performance Note:**
In solutions with hundreds of mapped types, source generation may add 5–15 seconds to build time. This is expected.

**Mitigation:**
- Run builds in parallel where possible
- Use faster hardware for development
- CI builds are typically unaffected due to better caching

## Diagnostics Limitations

### Missing Property Messages

⚠️ **Message Clarity:**
Diagnostic messages for complex nested scenarios may reference the outermost type. Trace the error through the nested types to understand the root cause.

See [Diagnostics Reference](diagnostics.md) for all diagnostic codes and remediation steps.

---

## Next Steps

- Check [Diagnostics Reference](diagnostics.md) for error codes and fixes
- Review [EF Integration Guide](ef-integration.md) for query translation details
- See [Quick Start](quick-start.md) for usage examples
