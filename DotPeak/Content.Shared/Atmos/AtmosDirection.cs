// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.AtmosDirection
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Atmos;

[Flags]
[FlagsFor(typeof (AtmosDirectionFlags))]
[Serializable]
public enum AtmosDirection
{
  Invalid = 0,
  North = 1,
  South = 2,
  East = 4,
  West = 8,
  NorthEast = East | North, // 0x00000005
  SouthEast = East | South, // 0x00000006
  NorthWest = West | North, // 0x00000009
  SouthWest = West | South, // 0x0000000A
  All = SouthWest | NorthEast, // 0x0000000F
}
