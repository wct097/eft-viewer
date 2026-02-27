# Third-Party License Notices

This document records the licenses for all third-party libraries used by
EFT Viewer. All dependencies must use permissive licenses (MIT, BSD,
Apache 2.0, ISC). GPL-licensed and revenue-gated dependencies are prohibited.

For dual-licensed libraries (e.g., GPL **or** MIT), the permissive license
must be elected and documented here.

---

## Dependency Licensing Policy

EFT Viewer is licensed under the MIT License. To preserve license
compatibility and avoid copyleft obligations:

1. **Prohibited**: GPL, AGPL, SSPL, or any copyleft-licensed dependencies
2. **Prohibited**: Revenue-gated or threshold-licensed dependencies (e.g.,
   "free only below $X revenue")
3. **Required**: All dependencies must use permissive licenses (MIT, BSD,
   Apache 2.0, ISC, PostgreSQL License, or similar)
4. **Dual-licensed**: Must elect the permissive license and document the
   election in this file

---

## Core Library Dependencies (EftViewer.Core)

### 1. System.Memory 4.6.3

| Field | Value |
|-------|-------|
| **Author** | Microsoft |
| **Website** | https://github.com/dotnet/runtime |
| **License** | MIT |
| **Package** | `System.Memory` v4.6.3 |
| **Modifications** | None |
| **Copyright** | Copyright (c) .NET Foundation and Contributors |

---

## Desktop Application Dependencies (EftViewer.Desktop)

### 2. Avalonia 11.3.11

| Field | Value |
|-------|-------|
| **Author** | AvaloniaUI OÜ |
| **Website** | https://github.com/AvaloniaUI/Avalonia |
| **License** | MIT |
| **Packages** | `Avalonia` v11.3.11, `Avalonia.Desktop` v11.3.11, `Avalonia.Themes.Fluent` v11.3.11, `Avalonia.Fonts.Inter` v11.3.11, `Avalonia.Diagnostics` v11.3.11 |
| **Modifications** | None |
| **Copyright** | Copyright (c) .NET Foundation and Contributors |

### 3. CommunityToolkit.Mvvm 8.2.1

| Field | Value |
|-------|-------|
| **Author** | Microsoft |
| **Website** | https://github.com/CommunityToolkit/dotnet |
| **License** | MIT |
| **Package** | `CommunityToolkit.Mvvm` v8.2.1 |
| **Modifications** | None |
| **Copyright** | Copyright (c) .NET Foundation and Contributors |

---

## Test Dependencies (EftViewer.Core.Tests)

### 4. xunit 2.9.2

| Field | Value |
|-------|-------|
| **Author** | .NET Foundation |
| **Website** | https://github.com/xunit/xunit |
| **License** | Apache 2.0 |
| **Packages** | `xunit` v2.9.2, `xunit.runner.visualstudio` v2.8.2 |
| **Modifications** | None |
| **Copyright** | Copyright (c) .NET Foundation and Contributors |

### 5. Microsoft.NET.Test.Sdk 17.12.0

| Field | Value |
|-------|-------|
| **Author** | Microsoft |
| **Website** | https://github.com/microsoft/vstest |
| **License** | MIT |
| **Package** | `Microsoft.NET.Test.Sdk` v17.12.0 |
| **Modifications** | None |
| **Copyright** | Copyright (c) Microsoft Corporation |

### 6. coverlet.collector 6.0.2

| Field | Value |
|-------|-------|
| **Author** | tonerdo |
| **Website** | https://github.com/coverlet-coverage/coverlet |
| **License** | MIT |
| **Package** | `coverlet.collector` v6.0.2 |
| **Modifications** | None |
| **Copyright** | Copyright (c) 2018 Toni Solarin-Sodara |

---

## Dual-Licensed Libraries -- Permissive License Elected

_No dual-licensed libraries are currently in use._

When a dual-licensed library is added, document the election here following
this format:

| Field | Value |
|-------|-------|
| **Library** | (name and version) |
| **Available licenses** | (e.g., MIT, GPL-2.0) |
| **Elected license** | **MIT** |
| **Reason** | GPL is incompatible with project licensing policy |

---

## Election Record

| Date | Action | Authorized By |
|------|--------|---------------|
| 2026-02-27 | Initial third-party license audit for all NuGet dependencies | William Tyler |

---

*This document is part of the license compliance effort for EFT Viewer.
See `.github/blocked-packages.json` for the automated enforcement blocklist.*
