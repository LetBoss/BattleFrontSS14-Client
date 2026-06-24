// Decompiled with JetBrains decompiler
// Type: Content.Shared.Physics.CollisionGroup
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Physics;

[Flags]
[FlagsFor(typeof (CollisionLayer))]
[FlagsFor(typeof (CollisionMask))]
public enum CollisionGroup
{
  None = 0,
  Opaque = 1,
  Impassable = 2,
  MidImpassable = 4,
  HighImpassable = 8,
  LowImpassable = 16, // 0x00000010
  GhostImpassable = 32, // 0x00000020
  BulletImpassable = 64, // 0x00000040
  InteractImpassable = 128, // 0x00000080
  DoorPassable = 256, // 0x00000100
  MobCollision = 16777216, // 0x01000000
  BarbedBarricade = 33554432, // 0x02000000
  BarricadeImpassable = 67108864, // 0x04000000
  XenoProjectileImpassable = 134217728, // 0x08000000
  DropshipImpassable = 268435456, // 0x10000000
  VehicleImpassable = 536870912, // 0x20000000
  MapGrid = -2147483648, // 0x80000000
  AllMask = -1, // 0xFFFFFFFF
  SingularityLayer = DoorPassable | InteractImpassable | BulletImpassable | LowImpassable | HighImpassable | MidImpassable | Impassable | Opaque, // 0x000001DF
  MobMask = LowImpassable | HighImpassable | MidImpassable | Impassable, // 0x0000001E
  MobLayer = BulletImpassable | Opaque, // 0x00000041
  SmallMobMask = LowImpassable | Impassable, // 0x00000012
  SmallMobLayer = MobLayer, // 0x00000041
  FlyingMobMask = HighImpassable | Impassable, // 0x0000000A
  FlyingMobLayer = SmallMobLayer, // 0x00000041
  LargeMobMask = FlyingMobMask | VehicleImpassable | LowImpassable | MidImpassable, // 0x2000001E
  LargeMobLayer = FlyingMobLayer | LowImpassable | HighImpassable | MidImpassable, // 0x0000005D
  MachineMask = SmallMobMask | MidImpassable, // 0x00000016
  MachineLayer = FlyingMobLayer | LowImpassable | MidImpassable, // 0x00000055
  ConveyorMask = MachineMask | DoorPassable, // 0x00000116
  CrateMask = FlyingMobMask | LowImpassable, // 0x0000001A
  TableMask = MidImpassable | Impassable, // 0x00000006
  TableLayer = MidImpassable, // 0x00000004
  TabletopMachineMask = FlyingMobMask, // 0x0000000A
  TabletopMachineLayer = FlyingMobLayer, // 0x00000041
  GlassAirlockLayer = TableLayer | InteractImpassable | BulletImpassable | HighImpassable, // 0x000000CC
  AirlockLayer = GlassAirlockLayer | Opaque, // 0x000000CD
  HumanoidBlockLayer = TableLayer | HighImpassable, // 0x0000000C
  SlipLayer = TableLayer | LowImpassable, // 0x00000014
  ItemMask = TabletopMachineMask, // 0x0000000A
  ThrownItem = ItemMask | BulletImpassable, // 0x0000004A
  WallLayer = ThrownItem | SlipLayer | InteractImpassable | Opaque, // 0x000000DF
  GlassLayer = ThrownItem | SlipLayer | InteractImpassable, // 0x000000DE
  HalfWallLayer = SlipLayer, // 0x00000014
  SpecialWallLayer = HalfWallLayer | TabletopMachineLayer | HighImpassable, // 0x0000005D
  FullTileMask = HalfWallLayer | ItemMask | InteractImpassable, // 0x0000009E
  FullTileLayer = SpecialWallLayer | InteractImpassable, // 0x000000DD
  SubfloorMask = SmallMobMask, // 0x00000012
  AirdropLayer = HalfWallLayer | Opaque, // 0x00000015
}
