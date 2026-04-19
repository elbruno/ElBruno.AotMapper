# Trinity v0.6 — ProjectTo Implementation Decisions

**Date:** 2025-01-31  
**Author:** Trinity (Integration Dev)  
**Status:** PARTIAL — Generator changes complete, build system issues blocking full validation

---

## 1. EF Projection Generation Strategy

**Decision:** Generate `ProjectToXxxDto()` extension methods for `IQueryable<T>` when mappings are EF-compatible.

**EF-Safety Detection Rules:**
- ✅ **Allowed:**
  - Direct property-to-property mapping
  - Nested object mapping via `[MapFrom]` (inline property assignments)
  - Enum-to-string via `.ToString()`
  - Enum-to-enum casts

- ❌ **Blocked (not EF-translatable):**
  - Custom converters (`IMapConverter<,>`) — method calls not translatable
  - Collection mappings (`.Select().ToList()`) — projection context doesn't support
  - String-to-enum (`Enum.Parse`) — not a valid SQL operation

**Implementation:**
- New file: `src/ElBruno.AotMapper.Generator/EfProjectionEmitter.cs`
- Hook in `MappingEmitter.cs`: single-line addition after `ToXxxList()` generation
- For incompatible mappings: emit XML doc comment explaining why + AOTMAP007 diagnostic

---

## 2. AOTMAP007 Diagnostic

**Decision:** Informational diagnostic (severity: Info) to notify developers when ProjectTo cannot be generated.

**Rationale:**
- Not an error — the `ToXxx()` method still works fine (in-memory mapping)
- Developers should know why they can't use server-side projection
- Message format: `"Mapping from '{Source}' to '{Dest}' cannot generate ProjectTo method: {reason}"`

**Examples:**
- "contains custom converter (not EF-translatable)"
- "contains collection mapping (not supported in ProjectTo)"
- "contains String-to-Enum conversion (use Enum.Parse in post-processing)"

---

## 3. AotMapperOptions API Surface

**Decision:** Add `AddAotMapper(Action<AotMapperOptions>)` overload with empty options class.

**Rationale:**
- Lock API signature for v1 (breaking changes are expensive)
- Options class is empty for now but extensible for future:
  - Validation behavior toggles
  - Default null handling strategy
  - Performance/caching settings
- Parameterless overload still works (calls configured version with no-op action)

**Implementation:**
- New file: `src/ElBruno.AotMapper.AspNetCore/AotMapperOptions.cs`
- Updated: `AotMapperServiceCollectionExtensions.cs` with overload

---

## 4. Known Issues & Workarounds

### Build System Issue (dotnet SDK 10.0.300-preview)

**Problem:**  
The preview .NET SDK has a bug where `dotnet restore` targeting a `netstandard2.0` project creates `project.assets.json` with `net8.0` target instead of `netstandard2.0`. This breaks multi-project builds that reference the generator.

**Symptoms:**
```
error NETSDK1005: Assets file doesn't have a target for 'netstandard2.0'
```

**Workaround (used in this session):**
- Build generator standalone first: `dotnet build src/ElBruno.AotMapper.Generator/ElBruno.AotMapper.Generator.csproj`
- Then build samples/tests without triggering generator rebuild

**Long-term fix:**
- Use stable SDK (8.0.x) OR
- Multi-target generator to `netstandard2.0;net8.0` (but this complicates packaging)

---

## 5. Test Coverage

### Implemented:
- ✅ `ElBruno.AotMapper.EntityFramework.Tests` project created
- ✅ Test: `ApplyProjection_Helper_Works` — validates existing extension method
- ✅ Uses Sqlite in-memory database (not EF InMemory provider) for realistic SQL translation testing

### Deferred (blocked by build issue):
- ⏸️ `ProjectTo_Translates_To_SQL` — needs generated `ProjectToXxxDto()` method
- ⏸️ `ProjectTo_With_Nested_Mapping` — needs nested projection generation
- Tests are stubbed with `[Fact(Skip = "...")]` placeholders

---

## 6. Sample Update (EfProjectionSample)

**Deferred:** Task 2 blocked by build system issue.  
Once generator builds successfully in multi-project context:
- Replace manual `.Select(o => new OrderDto(...))` in `Program.cs`
- Use generated `.ProjectToOrderDto()` method
- Verify SQL output via EF logging

---

## Next Steps (for Neo or future Trinity session)

1. **Fix SDK issue:**  
   - Test with stable .NET 8 SDK
   - OR add CI workaround (build generator separately before main build)

2. **Complete ProjectTo generation:**
   - Handle nested mappings properly (inline nested properties)
   - Test with complex scenarios (multiple nesting levels, nullable navigations)

3. **Validate EfProjectionSample:**
   - Run sample
   - Inspect generated SQL (should show single query with inline projection)

4. **Un-skip tests:**
   - Implement and run `ProjectTo_Translates_To_SQL`
   - Add coverage for edge cases (nullable enums, multiple properties)

---

**Files Changed:**
- `src/ElBruno.AotMapper.Generator/DiagnosticDescriptors.cs` — added AOTMAP007
- `src/ElBruno.AotMapper.Generator/EfProjectionEmitter.cs` — NEW
- `src/ElBruno.AotMapper.Generator/IsExternalInit.cs` — NEW (polyfill for netstandard2.0)
- `src/ElBruno.AotMapper.Generator/Models/PropertyRename.cs` — NEW (equatable wrapper)
- `src/ElBruno.AotMapper.Generator/AotMapperGenerator.cs` — updated to use PropertyRename, emit AOTMAP007
- `src/ElBruno.AotMapper.Generator/MappingEmitter.cs` — added ProjectTo hook
- `src/ElBruno.AotMapper.AspNetCore/AotMapperOptions.cs` — NEW
- `src/ElBruno.AotMapper.AspNetCore/AotMapperServiceCollectionExtensions.cs` — added options overload
- `src/tests/ElBruno.AotMapper.EntityFramework.Tests/` — NEW test project
- `ElBruno.AotMapper.slnx` — added EF tests project
