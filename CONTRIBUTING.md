# Contributing to ElBruno.AotMapper

Thank you for considering contributing to ElBruno.AotMapper! This document provides guidance on how to contribute.

## Repository Layout

```
ElBruno.AotMapper/
├── src/
│   ├── ElBruno.AotMapper*/          # Library projects
│   ├── tests/                       # xUnit test projects
│   └── samples/                     # Sample applications
├── docs/                            # Documentation (guides, troubleshooting, blog)
└── .github/workflows/               # CI/CD pipelines
```

## Building and Testing

### Prerequisites

- .NET 8 SDK or later (use `dotnet --version` to check)

### Build

```bash
dotnet build ElBruno.AotMapper.slnx
```

### Run Tests

```bash
dotnet test ElBruno.AotMapper.slnx
```

All tests must pass before submitting a pull request.

## Branching Convention

Feature branches follow the naming pattern:

```
squad/<topic>
```

Examples:
- `squad/v0.6-hardening`
- `squad/ef-projection`
- `squad/bug-fix-incremental-cache`

## Commit Message Style

Keep commits atomic and clear:

```
[Area] Short description

Longer explanation if needed.

Closes #123
```

Examples:
- `[docs] Add troubleshooting section`
- `[generator] Fix incremental cache value equality`
- `[ef] Add ProjectTo extension for IQueryable`

## Code Ownership

- **Tank** — CI/CD pipelines (`.github/workflows/`)
- **Neo** — Generator implementation (source generator logic)
- **Trinity** — Integration packages (ASP.NET Core, EF Core)
- **Oracle** — Documentation, README, blog, troubleshooting
- **Morpheus** — Architecture decisions, coordination

## Discussion & Questions

- **Architectural decisions** → `.squad/decisions.md`
- **Feature requests** → GitHub Issues with label `enhancement`
- **Bug reports** → GitHub Issues with label `bug`
- **Design discussions** → GitHub Discussions

## Documentation

When adding a feature:

1. Update relevant docs in `docs/`
2. Update or add sample code in `src/samples/`
3. Update `CHANGELOG.md` if user-facing
4. Add diagnostic documentation if applicable

## License

All contributions are licensed under the MIT License.
