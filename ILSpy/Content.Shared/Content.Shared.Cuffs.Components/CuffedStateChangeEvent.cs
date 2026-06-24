using System.Runtime.InteropServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Cuffs.Components;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[ByRefEvent]
public readonly record struct CuffedStateChangeEvent;
