// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.Polymorph
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
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

public sealed class Polymorph : 
  EventEntityEffect<Content.Shared.EntityEffects.Effects.Polymorph>,
  ISerializationGenerated<Content.Shared.EntityEffects.Effects.Polymorph>,
  ISerializationGenerated
{
  [DataField("prototype", false, 1, false, false, typeof (PrototypeIdSerializer<Content.Shared.Polymorph.PolymorphPrototype>))]
  public string PolymorphPrototype { get; set; }

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-make-polymorph", ("chance", (object) this.Probability), ("entityname", (object) prototype.Index<EntityPrototype>((string) prototype.Index<Content.Shared.Polymorph.PolymorphPrototype>(this.PolymorphPrototype).Configuration.Entity).Name));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Content.Shared.EntityEffects.Effects.Polymorph target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<Content.Shared.EntityEffects.Effects.Polymorph> target1 = (EventEntityEffect<Content.Shared.EntityEffects.Effects.Polymorph>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Content.Shared.EntityEffects.Effects.Polymorph) target1;
    if (serialization.TryCustomCopy<Content.Shared.EntityEffects.Effects.Polymorph>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.PolymorphPrototype == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.PolymorphPrototype, ref target2, hookCtx, false, context))
      target2 = this.PolymorphPrototype;
    target.PolymorphPrototype = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Content.Shared.EntityEffects.Effects.Polymorph target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<Content.Shared.EntityEffects.Effects.Polymorph> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Content.Shared.EntityEffects.Effects.Polymorph target1 = (Content.Shared.EntityEffects.Effects.Polymorph) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<Content.Shared.EntityEffects.Effects.Polymorph>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Content.Shared.EntityEffects.Effects.Polymorph target1 = (Content.Shared.EntityEffects.Effects.Polymorph) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Content.Shared.EntityEffects.Effects.Polymorph EventEntityEffect<Content.Shared.EntityEffects.Effects.Polymorph>.Instantiate()
  {
    return new Content.Shared.EntityEffects.Effects.Polymorph();
  }
}
