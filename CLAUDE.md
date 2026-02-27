# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Overview

**Project Name**: EFT Viewer
**Purpose**: Open-source, cross-platform viewer for Electronic Fingerprint Transmission (EFT) files based on the ANSI/NIST-ITL biometric data interchange standard
**Tech Stack**: C# / .NET 8, Avalonia UI, .NET Standard 2.0 (core library)

## Key Architecture

- **Core Parser Library** (.NET Standard 2.0): Shared library for parsing EFT/ANSI-NIST-ITL files
- **Desktop UI** (Avalonia UI / .NET 8): Cross-platform desktop application (Windows, macOS, Linux)
- **WSQ Codec**: NBIS-based bindings for FBI wavelet compression
- **Future**: .NET MAUI for mobile (Android/iOS), Python CLI for scripting

### EFT File Structure
- Record-based format with tagged fields (`Type.Field:Value`)
- Type-1: Transaction information (required)
- Type-2: User-defined descriptive text
- Type-4/14: Fingerprint images (WSQ compressed)
- Type-10: Face, SMT images
- Type-15: Palmprint images

## Development Guidelines

### Code Style
- Follow C# coding conventions
- Use .NET Standard 2.0 for core library (maximum compatibility)
- Avalonia MVVM patterns for UI

### Testing Approach
- Unit tests for parser logic
- Integration tests with sample EFT files
- UI tests for Avalonia components

### Git Workflow

```
╔═══════════════════════════════════════════════════════════════════════════════╗
║  MANDATORY RULES - VIOLATION CAUSES REPOSITORY CORRUPTION                     ║
╠═══════════════════════════════════════════════════════════════════════════════╣
║  1. ALL pull requests target `develop` - NEVER target `main` directly         ║
║  2. ALL feature/fix branches are created FROM `develop`                       ║
║  3. ONLY `develop` merges into `main` (for releases)                          ║
║  4. NEVER delete the `develop` branch                                         ║
║  5. NEVER squash merge into `main` - use regular merge only                   ║
╚═══════════════════════════════════════════════════════════════════════════════╝
```

**Branch Structure:**
```
main (protected)     ← Only receives merges from develop, NEVER direct PRs
  │
  └── develop        ← ALL PRs target here, NEVER delete this branch
        │
        ├── feature/xyz   ← Branch from develop
        ├── fix/xyz       ← Branch from develop
        └── chore/xyz     ← Branch from develop
```

**Merge Strategy:**
| Source | Target | Merge Type | PR Required |
|--------|--------|------------|-------------|
| feature/* | develop | **Squash merge** | Yes |
| fix/* | develop | **Squash merge** | Yes |
| develop | main | **Regular merge** (NO squash) | Yes |

**Creating a Branch:**
```bash
git checkout develop
git pull origin develop
git checkout -b feature/my-feature   # Always branch from develop
```

**Creating a PR:**
```bash
git push -u origin feature/my-feature
gh pr create --base develop          # ALWAYS --base develop
```

**Releasing to Main:**
```bash
# Create PR from develop to main (regular merge, NOT squash)
gh pr create --base main --head develop --title "Release vX.Y.Z"
# After merge, tag the release
git checkout main && git pull
git tag vX.Y.Z && git push origin vX.Y.Z
```

See [docs/guides/git-workflow.md](docs/guides/git-workflow.md) for complete documentation.

## Dependency Licensing

This is an MIT-licensed open-source project. GPL-licensed dependencies are
prohibited -- they impose copyleft obligations that are incompatible with the
project's permissive licensing model. Revenue-gated or threshold-licensed
dependencies (e.g., "Community" licenses that are free only below a revenue
threshold) are also prohibited -- they create unpredictable cost exposure for
downstream users.

**Rules:**
- Only use dependencies with permissive licenses (MIT, BSD, Apache 2.0, ISC)
- For dual-licensed libraries (e.g., GPL **or** MIT), elect the permissive
  license and document the election in `THIRD-PARTY-LICENSES.md`
- Document all new dependencies in `THIRD-PARTY-LICENSES.md`
- Check `.github/blocked-packages.json` before adding any dependency
- When in doubt about a license, flag it for human review

## Important Context

- WSQ (Wavelet Scalar Quantization) is FBI's fingerprint compression standard (~15:1 ratio)
- NIST SP 500-290 defines the ANSI/NIST-ITL standard
- Control characters: GS (0x1D), RS (0x1E), US (0x1F), FS (0x1C) as separators
- Project is in active development (v0.1.0 released)

## AI Assistant Instructions

When assisting with this project:
1. Follow existing patterns and conventions
2. Ensure all new code includes appropriate tests
3. Use descriptive commit messages
4. Consider performance and security implications
5. Document complex logic

## Do's and Don'ts

**Do:**
- ✅ Read relevant code before making changes
- ✅ Ask clarifying questions when requirements are unclear
- ✅ Suggest improvements while respecting existing patterns
- ✅ Write comprehensive tests

**Don't:**
- ❌ Make breaking changes without discussion
- ❌ Skip error handling
- ❌ Ignore performance implications
- ❌ Add dependencies without justification
- ❌ Add GPL-licensed, AGPL-licensed, or revenue-gated dependencies (permissive licenses only)
