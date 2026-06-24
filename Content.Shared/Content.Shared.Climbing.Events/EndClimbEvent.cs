using System.Runtime.InteropServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Climbing.Events;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[ByRefEvent]
public readonly record struct EndClimbEvent;
