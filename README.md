# EFT Viewer

An open-source, cross-platform viewer for Electronic Fingerprint Transmission (EFT) files based on the ANSI/NIST-ITL biometric data interchange standard.

## Overview

EFT files are used by the ATF, FBI, and other agencies for electronic fingerprint submission. This tool allows users to open, parse, and view the contents of EFT files including:

- Transaction information (Type-1 records)
- Descriptive/demographic data (Type-2 records)
- Fingerprint images (Type-4, Type-14 records) with WSQ decompression
- Additional biometric data records

## Project Status

🚧 **Early Development**

### MVP Goals

- [ ] Open and parse EFT file structure
- [ ] Display record hierarchy and metadata fields
- [ ] Decode and display WSQ-compressed fingerprint images
- [ ] Windows desktop application

### Future Goals

- [ ] macOS support
- [ ] Android/iOS mobile apps
- [ ] Field validation against ANSI/NIST-ITL spec
- [ ] Export capabilities

## Technology Stack

| Component | Technology | Notes |
|-----------|------------|-------|
| Core Parser | C# / .NET Standard 2.0 | Shared library, broad compatibility |
| Desktop UI | Avalonia UI / .NET 8 | Cross-platform (Windows, macOS, Linux) |
| Mobile (future) | .NET MAUI | Android/iOS |
| WSQ Codec | NBIS-based bindings | FBI wavelet compression standard |
| Alt CLI | Python | Scripting/automation use cases |

### Why These Choices

**Avalonia UI over MAUI for desktop**: More mature cross-platform desktop support today. MAUI excels at mobile but Avalonia provides better Windows/macOS/Linux parity for desktop apps.

**.NET Standard 2.0 for core library**: Maximum compatibility across runtimes. The parser can be consumed by desktop, mobile, or even older .NET Framework projects if needed.

## EFT / ANSI-NIST-ITL Format Reference

The ANSI/NIST-ITL standard (NIST Special Publication 500-290) defines a record-based format for biometric data interchange.

### Record Types

| Type | Description |
|------|-------------|
| 1 | Transaction information (required, exactly one) |
| 2 | User-defined descriptive text |
| 4 | Grayscale fingerprint image (legacy, 8-bit) |
| 10 | Face, SMT (scars/marks/tattoos) image |
| 14 | Variable-resolution fingerprint image (current standard) |
| 15 | Variable-resolution palmprint image |

### Structure

Records contain tagged fields in the format `Type.Field:Value`. Fields are separated by control characters:

| Separator | Hex | Purpose |
|-----------|-----|---------|
| GS | 0x1D | Group separator (between fields) |
| RS | 0x1E | Record separator (between subfields) |
| US | 0x1F | Unit separator (between information items) |
| FS | 0x1C | File separator (end of record) |

### Image Encoding

Fingerprint images in Type-4 and Type-14 records are typically WSQ compressed (Wavelet Scalar Quantization), an FBI-developed format optimized for fingerprint ridge detail at ~15:1 compression.

**WSQ Resources:**
- NIST Biometric Image Software (NBIS): https://www.nist.gov/services-resources/software/nist-biometric-image-software-nbis
- Contains reference `dwsq` (decode) and `cwsq` (encode) implementations in C

## Building from Source

*Build instructions will be added as implementation progresses.*

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or VS Code with C# Dev Kit

## AI-Assisted Development

This project includes AI development tooling for Claude Code and other AI assistants.

### Quick Start

```bash
# Load project context at start of session
/prime

# Create AI-enhanced commits
/save

# Validate repository structure
/clean
```

### Structure

- `ai_docs/` - AI documentation and best practices
- `.claude/commands/` - Claude Code slash commands
- `CLAUDE.md` - Project-specific AI guidance

See `ai_docs/README.md` for detailed documentation.

## License

Copyright 2026 William Tyler

Licensed under the Apache License, Version 2.0. See [LICENSE](LICENSE) for details.
