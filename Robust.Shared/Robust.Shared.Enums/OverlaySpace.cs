using System;

namespace Robust.Shared.Enums;

[Flags]
public enum OverlaySpace : ushort
{
	None = 0,
	ScreenSpace = 2,
	WorldSpace = 4,
	WorldSpaceBelowFOV = 8,
	WorldSpaceEntities = 0x10,
	WorldSpaceGrids = 0x20,
	WorldSpaceBelowEntities = 0x40,
	ScreenSpaceBelowWorld = 0x80,
	WorldSpaceBelowWorld = 0x100,
	BeforeLighting = 0x200
}
