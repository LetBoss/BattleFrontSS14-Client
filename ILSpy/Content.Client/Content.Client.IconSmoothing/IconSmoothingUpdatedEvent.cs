using System.Runtime.InteropServices;
using Robust.Shared.GameObjects;

namespace Content.Client.IconSmoothing;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[ByRefEvent]
public readonly record struct IconSmoothingUpdatedEvent;
