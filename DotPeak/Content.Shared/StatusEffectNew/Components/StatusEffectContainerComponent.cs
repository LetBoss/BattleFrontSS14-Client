// Decompiled with JetBrains decompiler
// Type: Content.Shared.StatusEffectNew.Components.StatusEffectContainerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.StatusEffectNew.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedStatusEffectsSystem)})]
public sealed class StatusEffectContainerComponent : 
  Component,
  ISerializationGenerated<StatusEffectContainerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public HashSet<EntityUid> ActiveStatusEffects = new HashSet<EntityUid>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StatusEffectContainerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StatusEffectContainerComponent) target1;
    if (serialization.TryCustomCopy<StatusEffectContainerComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<EntityUid> target2 = (HashSet<EntityUid>) null;
    if (this.ActiveStatusEffects == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.ActiveStatusEffects, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<EntityUid>>(this.ActiveStatusEffects, hookCtx, context);
    target.ActiveStatusEffects = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StatusEffectContainerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    StatusEffectContainerComponent target1 = (StatusEffectContainerComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    StatusEffectContainerComponent target1 = (StatusEffectContainerComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    StatusEffectContainerComponent target1 = (StatusEffectContainerComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual StatusEffectContainerComponent Component.Instantiate()
  {
    return new StatusEffectContainerComponent();
  }
}
