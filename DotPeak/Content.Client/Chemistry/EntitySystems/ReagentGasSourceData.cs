// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.EntitySystems.ReagentGasSourceData
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos.Prototypes;
using Content.Shared.Chemistry.Reaction;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Chemistry.EntitySystems;

public sealed class ReagentGasSourceData : ReagentSourceData
{
  public readonly GasPrototype GasPrototype;

  public override int OutputCount => 1;

  public override string IdentifierString => Loc.GetString(this.GasPrototype.Name);

  public ReagentGasSourceData(
    List<ProtoId<MixingCategoryPrototype>> mixingType,
    GasPrototype gasPrototype)
    : base(mixingType)
  {
    this.GasPrototype = gasPrototype;
  }
}
