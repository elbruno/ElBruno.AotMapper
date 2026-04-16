# Oracle — History

## Project Context
- **Project:** ElBruno.AotMapper — AOT-friendly DTO mapper for .NET using Roslyn source generators
- **Tech Stack:** .NET 8/10, C#, NuGet
- **User:** Bruno Capuano
- **PRD:** docs/AOT_Mapper_PRD.md

## Learnings

### Image Generation Prompts Document (2024)
- **Outcome:** Created comprehensive `docs/image-generation-prompts.md` with detailed AI image generation prompts for all ElBruno.AotMapper visual assets
- **Coverage:** 7 major asset categories (NuGet logo, social preview, blog headers, social media, conference assets, documentation, YouTube thumbnail)
- **Tool Recommendations:** Provided guidance on DALL-E 3, Midjourney, and Ideogram with specific use-case recommendations
- **Key Design Decisions:**
  - **NuGet Icon:** 3 variants (Lightning Gear primary, Code Strand Mapper, Molecular Compiler) for stakeholder choice
  - **Social Media:** Complete LinkedIn carousel series (5 slides covering overview → features → use cases → getting started → CTA)
  - **Color Scheme:** Consistent use of .NET purple (#512BD4), electric blue (#0078D4), white; included hex codes in all prompts
  - **Visual Motifs:** Emphasized compile-time/code-generation themes with lightning bolts, gears, code brackets, transformation arrows
- **Technical Details:**
  - Each prompt includes platform recommendation, dimensions, negative prompt, and post-processing notes
  - Prompts follow modern design principles: flat design, geometric shapes, whitespace, high contrast for readability
  - Included accessibility and web optimization guidelines
  - Provided workflow recommendations for generation, review, export, and testing
- **Deliverable:** Professional DevRel resource enabling consistent, high-quality visual content generation across all marketing and documentation channels

### README and Documentation Suite (2024)
- **Outcome:** Created complete README.md and 6 comprehensive documentation files following ElBruno repository conventions
- **README Features:**
  - Badges row: CI Build, Publish to NuGet, License, GitHub stars (shields.io format)
  - Package table with NuGet and download badges for all 4 packages
  - Quick start section with installation and usage example
  - Per-package descriptions for core, generator, ASP.NET Core, and EF Core packages
  - Building from source instructions
  - Links to documentation and author information
- **Documentation Files Created:**
  1. **docs/quick-start.md** — 15-minute onboarding guide with step-by-step setup and advanced features (nested objects, collections, strict mode, partial methods)
  2. **docs/supported-mappings.md** — Complete feature matrix covering types (primitives, collections, enums, nullable), objects (classes, records, structs), nested mappings, property features
  3. **docs/known-limits.md** — Honest documentation of limitations with practical workarounds (complex converters, circular dependencies, dynamic properties, conditional mapping, EF translation limits)
  4. **docs/diagnostics.md** — Reference for all 5 diagnostic IDs (AOTMAP001–AOTMAP005) with causes, examples, and remediation steps; diagnostic workflow section
  5. **docs/ef-integration.md** — EF Core guide covering supported patterns (simple properties, nested objects, collections, enums), unsupported patterns (complex expressions, multi-level nesting, CLR functions) with clear examples and performance considerations
  6. **docs/migration-guide.md** — Step-by-step migration from AutoMapper, manual mapping, and other libraries; common scenarios; testing strategy; migration checklist
- **Key Principles Applied:**
  - **Explicitness:** All limitations clearly documented with honest workarounds
  - **AOT Focus:** Emphasized compile-time safety, NativeAOT compatibility, zero-reflection throughout
  - **Example-Driven:** Every feature and limitation includes runnable code examples
  - **Developer-Friendly:** Clear troubleshooting sections, visual indicators (✅/❌/⚠️), quick lookup tables
- **Technical Details:**
  - README conforms to ElBruno conventions: title → badges → tagline → description → packages table → quick start → features → per-package sections → building → docs → license → author → acknowledgments
  - Installation commands use `dotnet add package` (no XML snippets)
  - All badges use shields.io with correct URLs
  - Documentation links follow convention: point to docs/ folder
  - Author section includes: blog, YouTube, LinkedIn, Twitter, podcast
- **Deliverable:** Complete, production-ready documentation enabling developers to onboard, troubleshoot, and migrate to ElBruno.AotMapper with confidence
