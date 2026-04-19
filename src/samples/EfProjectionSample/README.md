# EfProjectionSample

Demonstrates ElBruno.AotMapper with EF Core projections using the generated `ProjectToXxx()` extension methods.

## Prerequisites

- .NET 8 SDK or later
- SQL Server LocalDB (or configure a different database in `appsettings.json`)

## What It Demonstrates

- EF Core integration with generated projections
- `ProjectTo<Dto>()` extension for `IQueryable<T>`
- SQL-translatable mapping patterns
- Distinguishing between in-memory mapping (`.Select().ToList()`) and SQL projection
- AOTMAP007 diagnostic for non-translatable patterns

## Key Files

- `Program.cs` — Database context setup, DI registration
- `Data/ApplicationDbContext.cs` — EF Core DbContext
- `Models/Order.cs` — Domain entity
- `Dtos/OrderDto.cs` — DTO with `[MapFrom]` attribute
- `Endpoints/OrderEndpoints.cs` — API routes demonstrating projections
- `appsettings.json` — Database connection string

## Run the Sample

```bash
cd src/samples/EfProjectionSample

# Restore dependencies
dotnet restore

# Apply migrations (creates database)
dotnet ef database update

# Build
dotnet build

# Run
dotnet run

# Test with curl
curl https://localhost:5001/api/orders
curl https://localhost:5001/api/orders/1
```

## What Happens

1. The application creates a database and seeds sample `Order` data
2. Endpoints use the generated `ProjectToOrderDto()` extension
3. EF Core translates the projection to SQL (single database round-trip)
4. Results are returned as JSON

Compare this to in-memory mapping—the projection is much more efficient!

## Architecture

```
EfProjectionSample
├── Program.cs                      # DbContext, DI setup
├── Data/
│   └── ApplicationDbContext.cs
├── Models/
│   └── Order.cs
├── Dtos/
│   └── OrderDto.cs                 # [MapFrom(typeof(Order))]
├── Endpoints/
│   └── OrderEndpoints.cs           # Uses ProjectToOrderDto()
└── appsettings.json                # DB connection
```

## Key Learning Points

- **Use `ProjectToXxx()` for queries:** Translates to SQL, minimal data transfer
- **Use in-memory mapping for transforms:** When the pattern doesn't translate cleanly
- **Check AOTMAP007 diagnostics:** If the generator rejects your pattern, switch to in-memory

## Learn More

- [EF Core Integration](../../docs/ef-integration.md)
- [AOT & Trimming Guarantees](../../docs/aot.md)
- [Troubleshooting](../../docs/troubleshooting.md)
