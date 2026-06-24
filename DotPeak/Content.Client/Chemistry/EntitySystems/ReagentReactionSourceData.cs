// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.EntitySystems.ReagentReactionSourceData
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chemistry.Reaction;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Chemistry.EntitySystems;

public sealed class ReagentReactionSourceData : ReagentSourceData
{
  public readonly ReactionPrototype ReactionPrototype;

  public override int OutputCount
  {
    get
    {
      return this.ReactionPrototype.Products.Count + this.ReactionPrototype.Reactants.Count<KeyValuePair<string, ReactantPrototype>>((Func<KeyValuePair<string, ReactantPrototype>, bool>) (r => r.Value.Catalyst));
    }
  }

  public override string IdentifierString => this.ReactionPrototype.ID;

  public ReagentReactionSourceData(
    List<ProtoId<MixingCategoryPrototype>> mixingType,
    ReactionPrototype reactionPrototype)
    : base(mixingType)
  {
    this.ReactionPrototype = reactionPrototype;
  }
}
