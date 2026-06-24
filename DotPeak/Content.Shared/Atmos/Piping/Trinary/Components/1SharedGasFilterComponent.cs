// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Piping.Trinary.Components.GasFilterBoundUserInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Atmos.Piping.Trinary.Components;

[NetSerializable]
[Serializable]
public sealed class GasFilterBoundUserInterfaceState : BoundUserInterfaceState
{
  public string FilterLabel { get; }

  public float TransferRate { get; }

  public bool Enabled { get; }

  public Gas? FilteredGas { get; }

  public GasFilterBoundUserInterfaceState(
    string filterLabel,
    float transferRate,
    bool enabled,
    Gas? filteredGas)
  {
    this.FilterLabel = filterLabel;
    this.TransferRate = transferRate;
    this.Enabled = enabled;
    this.FilteredGas = filteredGas;
  }
}
