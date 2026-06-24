using System.Runtime.InteropServices;

namespace Robust.Shared.GameObjects;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[ByRefEvent]
public readonly record struct EntityPausedEvent;
