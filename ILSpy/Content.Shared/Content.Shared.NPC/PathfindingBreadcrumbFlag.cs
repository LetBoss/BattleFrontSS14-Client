using System;

namespace Content.Shared.NPC;

[Flags]
public enum PathfindingBreadcrumbFlag : ushort
{
	None = 0,
	Invalid = 1,
	Space = 2,
	Door = 4,
	Access = 8,
	Climb = 0x10
}
