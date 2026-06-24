using System.Runtime.InteropServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Atmos.Piping.Components;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[ByRefEvent]
public readonly record struct AtmosDeviceDisabledEvent;
