# ADR-001: MVP Development Plan

**Status**: Accepted
**Date**: 2026-01-31
**Author**: Claude (AI) with Will Tyler

## Context

EFT Viewer is a new project to parse and display Electronic Fingerprint Transmission (EFT) files based on the ANSI/NIST-ITL standard (NIST SP 500-290). This document establishes the development plan for the MVP.

## Decision

We will build the MVP in two phases with a layered architecture prioritizing testability and cross-platform compatibility.

---

## Architecture Overview

```
EftViewer.sln
├── src/
│   ├── EftViewer.Core/          # .NET Standard 2.0 - Parser library
│   └── EftViewer.Desktop/       # .NET 8 - Avalonia UI application
└── tests/
    └── EftViewer.Core.Tests/    # xUnit test project
```

### Technology Choices

| Component | Technology | Rationale |
|-----------|------------|-----------|
| Core Parser | .NET Standard 2.0 | Maximum compatibility across runtimes |
| Desktop UI | Avalonia UI / .NET 8 | Mature cross-platform desktop support |
| MVVM | CommunityToolkit.Mvvm | Source generators, minimal boilerplate |
| Testing | xUnit | Industry standard, good tooling |
| WSQ Decode | AFIS.WSQ (deferred) | Best available C# wrapper for NBIS |

---

## Phase 1: Core Parser (EftViewer.Core)

### 1.1 Project Scaffolding

```bash
# From repo root
dotnet new sln -n EftViewer

# Core library
dotnet new classlib -n EftViewer.Core -o src/EftViewer.Core -f netstandard2.0
dotnet sln add src/EftViewer.Core
cd src/EftViewer.Core && dotnet add package System.Memory && cd ../..

# Test project
dotnet new xunit -n EftViewer.Core.Tests -o tests/EftViewer.Core.Tests
dotnet sln add tests/EftViewer.Core.Tests
cd tests/EftViewer.Core.Tests && dotnet add reference ../../src/EftViewer.Core && cd ../..
```

### 1.2 Data Models

```
EftViewer.Core/
├── Models/
│   ├── EftFile.cs              # Root container
│   ├── EftRecord.cs            # Base record class
│   ├── TransactionRecord.cs    # Type-1 record
│   ├── DescriptiveRecord.cs    # Type-2 record
│   ├── FingerprintRecord.cs    # Type-4/14 records
│   └── EftField.cs             # Field with tag and value
├── Parsing/
│   ├── EftParser.cs            # Main parser entry point
│   ├── RecordParser.cs         # Record-level parsing
│   └── FieldParser.cs          # Field-level parsing
├── Constants/
│   └── Separators.cs           # GS, RS, US, FS constants
├── Exceptions/
│   └── EftParseException.cs    # Parsing errors
└── Metadata/
    └── FieldDictionary.cs      # Tag → human-readable name mapping
```

### 1.3 Parsing Requirements

**Separator handling:**
- GS (0x1D) - Group separator (between fields)
- RS (0x1E) - Record separator (between subfields)
- US (0x1F) - Unit separator (between information items)
- FS (0x1C) - File separator (end of record)

**Record types to parse:**
- Type-1: Transaction information (required, exactly one)
- Type-2: User-defined descriptive text
- Type-4: Grayscale fingerprint image (legacy)
- Type-14: Variable-resolution fingerprint image

**Graceful degradation:**
- Unknown record types: Parse as generic EftRecord, flag as unsupported
- Malformed data: Capture parse position, continue where possible
- Binary fields: Store as byte[] without interpretation initially

### 1.4 Testing Strategy

**Synthetic test file:**
Create `tests/EftViewer.Core.Tests/TestData/synthetic.eft` containing:
- Valid Type-1 record with minimal required fields
- Type-2 record with sample demographic data
- Type-14 record with placeholder binary data (not real WSQ)

**Test cases:**
- Parse valid minimal file
- Parse file with multiple records
- Handle missing required fields
- Handle unknown record types
- Handle truncated/corrupted data
- Field value extraction accuracy

---

## Phase 2: Desktop UI (EftViewer.Desktop)

### 2.1 Project Scaffolding

```bash
# Install Avalonia templates if needed
dotnet new install Avalonia.Templates

# Create Avalonia app
dotnet new avalonia.app -n EftViewer.Desktop -o src/EftViewer.Desktop -f net8.0
dotnet sln add src/EftViewer.Desktop
cd src/EftViewer.Desktop
dotnet add reference ../EftViewer.Core
dotnet add package CommunityToolkit.Mvvm
cd ../..
```

### 2.2 UI Structure

```
EftViewer.Desktop/
├── App.axaml                   # Application entry
├── ViewModels/
│   ├── MainWindowViewModel.cs  # Main orchestrator
│   ├── RecordViewModel.cs      # Tree node for records
│   └── FieldViewModel.cs       # Tree node for fields
├── Views/
│   ├── MainWindow.axaml        # Main window layout
│   └── DetailView.axaml        # Right panel detail view
├── Services/
│   ├── IFileDialogService.cs   # Abstraction for testability
│   └── FileDialogService.cs    # Platform implementation
├── Converters/
│   └── ImageConverter.cs       # Fingerprint display (placeholder initially)
└── Features/
    └── ImageExporter.cs        # PNG export (nice-to-have)
```

### 2.3 UI Layout

```
┌─────────────────────────────────────────────────────────────┐
│ File   Help                                                 │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─────────────────┐  ┌───────────────────────────────────┐ │
│  │ EftFile         │  │ Detail View                       │ │
│  │ ├─ Type-1       │  │                                   │ │
│  │ │  ├─ 1.001     │  │ Field: 2.001                      │ │
│  │ │  ├─ 1.002     │  │ Tag: LEN                          │ │
│  │ │  └─ ...       │  │ Value: 00000150                   │ │
│  │ ├─ Type-2       │  │                                   │ │
│  │ │  ├─ 2.001     │  │ [For fingerprint records:         │ │
│  │ │  └─ ...       │  │  Display image or placeholder]    │ │
│  │ └─ Type-14      │  │                                   │ │
│  │    └─ ...       │  │                                   │ │
│  └─────────────────┘  └───────────────────────────────────┘ │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│ Status: sample.eft | 3 records | No warnings               │
└─────────────────────────────────────────────────────────────┘
```

### 2.4 MVVM Architecture

**MainWindowViewModel responsibilities:**
- File open command (via IFileDialogService)
- Hold current EftFile instance
- Manage tree selection state
- Expose selected item details

**Binding approach:**
- Use CommunityToolkit.Mvvm source generators
- `[ObservableProperty]` for bindable properties
- `[RelayCommand]` for commands

---

## Phase 3: WSQ Integration (Fast-Follow)

### Decision: Defer WSQ to Post-MVP

**Rationale:**
- No production-ready NuGet package exists
- Best option (AFIS.WSQ) requires native library integration
- Core parsing and UI can be validated with placeholder images
- Reduces MVP scope and risk

**Planned approach:**
1. Evaluate [AFIS.WSQ](https://github.com/alainrc2005/AFIS.WSQ) wrapper
2. If suitable: integrate as optional native dependency
3. If not: wrap NBIS `dwsq` directly via P/Invoke
4. Graceful fallback: show image dimensions when decode unavailable

**Placeholder implementation:**
- Display "Fingerprint Image" label with extracted metadata
- Show image dimensions if available in record
- Show compression type field value

---

## CI/CD Strategy

### Develop Branch (Fast)

**Trigger:** Push to `develop`, PR to `develop`
**Goal:** < 2 minutes
**File:** `.github/workflows/build.yml`

```yaml
- Restore packages
- Build solution (Release)
- Run unit tests
```

### Main Branch (Comprehensive)

**Trigger:** PR to `main`
**Goal:** < 5 minutes
**File:** `.github/workflows/release.yml`

```yaml
- All of above, plus:
- Build on multiple platforms (ubuntu, windows, macos)
- Code coverage report
- Create draft release artifacts (optional)
```

---

## Definition of Done (MVP)

- [ ] Solution scaffolded with all projects
- [ ] EftParser reads Type-1, Type-2, Type-4, Type-14 records
- [ ] Field dictionary maps common tags to human-readable names
- [ ] Synthetic test file created and committed
- [ ] Unit tests pass for parser
- [ ] Avalonia app opens EFT file via File > Open
- [ ] TreeView displays record/field hierarchy
- [ ] Detail view shows field information with human-readable names
- [ ] Fingerprint records show placeholder (WSQ deferred)
- [ ] Status bar shows file info and warnings
- [ ] Builds with no warnings on Windows
- [ ] CI workflow validates builds

### Nice-to-Have (MVP)
- [ ] PNG export for fingerprint images (requires WSQ)

---

## Work Sequence

### Sprint 1: Foundation
1. Scaffold solution structure
2. Implement data models
3. Implement field/record parsing
4. Create synthetic test file
5. Write parser unit tests

### Sprint 2: UI
6. Scaffold Avalonia app
7. Implement MainWindowViewModel
8. Build TreeView with record hierarchy
9. Build detail view panel
10. Add status bar

### Sprint 3: Polish
11. Error handling and partial parse recovery
12. CI/CD workflows
13. README updates with build instructions
14. Manual testing with real EFT file

### Sprint 4: WSQ (Post-MVP)
15. Evaluate and integrate WSQ decoder
16. Update fingerprint display
17. Additional record type support

---

## Resolved Questions

1. **Type-4 vs Type-14 priority**: Prioritize Type-14 (current standard, used by ATF eForm). Support Type-4 as fallback.
2. **Field definitions**: Yes, include a field dictionary mapping tag numbers to human-readable names (e.g., `1.002` → "Version Number"). Start with common fields, expand over time.
3. **Export functionality**: PNG export is nice-to-have for MVP. Add context menu on fingerprint records once WSQ decoding works.

---

## References

- [NIST SP 500-290](https://www.nist.gov/publications/data-format-interchange-fingerprint-facial-other-biometric-information-ansinist-itl-1) - ANSI/NIST-ITL standard
- [NBIS Software](https://www.nist.gov/services-resources/software/nist-biometric-image-software-nbis) - Reference WSQ implementation
- [AFIS.WSQ](https://github.com/alainrc2005/AFIS.WSQ) - C# WSQ wrapper
- [Avalonia Documentation](https://docs.avaloniaui.net/)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
