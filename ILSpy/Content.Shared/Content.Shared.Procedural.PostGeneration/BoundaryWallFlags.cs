using System;

namespace Content.Shared.Procedural.PostGeneration;

[Flags]
public enum BoundaryWallFlags : byte
{
	Rooms = 1,
	Corridors = 2
}
