using System.Runtime.InteropServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.DeviceLinking.Events;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[ByRefEvent]
public readonly record struct DeviceLinkOverloadedEvent;
