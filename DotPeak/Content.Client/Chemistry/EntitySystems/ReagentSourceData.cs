// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.EntitySystems.ReagentSourceData
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chemistry.Reaction;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Chemistry.EntitySystems;

public abstract class ReagentSourceData
{
  public readonly IReadOnlyList<ProtoId<MixingCategoryPrototype>> MixingType;

  public abstract int OutputCount { get; }

  public abstract string IdentifierString { get; }

  protected ReagentSourceData(List<ProtoId<MixingCategoryPrototype>> mixingType)
  {
    this.MixingType = (IReadOnlyList<ProtoId<MixingCategoryPrototype>>) mixingType;
  }
}
