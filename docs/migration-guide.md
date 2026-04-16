# Migration Guide

This guide helps you migrate from AutoMapper, manual mapping, or other mapping libraries to ElBruno.AotMapper.

## Why Migrate?

### Benefits of ElBruno.AotMapper

✅ **Compile-Time Safety** — Missing mappings caught at build time, not runtime  
✅ **AOT Compatible** — Works seamlessly with NativeAOT and assembly trimming  
✅ **No Reflection** — Generated code with zero runtime reflection overhead  
✅ **Performance** — Generated code rivals hand-written mapping  
✅ **Transparent** — Read the generated code; no "magic"  
✅ **Cloud-Native** — Perfect for serverless and containerized .NET applications  

### When to Migrate

- You're building NativeAOT applications
- You need trimming-safe code
- You want compile-time validation of mappings
- You prefer explicit code over configuration
- Performance and startup time matter

---

## Migration Paths

### From AutoMapper

#### Step 1: Inventory Your Mappings

List all `CreateMap` calls and mapping profiles:

```csharp
// AutoMapper (before)
config.CreateMap<Customer, CustomerDto>();
config.CreateMap<Order, OrderDto>();
config.CreateMap<OrderItem, OrderItemDto>();
```

#### Step 2: Convert to ElBruno.AotMapper

Replace each mapping profile with `[MapFrom]` attributes on destination types:

```csharp
// ElBruno.AotMapper (after)
[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(string Id, string Name, string Tier);

[MapFrom(typeof(Order))]
public sealed partial record OrderDto(string Id, decimal Total);

[MapFrom(typeof(OrderItem))]
public sealed partial record OrderItemDto(string ProductId, decimal Price);
```

#### Step 3: Update MapFrom Calls

Replace AutoMapper's `mapper.Map` with generated extension methods:

```csharp
// AutoMapper (before)
var dto = _mapper.Map<CustomerDto>(customer);

// ElBruno.AotMapper (after)
var dto = customer.ToCustomerDto();  // Generated extension method
```

#### Step 4: Handle Complex Mappings

For custom converters and formatters, use partial methods:

```csharp
// AutoMapper (before)
config.CreateMap<Order, OrderDto>()
    .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()));

// ElBruno.AotMapper (after)
[MapFrom(typeof(Order))]
public sealed partial record OrderDto(string Id, string StatusName)
{
    partial void MapFromPartial_OnMapEnd(Order source);
}

public sealed partial record OrderDto
{
    partial void MapFromPartial_OnMapEnd(Order source)
    {
        ((OrderDto)this) = ((OrderDto)this) with 
        { 
            StatusName = source.Status.ToString() 
        };
    }
}
```

#### Step 5: Remove AutoMapper

Uninstall AutoMapper packages and clean up:

```bash
dotnet remove package AutoMapper
dotnet remove package AutoMapper.Extensions.Microsoft.DependencyInjection
```

Remove `IMapper` injections and AutoMapper configuration code.

#### Step 6: Update DI Setup

```csharp
// AutoMapper (before)
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ElBruno.AotMapper (after)
builder.Services.AddAotMappers();
```

---

### From Manual Mapping

#### Step 1: Identify Mapping Patterns

Find all manual mapping methods in your codebase:

```csharp
// Manual mapping (before)
public static CustomerDto MapToDto(Customer customer)
{
    return new CustomerDto(
        customer.Id,
        customer.Name,
        customer.Tier.ToString()
    );
}
```

#### Step 2: Define DTOs with Attributes

Create DTOs annotated with `[MapFrom]`:

```csharp
[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(string Id, string Name, string Tier);
```

#### Step 3: Replace Mapping Methods

Remove manual mapping methods; use generated extension methods:

```csharp
// Before
var dto = MapToDto(customer);

// After
var dto = customer.ToCustomerDto();  // Generated
```

#### Step 4: Consolidate DTO Definitions

If your DTOs are scattered, consolidate them into dedicated files with mapping attributes.

---

### From Other Mapping Libraries

#### Step 1: List All Mappings

Inventory the mapping library's configuration:

```csharp
// Example: Another library
mapper.Register<Source, Destination>();
```

#### Step 2: Translate to ElBruno.AotMapper

Create `[MapFrom]` attributes on destination types:

```csharp
[MapFrom(typeof(Source))]
public sealed partial record Destination(/* properties */);
```

#### Step 3: Update Usage

Replace library-specific mapping calls with generated extension methods:

```csharp
// Before
var result = mapper.Map<Destination>(source);

// After
var result = source.ToDestination();  // Generated
```

---

## Common Scenarios

### Scenario: Nested Collections

#### AutoMapper
```csharp
config.CreateMap<Order, OrderDto>()
    .ForMember(dest => dest.Items, 
        opt => opt.MapFrom(src => src.Items));
```

#### ElBruno.AotMapper
```csharp
[MapFrom(typeof(OrderItem))]
public sealed partial record OrderItemDto(string ProductId, decimal Price);

[MapFrom(typeof(Order))]
public sealed partial record OrderDto(
    string Id,
    List<OrderItemDto> Items  // Nested collection automatically mapped
);

// Usage
var dto = order.ToOrderDto();
```

### Scenario: Property Remapping

#### AutoMapper
```csharp
config.CreateMap<Customer, CustomerDto>()
    .ForMember(dest => dest.CustomerId, 
        opt => opt.MapFrom(src => src.Id));
```

#### ElBruno.AotMapper
```csharp
[MapFrom(typeof(Customer))]
[MapProperty(nameof(Customer.Id), nameof(CustomerDto.CustomerId))]
public sealed partial record CustomerDto(string CustomerId, string Name);
```

### Scenario: Ignoring Properties

#### AutoMapper
```csharp
config.CreateMap<Customer, CustomerDto>()
    .ForMember(dest => dest.InternalNotes, opt => opt.Ignore());
```

#### ElBruno.AotMapper
```csharp
[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(
    string Id,
    string Name,
    [MapIgnore]
    string InternalNotes
);
```

### Scenario: EF Core Projections

#### AutoMapper with EF
```csharp
var dtos = context.Customers
    .ProjectTo<CustomerDto>(config)
    .ToList();
```

#### ElBruno.AotMapper with EF
```csharp
var dtos = await context.Customers
    .ProjectToCustomerDto()
    .ToListAsync();
```

---

## Handling Unsupported Scenarios

### Scenario: Complex Custom Logic

If AutoMapper had intricate custom converters:

```csharp
// AutoMapper (before)
config.CreateMap<Order, OrderDto>()
    .ForMember(dest => dest.Total, 
        opt => opt.MapFrom(src => src.Items.Sum(i => i.Price)));
```

**Solution:** Use partial methods for custom logic:

```csharp
[MapFrom(typeof(Order))]
public sealed partial record OrderDto(string Id, decimal Total)
{
    partial void MapFromPartial_OnMapEnd(Order source);
}

public sealed partial record OrderDto
{
    partial void MapFromPartial_OnMapEnd(Order source)
    {
        ((OrderDto)this) = ((OrderDto)this) with 
        { 
            Total = source.Items.Sum(i => i.Price) 
        };
    }
}
```

### Scenario: Runtime Configuration

If you dynamically configure mappings at runtime, you'll need to:

1. **Pre-define all mappings** at compile time with `[MapFrom]` attributes
2. **Generate extension methods** for each mapping
3. **Select the appropriate mapper** at runtime based on conditions

---

## Testing Migration

### Unit Tests

Update your tests to use generated methods:

```csharp
// Old: AutoMapper/manual mapping test
[Fact]
public void CustomerMappingTest()
{
    var customer = new Customer { Id = "1", Name = "Alice", Tier = Tier.Gold };
    var dto = _mapper.Map<CustomerDto>(customer);
    Assert.Equal("Alice", dto.Name);
}

// New: ElBruno.AotMapper test
[Fact]
public void CustomerMappingTest()
{
    var customer = new Customer { Id = "1", Name = "Alice", Tier = Tier.Gold };
    var dto = customer.ToCustomerDto();  // Generated method
    Assert.Equal("Alice", dto.Name);
}
```

### Integration Tests

If using EF Core:

```csharp
// Old: AutoMapper projections
var dtos = context.Customers
    .ProjectTo<CustomerDto>(config)
    .ToList();

// New: ElBruno.AotMapper projections
var dtos = await context.Customers
    .ProjectToCustomerDto()
    .ToListAsync();
```

### Performance Tests

Compare generated mappings with your old approach:

```csharp
var customer = new Customer { Id = "1", Name = "Alice", Tier = Tier.Gold };

// Benchmark generated mapping
for (int i = 0; i < 100000; i++)
{
    var dto = customer.ToCustomerDto();
}
```

Generated mappings should be **faster** or **on par** with manual mappings.

---

## Migration Checklist

- [ ] Inventory all mappings in your application
- [ ] Create DTOs with `[MapFrom]` attributes
- [ ] Install ElBruno.AotMapper packages
- [ ] Replace mapper.Map() calls with generated extension methods
- [ ] Update DI configuration (remove old mapper, add AddAotMappers())
- [ ] Update unit tests to use generated methods
- [ ] Test EF Core projections (if applicable)
- [ ] Remove old mapping library packages
- [ ] Run full test suite
- [ ] Verify AOT/trimming compatibility (if targeted)
- [ ] Deploy and monitor

---

## Performance Expectations

| Scenario | Expected Result |
|----------|-----------------|
| Simple object mapping | **5–15% faster** than AutoMapper or manual |
| Large object graphs | **On par or faster** (no reflection overhead) |
| Collections | **5–10% faster** (generated LINQ) |
| EF projections | **Same or faster** (more efficient SQL) |
| Startup time | **Significantly faster** (no reflection) |

---

## Troubleshooting Migration

### "Missing namespace" error

Ensure you've added:
```csharp
using ElBruno.AotMapper;
```

### "Method not found" error

Verify:
1. DTO class is `partial`
2. `[MapFrom]` attribute is present
3. Project has been rebuilt

### Compilation errors during migration

Check that all properties in the DTO match the source entity (or use `[MapProperty]` for remapping).

---

## Support

For questions or issues during migration:
- Check [Quick Start Guide](quick-start.md)
- Review [Supported Mappings](supported-mappings.md)
- See [Diagnostics Reference](diagnostics.md) for error codes
- Check [Known Limitations](known-limits.md) for constraints

---

## Next Steps

After successful migration:
1. Monitor performance in production
2. Enable AOT/trimming if targeting cloud-native deployments
3. Consider EF Core integration for improved query efficiency
4. Share feedback and experiences with the community
