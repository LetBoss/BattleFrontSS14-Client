// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Steps.TagConstructionGraphStep
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
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Construction.Steps;

[DataDefinition]
public sealed class TagConstructionGraphStep : 
  ArbitraryInsertConstructionGraphStep,
  ISerializationGenerated<TagConstructionGraphStep>,
  ISerializationGenerated
{
  [DataField("tag", false, 1, false, false, null)]
  private string? _tag;

  public override bool EntityValid(
    EntityUid uid,
    IEntityManager entityManager,
    IComponentFactory compFactory)
  {
    TagSystem entitySystem = entityManager.EntitySysManager.GetEntitySystem<TagSystem>();
    return !string.IsNullOrEmpty(this._tag) && entitySystem.HasTag(uid, ProtoId<TagPrototype>.op_Implicit(this._tag));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TagConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ArbitraryInsertConstructionGraphStep target1 = (ArbitraryInsertConstructionGraphStep) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TagConstructionGraphStep) target1;
    if (serialization.TryCustomCopy<TagConstructionGraphStep>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (!serialization.TryCustomCopy<string>(this._tag, ref str, hookCtx, false, context))
      str = this._tag;
    target._tag = str;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TagConstructionGraphStep target,
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
    TagConstructionGraphStep target1 = (TagConstructionGraphStep) target;
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
    TagConstructionGraphStep target1 = (TagConstructionGraphStep) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual TagConstructionGraphStep ArbitraryInsertConstructionGraphStep.Instantiate()
  {
    return new TagConstructionGraphStep();
  }
}
