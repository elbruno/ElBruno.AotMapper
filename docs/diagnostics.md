# Diagnostics Reference

This document lists all diagnostic messages emitted by ElBruno.AotMapper during code generation.

## Overview

The ElBruno.AotMapper source generator emits compile-time diagnostics (warnings and errors) when mapping definitions are incomplete, ambiguous, or unsupported. Each diagnostic has a unique ID and actionable remediation steps.

## Diagnostic IDs

### AOTMAP001: Missing Property in Destination Type

**Severity:** Error  
**Message:** `Property '{PropertyName}' in source type '{SourceType}' has no corresponding property in destination type '{DestinationType}'.`

**Cause:** A property exists in the source type but the destination type does not have a matching property.

**Example:**
```csharp
public class Customer
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string InternalCode { get; set; }  // ← No match in DTO
}

[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(string Id, string Name);
// ❌ Error: 'InternalCode' in Customer has no corresponding property in CustomerDto
```

**Remediation:**
1. **Add the missing property** to the destination type:
   ```csharp
   public sealed partial record CustomerDto(string Id, string Name, string InternalCode);
   ```

2. **Or ignore the property** using `[MapIgnore]`:
   ```csharp
   [MapFrom(typeof(Customer))]
   public sealed partial record CustomerDto(
       string Id,
       string Name,
       [MapIgnore]
       string InternalCode
   );
   ```

3. **Or remap it** to a different property name using `[MapProperty]`:
   ```csharp
   [MapFrom(typeof(Customer))]
   [MapProperty(nameof(Customer.InternalCode), nameof(CustomerDto.Code))]
   public sealed partial record CustomerDto(string Id, string Name, string Code);
   ```

---

### AOTMAP002: Type Mismatch

**Severity:** Error  
**Message:** `Cannot map property '{PropertyName}' from type '{SourceType}' to type '{DestinationType}'. No implicit conversion exists.`

**Cause:** The source property type and destination property type are incompatible and no automatic mapping is available.

**Example:**
```csharp
public class Customer
{
    public int Id { get; set; }
}

[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(string Id);  // ← int → string mismatch
// ❌ Error: Cannot map 'Id' from int to string
```

**Remediation:**
1. **Match the types** exactly:
   ```csharp
   public sealed partial record CustomerDto(int Id);
   ```

2. **Add a conversion method** if conversion is needed:
   ```csharp
   [MapFrom(typeof(Customer))]
   [MapProperty(nameof(Customer.Id), nameof(CustomerDto.Id))]
   public sealed partial record CustomerDto(string Id)
   {
       partial void MapFromPartial_OnMapEnd(Customer source);
   }

   public sealed partial record CustomerDto
   {
       partial void MapFromPartial_OnMapEnd(Customer source)
       {
           // Manually convert
           ((CustomerDto)this).Id = source.Id.ToString();
       }
   }
   ```

3. **Or use a compatible type** (if appropriate):
   ```csharp
   public sealed partial record CustomerDto(int Id);
   ```

---

### AOTMAP003: Nullability Mismatch

**Severity:** Warning (may be error in strict mode)  
**Message:** `Property '{PropertyName}' is nullable in source type but not in destination type. This may cause null reference exceptions.`

**Cause:** Source property is nullable but destination property is not, creating potential null reference issues.

**Example:**
```csharp
#nullable enable

public class Customer
{
    public string? Name { get; set; }  // ← nullable
}

[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(string Name);  // ← not nullable
// ⚠️ Warning: Name is nullable in source but not in destination
```

**Remediation:**
1. **Make destination nullable** to match source:
   ```csharp
   public sealed partial record CustomerDto(string? Name);
   ```

2. **Or provide a default value** for null cases:
   ```csharp
   public sealed partial record CustomerDto(string Name = "Unknown");
   ```

3. **Or add null handling** in a partial method:
   ```csharp
   public sealed partial record CustomerDto(string Name)
   {
       partial void MapFromPartial_OnMapEnd(Customer source);
   }

   public sealed partial record CustomerDto
   {
       partial void MapFromPartial_OnMapEnd(Customer source)
       {
           if (((CustomerDto)this).Name == null)
               ((CustomerDto)this) = ((CustomerDto)this) with { Name = "Unknown" };
       }
   }
   ```

---

### AOTMAP004: Unsupported Type

**Severity:** Error  
**Message:** `Type '{TypeName}' is not supported for automatic mapping. Supported types: primitives, collections, enums, nested mapped types.`

**Cause:** A property type is not in the list of supported types and no mapping is defined for it.

**Example:**
```csharp
public class CustomCollection : IEnumerable<string> { /* ... */ }

public class Data
{
    public CustomCollection Items { get; set; }
}

[MapFrom(typeof(Data))]
public sealed partial record DataDto(CustomCollection Items);
// ❌ Error: CustomCollection is not a supported type
```

**Remediation:**
1. **Use a supported collection type**:
   ```csharp
   public sealed partial record DataDto(List<string> Items);
   ```

2. **Or define a mapper** for the custom type:
   ```csharp
   [MapFrom(typeof(CustomCollection))]
   public sealed partial record CustomCollectionDto(List<string> Items);

   [MapFrom(typeof(Data))]
   public sealed partial record DataDto(CustomCollectionDto Items);
   ```

3. **Or handle it manually** in a partial method:
   ```csharp
   public sealed partial record DataDto(CustomCollection Items)
   {
       partial void MapFromPartial_OnMapEnd(Data source);
   }

   public sealed partial record DataDto
   {
       partial void MapFromPartial_OnMapEnd(Data source)
       {
           // Manually map or transform
       }
   }
   ```

---

### AOTMAP005: Conflicting Map Definitions

**Severity:** Error  
**Message:** `Conflicting mapping definitions for type '{SourceType}'. Multiple `[MapFrom]` attributes target the same source type.`

**Cause:** Multiple destination types have `[MapFrom]` pointing to the same source type with conflicting property mappings.

**Example:**
```csharp
[MapFrom(typeof(Customer))]
[MapProperty(nameof(Customer.Id), nameof(CustomerDto1.CustomerId))]
public sealed partial record CustomerDto1(string CustomerId);

[MapFrom(typeof(Customer))]
[MapProperty(nameof(Customer.Id), nameof(CustomerDto2.CustomerId))]
public sealed partial record CustomerDto2(string CustomerId, string Extra);

// ❌ Error: Conflicting definitions for Customer
```

**Remediation:**
1. **Different source types** – if these are truly different mappings, use unique source types
2. **Consolidate mappings** – merge into a single destination type if possible
3. **Use different attributes** – ensure each destination type has a unique mapping strategy:
   ```csharp
   [MapFrom(typeof(Customer))]
   public sealed partial record CustomerDto(string Id, string Name);

   // Alternative DTO for different use case
   [MapFrom(typeof(Customer))]
   public sealed partial record CustomerSummaryDto(string Id);
   ```

---

## Diagnostic Levels

| Level | Meaning | Action Required |
|-------|---------|-----------------|
| Error | Mapping cannot be generated | Fix before building |
| Warning | Mapping works but may be risky (e.g., nullability) | Review and fix if necessary |
| Info | Informational message | For awareness |

## Troubleshooting Workflow

1. **Build your project** and review any diagnostic messages
2. **Look up the diagnostic ID** (AOTMAP### format) in this document
3. **Follow the remediation steps** for that diagnostic
4. **Rebuild** to verify the fix

## Common Diagnostic Patterns

### Pattern: Property Name Mismatch

If you see AOTMAP001 and property names are close, use `[MapProperty]`:
```csharp
[MapProperty(nameof(Source.UserId), nameof(Destination.Id))]
```

### Pattern: Nullable Warnings

If you see AOTMAP003 warnings, decide whether to:
- Allow nulls: make destination nullable
- Forbid nulls: add default values
- Handle explicitly: add partial method logic

### Pattern: Type Mismatches

If you see AOTMAP002, the types must align. Use `[MapProperty]` with conversion logic if needed.

---

## See Also

- [Quick Start](quick-start.md) — Basic usage
- [Supported Mappings](supported-mappings.md) — Supported types and patterns
- [Known Limitations](known-limits.md) — Constraints and workarounds
