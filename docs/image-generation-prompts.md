# AI Image Generation Prompts for ElBruno.AotMapper

This document contains detailed prompts for generating all visual assets for the ElBruno.AotMapper library. Each prompt is optimized for specific AI image generators and designed to maintain visual consistency across the brand.

**Visual Identity:**
- **Primary Colors:** .NET Purple (#512BD4), Electric Blue (#0078D4), White (#FFFFFF)
- **Style:** Modern, technical, developer-focused, approachable
- **Motifs:** Code generation, compilation, performance optimization, clean flows

---

## 1. NuGet Package Logo (`nuget_logo.png`)

### Variant A: Lightning Gear (Recommended Primary)

**Platform:** Midjourney / DALL-E 3

**Dimensions:** 512x512px (scales to 128x128, 64x64)

**Prompt:**
```
A minimalist icon combining a stylized gear with a lightning bolt through its center. 
The gear is rendered in .NET purple (#512BD4), with 8 teeth. The lightning bolt is 
bright electric blue (#0078D4) and appears to be generating or flowing through the gear, 
suggesting high-performance code generation. The background is pure white. The design 
must be instantly recognizable at 64x64px with clean, solid lines and no gradients. 
Flat design, modern tech aesthetic, suitable for NuGet package branding.
```

**Negative Prompt:**
```
no realistic shading, no blurred edges, no photorealism, no complex gradients, 
no clutter, no shadows, no 3D effects
```

**Notes:**
- Must be square (512x512)
- Export with transparent background (PNG)
- Test at 64x64 and 128x128 to ensure clarity
- Consider creating solid color variants for light/dark backgrounds

---

### Variant B: Code Strand Mapper

**Platform:** Midjourney

**Dimensions:** 512x512px

**Prompt:**
```
A minimalist icon showing two parallel vertical code brackets or braces [ and ] connected 
by a glowing bridge or pathway in electric blue, suggesting transformation or mapping. 
The brackets are .NET purple (#512BD4). One bracket contains an 'A', the other contains 'B', 
representing source and destination DTO mapping. Clean, geometric, flat design on white 
background. Must work as a small icon. Modern, technical feel.
```

**Negative Prompt:**
```
no photorealism, no complex details, no shadows, no gradients, no text besides A and B, 
no clutter
```

**Notes:**
- Alternative to Variant A; test both with stakeholders
- Should convey "mapping" concept clearly

---

### Variant C: Molecular Compiler

**Platform:** Midjourney

**Dimensions:** 512x512px

**Prompt:**
```
A minimalist tech icon showing 3-4 interconnected circles or nodes arranged in a triangle, 
connected by glowing lines in electric blue. The nodes pulse or glow, suggesting real-time 
transformation. Color scheme: .NET purple circles, electric blue connecting lines. The design 
looks like a simple circuit or compilation pipeline. Flat, geometric, modern tech aesthetic. 
Suitable for NuGet icon, works at small sizes.
```

**Negative Prompt:**
```
no photorealism, no shadows, no gradients, no overly complex geometry, no abstract art, 
no blurry elements
```

**Notes:**
- Represents compilation/transformation pipeline
- Test clarity at icon sizes

---

## 2. GitHub Repository Social Preview (1280x640)

**Platform:** DALL-E 3 / Midjourney

**Dimensions:** 1280x640px

**Prompt:**
```
A modern, clean social media card for a .NET library called "ElBruno.AotMapper". 
The background is a subtle gradient from white to very light blue/gray. In the center, 
bold white typography reading "ElBruno.AotMapper" in a modern sans-serif font (like Inter or Segoe UI). 
Below it, the tagline in smaller text: "AOT-Friendly DTO Mapper for .NET". 

On the right side, a minimalist illustration showing a source code file transforming into 
a compiled output file, with a glowing pathway or arrow indicating the mapping process. 
Color scheme: .NET purple (#512BD4) and electric blue (#0078D4) for accents. 
The left side shows "@ElBruno" in a corner, with a small GitHub logo. 

The overall aesthetic is modern, technical, professional, and clean with plenty of whitespace. 
No cluttered design, no real photos, flat design elements only.
```

**Negative Prompt:**
```
no photorealism, no 3D rendering, no shadows, no complex gradients, no stock photo 
elements, no distorted text
```

**Notes:**
- This will appear when users share the repo link on social media
- Ensure text is readable at various zoom levels
- Test in GitHub's link preview renderer

---

## 3. Blog Post Header Images

### 3.1 "Introducing ElBruno.AotMapper" (Launch Post)

**Platform:** Midjourney

**Dimensions:** 1200x630px

**Prompt:**
```
A vibrant, modern tech-focused header image announcing a new .NET library. 
The background is a subtle geometric pattern of soft purple and blue lines suggesting 
code or data flow. In the center, bold white text "Introducing ElBruno.AotMapper" 
in a modern sans-serif font. Below, the tagline "High-Performance AOT-Safe Mapping 
for Modern .NET" in a slightly smaller font.

On either side, minimalist icons representing key benefits: a lightning bolt (for speed), 
a shield (for safety/AOT compatibility), and a gear (for code generation). 
All icons use .NET purple and electric blue. The overall feel is exciting, modern, 
and technical without being overwhelming. Clean, spacious layout.
```

**Negative Prompt:**
```
no photorealism, no stock photos, no overly complex gradients, no 3D effects, 
no cluttered design, no blurry text
```

**Notes:**
- This will be the hero image on the blog post
- Ensure text legibility on all backgrounds
- Consider responsive cropping for mobile

---

### 3.2 "Getting Started with ElBruno.AotMapper" (Tutorial)

**Platform:** DALL-E 3

**Dimensions:** 1200x630px

**Prompt:**
```
A friendly, educational header image for a "Getting Started" guide. 
The background shows a soft gradient from light purple to light blue. 
In the foreground, a minimalist workflow illustration showing 4 simple steps in a circle: 
1) A code file with an annotation symbol, 2) A compiler/gear, 3) A checkmark or lightning bolt, 
4) A play button or running application.

Text overlays: "Getting Started with" in smaller text, "ElBruno.AotMapper" in large, bold text. 
A small .NET logo or purple gear icon in one corner. The style is clean, welcoming, 
and educational. Flat design, modern, with plenty of whitespace.
```

**Negative Prompt:**
```
no photorealism, no photo textures, no 3D rendering, no overly abstract art, 
no confusing or cluttered design
```

**Notes:**
- Should feel accessible and educational, not intimidating
- The workflow should be clearly readable
- Works well as a blog cover and social media preview

---

### 3.3 "AOT-Safe Mapping in .NET" (Technical Deep Dive)

**Platform:** Midjourney

**Dimensions:** 1200x630px

**Prompt:**
```
A technical, sophisticated header image for an in-depth article about AOT (Ahead-of-Time) 
compilation and DTO mapping in .NET. The background is a dark gradient from deep purple 
(#512BD4) to a darker blue, with subtle geometric lines or code snippets faintly visible.

In the center, a stylized comparison: On the left, a blurred/faded image representing 
"Runtime Reflection" (dull colors, complex); on the right, a sharp, bright image representing 
"AOT Compilation" (electric blue, clean lines, geometric shapes). An arrow or path connects them.

Text: "AOT-Safe Mapping" in large, bold white text. "Compile-Time Performance" in smaller white text below. 
The overall feel is professional, technical, and modern. No photographs.
```

**Negative Prompt:**
```
no photorealism, no photo blending, no 3D effects, no overly complex gradients, 
no cluttered technical diagrams, no watermarks
```

**Notes:**
- Appeals to technical .NET developers
- Darker color scheme suggests sophistication and depth
- Consider adding subtle code elements or hexagonal patterns

---

### 3.4 "Benchmarking ElBruno.AotMapper" (Performance Post)

**Platform:** Midjourney

**Dimensions:** 1200x630px

**Prompt:**
```
A dynamic, performance-focused header image for a benchmarking article. 
The background features an upward-trending graph or bar chart in electric blue and purple, 
subtly visible but not overwhelming. The overall background is a light gray or soft white gradient.

In the center, stylized speedometer or gauge dial in .NET purple showing "max performance" 
or a lightning bolt striking downward (representing speed). Bold white text reads 
"Benchmarking ElBruno.AotMapper" with "Performance Metrics Revealed" as a subtitle.

The aesthetic is energetic, modern, and metrics-focused. Flat design with geometric elements. 
No 3D, no photorealism.
```

**Negative Prompt:**
```
no photorealism, no 3D gauge rendering, no realistic speedometer photos, no cluttered charts, 
no overly complex gradients, no stock chart images
```

**Notes:**
- Should convey speed and optimization
- Appeal to developers interested in performance
- Consider animated versions for video thumbnails

---

## 4. Social Media Promo Images

### 4.1 Twitter/X Card (Announcement)

**Platform:** DALL-E 3

**Dimensions:** 1200x675px

**Prompt:**
```
A vibrant, eye-catching announcement card for Twitter/X. The background is a bold gradient 
from .NET purple (#512BD4) to electric blue (#0078D4). In the center, large bold white text 
reads "ElBruno.AotMapper" with the tagline "AOT-Friendly DTO Mapper for .NET" below in a 
slightly smaller font.

Four bullet points or icons highlight key benefits arranged around the text:
- Lightning bolt: "Compile-time magic"
- Shield: "NativeAOT compatible"  
- Zap: "Zero reflection overhead"
- Code brackets: "Type-safe mapping"

A subtle link card footer shows "github.com/ElBruno/AotMapper" or similar. 
Modern, clean, exciting. Maximum visual impact in a 1200x675 space.
```

**Negative Prompt:**
```
no photorealism, no clutter, no overly complex designs, no readability issues, 
no tiny fonts, no poor contrast
```

**Notes:**
- This will be embedded in X/Twitter links
- Ensure text is bold and readable
- Icons should be simple and instantly recognizable
- Consider A/B testing multiple versions

---

### 4.2 LinkedIn Professional Announcement

**Platform:** DALL-E 3

**Dimensions:** 1200x627px

**Prompt:**
```
A professional, polished LinkedIn image announcing the ElBruno.AotMapper library. 
The background features a clean gradient from white to very light purple or blue, 
giving a professional tech feel. The left side shows the .NET logo or a clean gear/code icon 
in .NET purple.

Center text in a professional sans-serif font: "Introducing ElBruno.AotMapper" (large), 
followed by "An AOT-Safe DTO Mapping Solution for Modern .NET Applications" (medium), 
and "Now available on NuGet" (smaller, in a subtle call-to-action style).

The right side shows a minimalist graphic: a source code block transforming into a compiled 
output block, with a glowing blue arrow indicating the transformation. 

Overall aesthetic: Professional, clean, corporate tech, suitable for LinkedIn's professional audience. 
Plenty of whitespace.
```

**Negative Prompt:**
```
no photorealism, no cluttered design, no overly casual styling, no poor contrast, 
no tiny or unreadable text, no cheesy stock photos
```

**Notes:**
- LinkedIn's professional audience expects polish
- Include a clear CTA ("Learn More", "NuGet Link", etc.)
- Test colors for accessibility on LinkedIn's white background

---

### 4.3 LinkedIn Carousel - Slide 1: Overview

**Platform:** Midjourney

**Dimensions:** 1080x1080px

**Prompt:**
```
The first slide of a LinkedIn carousel explaining ElBruno.AotMapper. 
A clean, professional square image with the title "ElBruno.AotMapper" in large, bold text 
centered at the top in .NET purple. Below that, a subtitle: "AOT-Friendly DTO Mapping" 
on one line, "for Modern .NET" on the next.

The background is a subtle gradient from white to light purple. A minimalist icon showing 
two data structures (records/classes) on the left and right with a glowing blue arrow and 
transform symbol between them (suggesting mapping). 

At the bottom, a small badge or label: "Slide 1 of 5" or "Overview" in a smaller font. 
The overall design is clean, professional, and visually appealing.
```

**Negative Prompt:**
```
no photorealism, no 3D effects, no cluttered design, no poor readability, 
no overwhelming graphics, no irrelevant imagery
```

**Notes:**
- First slide sets the tone; keep it simple and intriguing
- Include slide counter so viewers know there's a series

---

### 4.4 LinkedIn Carousel - Slide 2: Key Features

**Platform:** DALL-E 3

**Dimensions:** 1080x1080px

**Prompt:**
```
A carousel slide highlighting the key features of ElBruno.AotMapper in a visually organized way. 
A white background with the title "Key Features" at the top in .NET purple.

Below, 4 feature boxes arranged in a 2x2 grid, each with an icon and label:
1. Lightning bolt icon + "Compile-Time Generation" (in electric blue)
2. Shield icon + "NativeAOT Compatible" (in purple)
3. Code brackets + "Type-Safe Mapping" (in blue)
4. Zap icon + "Zero Reflection Overhead" (in purple)

Each box has a subtle border or background color. The design is clean, modern, 
and professional. "Slide 2 of 5" at the bottom.
```

**Negative Prompt:**
```
no photorealism, no 3D effects, no cluttered boxes, no poor contrast, no tiny text, 
no overwhelming colors
```

**Notes:**
- Each feature should be instantly understandable with icon + label
- Use consistent icon style and colors
- Maintain professional appearance for LinkedIn

---

### 4.5 LinkedIn Carousel - Slide 3: Use Cases

**Platform:** Midjourney

**Dimensions:** 1080x1080px

**Prompt:**
```
A carousel slide showing real-world use cases for ElBruno.AotMapper. 
White background with "Use Cases" in large .NET purple text at the top.

Below, three illustrated scenarios in a vertical or staggered layout:
1. ASP.NET Core API → Icon showing API with DTO conversion → "API Contract Mapping"
2. Entity Framework → Code blocks → "Entity to DTO Mapping"
3. Microservices → Multi-arrow diagram → "Inter-Service Data Mapping"

Each scenario uses .NET purple and electric blue accents. Simple, clean illustrations. 
"Slide 3 of 5" at the bottom in gray text.
```

**Negative Prompt:**
```
no photorealism, no cluttered diagrams, no poor text contrast, no 3D rendering, 
no confusing arrows or connections
```

**Notes:**
- Should resonate with .NET developers' real problems
- Keep illustrations simple and clear
- Maintain visual consistency with Slides 1-2

---

### 4.6 LinkedIn Carousel - Slide 4: Getting Started

**Platform:** DALL-E 3

**Dimensions:** 1080x1080px

**Prompt:**
```
A carousel slide with a "Getting Started" workflow. White background, 
"Getting Started" title in .NET purple at the top.

A step-by-step process flow illustrated with 4 connected circles or boxes:
1. "Install" – NuGet package icon, electric blue
2. "Annotate" – Code file with annotation symbol, purple
3. "Build" – Compiler/gear icon, blue
4. "Use" – Running app or play button, purple

Simple arrow or line connecting each step, flowing left to right or in a circle. 
"Slide 4 of 5" at the bottom. Clean, instructional design.
```

**Negative Prompt:**
```
no photorealism, no 3D flow charts, no complex diagrams, no poor readability, 
no cluttered design, no tiny arrows or connectors
```

**Notes:**
- Should feel accessible and actionable
- Clear progression from install to usage
- Use consistent iconography

---

### 4.7 LinkedIn Carousel - Slide 5: CTA & Social Links

**Platform:** DALL-E 3

**Dimensions:** 1080x1080px

**Prompt:**
```
The final carousel slide with a strong call-to-action. Gradient background from 
.NET purple to electric blue. Large white text: "Ready to boost your .NET mapping?"

Below, prominent buttons or badges (design-only, not interactive):
- "View on GitHub" 
- "NuGet Package"
- "Read Docs"
- "Try Now"

Social links at the bottom: Twitter icon, LinkedIn icon, GitHub icon in white. 
A subtle ElBruno.AotMapper logo or gear icon in one corner. "Slide 5 of 5" and 
"Made by @ElBruno" at the very bottom. Professional, exciting, action-oriented.
```

**Negative Prompt:**
```
no photorealism, no poor contrast for text, no confusing layout, no 3D effects, 
no overly complex gradient, no small or illegible text
```

**Notes:**
- Final slide should encourage action
- Include links/CTAs viewers can reference
- Maintain brand colors throughout

---

### 4.8 Twitter/X Thread Header

**Platform:** DALL-E 3

**Dimensions:** 1200x675px

**Prompt:**
```
A visually striking header image for a Twitter/X thread about ElBruno.AotMapper. 
The background is a bold gradient from electric blue to .NET purple. 

In the center, a large compass or target icon in white, with code snippets or 
source-to-destination arrows radiating outward, suggesting direction/mapping. 
Bold white text reads "🧵 AOT Mapping Explained" or "🧵 Let's talk about ElBruno.AotMapper".

A small counter or badge in the top right: "📍 Thread" or "🔗 1/N". The overall feel 
is energetic, modern, and thread-friendly. Clean, punchy, immediately recognizable in feeds.
```

**Negative Prompt:**
```
no photorealism, no 3D effects, no cluttered design, no poor contrast, 
no tiny fonts, no abstract imagery that doesn't convey the topic
```

**Notes:**
- Must stand out in X/Twitter feed
- Text should be bold and readable at small sizes
- Consider multiple thread topics and variations

---

## 5. Conference/Presentation Assets

### 5.1 Session Title Slide (1920x1080)

**Platform:** Midjourney

**Dimensions:** 1920x1080px

**Prompt:**
```
A professional conference session title slide for a tech talk about ElBruno.AotMapper. 
The background is a modern geometric pattern: subtle triangles and lines in shades of 
.NET purple and electric blue on a dark background (dark blue or near-black), creating 
a sense of motion and technology.

In the center, large, bold white text:
"ElBruno.AotMapper"
"High-Performance DTO Mapping for AOT-Compiled .NET Applications"

Below, in a smaller font:
"@ElBruno"
"[Conference Name] [Year]"

In a bottom corner, the speaker's name and social handles (Twitter, GitHub) in white. 
A subtle .NET logo or gear icon in one corner. The overall aesthetic is professional, 
modern, and conference-ready. High contrast, readable from a distance.
```

**Negative Prompt:**
```
no photorealism, no 3D rendering, no cluttered design, no poor contrast, 
no small fonts, no clip art, no unprofessional styling
```

**Notes:**
- Slides will be projected; ensure text is large and readable
- High contrast between background and text
- Include speaker info for credibility
- Test on a projector before presenting

---

### 5.2 Feature Comparison Slide

**Platform:** DALL-E 3

**Dimensions:** 1920x1080px

**Prompt:**
```
A comparison slide for a presentation comparing "Reflection-Based Mapping" vs 
"AOT-Safe Mapping with ElBruno.AotMapper". 

The slide has a dark background with a vertical divider line in the middle. 
Left side (red/orange tint): "Traditional Reflection-Based Mapping" with icons showing:
- Clock/watch icon (slow at runtime)
- Crossed-out binary (incompatible with AOT/trimming)
- Memory chunks (high memory overhead)
- Code file with X mark (can cause JIT compilation)

Right side (green/blue tint): "ElBruno.AotMapper" with icons showing:
- Lightning bolt (compile-time generation)
- Checkmark + binary (AOT/trimming compatible)
- Minimal memory (efficient)
- Checkmark on code (no runtime surprises)

A bold central arrow or balance scale shows the comparison. Text is large and white. 
Professional, clear, and impactful.
```

**Negative Prompt:**
```
no photorealism, no 3D effects, no cluttered diagrams, no poor readability, 
no overly complex icons, no small or illegible text
```

**Notes:**
- This is a critical educational slide
- Use clear visual metaphors (lightning = fast, X = incompatible)
- Ensure the comparison is obvious from across a conference room
- Consider animating the slide to reveal each comparison point

---

### 5.3 Architecture Diagram Prompt

**Platform:** Midjourney

**Dimensions:** 1920x1080px

**Prompt:**
```
A clean, professional architecture diagram showing the ElBruno.AotMapper source generator 
pipeline. The slide background is dark (dark blue or charcoal) with a subtle gradient.

The diagram flows left to right and shows 4 main stages:

1. **Source Stage** (left): Code files (.cs) with annotations [Mapper] highlighted. 
   Icons: file icon, annotation symbol in electric blue.

2. **Generator Stage** (center-left): A gear or compiler icon with code flowing through it. 
   Label: "Roslyn Source Generator" in white text. Color: .NET purple.

3. **Generation Output** (center-right): Generated C# code blocks visible (syntax highlighted). 
   Label: "IL-safe mapping code" in white. Color: electric blue.

4. **Runtime** (right): Application/running process icon with lightning bolt. 
   Label: "Zero-reflection invocation" in white. Color: .NET purple.

Arrows flow between each stage in electric blue or white. The overall design is technical 
but clean, with good spacing and clear labels. All text in white, highly readable.
```

**Negative Prompt:**
```
no photorealism, no 3D rendering, no cluttered or overlapping elements, no poor contrast, 
no tiny text, no confusing flow, no irrelevant imagery
```

**Notes:**
- This is a key technical slide for dev-focused talks
- Ensure the pipeline flow is obvious (left to right)
- Consider this for both slides and technical blog posts
- May need animation to reveal each stage

---

## 6. Documentation Assets

### 6.1 Quick Start Flow Diagram

**Platform:** DALL-E 3

**Dimensions:** 1200x400px

**Prompt:**
```
A horizontal flow diagram for documentation showing the quick start process for 
ElBruno.AotMapper. Four clear, sequential steps with icons:

1. **Install** – Package icon or terminal with "dotnet add package ElBruno.AotMapper" 
   text, electric blue accents.

2. **Annotate** – Code file icon with [Mapper] attribute highlighted, .NET purple.

3. **Build** – Compiler/gear icon with checkmark, representing successful build, 
   electric blue.

4. **Use** – Running application icon or play button, .NET purple.

Clean white background, simple geometric icons, connecting arrows in electric blue. 
Text labels below each icon in a modern sans-serif font. Suitable for embedding in docs, 
GitHub README, and blog posts.
```

**Negative Prompt:**
```
no photorealism, no 3D effects, no cluttered layout, no poor text contrast, 
no overly complex icons, no unnecessary decoration
```

**Notes:**
- Must be clear and scannable
- Icons should be simple and universally understood
- Consider SVG format for web use
- This might be good to embed in multiple docs pages

---

### 6.2 Architecture Overview Diagram

**Platform:** Midjourney

**Dimensions:** 1200x800px

**Prompt:**
```
A comprehensive but clean architecture diagram for the ElBruno.AotMapper documentation. 
The background is white with subtle gray grid lines in the background.

The diagram shows:

**Input Layer** (top-left): Source code files with class definitions and [Mapper] attributes. 
Icons: C# file, annotation symbol.

**Compilation Layer** (center-top): Roslyn compiler with source generator plugin. 
Icon: gear/compiler symbol, arrows flowing into it from input.

**Generation Layer** (center): Generated code blocks showing the created mapper classes. 
Code snippets visible but not overwhelming.

**Output Layer** (bottom-right): Running .NET application using the generated mappers. 
Icons: application window, lightning bolt indicating performance.

**Features Callouts** (around the diagram):
- "Compile-time magic" – AOT safe
- "Type-safe" – Full IntelliSense support
- "Zero reflection" – Native AOT compatible
- "High performance" – Lightning bolt icon

Arrows flow logically through the diagram in electric blue. All text is clear and professional. 
The overall layout is organized and easy to understand.
```

**Negative Prompt:**
```
no photorealism, no 3D rendering, no overly cluttered diagrams, no poor flow, 
no tiny or unreadable text, no confusing arrows or connections
```

**Notes:**
- This is the "big picture" diagram
- Should appear in README and architecture docs
- Consider making an interactive version (SVG with hover tooltips)
- May be adapted for presentations

---

## 7. YouTube Thumbnail (1280x720)

**Platform:** Midjourney

**Dimensions:** 1280x720px

**Prompt:**
```
An eye-catching YouTube video thumbnail for a video titled "Building an AOT Mapper for .NET". 
The background is a bold gradient from .NET purple (#512BD4) to electric blue (#0078D4).

In the top-left corner, a large, bold text overlay: "AOT MAPPER" in white, high contrast. 
Below that, in slightly smaller text: ".NET Source Gen" or "Compile-Time Magic".

The center of the thumbnail features a large, dynamic lightning bolt icon in bright white, 
striking downward with a glowing effect, conveying speed and power. Overlaid on the lightning 
bolt is a subtle code bracket or gear symbol in the background, suggesting technical content.

In the bottom-right corner, a small speaker photo frame (if available) or just the 
@ElBruno handle/logo.

The overall design is high-energy, visually appealing, and thumb-stopping. 
Text is large and bold, readable at small sizes. No clutter, just impact.
```

**Negative Prompt:**
```
no photorealism, no 3D effects, no cluttered elements, no small fonts, 
no poor contrast, no generic stock thumbnails, no overly complex imagery
```

**Notes:**
- YouTube thumbnails must be eye-catching and readable at thumbnail size (100-150px wide)
- High contrast between background and foreground elements
- Test at actual YouTube thumbnail size before finalizing
- Consider A/B testing a few variations
- Ensure it works both light and dark YouTube backgrounds
- May need variations for different video topics (benchmarking, tutorials, etc.)

---

## Image Generation Tool Recommendations

| Tool | Best For | Pros | Cons |
|------|----------|------|------|
| **DALL-E 3** | Social cards, blog headers, quick prompts | Fast, intuitive, good for text overlays | Less consistent style, fewer fine details |
| **Midjourney** | Icons, technical diagrams, consistent branding | Excellent consistency, great for iterations, fine details | Requires subscription, learning curve |
| **Ideogram** | Text-heavy designs, precise typography | Best for text rendering, modern aesthetics | Limited in some technical concepts |
| **Adobe Firefly** | Quick variations, enterprise integration | Good for A/B testing, Adobe integration | Newer, less established |

---

## General Prompting Guidelines

1. **Color References**: Always include hex codes (#512BD4, #0078D4) in prompts for consistency
2. **Dimensions**: Specify required dimensions to avoid cropping surprises
3. **Negative Prompts**: Use "no photorealism, no 3D effects, no clutter" as baseline
4. **Iterations**: Generate 3-5 variations of each asset and have stakeholders choose
5. **Accessibility**: Always include alt text descriptions for web use
6. **Testing**: View all assets at intended sizes and backgrounds before finalizing
7. **Brand Consistency**: Use the same visual elements (lightning, gears, code brackets) across assets
8. **Whitespace**: Modern design needs breathing room; avoid cluttered compositions

---

## Post-Generation Tasks

1. **Export**: Save all images in multiple formats (PNG, WebP, JPG) and resolutions
2. **Optimization**: Compress for web without losing quality (use TinyPNG, ImageOptim)
3. **Icon Testing**: Test NuGet logo at 64x64, 128x128, and 256x256 sizes
4. **Brand Review**: Have @ElBruno or team review finalized assets
5. **Accessibility**: Add alt text to all web-deployed images
6. **Versioning**: Keep source prompts and generation parameters for future updates
7. **Archive**: Store original high-res versions (300 DPI for print if needed)

---

## Example Workflow

1. Generate 3-5 variations of each asset using recommended tool
2. Create a shared album (Google Drive, Figma, or similar) for team review
3. Get feedback and refine top choices with additional prompts
4. Export finalized images in required dimensions and formats
5. Place in appropriate folders (`images/`, `docs/assets/`, etc.)
6. Test in intended contexts (README, social media, blog)
7. Document final asset URLs and paths in team resources

---

*Last Updated: 2024*  
*For questions or updates to these prompts, contact @ElBruno*
