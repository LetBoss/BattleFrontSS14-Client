using System.Runtime.InteropServices;

namespace Robust.Shared.GameObjects;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[ComponentEvent]
public readonly record struct ComponentAdd;
