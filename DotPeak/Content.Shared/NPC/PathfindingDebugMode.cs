// Decompiled with JetBrains decompiler
// Type: Content.Shared.NPC.PathfindingDebugMode
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;

#nullable disable
namespace Content.Shared.NPC;

[Flags]
public enum PathfindingDebugMode : ushort
{
  None = 0,
  Breadcrumbs = 1,
  Chunks = 2,
  Crumb = 4,
  Polys = 8,
  PolyNeighbors = 16, // 0x0010
  Poly = 32, // 0x0020
  Routes = 64, // 0x0040
  RouteCosts = 128, // 0x0080
  Steering = 256, // 0x0100
}
