// Decompiled with JetBrains decompiler
// Type: Content.Client.Explosion.TriggerOnProximityComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Explosion.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Explosion;

[RegisterComponent]
[Access(new Type[] {typeof (TriggerSystem)})]
public sealed class TriggerOnProximityComponent : 
  SharedTriggerOnProximityComponent,
  ISerializationGenerated<TriggerOnProximityComponent>,
  ISerializationGenerated
{
  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TriggerOnProximityComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SharedTriggerOnProximityComponent target1 = (SharedTriggerOnProximityComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TriggerOnProximityComponent) target1;
    serialization.TryCustomCopy<TriggerOnProximityComponent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TriggerOnProximityComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref SharedTriggerOnProximityComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TriggerOnProximityComponent target1 = (TriggerOnProximityComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (SharedTriggerOnProximityComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TriggerOnProximityComponent target1 = (TriggerOnProximityComponent) target;
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
    TriggerOnProximityComponent target1 = (TriggerOnProximityComponent) target;
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
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual TriggerOnProximityComponent SharedTriggerOnProximityComponent.Instantiate()
  {
    return new TriggerOnProximityComponent();
  }
}
