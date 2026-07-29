# Dreamine.PLC.Wpf

[![CI](https://github.com/CodeMaru-Dreamine/Dreamine.PLC.Wpf/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/CodeMaru-Dreamine/Dreamine.PLC.Wpf/actions/workflows/ci.yml)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=CodeMaru-Dreamine_Dreamine.PLC.Wpf&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CodeMaru-Dreamine_Dreamine.PLC.Wpf)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=CodeMaru-Dreamine_Dreamine.PLC.Wpf&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=CodeMaru-Dreamine_Dreamine.PLC.Wpf)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CodeMaru-Dreamine_Dreamine.PLC.Wpf&metric=coverage)](https://sonarcloud.io/summary/new_code?id=CodeMaru-Dreamine_Dreamine.PLC.Wpf)

[![License](https://img.shields.io/badge/license-MIT-2496ED.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8-512BD4.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![NuGet](https://img.shields.io/nuget/v/Dreamine.PLC.Wpf.svg)](https://www.nuget.org/packages/Dreamine.PLC.Wpf)
[![Downloads](https://img.shields.io/nuget/dt/Dreamine.PLC.Wpf.svg)](https://www.nuget.org/packages/Dreamine.PLC.Wpf)

[![Docs](https://img.shields.io/badge/%F0%9F%93%98%20Docs-dreamine.kr-2496ED)](https://dreamine.kr/libraries?lang=en)
[![Guide](https://img.shields.io/badge/%F0%9F%93%98%20Guide-dreamine.kr-2496ED)](https://dreamine.kr/guide?lang=en)
[![Playground](https://img.shields.io/badge/%F0%9F%8E%AE%20Playground-dreamine.kr-7B2CBF)](https://dreamine.kr/playground?lang=en)
[![Book](https://img.shields.io/badge/%F0%9F%93%96%20Book-Practical%20MVVM%20Architecture-black)](https://bookk.co.kr/bookStore/69c0f1b41461ec1ae849a0f6)

[Korean documentation](./README_KO.md)

WPF monitoring and diagnostic UI components for Dreamine PLC communication.

This package provides a reusable PLC monitor view that can bind to any `IPlcClient` implementation from the Dreamine PLC package family.

## Features

- PLC connection state display
- Selectable client binding flow
- Bit read/write diagnostics
- Word read/write diagnostics
- Operation log display
- Faulted/disconnected state handling
- Reusable WPF monitor control

## Supported client types through the sample

The monitor can be used with any `IPlcClient`. The current SampleSmart integration validates:

- InMemory PLC Client
- Dreamine Simulator TCP Client
- Mitsubishi MC TCP Client
- Mitsubishi MC UDP Client
- Omron FINS TCP Client
- Omron FINS UDP Client

## Sample mode matching rule

The sample contains one unified PLC Protocol page. For simulator-based testing, server and client modes must match.

```text
SimulatorTcp ↔ SimulatorTcp
McTcp        ↔ McTcp
McUdp        ↔ McUdp
FinsTcp      ↔ FinsTcp
FinsUdp      ↔ FinsUdp
```

If the mode does not match, communication failure is expected.

## PC-to-PC test requirement

When testing between two PCs, open the selected protocol port on the server PC.

Example for port `55000`:

```powershell
New-NetFirewallRule -DisplayName "Dreamine PLC TCP 55000" -Direction Inbound -Protocol TCP -LocalPort 55000 -Action Allow
New-NetFirewallRule -DisplayName "Dreamine PLC UDP 55000" -Direction Inbound -Protocol UDP -LocalPort 55000 -Action Allow
```

Use an Administrator PowerShell session. The client PC usually does not need inbound rules unless it also runs a server.

## Physical PLC test notice

The WPF monitor can connect to real PLC clients, but physical PLC validation must be performed separately.

Before connecting to a physical PLC, verify:

- PLC IP address and port
- Protocol mode
- TCP/UDP firewall path
- PLC Ethernet module settings
- PLC memory area mapping
- Safe polling interval
- Write operation safety

Do not use 1ms polling against physical PLCs. Use 100ms to 500ms for monitoring and event-driven writes for control signals.

## Package scope

This package does not implement vendor protocols directly. It only provides UI components and view models.

Vendor protocol implementations belong to:

- `Dreamine.PLC.Mitsubishi.MC`
- `Dreamine.PLC.Omron.Fins`
- Future vendor packages

## License

MIT License.
