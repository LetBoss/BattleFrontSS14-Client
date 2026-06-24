// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.ChemMasterBoundUserInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chemistry;

[NetSerializable]
[Serializable]
public sealed class ChemMasterBoundUserInterfaceState : BoundUserInterfaceState
{
  public readonly ContainerInfo? InputContainerInfo;
  public readonly ContainerInfo? OutputContainerInfo;
  public readonly IReadOnlyList<ReagentQuantity> BufferReagents;
  public readonly ChemMasterMode Mode;
  public readonly ChemMasterSortingType SortingType;
  public readonly FixedPoint2? BufferCurrentVolume;
  public readonly uint SelectedPillType;
  public readonly uint PillDosageLimit;
  public readonly bool UpdateLabel;

  public ChemMasterBoundUserInterfaceState(
    ChemMasterMode mode,
    ChemMasterSortingType sortingType,
    ContainerInfo? inputContainerInfo,
    ContainerInfo? outputContainerInfo,
    IReadOnlyList<ReagentQuantity> bufferReagents,
    FixedPoint2 bufferCurrentVolume,
    uint selectedPillType,
    uint pillDosageLimit,
    bool updateLabel)
  {
    this.InputContainerInfo = inputContainerInfo;
    this.OutputContainerInfo = outputContainerInfo;
    this.BufferReagents = bufferReagents;
    this.Mode = mode;
    this.SortingType = sortingType;
    this.BufferCurrentVolume = new FixedPoint2?(bufferCurrentVolume);
    this.SelectedPillType = selectedPillType;
    this.PillDosageLimit = pillDosageLimit;
    this.UpdateLabel = updateLabel;
  }
}
