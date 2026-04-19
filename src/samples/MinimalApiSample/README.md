# MinimalApiSample

A minimal ASP.NET Core Web API demonstrating ElBruno.AotMapper with dependency injection and endpoint routing.

## Prerequisites

- .NET 8 SDK or later
- Visual Studio, VS Code, or JetBrains Rider (optional)

## What It Demonstrates

- Basic `[MapFrom]` attribute setup
- Generated extension methods (`.ToXxx()`)
- Dependency injection with `AddAotMapper()`
- RESTful endpoint mapping request/response DTOs
- Property renaming with `[MapProperty]`

## Key Files

- `Program.cs` — Application entry point, service registration
- `Models/` — Domain entities
- `Dtos/` — DTO classes with `[MapFrom]` attributes
- `Endpoints/` — Minimal API endpoints using generated mappers

## Run the Sample

```bash
cd src/samples/MinimalApiSample

# Restore and build
dotnet restore
dotnet build

# Run
dotnet run

# Test with curl
curl https://localhost:5001/api/customers
curl -X POST https://localhost:5001/api/customers \
  -H "Content-Type: application/json" \
  -d '{"name":"Alice","tier":"Gold"}'
```

## Expected Output

The API will:
1. Accept JSON requests with `Customer` data
2. Map them to `CustomerDto` using the generated mapper
3. Return the mapped response

Check `https://localhost:5001/swagger` (if Swagger is enabled) to explore endpoints interactively.

## Architecture

```
MinimalApiSample
├── Program.cs          # DI setup, endpoint mapping
├── Models/
│   └── Customer.cs
├── Dtos/
│   └── CustomerDto.cs  # [MapFrom(typeof(Customer))]
└── Endpoints/
    └── CustomerEndpoints.cs
```

## Learn More

- [Quick Start Guide](../../docs/quick-start.md)
- [Supported Mappings](../../docs/supported-mappings.md)
- [ASP.NET Core Integration](../../docs/aspnetcore.md)
