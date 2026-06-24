// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Steps.MultipleTagsConstructionGraphStep
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Construction.Steps;

public sealed class MultipleTagsConstructionGraphStep : 
  ArbitraryInsertConstructionGraphStep,
  ISerializationGenerated<MultipleTagsConstructionGraphStep>,
  ISerializationGenerated
{
  [DataField("allTags", false, 1, false, false, null)]
  private List<ProtoId<TagPrototype>>? _allTags;
  [DataField("anyTags", false, 1, false, false, null)]
  private List<ProtoId<TagPrototype>>? _anyTags;

  private static bool IsNullOrEmpty<T>(ICollection<T>? list) => list == null || list.Count == 0;

  public override bool EntityValid(
    EntityUid uid,
    IEntityManager entityManager,
    IComponentFactory compFactory)
  {
    if (MultipleTagsConstructionGraphStep.IsNullOrEmpty<ProtoId<TagPrototype>>((ICollection<ProtoId<TagPrototype>>) this._allTags) && MultipleTagsConstructionGraphStep.IsNullOrEmpty<ProtoId<TagPrototype>>((ICollection<ProtoId<TagPrototype>>) this._anyTags))
      return false;
    TagSystem entitySystem = entityManager.EntitySysManager.GetEntitySystem<TagSystem>();
    return (this._allTags == null || entitySystem.HasAllTags(uid, this._allTags)) && (this._anyTags == null || entitySystem.HasAnyTag(uid, this._anyTags));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MultipleTagsConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ArbitraryInsertConstructionGraphStep target1 = (ArbitraryInsertConstructionGraphStep) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MultipleTagsConstructionGraphStep) target1;
    if (serialization.TryCustomCopy<MultipleTagsConstructionGraphStep>(this, ref target, hookCtx, false, context))
      return;
    List<ProtoId<TagPrototype>> protoIdList1 = (List<ProtoId<TagPrototype>>) null;
    if (!serialization.TryCustomCopy<List<ProtoId<TagPrototype>>>(this._allTags, ref protoIdList1, hookCtx, true, context))
      protoIdList1 = serialization.CreateCopy<List<ProtoId<TagPrototype>>>(this._allTags, hookCtx, context, false);
    target._allTags = protoIdList1;
    List<ProtoId<TagPrototype>> protoIdList2 = (List<ProtoId<TagPrototype>>) null;
    if (!serialization.TryCustomCopy<List<ProtoId<TagPrototype>>>(this._anyTags, ref protoIdList2, hookCtx, true, context))
      protoIdList2 = serialization.CreateCopy<List<ProtoId<TagPrototype>>>(this._anyTags, hookCtx, context, false);
    target._anyTags = protoIdList2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MultipleTagsConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref ArbitraryInsertConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MultipleTagsConstructionGraphStep target1 = (MultipleTagsConstructionGraphStep) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (ArbitraryInsertConstructionGraphStep) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MultipleTagsConstructionGraphStep target1 = (MultipleTagsConstructionGraphStep) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual MultipleTagsConstructionGraphStep ArbitraryInsertConstructionGraphStep.Instantiate()
  {
    return new MultipleTagsConstructionGraphStep();
  }
}
