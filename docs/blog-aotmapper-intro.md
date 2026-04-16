# 🗺️ AOT-Friendly DTO Mapping in .NET — Meet ElBruno.AotMapper

Hi!

You know that moment when you're building a .NET app — maybe a Minimal API, maybe something targeting NativeAOT — and you need to map your entities to DTOs?

So you reach for AutoMapper or Mapster, and then you remember: **runtime reflection doesn't play nice with AOT and trimming.** 😅

Yeah, me too. That's exactly why I built:

👉 **ElBruno.AotMapper**

A Roslyn source-generator library that creates all your mapping code **at compile time**. No reflection. No dynamic IL. Just clean, generated C# that works everywhere — including NativeAOT and trimmed apps.

Let me show you how it works. ⬇️

---

## 📦 Install

Two packages — the core attributes and the generator:

```bash
dotnet add package ElBruno.AotMapper
dotnet add package ElBruno.AotMapper.Generator
```

That's it. The generator runs during build and creates extension methods for you automatically.

---

## 🚀 The Simplest Mapping — One Attribute, Done

Let's start with the basics. You have a `Product` entity and you need a DTO:

```csharp
using ElBruno.AotMapper;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public ProductCategory Category { get; set; }
    public List<string> Tags { get; set; } = new();
}

public enum ProductCategory { Library, Tool, Framework }

[MapFrom(typeof(Product))]
public partial record ProductDto(int Id, string Name, decimal Price, string Category, List<string> Tags);
```

See that `[MapFrom]` attribute? That's the magic. The generator sees it, matches properties by name, and creates a `.ToProductDto()` extension method for you.

Now you use it like this:

```csharp
var product = new Product
{
    Id = 42,
    Name = "ElBruno.AotMapper",
    Price = 0.0m,
    Category = ProductCategory.Library,
    Tags = new List<string> { "aot", "mapper", "dotnet" }
};

var dto = product.ToProductDto();
Console.WriteLine($"Product: {dto.Name} (#{dto.Id})");
Console.WriteLine($"  Category: {dto.Category}");
Console.WriteLine($"  Tags: {string.Join(", ", dto.Tags)}");
```

**No reflection. No runtime overhead. AOT safe. ✅**

---

## 🔀 Remapping Properties with `[MapProperty]`

What if your source and destination properties don't have the same name? Easy — use `[MapProperty]`:

```csharp
using ElBruno.AotMapper;

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public CustomerTier Tier { get; set; }
}

public enum CustomerTier { Standard, Premium, Enterprise }

[MapFrom(typeof(Customer))]
[MapProperty(nameof(Customer.FirstName), nameof(CustomerDto.Name))]
public partial record CustomerDto(int Id, string Name, string Email, string Tier);
```

Here `FirstName` on the source gets mapped to `Name` on the DTO. The generator handles it at build time — zero surprises at runtime.

---

## 🌐 Minimal API — Full Example

Here's where it gets fun. Let's use AotMapper in an ASP.NET Core Minimal API.

First, add the ASP.NET Core integration:

```bash
dotnet add package ElBruno.AotMapper.AspNetCore
```

Then wire it up:

```csharp
using MinimalApiSample.Models;
using ElBruno.AotMapper.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAotMapper();

var app = builder.Build();

var customers = new List<Customer>
{
    new() { Id = 1, FirstName = "Bruno", LastName = "Capuano",
            Email = "bruno@example.com", Tier = CustomerTier.Enterprise },
    new() { Id = 2, FirstName = "Jane", LastName = "Smith",
            Email = "jane@example.com", Tier = CustomerTier.Premium }
};

app.MapGet("/customers", () => customers.Select(c => c.ToCustomerDto()));

app.MapGet("/customers/{id}", (int id) =>
{
    var customer = customers.FirstOrDefault(c => c.Id == id);
    return customer is null ? Results.NotFound() : Results.Ok(customer.ToCustomerDto());
});

app.Run();
```

Look at those endpoint handlers — `c.ToCustomerDto()` is the generated extension method. Clean, type-safe, and AOT-ready. 🎯

---

## 🗄️ EF Core — In-Memory Mapping and SQL Projections

If you're using Entity Framework Core, there's a package for that:

```bash
dotnet add package ElBruno.AotMapper.EntityFramework
```

Here's a real example with orders:

```csharp
using ElBruno.AotMapper;
using Microsoft.EntityFrameworkCore;

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime OrderDate { get; set; }
}

public enum OrderStatus { Pending, Processing, Shipped, Delivered }

[MapFrom(typeof(Order))]
public partial record OrderDto(int Id, string CustomerName, decimal TotalAmount, string Status);
```

**Option 1 — In-memory mapping** (load entities, then map):

```csharp
var orders = await db.Orders.ToListAsync();
var dtos = orders.Select(o => o.ToOrderDto());

foreach (var dto in dtos)
{
    Console.WriteLine($"Order #{dto.Id}: {dto.CustomerName} - ${dto.TotalAmount} ({dto.Status})");
}
```

**Option 2 — Queryable projection** (translates to SQL):

```csharp
var projectedOrders = await db.Orders
    .Where(o => o.TotalAmount > 100)
    .Select(o => new OrderDto(o.Id, o.CustomerName, o.TotalAmount, o.Status.ToString()))
    .ToListAsync();
```

The second option pushes the projection down to SQL — only the columns you need are retrieved. 💪

---

## 🧩 More Attributes You Should Know

The library has a few more tricks:

| Attribute | What it does |
|-----------|-------------|
| `[MapFrom(typeof(T))]` | Map **from** a source type to this DTO |
| `[MapTo(typeof(T))]` | Map **to** a destination type from this source |
| `[MapProperty("src", "dest")]` | Remap a property with a different name |
| `[MapIgnore]` | Skip a property during mapping |
| `[MapConverter(typeof(T))]` | Use a custom converter for a property |

And for custom converters, you implement `IMapConverter<TSource, TDestination>`:

```csharp
public class UpperCaseConverter : IMapConverter<string, string>
{
    public string Convert(string source) => source.ToUpperInvariant();
}
```

---

## 🏗️ How It Works Under the Hood

The flow is simple:

1. You add `[MapFrom]` (or `[MapTo]`) to your DTO
2. During build, the Roslyn incremental source generator kicks in
3. It matches source and destination properties by name and type
4. It generates strongly-typed extension methods (like `.ToProductDto()`)
5. If something doesn't match, you get a **compile-time error** — not a runtime surprise

That's the whole point: **move the errors to build time, move the mapping to generated code, and keep the runtime clean.**

---

## 📂 Samples

The repo includes three complete samples you can run right now:

- 🔧 **[NativeAotSample](https://github.com/elbruno/ElBruno.AotMapper/tree/main/src/samples/NativeAotSample)** — Basic mapping in a NativeAOT console app
- 🌐 **[MinimalApiSample](https://github.com/elbruno/ElBruno.AotMapper/tree/main/src/samples/MinimalApiSample)** — ASP.NET Core Minimal API with DI
- 🗄️ **[EfProjectionSample](https://github.com/elbruno/ElBruno.AotMapper/tree/main/src/samples/EfProjectionSample)** — EF Core with in-memory and SQL projections

---

## Resources

- 📦 **NuGet**: [ElBruno.AotMapper](https://www.nuget.org/packages/ElBruno.AotMapper)
- 🐙 **GitHub**: [github.com/elbruno/ElBruno.AotMapper](https://github.com/elbruno/ElBruno.AotMapper)
- 📖 **Docs**: [Quick Start](quick-start.md) | [Supported Mappings](supported-mappings.md) | [EF Integration](ef-integration.md)
- 📜 **License**: MIT

---

Happy coding! 🚀

---

## 👋 About the Author

**Made with ❤️ by [Bruno Capuano (ElBruno)](https://github.com/elbruno)**

- 📝 Blog: [elbruno.com](https://elbruno.com)
- 📺 YouTube: [youtube.com/elbruno](https://youtube.com/elbruno)
- 🔗 LinkedIn: [linkedin.com/in/elbruno](https://linkedin.com/in/elbruno)
- 𝕏 Twitter: [twitter.com/elbruno](https://twitter.com/elbruno)
- 🎙️ Podcast: [notienenombre.com](https://notienenombre.com)
