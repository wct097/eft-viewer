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

**Branch Strategy:**
- `main` - Buildable milestones only; represents stable releases
- `develop` - Buildable incremental code; integration branch for ongoing work
- Work branches - Cut from `develop` for features, fixes, etc.

**Merge Strategy:**
- Work branches → `develop`: **Squash merge** via pull request
- `develop` → `main`: **Regular merge** (no squash) to preserve milestone history

**Workflow:**
1. Create work branch from `develop`
2. Develop and commit freely on work branch
3. Open PR to `develop`, squash merge when ready
4. Periodically merge `develop` into `main` for milestones

**Commands:**
- Use `/save` command for AI-enhanced commits
- Descriptive commit messages

## Important Context

- WSQ (Wavelet Scalar Quantization) is FBI's fingerprint compression standard (~15:1 ratio)
- NIST SP 500-290 defines the ANSI/NIST-ITL standard
- Control characters: GS (0x1D), RS (0x1E), US (0x1F), FS (0x1C) as separators
- Project is in early development (MVP phase)

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
