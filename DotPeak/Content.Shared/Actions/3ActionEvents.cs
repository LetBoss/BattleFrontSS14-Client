// Decompiled with JetBrains decompiler
// Type: Content.Shared.Actions.EntityTargetActionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Actions;

public abstract class EntityTargetActionEvent : 
  BaseActionEvent,
  ISerializationGenerated<EntityTargetActionEvent>,
  ISerializationGenerated
{
  public EntityUid Target;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref EntityTargetActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BaseActionEvent target1 = (BaseActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EntityTargetActionEvent) target1;
    serialization.TryCustomCopy<EntityTargetActionEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref EntityTargetActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref BaseActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityTargetActionEvent target1 = (EntityTargetActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (BaseActionEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityTargetActionEvent target1 = (EntityTargetActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual EntityTargetActionEvent BaseActionEvent.Instantiate()
  {
    throw new NotImplementedException();
  }
}
