// Decompiled with JetBrains decompiler
// Type: Content.Shared.Fluids.PuddleDebugOverlayData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Fluids;

[NetSerializable]
[Serializable]
public readonly struct PuddleDebugOverlayData(Vector2i pos, FixedPoint2 currentVolume)
{
  public readonly Vector2i Pos = pos;
  public readonly FixedPoint2 CurrentVolume = currentVolume;
}
