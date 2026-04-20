# Project Context

## Project Overview
**Project Name**: EFT Viewer
**Description**: Open-source, cross-platform viewer for Electronic Fingerprint Transmission (EFT) files based on the ANSI/NIST-ITL biometric data interchange standard (NIST SP 500-290).
**Repository**: https://github.com/wct097/eft-viewer

## Technology Stack
- **Language**: C# (.NET 8 for the desktop app, .NET Standard 2.0 for the core parser library)
- **UI Framework**: Avalonia UI (MVVM, cross-platform — Windows, macOS, Linux)
- **Imaging**: NBIS-based WSQ codec bindings for FBI wavelet compression
- **Testing**: xUnit
- **Database**: none

## Architecture

### Project Structure
```
src/
├── EftViewer.Core/       # Parser library (.NET Standard 2.0)
└── EftViewer.Desktop/    # Avalonia UI application (.NET 8)
tests/
└── EftViewer.Core.Tests/ # Unit tests (xUnit)
docs/                     # Architecture, guides, feature tracking
samples/                  # Synthetic EFT files for testing
scripts/                  # Validation, code review, git author tooling
```

### Key Decisions
- Core parser targets **.NET Standard 2.0** for maximum portability (future MAUI/Xamarin/Python reuse).
- Desktop app uses Avalonia (MVVM) so a single codebase targets Windows/macOS/Linux.
- WSQ decompression uses NBIS bindings rather than a managed reimplementation.
- Record-based EFT format parsed with tagged fields (`Type.Field:Value`), control characters `GS (0x1D)`, `RS (0x1E)`, `US (0x1F)`, `FS (0x1C)` as separators.

## Development Workflow

### Common Commands
```bash
dotnet restore EftViewer.sln
dotnet build EftViewer.sln --configuration Release
dotnet test tests/EftViewer.Core.Tests/EftViewer.Core.Tests.csproj
dotnet run --project src/EftViewer.Desktop/EftViewer.Desktop.csproj
```

### Branch Strategy
- **main**: the only long-lived branch; protected, production-ready
- **feature/\*, fix/\*, chore/\***: short-lived topic branches; squash-merged into `main` via PR and deleted after merge

See [docs/guides/git-workflow.md](guides/git-workflow.md) for the full workflow.

## Coding Standards
- Follow standard C# conventions (PascalCase for types/methods, camelCase for locals/params).
- Core library code must stay compatible with .NET Standard 2.0 (no .NET-only APIs).
- Use Avalonia MVVM patterns for UI code (ViewModels under `ViewModels/`, views under `Views/`).
- All new parser logic requires unit tests in `tests/EftViewer.Core.Tests/`.

## Important Context for AI Tools
- **EFT file format** is record-based with tagged fields. Type-1 (transaction info) is required; Type-4/14 contain fingerprint images (WSQ compressed); Type-10 contains face/SMT images; Type-15 contains palmprints.
- **WSQ** is the FBI's wavelet compression standard (~15:1 ratio). Decoding uses NBIS bindings under `src/EftViewer.Core/Imaging/`.
- **Do not** add dependencies to the core library that aren't .NET Standard 2.0 compatible.
- **Sample files** under `samples/` are synthetic and safe to commit; never commit real biometric data.
- **Security**: this viewer handles sensitive biometric data. Treat all file inputs as untrusted; fail closed on malformed records.

---
*Last updated: 2026-04-20*
