# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.6.0] — 2026-04-19

### Added

- **`[MapIgnore]` attribute** — Now fully functional for excluding properties from generated mappings
- **`[MapConverter]` attribute** — Now fully functional for custom type converters with instantiation support
- **EF Core `ProjectTo` IQueryable extension** — New `ProjectToXxx()` methods for safe SQL-friendly projections
- **`Dictionary<K,V>` support** — Generic dictionary mapping now supported in generated code
- **Enum↔int conversion** — Automatic bidirectional conversion between enums and their integer values
- **AOTMAP006 diagnostic** — New rule for inaccessible members in mapping pipeline
- **AOTMAP007 diagnostic** — New rule for EF projection patterns that cannot be translated to SQL
- **HelpLinkUri on all diagnostics** — Diagnostics now link to online documentation
- **`AotMapperOptions` overload** — New constructor overload for `AddAotMapper()` with configuration

### Changed

- **Incremental generator caching** — Fixed value equality comparison for better incremental build performance
- **`AddAotMapper()` overloads** — Added new overload to support future extensibility while maintaining compatibility

### Fixed

- **Incremental generator compilation capture** — Generator no longer captures the full `Compilation` object, which was preventing IDE responsiveness during editing

### Security

- **Dependabot enabled** — Automated dependency vulnerability scanning and updates
- **CodeQL analysis enabled** — Static code analysis for security vulnerabilities and code quality
- **GitHub Actions pinned to commit SHAs** — All workflow actions pinned to specific commit hashes for supply chain security
- **NativeAOT publish verification** — CI smoke test added to verify NativeAOT publish succeeds without warnings

### Known Limitations / v0.7 Roadmap

The following scenarios are tracked by skipped tests in the suite and will be addressed in v0.7:

- **AOTMAP004** for repeated `[MapFrom(typeof(SameSource))]` on the same destination
- **Open generic source types** in `MapFrom` (e.g., `Source<T>`)
- **Records with primary-constructor parameters mixed with extra mutable properties**

## [0.5.0]

See [commit history](https://github.com/elbruno/ElBruno.AotMapper/commits) for detailed changes in the 0.5.0 release.
