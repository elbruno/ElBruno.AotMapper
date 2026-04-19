# Decisions Log — ElBruno.AotMapper

## Badge Styling for New Package Release

**Status:** ✅ Implemented  
**Decider:** Oracle (DevRel)  
**Date:** 2026-04-17T14:06Z

### Decision

Apply **shields.io flat-square styling with neutral color override** to the README.md Packages table badges to ensure professional presentation at 0 downloads.

#### Specifics

- **All badges:** Add `?style=flat-square` parameter for modern, flat design aesthetic
- **Download badges only:** Add `?color=informational` to override the default red at 0 downloads

#### Example URLs

```
Version badge:
https://img.shields.io/nuget/v/ElBruno.AotMapper.svg?style=flat-square

Download badge:
https://img.shields.io/nuget/dt/ElBruno.AotMapper.svg?style=flat-square&color=informational
```

### Rationale

1. **First Impression:** New packages ship with 0 downloads. The default shields.io styling renders download counts at 0 with a red background, creating a false impression of failure/issues.

2. **Visual Consistency:** The flat-square style provides a modern, clean appearance consistent with contemporary NuGet package dashboards and professional open-source projects.

3. **Semantic Accuracy:** The informational (blue) color accurately conveys neutral status for a new release, avoiding the negative connotation of red.

4. **Future-Proof:** As downloads accumulate, the badge will automatically scale and color dynamically based on NuGet's live data, no URL changes needed.

### Consequences

- ✅ README Packages table now presents professionally on first publication
- ✅ Badges automatically update as downloads grow
- ✅ Styling is consistent across all 4 packages
- ✅ No maintenance required as package metrics evolve
- ❌ Requires manual documentation update if shields.io styling conventions change (low probability)

### Scope

- **Modified:** `README.md` lines 25–30 (Packages table)
- **Verified:** `docs/blog-aotmapper-intro.md` (no changes needed)

---

**Related Files:**  
- `.squad/agents/oracle/history.md` — Learning entry documenting implementation
- `.squad/orchestration-log/2026-04-17T1406-oracle.md` — Orchestration record
- `.squad/log/2026-04-17T1406-badge-dashboard-fix.md` — Session log
