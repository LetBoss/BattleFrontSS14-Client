// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Reagent.ReactiveReagentEffectEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.EntityEffects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chemistry.Reagent;

[DataDefinition]
public sealed class ReactiveReagentEffectEntry : 
  ISerializationGenerated<ReactiveReagentEffectEntry>,
  ISerializationGenerated
{
  [DataField("methods", false, 1, true, false, null)]
  public HashSet<ReactionMethod> Methods;
  [DataField("effects", false, 1, true, false, null)]
  public EntityEffect[] Effects;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ReactiveReagentEffectEntry target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ReactiveReagentEffectEntry>(this, ref target, hookCtx, false, context))
      return;
    HashSet<ReactionMethod> reactionMethodSet = (HashSet<ReactionMethod>) null;
    if (this.Methods == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ReactionMethod>>(this.Methods, ref reactionMethodSet, hookCtx, true, context))
      reactionMethodSet = serialization.CreateCopy<HashSet<ReactionMethod>>(this.Methods, hookCtx, context, false);
    target.Methods = reactionMethodSet;
    EntityEffect[] entityEffectArray = (EntityEffect[]) null;
    if (this.Effects == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityEffect[]>(this.Effects, ref entityEffectArray, hookCtx, true, context))
      entityEffectArray = serialization.CreateCopy<EntityEffect[]>(this.Effects, hookCtx, context, false);
    target.Effects = entityEffectArray;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ReactiveReagentEffectEntry target,
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
    ReactiveReagentEffectEntry target1 = (ReactiveReagentEffectEntry) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public ReactiveReagentEffectEntry Instantiate() => new ReactiveReagentEffectEntry();
}
