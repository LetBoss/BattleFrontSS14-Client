// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.EntitySystems.ReagentEntitySourceData
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reaction;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Chemistry.EntitySystems;

public sealed class ReagentEntitySourceData : ReagentSourceData
{
  public readonly EntityPrototype SourceEntProto;
  public readonly Solution Solution;

  public override int OutputCount => this.Solution.Contents.Count;

  public override string IdentifierString => this.SourceEntProto.Name;

  public ReagentEntitySourceData(
    List<ProtoId<MixingCategoryPrototype>> mixingType,
    EntityPrototype sourceEntProto,
    Solution solution)
    : base(mixingType)
  {
    this.SourceEntProto = sourceEntProto;
    this.Solution = solution;
  }
}
