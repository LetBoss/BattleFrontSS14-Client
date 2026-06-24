using System.Runtime.InteropServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Ranged.Events;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[ByRefEvent]
public readonly record struct GunCycledEvent;
