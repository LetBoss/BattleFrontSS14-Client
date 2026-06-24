// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.EventEntityEffect`1
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects;

public abstract class EventEntityEffect<T> : 
  EntityEffect,
  ISerializationGenerated<EventEntityEffect<T>>,
  ISerializationGenerated
  where T : EntityEffect
{
  public override void Effect(EntityEffectBaseArgs args)
  {
    if (!(this is T effect))
      return;
    ExecuteEntityEffectEvent<T> toRaise = new ExecuteEntityEffectEvent<T>(effect, args);
    args.EntityManager.EventBus.RaiseEvent<ExecuteEntityEffectEvent<T>>(EventSource.Local, ref toRaise);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref EventEntityEffect<T> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<T>) target1;
    serialization.TryCustomCopy<EventEntityEffect<T>>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref EventEntityEffect<T> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<T> target1 = (EventEntityEffect<T>) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityEffect) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<T> target1 = (EventEntityEffect<T>) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual EventEntityEffect<T> EntityEffect.Instantiate() => throw new NotImplementedException();
}
