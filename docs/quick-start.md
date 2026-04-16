# Quick Start Guide

Get started with ElBruno.AotMapper in under 15 minutes.

## Step 1: Install the Packages

Add the core package and the source generator to your project:

```bash
dotnet add package ElBruno.AotMapper
dotnet add package ElBruno.AotMapper.Generator
```

- **ElBruno.AotMapper** provides the mapping attributes and abstractions
- **ElBruno.AotMapper.Generator** is the Roslyn incremental source generator that creates the mapping code at compile time (it's a dev-only dependency)

## Step 2: Import the Namespace

Add the using statement to your files where you define DTOs:

```csharp
using ElBruno.AotMapper;
```

## Step 3: Define Your Source Entity

Create a source entity or model. This can be a class, record, or struct:

```csharp
public class Customer
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public CustomerTier Tier { get; set; }
}

public enum CustomerTier
{
    Bronze,
    Silver,
    Gold
}
```

## Step 4: Define Your Destination DTO

Create your destination DTO and annotate it with `[MapFrom]`:

```csharp
[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(
    string Id,
    string Name,
    string Email,
    string Tier
);
```

**Key points:**
- Use `[MapFrom(typeof(SourceType))]` to mark the destination type
- Your class **must be `partial`** — the generator adds methods to it
- Property names should match the source type (exact match)
- For non-matching names, use `[MapProperty]` (see advanced usage below)

## Step 5: Build Your Project

Simply build your project:

```bash
dotnet build
```

The generator will:
1. Scan for `[MapFrom]` attributes
2. Validate that all properties can be mapped
3. Generate extension methods like `ToCustomerDto()`
4. Emit diagnostics if there are issues

## Step 6: Use the Generated Mapper

The generator creates extension methods on the source type:

```csharp
var customer = new Customer
{
    Id = "cust-123",
    Name = "Alice Johnson",
    Email = "alice@example.com",
    Tier = CustomerTier.Gold
};

// Generated extension method
CustomerDto dto = customer.ToCustomerDto();

Console.WriteLine($"Name: {dto.Name}, Tier: {dto.Tier}");
// Output: Name: Alice Johnson, Tier: Gold
```

## Step 7: Use in LINQ Queries (Optional)

If you're using Entity Framework Core, install the EF integration package:

```bash
dotnet add package ElBruno.AotMapper.EntityFramework
```

Then use the generated projection method in your queries:

```csharp
var dbContext = new MyDbContext();

// Generated projection extension method
IQueryable<CustomerDto> dtos = dbContext.Customers
    .Where(c => c.Tier == CustomerTier.Gold)
    .ProjectToCustomerDto();

List<CustomerDto> results = await dtos.ToListAsync();
```

## Advanced Features

### Remapping Properties

If a property name doesn't match between source and destination, use `[MapProperty]`:

```csharp
[MapFrom(typeof(Customer))]
[MapProperty(nameof(Customer.Email), nameof(CustomerDto.ContactEmail))]
public sealed partial record CustomerDto(
    string Id,
    string Name,
    string ContactEmail,
    string Tier
);
```

### Ignoring Properties

Skip properties that shouldn't be mapped:

```csharp
[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(
    string Id,
    string Name,
    [MapIgnore]
    string Email,
    string Tier
);
```

### Strict Mode

Require all properties to be explicitly mapped (no defaults):

```csharp
[MapFrom(typeof(Customer), Strict = true)]
public sealed partial record CustomerDto(
    string Id,
    string Name,
    string Email,
    string Tier
);
```

### Nested Objects

Nested objects are automatically mapped if both source and destination types are defined:

```csharp
public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
}

[MapFrom(typeof(Address))]
public sealed partial record AddressDto(string Street, string City);

public class CustomerWithAddress
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
}

[MapFrom(typeof(CustomerWithAddress))]
public sealed partial record CustomerWithAddressDto(
    string Id,
    string Name,
    AddressDto Address
);

// Generated code handles nested mapping
var customer = new CustomerWithAddress
{
    Id = "c1",
    Name = "Alice",
    Address = new Address { Street = "123 Main St", City = "Seattle" }
};

var dto = customer.ToCustomerWithAddressDto();
// dto.Address is automatically mapped to AddressDto
```

### Collections

Arrays and collection types are supported:

```csharp
public class Order
{
    public string Id { get; set; }
    public List<OrderItem> Items { get; set; }
}

[MapFrom(typeof(OrderItem))]
public sealed partial record OrderItemDto(string ProductId, decimal Price);

[MapFrom(typeof(Order))]
public sealed partial record OrderDto(
    string Id,
    List<OrderItemDto> Items
);

// Generated code handles collection mapping
var order = new Order
{
    Id = "ord-1",
    Items = new List<OrderItem>
    {
        new OrderItem { ProductId = "p1", Price = 19.99m },
        new OrderItem { ProductId = "p2", Price = 29.99m }
    }
};

var dto = order.ToOrderDto();
// dto.Items is a List<OrderItemDto> with mapped items
```

## Troubleshooting

### "Mapping not generated" / No extension methods

1. Verify your DTO class is `partial`
2. Check that `[MapFrom]` is imported correctly
3. Ensure property names match exactly (or use `[MapProperty]`)
4. Rebuild the project
5. Check the build output for diagnostic warnings/errors

### Build errors

The generator emits clear diagnostic messages. Look for messages like:
- `AOTMAP001: Missing property mapping for 'PropertyName'`
- `AOTMAP002: Type mismatch between source and destination`

See [Diagnostics Reference](diagnostics.md) for all diagnostic IDs and fixes.

### EF Core queries don't translate

Not all C# expressions translate to SQL. The library generates projection helpers for common patterns. For complex scenarios:
1. Use `ToList()` first, then apply `.ToDto()` in memory
2. See [EF Integration Guide](ef-integration.md) for supported patterns

---

Next: Read [Supported Mappings](supported-mappings.md) to learn about all available mapping scenarios.
