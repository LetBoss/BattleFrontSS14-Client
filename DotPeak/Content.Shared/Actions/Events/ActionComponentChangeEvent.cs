// Decompiled with JetBrains decompiler
// Type: Content.Shared.Actions.Events.ActionComponentChangeEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Actions.Events;

[Virtual]
public class ActionComponentChangeEvent : 
  InstantActionEvent,
  ISerializationGenerated<ActionComponentChangeEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public ComponentRegistry Components = new ComponentRegistry();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref ActionComponentChangeEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InstantActionEvent target1 = (InstantActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ActionComponentChangeEvent) target1;
    if (serialization.TryCustomCopy<ActionComponentChangeEvent>(this, ref target, hookCtx, false, context))
      return;
    ComponentRegistry componentRegistry = (ComponentRegistry) null;
    if (this.Components == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.Components, ref componentRegistry, hookCtx, false, context))
      componentRegistry = serialization.CreateCopy<ComponentRegistry>(this.Components, hookCtx, context, false);
    target.Components = componentRegistry;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref ActionComponentChangeEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref InstantActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ActionComponentChangeEvent target1 = (ActionComponentChangeEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (InstantActionEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ActionComponentChangeEvent target1 = (ActionComponentChangeEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ActionComponentChangeEvent InstantActionEvent.Instantiate()
  {
    return new ActionComponentChangeEvent();
  }
}
