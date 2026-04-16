namespace ElBruno.AotMapper.EntityFramework;

/// <summary>
/// EF Core compatibility notes for AotMapper projections.
/// 
/// Supported patterns (translate to SQL):
/// - Simple property mapping (same name or renamed)
/// - Enum to int / int to enum
/// - Nested object projection (if inner type also has a mapping)
/// 
/// NOT supported in projections (must use in-memory mapping):
/// - Custom converters (IMapConverter implementations)
/// - Complex string formatting / concatenation
/// - Method calls that EF cannot translate
/// 
/// When a mapping contains unsupported patterns, the generator will emit
/// a warning diagnostic and skip projection generation for that mapping.
/// </summary>
internal static class EfCompatibilityNotes { }
