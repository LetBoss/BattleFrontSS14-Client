using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DrawDepth;

[ConstantsFor(typeof(DrawDepth))]
public enum DrawDepth
{
	LowFloors = -20,
	ThickPipe = -19,
	ThickWire = -18,
	ThinPipeAlt2 = -17,
	ThinPipeAlt1 = -16,
	ThinPipe = -15,
	ThinWire = -14,
	BelowFloor = -13,
	FloorTiles = -12,
	FloorObjects = -11,
	Puddles = -10,
	HighFloorObjects = -5,
	DeadMobs = -4,
	SmallMobs = -3,
	Walls = -2,
	WallTops = -1,
	Objects = 0,
	SmallObjects = 1,
	WallMountedItems = 2,
	LargeObjects = 3,
	Items = 4,
	BelowMobs = 5,
	Mobs = 6,
	OverMobs = 7,
	Doors = 8,
	BlastDoors = 9,
	Overdoors = 10,
	Effects = 11,
	Ghosts = 12,
	Overlays = 13
}
