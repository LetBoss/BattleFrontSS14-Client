// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.CreateEntityReactionEffect
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
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
namespace Content.Shared.EntityEffects.Effects;

[DataDefinition]
public sealed class CreateEntityReactionEffect : 
  EventEntityEffect<CreateEntityReactionEffect>,
  ISerializationGenerated<CreateEntityReactionEffect>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string Entity;
  [DataField(null, false, 1, false, false, null)]
  public uint Number = 1;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-create-entity-reaction-effect", ("chance", (object) this.Probability), ("entname", (object) IoCManager.Resolve<IPrototypeManager>().Index<EntityPrototype>(this.Entity).Name), ("amount", (object) this.Number));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CreateEntityReactionEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<CreateEntityReactionEffect> target1 = (EventEntityEffect<CreateEntityReactionEffect>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CreateEntityReactionEffect) target1;
    if (serialization.TryCustomCopy<CreateEntityReactionEffect>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Entity == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Entity, ref target2, hookCtx, false, context))
      target2 = this.Entity;
    target.Entity = target2;
    uint target3 = 0;
    if (!serialization.TryCustomCopy<uint>(this.Number, ref target3, hookCtx, false, context))
      target3 = this.Number;
    target.Number = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CreateEntityReactionEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<CreateEntityReactionEffect> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CreateEntityReactionEffect target1 = (CreateEntityReactionEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<CreateEntityReactionEffect>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CreateEntityReactionEffect target1 = (CreateEntityReactionEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual CreateEntityReactionEffect EventEntityEffect<CreateEntityReactionEffect>.Instantiate()
  {
    return new CreateEntityReactionEffect();
  }
}
