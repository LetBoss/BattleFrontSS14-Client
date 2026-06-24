// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.ContainerInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chemistry;

[NetSerializable]
[Serializable]
public sealed class ContainerInfo
{
  public readonly string DisplayName;
  public readonly FixedPoint2 CurrentVolume;
  public readonly FixedPoint2 MaxVolume;

  public List<(string Id, FixedPoint2 Quantity)>? Entities { get; init; }

  public List<ReagentQuantity>? Reagents { get; init; }

  public ContainerInfo(string displayName, FixedPoint2 currentVolume, FixedPoint2 maxVolume)
  {
    this.DisplayName = displayName;
    this.CurrentVolume = currentVolume;
    this.MaxVolume = maxVolume;
  }
}
