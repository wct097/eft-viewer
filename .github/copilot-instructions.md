# GitHub Copilot Instructions

This document provides context and guidelines for GitHub Copilot when assisting with this repository.

## Project Context

**EFT Viewer** is an open-source, cross-platform viewer for Electronic Fingerprint
Transmission (EFT) files based on the ANSI/NIST-ITL biometric data interchange standard.

- **Tech Stack**: C# / .NET 8, Avalonia UI, .NET Standard 2.0 (core library)
- **License**: MIT
- See `CLAUDE.md` for full project architecture and guidelines.

## Coding Standards

- Follow C# coding conventions
- Use .NET Standard 2.0 for core library (maximum compatibility)
- Avalonia MVVM patterns for UI
- Follow existing code patterns and conventions

## Dependency Licensing

This is an MIT-licensed open-source project. All dependencies must use permissive licenses.

- **Prohibited**: GPL, AGPL, SSPL, or any copyleft-licensed dependencies
- **Prohibited**: Revenue-gated or threshold-licensed dependencies (e.g., "free under $X revenue")
- **Required**: Only permissive licenses (MIT, BSD, Apache 2.0, ISC)
- **Dual-licensed**: Elect the permissive license and document in `THIRD-PARTY-LICENSES.md`
- **Before adding**: Check `.github/blocked-packages.json` for blocked packages

## Git Workflow

- ALL pull requests target `develop`, NEVER `main` directly
- Feature/fix branches are created FROM `develop`
- Use squash merges into `develop`, regular merges from `develop` to `main`

## Testing Requirements

- Unit tests for parser logic
- Integration tests with sample EFT files
- All new code should include appropriate tests

## Security Guidelines

- Never hardcode credentials
- Validate all inputs
- Follow OWASP guidelines
