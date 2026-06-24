using System;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Serialization;

namespace Content.Shared.Physics;

[Flags]
[FlagsFor(typeof(CollisionLayer))]
[FlagsFor(typeof(CollisionMask))]
public enum CollisionGroup
{
	None = 0,
	Opaque = 1,
	Impassable = 2,
	MidImpassable = 4,
	HighImpassable = 8,
	LowImpassable = 0x10,
	GhostImpassable = 0x20,
	BulletImpassable = 0x40,
	InteractImpassable = 0x80,
	DoorPassable = 0x100,
	MobCollision = 0x1000000,
	BarbedBarricade = 0x2000000,
	BarricadeImpassable = 0x4000000,
	XenoProjectileImpassable = 0x8000000,
	DropshipImpassable = 0x10000000,
	VehicleImpassable = 0x20000000,
	MapGrid = int.MinValue,
	AllMask = -1,
	SingularityLayer = 0x1DF,
	MobMask = 0x1E,
	MobLayer = 0x41,
	SmallMobMask = 0x12,
	SmallMobLayer = 0x41,
	FlyingMobMask = 0xA,
	FlyingMobLayer = 0x41,
	LargeMobMask = 0x2000001E,
	LargeMobLayer = 0x5D,
	MachineMask = 0x16,
	MachineLayer = 0x55,
	ConveyorMask = 0x116,
	CrateMask = 0x1A,
	TableMask = 6,
	TableLayer = 4,
	TabletopMachineMask = 0xA,
	TabletopMachineLayer = 0x41,
	GlassAirlockLayer = 0xCC,
	AirlockLayer = 0xCD,
	HumanoidBlockLayer = 0xC,
	SlipLayer = 0x14,
	ItemMask = 0xA,
	ThrownItem = 0x4A,
	WallLayer = 0xDF,
	GlassLayer = 0xDE,
	HalfWallLayer = 0x14,
	SpecialWallLayer = 0x5D,
	FullTileMask = 0x9E,
	FullTileLayer = 0xDD,
	SubfloorMask = 0x12,
	AirdropLayer = 0x15
}
