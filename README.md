# Dreamine.PLC.Wpf

WPF monitoring and diagnostic UI components for Dreamine PLC communication.

## Included

- `PlcMonitorView`
- `PlcMonitorViewModel`
- InMemory PLC client default binding
- Read/Write Bits
- Read/Write Words
- Operation log grid
- Connection state indicator

The view depends only on `IPlcClient`, `Dreamine.PLC.Abstractions`, and `Dreamine.PLC.Core`.
Vendor-specific PLC adapters should be created outside this UI package and injected through `PlcMonitorViewModel.SetClient`.
