using System.Runtime.InteropServices;
using Robust.Shared.GameObjects;

namespace Robust.Shared.Spawners;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[ByRefEvent]
public readonly record struct TimedDespawnEvent;
