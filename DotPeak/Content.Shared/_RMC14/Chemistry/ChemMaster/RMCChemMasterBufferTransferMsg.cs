// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.ChemMaster.RMCChemMasterBufferTransferMsg
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.ChemMaster;

[NetSerializable]
[Serializable]
public sealed class RMCChemMasterBufferTransferMsg(
  ProtoId<ReagentPrototype> reagent,
  FixedPoint2 amount) : BoundUserInterfaceMessage
{
  public readonly ProtoId<ReagentPrototype> Reagent = reagent;
  public readonly FixedPoint2 Amount = amount;
}
