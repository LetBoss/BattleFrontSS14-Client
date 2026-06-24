using System.Runtime.InteropServices;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Evacuation;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[ByRefEvent]
public readonly record struct EvacuationEnabledEvent;
