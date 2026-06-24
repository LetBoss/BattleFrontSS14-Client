// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.EffectConditions.HasTag
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Tag;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class HasTag : 
  EntityEffectCondition,
  ISerializationGenerated<HasTag>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (PrototypeIdSerializer<TagPrototype>))]
  public string Tag;
  [DataField(null, false, 1, false, false, null)]
  public bool Invert;

  public override bool Condition(EntityEffectBaseArgs args)
  {
    TagComponent component;
    return args.EntityManager.TryGetComponent<TagComponent>(args.TargetEntity, out component) && args.EntityManager.System<TagSystem>().HasTag(component, (ProtoId<TagPrototype>) this.Tag) ^ this.Invert;
  }

  public override string GuidebookExplanation(IPrototypeManager prototype)
  {
    return Loc.GetString("reagent-effect-condition-guidebook-has-tag", ("tag", (object) this.Tag), ("invert", (object) this.Invert));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HasTag target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffectCondition target1 = (EntityEffectCondition) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HasTag) target1;
    if (serialization.TryCustomCopy<HasTag>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Tag == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Tag, ref target2, hookCtx, false, context))
      target2 = this.Tag;
    target.Tag = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Invert, ref target3, hookCtx, false, context))
      target3 = this.Invert;
    target.Invert = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HasTag target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityEffectCondition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    HasTag target1 = (HasTag) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityEffectCondition) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    HasTag target1 = (HasTag) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual HasTag EntityEffectCondition.Instantiate() => new HasTag();
}
