# Entity Framework Core Integration

ElBruno.AotMapper provides EF Core integration for safe query projections and DTO mapping in database queries.

## Installation

Install the EF Core integration package:

```bash
dotnet add package ElBruno.AotMapper.EntityFramework
```

This package requires:
- `Microsoft.EntityFrameworkCore` (8.0+)
- `ElBruno.AotMapper`
- `ElBruno.AotMapper.Generator`

## Overview

The integration provides two main capabilities:

1. **Projection Methods** — `ProjectToDto()` for EF query translation
2. **Include/ThenInclude Helpers** — Compile-time safe navigation of relationships

### Key Principle

**EF projection support is limited to patterns that translate to SQL.** The library is explicit about what will and won't translate, with clear diagnostics and documentation.

## Basic Usage

### Define Entity and DTO

```csharp
// Entity
public class Customer
{
    public string Id { get; set; }
    public string Name { get; set; }
    public CustomerTier Tier { get; set; }
}

// DTO
[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(string Id, string Name, string Tier);
```

### Use ProjectToDto() in Queries

```csharp
var context = new MyDbContext();

// Generated projection method translates to SQL
var customers = await context.Customers
    .ProjectToCustomerDto()
    .ToListAsync();
```

This generates a SQL query that selects only the mapped columns, reducing data transfer.

### Filtered Projections

```csharp
var goldCustomers = await context.Customers
    .Where(c => c.Tier == CustomerTier.Gold)
    .ProjectToCustomerDto()
    .ToListAsync();
```

The `Where` clause executes in SQL before projection.

## Supported Patterns

### ✅ Simple Property Mapping

```csharp
[MapFrom(typeof(Product))]
public sealed partial record ProductDto(string Id, string Name, decimal Price);

var products = await context.Products
    .ProjectToProductDto()
    .ToListAsync();
// Translates to: SELECT Id, Name, Price FROM Products
```

### ✅ Nested Object Mapping

```csharp
public class Order
{
    public string Id { get; set; }
    public Customer Customer { get; set; }  // Foreign key navigation
    public decimal Total { get; set; }
}

[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(string Id, string Name);

[MapFrom(typeof(Order))]
public sealed partial record OrderDto(
    string Id,
    CustomerDto Customer,
    decimal Total
);

var orders = await context.Orders
    .Include(o => o.Customer)
    .ProjectToOrderDto()
    .ToListAsync();
```

The generator recognizes the nested `CustomerDto` and includes the foreign key navigation.

### ✅ Collection Navigation

Collections of primitives and mapped DTOs are supported:

```csharp
public class Order
{
    public string Id { get; set; }
    public List<OrderItem> Items { get; set; }
}

[MapFrom(typeof(OrderItem))]
public sealed partial record OrderItemDto(string ProductId, decimal Quantity, decimal Price);

[MapFrom(typeof(Order))]
public sealed partial record OrderDto(
    string Id,
    List<OrderItemDto> Items
);

var orders = await context.Orders
    .Include(o => o.Items)
    .ProjectToOrderDto()
    .ToListAsync();
```

### ✅ Enum Mapping

```csharp
public enum OrderStatus { Pending, Shipped, Delivered }

public class Order
{
    public OrderStatus Status { get; set; }
}

[MapFrom(typeof(Order))]
public sealed partial record OrderDto(string Status);  // Enum to string

var orders = await context.Orders
    .ProjectToOrderDto()
    .ToListAsync();
// Status is converted to string in the SQL query
```

### ✅ Filtering & Ordering Before Projection

```csharp
var recentOrders = await context.Orders
    .Where(o => o.CreatedAt > DateTime.UtcNow.AddDays(-7))
    .OrderByDescending(o => o.CreatedAt)
    .ProjectToOrderDto()
    .ToListAsync();
```

Filters and sorting happen in SQL before projection.

### ✅ Pagination Before Projection

```csharp
int pageSize = 20;
int pageNumber = 1;

var page = await context.Customers
    .OrderBy(c => c.Name)
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ProjectToCustomerDto()
    .ToListAsync();
```

Pagination happens in SQL for efficient data retrieval.

## Unsupported Patterns

### ❌ Complex Expressions in Select

```csharp
// ❌ This will NOT translate to SQL
var orders = context.Orders
    .ProjectToOrderDto()
    .Select(o => new { o.Id, TotalFormatted = $"Total: {o.Total}" })
    .ToList();
```

**Workaround:** Apply formatting after materialization:
```csharp
var orders = await context.Orders
    .ProjectToOrderDto()
    .ToListAsync();

var formatted = orders.Select(o => new { o.Id, TotalFormatted = $"Total: {o.Total}" }).ToList();
```

### ❌ Multiple Levels of Nested Collections

```csharp
// ❌ This may not translate
[MapFrom(typeof(Order))]
public sealed partial record OrderDto(
    string Id,
    List<OrderDetailDto> Details  // Which itself has a collection
);

// Collection + Nested Collection + Nested Collection = May fail
var orders = await context.Orders
    .Include(o => o.Details)
    .ThenInclude(d => d.Items)  // ← Triple nesting
    .ProjectToOrderDto()
    .ToListAsync();
```

**Workaround:** Load complex nested data in memory:
```csharp
var orders = await context.Orders
    .Include(o => o.Details)
    .ToListAsync();

var dtos = orders.Select(o => o.ToOrderDto()).ToList();
```

### ❌ String Methods & CLR Functions

```csharp
[MapFrom(typeof(Product))]
public sealed partial record ProductDto(string NameUpper);

// ❌ May not translate - depends on EF provider and String.ToUpper support
var products = await context.Products
    .Select(p => new { NameUpper = p.Name.ToUpper() })
    .ProjectToProductDto()
    .ToListAsync();
```

**Workaround:** Use supported SQL functions or materialize first:
```csharp
var products = await context.Products.ToListAsync();
var dtos = products.Select(p => new ProductDto(p.Name.ToUpper())).ToList();
```

### ❌ Null Coalescing in Complex Expressions

```csharp
[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(
    string Name,
    string ContactEmail = "unknown@example.com"  // Default in DTO
);

// ❌ Null coalescing may not translate correctly
var customers = await context.Customers
    .Select(c => new { c.Name, c.Email ?? "unknown@example.com" })
    .ProjectToCustomerDto()
    .ToListAsync();
```

**Workaround:** Let the DTO defaults handle nulls in memory:
```csharp
var customers = await context.Customers
    .ProjectToCustomerDto()
    .ToListAsync();
```

## Performance Considerations

### Benefits of ProjectToDto()

✅ **Reduced Data Transfer:** Only mapped columns are selected from the database  
✅ **Efficient Filtering:** `Where` clauses run in SQL before projection  
✅ **Pagination:** `Skip`/`Take` execute in SQL for large datasets  

### When to Use ProjectToDto()

- **Large datasets** — projection reduces columns transferred
- **Simple DTOs** — basic property mapping
- **Query composition** — using `Where`, `OrderBy`, `Skip`, `Take` before projection

### When to Materialize First

- **Complex nested data** — multi-level collections or relationships
- **Custom LINQ operators** — that don't translate to SQL
- **Small datasets** — when in-memory mapping is simpler and faster enough

## Explicit Non-Translation Scenarios

### Example: Materialization Strategy

```csharp
// ❌ Uncertain translation
var complex = context.Orders
    .Where(o => o.Items.Count > 5)  // Collection count in SQL
    .ProjectToOrderDto()
    .ToListAsync();

// ✅ Explicit and predictable
var orders = await context.Orders
    .Include(o => o.Items)
    .ToListAsync();

var result = orders
    .Where(o => o.Items.Count > 5)
    .Select(o => o.ToOrderDto())
    .ToList();
```

## EF Provider Compatibility

The library targets common EF Core providers. Provider-specific support:

| Provider | Status | Notes |
|----------|--------|-------|
| SQL Server | ✅ Supported | Full support for projected mappings |
| SQLite | ✅ Supported | Some string functions may differ |
| PostgreSQL | ✅ Supported | Via Npgsql EF Core provider |
| MySQL | ✅ Supported | Via Pomelo MySQL EF Core provider |
| In-Memory | ⚠️ Limited | `ProjectToDto()` materializes in memory |

## Best Practices

1. **Start with `ProjectToDto()`** — it usually works for simple mappings
2. **Add `.ToListAsync()` before complex LINQ** — to ensure SQL translation
3. **Test queries** — verify they execute in SQL, not in-memory
4. **Use `.AsNoTracking()`** for read-only queries — improves performance:
   ```csharp
   var customers = await context.Customers
       .AsNoTracking()
       .ProjectToCustomerDto()
       .ToListAsync();
   ```
5. **Profile your queries** — use EF logging to see generated SQL

## Troubleshooting

### Query Not Translating to SQL

If you see `LINQ to Entities does not recognize the method...`:

1. **Check provider support** — ensure the operation is supported by your database
2. **Materialize early** — use `.ToListAsync()` before the problematic LINQ operation
3. **Use SqlFunctions** or provider-specific methods for advanced SQL operations

Example:
```csharp
// ❌ Fails - custom CLR method
var data = context.Orders
    .Where(o => MyHelper.CalculateDiscount(o.Total) > 0.1m)
    .ProjectToOrderDto()
    .ToListAsync();

// ✅ Works - materialize first
var orders = await context.Orders.ToListAsync();
var data = orders
    .Where(o => MyHelper.CalculateDiscount(o.Total) > 0.1m)
    .Select(o => o.ToOrderDto())
    .ToList();
```

### Projection Produces Unexpected Null Values

If projected DTOs have null values when they shouldn't:

1. **Check Include statements** — ensure foreign key relationships are included
2. **Verify column mapping** — ensure all required columns are selected
3. **Test the SQL** — run the generated SQL query directly against your database

## See Also

- [Quick Start](quick-start.md) — Basic mapping usage
- [Supported Mappings](supported-mappings.md) — Mapping features
- [Known Limitations](known-limits.md) — General constraints
- [EF Core Documentation](https://docs.microsoft.com/en-us/ef/core/) — Official EF Core docs
