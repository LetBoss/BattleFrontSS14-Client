// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.EventEntityEffectCondition`1
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

public abstract class EventEntityEffectCondition<T> : 
  EntityEffectCondition,
  ISerializationGenerated<EventEntityEffectCondition<T>>,
  ISerializationGenerated
  where T : EventEntityEffectCondition<T>
{
  public override bool Condition(EntityEffectBaseArgs args)
  {
    if (!(this is T obj))
      return false;
    CheckEntityEffectConditionEvent<T> toRaise = new CheckEntityEffectConditionEvent<T>()
    {
      Condition = obj,
      Args = args
    };
    args.EntityManager.EventBus.RaiseEvent<CheckEntityEffectConditionEvent<T>>(EventSource.Local, ref toRaise);
    return toRaise.Result;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref EventEntityEffectCondition<T> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffectCondition target1 = (EntityEffectCondition) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffectCondition<T>) target1;
    serialization.TryCustomCopy<EventEntityEffectCondition<T>>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref EventEntityEffectCondition<T> target,
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
    EventEntityEffectCondition<T> target1 = (EventEntityEffectCondition<T>) target;
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
    EventEntityEffectCondition<T> target1 = (EventEntityEffectCondition<T>) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual EventEntityEffectCondition<T> EntityEffectCondition.Instantiate()
  {
    throw new NotImplementedException();
  }
}
