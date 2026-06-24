// Decompiled with JetBrains decompiler
// Type: Content.Shared.Magic.Events.ChangeComponentsSpellEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Magic.Events;

public sealed class ChangeComponentsSpellEvent : 
  EntityTargetActionEvent,
  ISerializationGenerated<ChangeComponentsSpellEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AlwaysPushInheritance]
  public ComponentRegistry ToAdd = new ComponentRegistry();
  [DataField(null, false, 1, false, false, null)]
  [AlwaysPushInheritance]
  public HashSet<string> ToRemove = new HashSet<string>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ChangeComponentsSpellEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityTargetActionEvent target1 = (EntityTargetActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ChangeComponentsSpellEvent) target1;
    if (serialization.TryCustomCopy<ChangeComponentsSpellEvent>(this, ref target, hookCtx, false, context))
      return;
    ComponentRegistry target2 = (ComponentRegistry) null;
    if (this.ToAdd == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.ToAdd, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ComponentRegistry>(this.ToAdd, hookCtx, context);
    target.ToAdd = target2;
    HashSet<string> target3 = (HashSet<string>) null;
    if (this.ToRemove == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<string>>(this.ToRemove, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<string>>(this.ToRemove, hookCtx, context);
    target.ToRemove = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ChangeComponentsSpellEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityTargetActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ChangeComponentsSpellEvent target1 = (ChangeComponentsSpellEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityTargetActionEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ChangeComponentsSpellEvent target1 = (ChangeComponentsSpellEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ChangeComponentsSpellEvent EntityTargetActionEvent.Instantiate()
  {
    return new ChangeComponentsSpellEvent();
  }
}
