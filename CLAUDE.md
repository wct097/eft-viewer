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

This project follows **GitHub Flow**: a single long-lived `main` branch with short-lived topic branches.

```
╔═══════════════════════════════════════════════════════════════════════════════╗
║  MANDATORY RULES                                                              ║
╠═══════════════════════════════════════════════════════════════════════════════╣
║  1. `main` is the only long-lived branch                                      ║
║  2. ALL work happens on short-lived `feature/*`, `fix/*`, `chore/*` branches  ║
║  3. ALL pull requests target `main`                                           ║
║  4. ALL merges into `main` are **squash merges**                              ║
║  5. Delete the topic branch (local + remote) after the PR merges              ║
╚═══════════════════════════════════════════════════════════════════════════════╝
```

**Branch Structure:**
```
main (protected, only long-lived branch)
  │
  ├── feature/xyz   ← Branch from main, PR back to main, delete after merge
  ├── fix/xyz       ← Branch from main, PR back to main, delete after merge
  └── chore/xyz     ← Branch from main, PR back to main, delete after merge
```

**Merge Strategy:**
| Source | Target | Merge Type | PR Required |
|--------|--------|------------|-------------|
| feature/* | main | **Squash merge** | Yes |
| fix/* | main | **Squash merge** | Yes |
| chore/* | main | **Squash merge** | Yes |

**Creating a Branch:**
```bash
git checkout main
git pull origin main
git checkout -b feature/my-feature   # Always branch from main
```

**Creating a PR:**
```bash
git push -u origin feature/my-feature
gh pr create --base main             # All PRs target main
```

**After the PR merges:**
```bash
git checkout main
git pull origin main
git branch -d feature/my-feature             # delete locally
git push origin --delete feature/my-feature  # delete on remote (if not auto-deleted)
```

**Releasing:**
```bash
# Tag directly from main after the release PR merges
git checkout main && git pull
git tag vX.Y.Z && git push origin vX.Y.Z
```

See [docs/guides/git-workflow.md](docs/guides/git-workflow.md) for complete documentation.

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
