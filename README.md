# Dreamine.PLC.Wpf

This package provides WPF monitoring and diagnostic UI components for the Dreamine PLC communication stack.

## Purpose

`Dreamine.PLC.Wpf` is part of the Dreamine PLC package family.

The package is designed to keep PLC communication code separated by responsibility:

- Abstractions define contracts.
- Core provides shared runtime infrastructure.
- Vendor adapters implement device-specific communication.
- WPF provides monitoring and diagnostic UI components.

## Features

- PLC connection status monitoring UI boundary
- PLC memory/device watch panel structure
- Diagnostic view model foundation
- WPF-friendly dispatcher integration point
- UI components separated from vendor-specific PLC adapters


## Project References

- `Dreamine.PLC.Abstractions`
- `Dreamine.PLC.Core`

## Target Framework

```xml
<TargetFramework>net8.0-windows</TargetFramework>
```

## Package Metadata

| Item | Value |
|---|---|
| PackageId | `Dreamine.PLC.Wpf` |
| Version | `1.0.0` |
| License | `MIT` |
| Repository | `https://github.com/CodeMaru-Dreamine/Dreamine.PLC.Wpf` |
| Project URL | `https://github.com/CodeMaru-Dreamine/Dreamine.PLC.FullKit` |

## Architecture Rule

This repository must not reference application-level projects.

Dependency direction must remain one-way:

```text
Abstractions
    ▲
    │
Core
    ▲
    │
Vendor Adapter / WPF UI Component
```

## License

This project is licensed under the MIT License.
