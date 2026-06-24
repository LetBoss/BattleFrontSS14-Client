// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.PipeDirection
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Atmos;

[Flags]
[NetSerializable]
[Serializable]
public enum PipeDirection
{
  None = 0,
  North = 1,
  South = 2,
  West = 4,
  East = 8,
  Longitudinal = South | North, // 0x00000003
  Lateral = East | West, // 0x0000000C
  NWBend = West | North, // 0x00000005
  NEBend = East | North, // 0x00000009
  SWBend = West | South, // 0x00000006
  SEBend = East | South, // 0x0000000A
  TNorth = NEBend | West, // 0x0000000D
  TSouth = SEBend | West, // 0x0000000E
  TWest = SWBend | North, // 0x00000007
  TEast = SEBend | North, // 0x0000000B
  Fourway = TEast | West, // 0x0000000F
  All = -1, // 0xFFFFFFFF
}
