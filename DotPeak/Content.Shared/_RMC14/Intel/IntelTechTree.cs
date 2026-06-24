// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Intel.IntelTechTree
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Intel.Tech;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Intel;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class IntelTechTree : ISerializationGenerated<IntelTechTree>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 Points;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 TotalEarned;
  [DataField(null, false, 1, false, false, null)]
  public IntelObjectiveAmount Documents;
  [DataField(null, false, 1, false, false, null)]
  public IntelObjectiveAmount UploadData;
  [DataField(null, false, 1, false, false, null)]
  public IntelObjectiveAmount RetrieveItems;
  [DataField(null, false, 1, false, false, null)]
  public IntelObjectiveAmount Miscellaneous;
  [DataField(null, false, 1, false, false, null)]
  public int AnalyzeChemicals;
  [DataField(null, false, 1, false, false, null)]
  public int RescueSurvivors;
  [DataField(null, false, 1, false, false, null)]
  public int RecoverCorpses;
  [DataField(null, false, 1, false, false, null)]
  public bool ColonyCommunications;
  [DataField(null, false, 1, false, false, null)]
  public bool ColonyPower;
  [DataField(null, false, 1, false, false, null)]
  public int Tier;
  [DataField(null, false, 1, false, false, null)]
  public List<List<TechOption>> Options = new List<List<TechOption>>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<LocId, Dictionary<NetEntity, string>> Clues = new Dictionary<LocId, Dictionary<NetEntity, string>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IntelTechTree target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<IntelTechTree>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target1 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Points, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<FixedPoint2>(this.Points, hookCtx, context);
    target.Points = target1;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.TotalEarned, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.TotalEarned, hookCtx, context);
    target.TotalEarned = target2;
    IntelObjectiveAmount target3 = new IntelObjectiveAmount();
    if (!serialization.TryCustomCopy<IntelObjectiveAmount>(this.Documents, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<IntelObjectiveAmount>(this.Documents, hookCtx, context);
    target.Documents = target3;
    IntelObjectiveAmount target4 = new IntelObjectiveAmount();
    if (!serialization.TryCustomCopy<IntelObjectiveAmount>(this.UploadData, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<IntelObjectiveAmount>(this.UploadData, hookCtx, context);
    target.UploadData = target4;
    IntelObjectiveAmount target5 = new IntelObjectiveAmount();
    if (!serialization.TryCustomCopy<IntelObjectiveAmount>(this.RetrieveItems, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<IntelObjectiveAmount>(this.RetrieveItems, hookCtx, context);
    target.RetrieveItems = target5;
    IntelObjectiveAmount target6 = new IntelObjectiveAmount();
    if (!serialization.TryCustomCopy<IntelObjectiveAmount>(this.Miscellaneous, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<IntelObjectiveAmount>(this.Miscellaneous, hookCtx, context);
    target.Miscellaneous = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.AnalyzeChemicals, ref target7, hookCtx, false, context))
      target7 = this.AnalyzeChemicals;
    target.AnalyzeChemicals = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.RescueSurvivors, ref target8, hookCtx, false, context))
      target8 = this.RescueSurvivors;
    target.RescueSurvivors = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.RecoverCorpses, ref target9, hookCtx, false, context))
      target9 = this.RecoverCorpses;
    target.RecoverCorpses = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.ColonyCommunications, ref target10, hookCtx, false, context))
      target10 = this.ColonyCommunications;
    target.ColonyCommunications = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.ColonyPower, ref target11, hookCtx, false, context))
      target11 = this.ColonyPower;
    target.ColonyPower = target11;
    int target12 = 0;
    if (!serialization.TryCustomCopy<int>(this.Tier, ref target12, hookCtx, false, context))
      target12 = this.Tier;
    target.Tier = target12;
    List<List<TechOption>> target13 = (List<List<TechOption>>) null;
    if (this.Options == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<List<TechOption>>>(this.Options, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<List<List<TechOption>>>(this.Options, hookCtx, context);
    target.Options = target13;
    Dictionary<LocId, Dictionary<NetEntity, string>> target14 = (Dictionary<LocId, Dictionary<NetEntity, string>>) null;
    if (this.Clues == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<LocId, Dictionary<NetEntity, string>>>(this.Clues, ref target14, hookCtx, true, context))
      target14 = serialization.CreateCopy<Dictionary<LocId, Dictionary<NetEntity, string>>>(this.Clues, hookCtx, context);
    target.Clues = target14;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IntelTechTree target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    IntelTechTree target1 = (IntelTechTree) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public IntelTechTree Instantiate() => new IntelTechTree();
}
