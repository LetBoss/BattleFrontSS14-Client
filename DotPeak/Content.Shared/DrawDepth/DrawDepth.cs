// Decompiled with JetBrains decompiler
// Type: Content.Shared.DrawDepth.DrawDepth
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;

#nullable disable
namespace Content.Shared.DrawDepth;

[ConstantsFor(typeof (Robust.Shared.GameObjects.DrawDepth))]
public enum DrawDepth
{
  LowFloors = -20, // 0xFFFFFFEC
  ThickPipe = -19, // 0xFFFFFFED
  ThickWire = -18, // 0xFFFFFFEE
  ThinPipeAlt2 = -17, // 0xFFFFFFEF
  ThinPipeAlt1 = -16, // 0xFFFFFFF0
  ThinPipe = -15, // 0xFFFFFFF1
  ThinWire = -14, // 0xFFFFFFF2
  BelowFloor = -13, // 0xFFFFFFF3
  FloorTiles = -12, // 0xFFFFFFF4
  FloorObjects = -11, // 0xFFFFFFF5
  Puddles = -10, // 0xFFFFFFF6
  HighFloorObjects = -5, // 0xFFFFFFFB
  DeadMobs = -4, // 0xFFFFFFFC
  SmallMobs = -3, // 0xFFFFFFFD
  Walls = -2, // 0xFFFFFFFE
  WallTops = -1, // 0xFFFFFFFF
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
  Overdoors = 10, // 0x0000000A
  Effects = 11, // 0x0000000B
  Ghosts = 12, // 0x0000000C
  Overlays = 13, // 0x0000000D
}
