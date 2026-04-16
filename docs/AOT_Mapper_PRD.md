# Product Requirements Document (PRD)
## Project: AOT-friendly DTO Mapper for .NET

**Version:** v1.0 draft  
**Date:** April 15, 2026  

---

## Suggested library names

Here are naming options that fit your current family style (`ElBruno.LocalLLMs`, `ElBruno.LocalEmbeddings`, `ElBruno.Text2Image`, etc.).

### Strongest single-package names
1. **ElBruno.AotMapper**  
   Clear, direct, and very aligned with the main value proposition.

2. **ElBruno.MapGen**  
   Shorter and more brandable; highlights compile-time generation.

3. **ElBruno.DtoMapper**  
   Very understandable for enterprise developers, though a bit less differentiated.

4. **ElBruno.NativeMapper**  
   Nice positioning around NativeAOT, but broader and slightly less precise.

5. **ElBruno.SourceMapper**  
   Strong tie to source generators; good technical branding.

### Good names for a collection of libraries
If you want a family instead of a single package, these work well:

- **ElBruno.AotMapper**  
  - `ElBruno.AotMapper`
  - `ElBruno.AotMapper.Generator`
  - `ElBruno.AotMapper.AspNetCore`
  - `ElBruno.AotMapper.EntityFramework`

- **ElBruno.MapGen**
  - `ElBruno.MapGen`
  - `ElBruno.MapGen.Generator`
  - `ElBruno.MapGen.AspNetCore`
  - `ElBruno.MapGen.EntityFramework`

- **ElBruno.SourceMapper**
  - `ElBruno.SourceMapper`
  - `ElBruno.SourceMapper.Generator`
  - `ElBruno.SourceMapper.AspNetCore`
  - `ElBruno.SourceMapper.EntityFramework`

### Recommendation
My top recommendation is:

**ElBruno.AotMapper**

Why:
- instantly communicates the NativeAOT / trimming story
- fits well with your existing portfolio
- works well both as a main package and as a package family
- easy to explain in blogs, sessions, and NuGet

---

## Executive summary

Build a free, source-generator-based mapping library for .NET that converts entities, records, and API contracts without runtime reflection, while remaining compatible with NativeAOT, trimming, and modern ASP.NET Core workloads.

The goal is to provide a mapping experience that feels as easy as popular mappers, but with compile-time generation, explicit diagnostics, predictable performance, and a strong story for enterprise and cloud-native .NET development.

---

## 1. Problem statement

Teams building modern .NET APIs often want three things at the same time:

- fast object mapping
- zero-reflection startup behavior
- compatibility with NativeAOT and trimming

Existing options usually force trade-offs:

- Runtime reflection or expression compilation increases startup cost and AOT risk.
- Manual mapping is predictable but repetitive and expensive to maintain at scale.
- Common mapping libraries do not always compose cleanly with trimming, source generation, or EF query projection.
- Teams want compile-time validation, not production-time surprises.

---

## 2. Product vision

Deliver the most developer-friendly compile-time mapper for .NET: easy to adopt, explicit by default, benchmarkable, and safe for NativeAOT workloads.

The product should feel modern, boring in production, and easy to trust in enterprise environments.

### Product principles

- **Compile-time first:** generate code instead of interpreting mapping rules at runtime.
- **AOT-safe by design:** no required runtime reflection in the happy path.
- **Predictable behavior:** explicit diagnostics for missing mappings, nullability mismatches, and unsupported transforms.
- **Incremental adoption:** work file-by-file or DTO-by-DTO without forcing a full-platform rewrite.
- **Strong defaults:** simple setup for 80% of scenarios, extension points for advanced cases.

---

## 3. Target users and personas

### Primary users
- .NET API developers building DTO-heavy applications
- teams adopting NativeAOT or trimmed deployments
- enterprise developers using EF Core and layered architectures
- library authors and platform teams who need compile-time-safe mapping

### Secondary users
- developers migrating away from reflection-heavy mappers
- performance-focused teams benchmarking cold start and allocation
- teams building internal platforms and reusable API templates

### Personas
1. **API Developer**  
   Wants quick DTO generation, readable code, and minimal boilerplate.

2. **Platform Engineer**  
   Wants predictable builds, deterministic generation, and analyzer support.

3. **Performance-minded Architect**  
   Wants low startup overhead, low allocations, and benchmarkable output.

4. **Enterprise Team Lead**  
   Wants easy onboarding, good docs, and a low-risk migration path from manual or runtime mapping.

---

## 4. Goals, non-goals, and success criteria

### Goals for v1

- Generate strongly typed mappers between source and destination models using Roslyn incremental generators.
- Support common property mapping scenarios for classes, records, structs, nested models, collections, enums, and nullable reference types.
- Provide compile-time diagnostics with clear fixes when mappings are incomplete or ambiguous.
- Provide optional EF-friendly projection helpers where feasible without hiding query translation limits.
- Ship samples for ASP.NET Core minimal APIs, MVC, and NativeAOT console/API scenarios.

### Non-goals for v1

- No runtime reflection fallback in the default package.
- No full AutoMapper parity for every dynamic scenario.
- No visual designer, no large framework abstraction, and no cross-language support.
- No attempt to become an ORM or repository framework.

### Success criteria

- Users can onboard in under 15 minutes from install to first successful generated map.
- Generated mapping performs at or near manual mapping in microbenchmarks.
- Package works in trimmed and NativeAOT sample apps without special hacks.
- Issue tracker shows low confusion around nullability, nested mapping, and registration.

---

## 5. Scope

### In scope

- Attribute-based and optional convention-based mapping definitions.
- One-to-one type maps, nested maps, collection maps, enum/string conversion helpers.
- Custom property rules, ignore rules, default values, and limited transformation callbacks.
- Generated extension methods such as `ToDto()`, `ToEntity()`, and mapper service registration.
- Analyzer diagnostics and code-fix suggestions where reasonable.

### Out of scope

- Arbitrary runtime-configured mappings loaded from JSON or DB.
- Graph diff/merge semantics for persistence tracking.
- Bidirectional change tracking or patch document engine.
- Universal projection of any arbitrary LINQ expression with no limitations.

---

## 6. User experience

### Desired first-run experience

A developer installs one NuGet package, adds a mapping attribute, builds the project, and immediately sees generated methods with IntelliSense support.

### Example

```csharp
[MapFrom(typeof(Customer))]
public sealed partial record CustomerDto(string Id, string Name, string Tier);

// generated usage
CustomerDto dto = customer.ToCustomerDto();
IQueryable<CustomerDto> query = customers.ProjectToCustomerDto();
```

### Developer experience requirements

- Clear namespace model and generated code discoverability.
- Actionable diagnostics that name the missing property and the candidate fix.
- Generated code should be debuggable and readable in the `obj/generated` output.
- Opt-in strict mode for teams that want all unmapped properties treated as errors.

---

## 7. Functional requirements

### Mapping definition
- Support `[MapFrom]` and related attributes on destination types.
- Support property remapping with explicit attributes such as `[MapProperty]`.
- Support ignore semantics, default values, and custom converters.
- Allow partial-method hooks for pre/post mapping where safe.

### Type support
- classes
- records
- structs
- nested object graphs
- arrays and collections
- enum-to-string and string-to-enum helpers
- nullable reference types

### Generated APIs
- destination-focused extension methods such as `source.ToCustomerDto()`
- optional service registration helpers for DI
- explicit projection APIs for EF Core scenarios

### Diagnostics
- missing member mapping
- type mismatch
- nullability mismatch
- unsupported transform
- conflicting map definitions

---

## 8. Non-functional requirements

- **Performance:** generated mapping for simple POCOs should stay within approximately 5–15% of hand-written mapping in throughput benchmarks; simple scenarios should aim for parity.
- **Startup:** package should add negligible startup overhead beyond assembly load and JIT/AOT code path selection.
- **Allocation:** simple object mapping should avoid intermediate allocations beyond destination creation and required collection materialization.
- **Compatibility:** official support for .NET 10; evaluate .NET 8 compatibility if it does not compromise the product story.
- **Reliability:** deterministic generation in CI across Windows, Linux, and macOS build agents.
- **Observability:** benchmark reports and analyzer docs published with each release.

---

## 9. Technical architecture

### Package structure

- **Core package:** attributes, shared abstractions, lightweight runtime helpers
- **Generator package:** Roslyn incremental source generator and analyzers
- **ASP.NET Core package:** DI registration and convenience extensions
- **EntityFramework package:** optional projection helpers and integration samples
- **Benchmarks package/app:** BenchmarkDotNet suite for regression tracking

### High-level design

- Developers annotate destination types or mapping profiles.
- The incremental generator scans syntax and semantic model and builds a mapping graph.
- The generator emits static partial methods and extension methods per map.
- Analyzers validate unsupported scenarios and suggest annotations or manual hooks.
- Optional helper packages add DI and EF composition, but the core remains dependency-light.

### Public API direction

```csharp
[MapFrom(typeof(Customer), Strict = true)]
[MapProperty(nameof(Customer.CustomerId), nameof(CustomerDto.Id))]
public sealed partial record CustomerDto(string Id, string Name, string Tier);

// generated usage
var dto = customer.ToCustomerDto();
```

---

## 10. EF Core and projection strategy

EF integration should be treated as a high-value convenience layer, not a magical promise that every query will translate.

### Principles
- Generate projection expressions only for supported member patterns.
- Fail early with diagnostics or docs when a custom transform cannot translate to SQL.
- Prefer explicit API names that communicate intent, such as `ProjectToCustomerDto()`.
- Maintain a compatibility matrix for common providers used in samples, starting with SQL Server and SQLite.

### Important product stance
This library should not pretend to solve arbitrary LINQ translation. It should be explicit, documented, and predictable.

---

## 11. Diagnostics and error model

The analyzer and generator experience is a major part of the product.

### Required diagnostic scenarios

- **Missing source member:** warn or error with exact destination member name.
- **Type mismatch:** suggest supported converter attribute or manual partial method.
- **Nullability mismatch:** explain risk and available fixes.
- **Unsupported projection transform:** route developer to in-memory map or supported expression pattern.
- **Duplicate or conflicting map definitions:** fail the build with deterministic ordering and clear file locations.

### Diagnostic quality bar
Each diagnostic should include:
- a stable ID
- a short explanation
- a likely fix
- a link to docs when needed

---

## 12. Testing strategy

### Automated tests

- Generator snapshot tests for emitted code and diagnostics.
- Compile tests using small fixture projects covering classes, records, structs, collections, and nested models.
- Integration tests for ASP.NET Core minimal API sample.
- Trim + NativeAOT smoke tests in CI.
- EF translation tests for supported projection scenarios.

### Benchmarks

- Compare against manual mapping for common DTO sizes.
- Compare against at least one popular existing mapper in non-AOT scenarios for context.
- Track allocation count, throughput, and cold-start sample measurements.

### CI matrix
- Windows
- Linux
- macOS

---

## 13. Security, licensing, and OSS expectations

- Use MIT license for maximal adoption unless there is a strong branding or governance reason otherwise.
- No telemetry in v1.
- No hidden network behavior, code download, or reflection-based plugin loading.
- Contributor guide should define code style, PR expectations, and benchmark update policy.

---

## 14. Delivery plan

### Phase 0 — Naming, repo, and design
- finalize package naming
- create repo structure
- define API conventions
- publish draft roadmap

### Phase 1 — Core generation
- implement core attributes
- implement basic one-to-one mapping
- generate extension methods
- support classes, records, structs, nullable reference types

### Phase 2 — Diagnostics and developer experience
- add analyzer rules
- add clear errors and docs
- improve generated code readability
- add strict mode

### Phase 3 — Collections, nesting, and converters
- nested models
- collections
- enum/string conversion
- default values and ignore rules

### Phase 4 — ASP.NET Core and EF helpers
- DI helpers
- sample apps
- EF projection helpers for supported patterns
- provider compatibility matrix

### Phase 5 — Quality and release
- benchmark suite
- NativeAOT smoke tests
- README and docs
- NuGet publication
- launch content

---

## 15. Repository structure

```text
/src
  /ElBruno.AotMapper
  /ElBruno.AotMapper.Generator
  /ElBruno.AotMapper.AspNetCore
  /ElBruno.AotMapper.EntityFramework
/samples
  /MinimalApiSample
  /NativeAotSample
  /EfProjectionSample
/tests
  /GeneratorTests
  /IntegrationTests
  /AotSmokeTests
/benchmarks
  /ElBruno.AotMapper.Benchmarks
/docs
```

---

## 16. Risks and mitigations

### Risk: generator complexity grows too fast
**Mitigation:** keep v1 opinionated and narrow; push advanced scenarios to backlog.

### Risk: EF projection expectations become unrealistic
**Mitigation:** use explicit APIs, examples, and a documented support matrix.

### Risk: diagnostic quality is weak
**Mitigation:** treat analyzers as a first-class feature, not a side effect.

### Risk: package naming becomes confusing
**Mitigation:** keep one strong root package and use suffixes consistently.

### Risk: adoption stalls
**Mitigation:** ship excellent samples, benchmarks, and migration docs early.

---

## 17. Acceptance criteria for v1 preview

- Package published to NuGet with README, samples, license, and changelog.
- At least three end-to-end samples compile and run: Minimal API, EF projection, NativeAOT console or API.
- Benchmarks published in repo and linked in package docs.
- Core scenarios covered by automated tests with CI across major OS targets.
- Documentation includes quick start, supported mappings, known limits, and troubleshooting.

---

## 18. Launch checklist

- Reserve final package name and GitHub repository.
- Publish logo, README, docs site, and first blog post.
- Create short demo video and benchmark narrative.
- Prepare issue templates: bug, feature request, unsupported mapping scenario.
- Publish roadmap for v1.x and v2 candidates.

---

## 19. Backlog candidates after v1

- Two-way update mapping into existing instances.
- Patch/update semantics for API commands.
- Richer converter ecosystem and reusable mapping profiles.
- Razor/Blazor helper samples.
- OpenAPI-aware DTO generation or integration.

---

## 20. Recommendation

**Proceed.**

This product is well aligned with current .NET trends:

- NativeAOT
- trimming
- minimal APIs
- source generators
- performance-aware cloud workloads

It is also small enough to build incrementally and public enough to become useful as an OSS NuGet package and content engine for blog posts, demos, videos, and conference sessions.

---

## Final naming recommendation

If you want one direct recommendation for the library family, use:

**ElBruno.AotMapper**

Suggested package family:

- `ElBruno.AotMapper`
- `ElBruno.AotMapper.Generator`
- `ElBruno.AotMapper.AspNetCore`
- `ElBruno.AotMapper.EntityFramework`
