using System.Runtime.InteropServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Magic.Systems;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[ByRefEvent]
public readonly record struct AnimateSpellEvent;
