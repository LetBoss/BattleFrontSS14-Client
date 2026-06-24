using System;

namespace Content.Shared.NPC;

[Flags]
public enum PathfindingDebugMode : ushort
{
	None = 0,
	Breadcrumbs = 1,
	Chunks = 2,
	Crumb = 4,
	Polys = 8,
	PolyNeighbors = 0x10,
	Poly = 0x20,
	Routes = 0x40,
	RouteCosts = 0x80,
	Steering = 0x100
}
