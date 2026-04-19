# Troubleshooting

Common issues and solutions for ElBruno.AotMapper.

## Generated Method Not Appearing

### Symptom

You defined a `[MapFrom]` attribute but the extension method isn't generated.

### Solutions

**1. Check the mapping direction:**

`[MapFrom]` marks the **destination** type. The generated method is `source.ToDestination()`:

```csharp
// ✅ Correct: destination has [MapFrom]
[MapFrom(typeof(Customer))]
public record CustomerDto { }

// Generated: Customer.ToCustomerDto()
var dto = customer.ToCustomerDto();
```

```csharp
// ❌ Wrong: source has [MapFrom]
[MapFrom(typeof(OrderDto))]
public class Order { }

// No method generated!
```

Use `[MapTo]` to decorate the source instead:

```csharp
[MapTo(typeof(CustomerDto))]
public class Customer { }

// Still generates: Customer.ToCustomerDto()
```

**2. Rebuild the project:**

```bash
dotnet clean
dotnet build
```

The incremental generator caches results; a clean build forces regeneration.

**3. Check for diagnostic errors:**

Look at the Error List or build output for AOTMAP00x diagnostics. Fix them before the generator can emit methods.

---

## Diagnostic Errors (AOTMAP00x)

### Symptom

Build fails with `error AOTMAP00x: ...`

### Solution

See [Diagnostics Reference](docs/diagnostics.md) for each diagnostic ID and how to fix it.

Common ones:
- **AOTMAP001** — Missing or ambiguous property mapping
- **AOTMAP002** — Incompatible types in mapping
- **AOTMAP006** — Inaccessible constructor or property
- **AOTMAP007** — EF projection pattern not supported

---

## EF Core: "Cannot Translate" Error at Runtime

### Symptom

You're using `ProjectToXxx()` on an `IQueryable<T>`, and EF throws:

```
The LINQ expression '...' could not be translated. Either rewrite the query in a way that LINQ to Entities understands, or switch to client evaluation...
```

### Cause

The mapping contains patterns that don't translate to SQL (e.g., method calls, complex expressions).

### Solution

**1. Use in-memory mapping instead:**

```csharp
// ❌ May not translate
var dtos = context.Orders
    .ProjectToOrderDto()  // Full query translation
    .ToListAsync();

// ✅ Always works
var dtos = await context.Orders
    .AsNoTracking()
    .ToListAsync()
    .ContinueWith(task =>
        task.Result.Select(o => o.ToOrderDto()).ToList()
    );
```

**2. Use manual `.Select()` for supported patterns:**

```csharp
// ✅ Translates to SQL
var dtos = context.Orders
    .Select(o => new OrderDto
    {
        Id = o.Id,
        CreatedAt = o.CreatedAt
    })
    .ToListAsync();
```

**3. Check for AOTMAP007 diagnostic:**

If you see `AOTMAP007`, the pattern is known to not translate. Rewrite your mapping to use simpler properties or switch to in-memory mapping.

---

## AOT: Trimming Warnings at Publish

### Symptom

Running `dotnet publish -p:PublishAot=true` produces warnings about trimmed types:

```
ILLink warning IL2026: ...Parameter 'type' of method 'Activator.CreateInstance' cannot be statically analyzed...
```

### Cause

Your mapping uses reflection, dynamic construction, or types that can't be trimmed.

### Solutions

**1. Ensure constructors are public:**

```csharp
// ❌ Won't work with trimming
public partial record OrderDto(string Id)
{
    private OrderDto() { }  // private ctor
}

// ✅ Works with trimming
public partial record OrderDto(string Id);
```

**2. Use explicit `[MapConverter]`:**

```csharp
[MapFrom(typeof(Order))]
public partial record OrderDto
{
    [MapConverter(typeof(MyConverter))]
    public string Status { get; init; }
}

// Converter must have public parameterless ctor
public class MyConverter : IMapConverter<OrderStatus, string>
{
    public string Convert(OrderStatus source) => source.ToString();
}
```

**3. Mark DTOs as preserved (if in a trimmed assembly):**

In the trimmed assembly's `.csproj`:

```xml
<ItemGroup>
    <RuntimeHostConfigurationOption Include="System.Runtime.TrimmerRootAssembly" Value="MyApp.Dto" />
</ItemGroup>
```

**4. Verify with NativeAOT locally:**

```bash
dotnet publish -p:PublishAot=true
./bin/Release/net8.0/win-x64/publish/app
```

---

## Tests Can't See Generated Mappers

### Symptom

Test project compiles, but when you call `.ToXxx()`, it's not found.

### Cause

The test project doesn't have `InternalsVisibleTo` set up for the generator.

### Solution

In the **library project** `.csproj`, add:

```xml
<ItemGroup>
    <InternalsVisibleTo Include="MyApp.Tests" />
</ItemGroup>
```

This allows the test project to see the generated extension methods (which are internal by default).

---

## Need Help?

- **GitHub Issues**: Report bugs or ask questions at [elbruno/ElBruno.AotMapper/issues](https://github.com/elbruno/ElBruno.AotMapper/issues)
- **GitHub Discussions**: General questions at [elbruno/ElBruno.AotMapper/discussions](https://github.com/elbruno/ElBruno.AotMapper/discussions)
- **Blog**: [elbruno.com](https://elbruno.com) for tutorials and deep dives
