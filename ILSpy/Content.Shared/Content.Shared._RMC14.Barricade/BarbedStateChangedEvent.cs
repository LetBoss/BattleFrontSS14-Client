using System.Runtime.InteropServices;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Barricade;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[ByRefEvent]
public record struct BarbedStateChangedEvent;
