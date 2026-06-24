using System.Runtime.InteropServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Nutrition;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[ByRefEvent]
public record struct SliceFoodEvent();
