# Supported Mappings

This document lists all mapping scenarios supported by ElBruno.AotMapper.

## Type Support

### Basic Types

✅ **Supported:**
- `string`
- `int`, `long`, `short`, `byte`
- `float`, `double`, `decimal`
- `bool`
- `Guid`
- `DateTime`, `DateTimeOffset`, `TimeSpan`
- `Uri`
- Any value type or reference type that doesn't require special handling

### Collections

✅ **Supported:**
- `T[]` (arrays)
- `List<T>`
- `IList<T>`
- `ICollection<T>`
- `IEnumerable<T>`
- `IReadOnlyList<T>`
- `IReadOnlyCollection<T>`
- `Dictionary<K, V>` (key-value pairs)
- `IDictionary<K, V>`
- `IReadOnlyDictionary<K, V>`
- `HashSet<T>`
- `Queue<T>`, `Stack<T>`

Collections are automatically materialized during mapping:
```csharp
public class Order
{
    public List<OrderItem> Items { get; set; }
}

[MapFrom(typeof(Order))]
public sealed partial record OrderDto(List<OrderItemDto> Items);

// Items collection is automatically mapped
var dto = order.ToOrderDto();
```

### Enums

✅ **Supported:**
- Enum-to-enum mapping (by value)
- Enum-to-string conversion (case-sensitive)
- String-to-enum conversion
- Nullable enum handling

```csharp
public enum OrderStatus { Pending, Shipped, Delivered }

[MapFrom(typeof(Order))]
public sealed partial record OrderDto(string StatusName);
// Generates: StatusName = order.Status.ToString()
```

### Nullable Types

✅ **Supported:**
- `T?` (nullable value types)
- `T?` with `#nullable enable` (nullable reference types)
- `Nullable<T>` (explicit nullable wrapper)

The generator validates nullability mismatches at compile time:
```csharp
public class Entity
{
    public string? OptionalField { get; set; }
    public string RequiredField { get; set; }
}

[MapFrom(typeof(Entity))]
public sealed partial record Dto(
    string? OptionalField,  // ✅ matches
    string RequiredField    // ✅ matches
);
```

## Object Mapping

### Classes

✅ **Supported:**
- Public classes
- Internal classes (via `InternalsVisibleTo`)
- Abstract base classes (if all properties are on derived concrete types)
- Any class with accessible properties

```csharp
public class Customer { public string Name { get; set; } }

[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(string Name);
```

### Records

✅ **Supported:**
- Record classes
- Record structs
- Positional records
- Records with init-only properties

```csharp
public record Customer(string Id, string Name);

[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(string Id, string Name);
```

### Structs

✅ **Supported:**
- Public structs
- Value type structs
- Readonly structs

```csharp
public struct Point { public int X { get; set; } public int Y { get; set; } }

[MapFrom(typeof(Point))]
public sealed partial record PointDto(int X, int Y);
```

## Nested Objects

✅ **Supported:**
- One-level deep nesting
- Multi-level nesting (recursive)
- Nested collections

Nested objects are automatically mapped if a corresponding `[MapFrom]` is defined for the nested type:

```csharp
public class Address { public string City { get; set; } }

[MapFrom(typeof(Address))]
public sealed partial record AddressDto(string City);

public class Customer { public Address Address { get; set; } }

[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(AddressDto Address);

// Nested mapping is automatic
var customer = new Customer { Address = new Address { City = "Seattle" } };
var dto = customer.ToCustomerDto();
// dto.Address is AddressDto with City = "Seattle"
```

## Property Mapping Features

### Property Remapping

Use `[MapProperty]` to remap non-matching property names:

```csharp
[MapFrom(typeof(Customer))]
[MapProperty(nameof(Customer.CustomerId), nameof(CustomerDto.Id))]
public sealed partial record CustomerDto(string Id, string Name);
```

### Ignore Properties

Use `[MapIgnore]` to skip properties:

```csharp
[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(
    string Id,
    string Name,
    [MapIgnore] string InternalField
);
```

### Default Values

Properties with default values are handled automatically:

```csharp
[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(
    string Id,
    string Name = "Unknown"  // Default if source is null
);
```

### Strict Mode

Require all properties to be explicitly handled:

```csharp
[MapFrom(typeof(Customer), Strict = true)]
public sealed partial record CustomerDto(string Id, string Name);
```

In strict mode, any unmapped properties generate a compile-time error.

## Partial Methods (Advanced)

✅ **Supported:**
- Pre-mapping customization via `MapFromPartial_OnMapStart`
- Post-mapping customization via `MapFromPartial_OnMapEnd`

```csharp
[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(string Id, string Name)
{
    partial void MapFromPartial_OnMapStart(Customer source);
    partial void MapFromPartial_OnMapEnd(Customer source);
}

// Implementation in another file
public sealed partial record CustomerDto
{
    partial void MapFromPartial_OnMapStart(Customer source)
    {
        Console.WriteLine($"Starting mapping for customer {source.Id}");
    }

    partial void MapFromPartial_OnMapEnd(Customer source)
    {
        Console.WriteLine($"Mapping completed");
    }
}
```

## Property Access Levels

✅ **Supported:**
- Public properties
- Internal properties (with `InternalsVisibleTo`)
- Properties with private setters (init-only is preferred)
- Properties with backing fields

The mapper respects .NET access modifiers and works within encapsulation boundaries.

## Special Scenarios

### Same-Type Mapping

✅ **Supported:**
Mapping a type to itself (useful for DTOs of DTOs):

```csharp
[MapFrom(typeof(CustomerDto))]
public sealed partial record CustomerDtoV2(string Id, string Name);
```

### Polymorphic Collections

⚠️ **Partial Support:**
Collections of base types are supported; runtime polymorphism is not automatically discovered. Explicitly map each concrete type.

### Anonymous Types

❌ **Not Supported:**
Anonymous types cannot be decorated with attributes and are not recommended for public APIs.

## Known Limitations & Workarounds

See [Known Limitations](known-limits.md) for:
- Unsupported mapping patterns
- Runtime reflection fallback patterns
- Workarounds for edge cases
- Performance expectations

---

Next: Read [Known Limitations](known-limits.md) to understand constraints and workarounds.
