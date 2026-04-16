# ElBruno.AotMapper

[![CI Build](https://github.com/elbruno/ElBruno.AotMapper/actions/workflows/build.yml/badge.svg)](https://github.com/elbruno/ElBruno.AotMapper/actions/workflows/build.yml)
[![Publish to NuGet](https://github.com/elbruno/ElBruno.AotMapper/actions/workflows/publish.yml/badge.svg)](https://github.com/elbruno/ElBruno.AotMapper/actions/workflows/publish.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![GitHub stars](https://img.shields.io/github/stars/elbruno/ElBruno.AotMapper)](https://github.com/elbruno/ElBruno.AotMapper/stargazers)

## AOT-friendly compile-time DTO mapper for .NET 🗺️

**ElBruno.AotMapper** is a Roslyn-based source-generator library that creates compile-time DTO mapping code for .NET. No runtime reflection, no dynamic IL emission—just fast, AOT-safe, and predictable generated mappers. Perfect for NativeAOT deployments, trimmed applications, and cloud-native workloads.

### Why ElBruno.AotMapper?

- **Compile-time generation** — All mapping logic is generated during build; zero runtime reflection in the happy path
- **AOT & Trimming safe** — Works seamlessly with NativeAOT and assembly trimming
- **Strong diagnostics** — Clear compile-time errors for incomplete or ambiguous mappings
- **Performance parity** — Generated code stays within 5–15% of hand-written mapping
- **EF Core friendly** — Optional projection helpers for supported query patterns
- **ASP.NET Core ready** — Dependency injection helpers for easy service registration

---

## Packages

| Package | NuGet | Downloads | Description |
|---------|-------|-----------|-------------|
| **ElBruno.AotMapper** | [![NuGet](https://img.shields.io/nuget/v/ElBruno.AotMapper.svg)](https://www.nuget.org/packages/ElBruno.AotMapper) | [![Downloads](https://img.shields.io/nuget/dt/ElBruno.AotMapper.svg)](https://www.nuget.org/packages/ElBruno.AotMapper) | Core attributes and abstractions |
| **ElBruno.AotMapper.Generator** | [![NuGet](https://img.shields.io/nuget/v/ElBruno.AotMapper.Generator.svg)](https://www.nuget.org/packages/ElBruno.AotMapper.Generator) | [![Downloads](https://img.shields.io/nuget/dt/ElBruno.AotMapper.Generator.svg)](https://www.nuget.org/packages/ElBruno.AotMapper.Generator) | Roslyn incremental source generator (dev dependency) |
| **ElBruno.AotMapper.AspNetCore** | [![NuGet](https://img.shields.io/nuget/v/ElBruno.AotMapper.AspNetCore.svg)](https://www.nuget.org/packages/ElBruno.AotMapper.AspNetCore) | [![Downloads](https://img.shields.io/nuget/dt/ElBruno.AotMapper.AspNetCore.svg)](https://www.nuget.org/packages/ElBruno.AotMapper.AspNetCore) | DI registration and ASP.NET Core extensions |
| **ElBruno.AotMapper.EntityFramework** | [![NuGet](https://img.shields.io/nuget/v/ElBruno.AotMapper.EntityFramework.svg)](https://www.nuget.org/packages/ElBruno.AotMapper.EntityFramework) | [![Downloads](https://img.shields.io/nuget/dt/ElBruno.AotMapper.EntityFramework.svg)](https://www.nuget.org/packages/ElBruno.AotMapper.EntityFramework) | EF Core integration and projection helpers |

---

## Quick Start

### Installation

Install the core package and the generator (as a dev dependency):

```bash
dotnet add package ElBruno.AotMapper
dotnet add package ElBruno.AotMapper.Generator
```

### Define a Mapping

Annotate your destination DTO class with `[MapFrom]`:

```csharp
using ElBruno.AotMapper;

// Source entity
public class Customer
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Tier { get; set; }
}

// Destination DTO with mapping attribute
[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(string Id, string Name, string Tier);
```

### Build & Use

Just build your project—the generator creates extension methods automatically:

```csharp
var customer = new Customer { Id = "123", Name = "Alice", Tier = "Gold" };

// Generated extension method
CustomerDto dto = customer.ToCustomerDto();

// Also works with LINQ queries (when using ElBruno.AotMapper.EntityFramework)
IQueryable<CustomerDto> dtos = context.Customers.ProjectToCustomerDto();
```

For more details, see [Getting Started Guide](docs/quick-start.md).

---

## Supported Mappings

ElBruno.AotMapper supports:

- **Classes, records, and structs** — All reference and value types
- **Nested objects** — Recursive mapping of complex object graphs
- **Collections** — Arrays, `List<T>`, `IEnumerable<T>`, and other collection types
- **Enums** — Enum-to-string and string-to-enum conversion helpers
- **Nullable reference types** — Full nullability awareness and validation
- **Custom property rules** — Remap properties, set defaults, and ignore fields
- **Partial method hooks** — Optional pre/post-mapping customization
- **EF Core projections** — Safe query translation for supported patterns

See [Supported Mappings](docs/supported-mappings.md) for complete details and examples.

---

## Core Package: ElBruno.AotMapper

The core package provides the mapping attributes and lightweight abstractions.

```bash
dotnet add package ElBruno.AotMapper
```

**Key attributes:**
- `[MapFrom(Type)]` — Mark destination type as mappable from a source type
- `[MapProperty(sourceProperty, destinationProperty)]` — Remap a property
- `[MapIgnore]` — Skip a property in the mapping
- `[MapConverter]` — Specify a custom type converter

No runtime dependencies; works standalone in any .NET project.

---

## Generator Package: ElBruno.AotMapper.Generator

The Roslyn incremental source generator that creates mapping code at compile time. Install as a dev dependency:

```bash
dotnet add package ElBruno.AotMapper.Generator --prerelease
```

The generator:
- Scans for `[MapFrom]` attributes during build
- Generates strongly-typed extension methods (e.g., `ToCustomerDto()`)
- Emits compile-time diagnostics for incomplete or invalid mappings
- Works with incremental builds for fast iteration

---

## ASP.NET Core Package: ElBruno.AotMapper.AspNetCore

Streamline dependency injection and ASP.NET Core integration:

```bash
dotnet add package ElBruno.AotMapper.AspNetCore
```

**Usage:**

Register mappers in your service collection:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add AOT mapper services
builder.Services.AddAotMapper();

var app = builder.Build();
// ... your endpoints
```

Generated mappers are then available as extension methods or injected services.

---

## Entity Framework Core Package: ElBruno.AotMapper.EntityFramework

Simplify EF Core query projections with AOT-safe mappers:

```bash
dotnet add package ElBruno.AotMapper.EntityFramework
```

**Usage:**

Use generated extension methods for in-memory mapping, or manual Select for projections:

```csharp
[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(string Id, string Name, string Tier);

// In-memory mapping (generated extension method)
var dtos = customers.Select(c => c.ToCustomerDto()).ToList();

// EF Core projection (manual Select for SQL translation)
var projected = context.Customers
    .Select(c => new CustomerDto(c.Id, c.Name, c.Tier.ToString()))
    .ToListAsync();
```

**Important:** EF projection support is limited to patterns that translate safely to SQL. See [EF Integration Guide](docs/ef-integration.md) for supported and unsupported scenarios.

---

## Building from Source

Clone the repository and build with .NET 8 SDK or later:

```bash
git clone https://github.com/elbruno/ElBruno.AotMapper.git
cd ElBruno.AotMapper

# Restore dependencies
dotnet restore ElBruno.AotMapper.slnx

# Build
dotnet build ElBruno.AotMapper.slnx

# Run tests
dotnet test ElBruno.AotMapper.slnx
```

The solution file (`ElBruno.AotMapper.slnx`) includes all library, test, and sample projects.

---

## Documentation

- **[Quick Start Guide](docs/quick-start.md)** — Installation, basic usage, first mapping
- **[Supported Mappings](docs/supported-mappings.md)** — Complete feature matrix and examples
- **[Known Limitations](docs/known-limits.md)** — Current constraints and workarounds
- **[Diagnostics Reference](docs/diagnostics.md)** — All diagnostic IDs and remediation steps
- **[EF Core Integration](docs/ef-integration.md)** — Projection helpers and query translation
- **[Migration Guide](docs/migration-guide.md)** — Migrating from AutoMapper or manual mapping

---

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

---

## Author

**Bruno Capuano** (ElBruno)

- 🌐 Blog: https://elbruno.com
- 📺 YouTube: https://youtube.com/@inthelabs
- 💼 LinkedIn: https://linkedin.com/in/inthelabs
- 𝕏 Twitter: https://twitter.com/inthelabs
- 🎙️ Podcast: https://inthelabs.dev

---

## Acknowledgments

This library is built on [Roslyn](https://github.com/dotnet/roslyn) and inspired by the .NET ecosystem's move toward compile-time solutions for NativeAOT and trimmed applications. Special thanks to the .NET community for feedback and contributions.
