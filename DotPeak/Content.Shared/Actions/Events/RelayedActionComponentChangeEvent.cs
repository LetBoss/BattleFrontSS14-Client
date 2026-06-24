// Decompiled with JetBrains decompiler
// Type: Content.Shared.Actions.Events.RelayedActionComponentChangeEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Actions.Events;

public sealed class RelayedActionComponentChangeEvent : 
  ActionComponentChangeEvent,
  ISerializationGenerated<RelayedActionComponentChangeEvent>,
  ISerializationGenerated
{
  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RelayedActionComponentChangeEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ActionComponentChangeEvent target1 = (ActionComponentChangeEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RelayedActionComponentChangeEvent) target1;
    serialization.TryCustomCopy<RelayedActionComponentChangeEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RelayedActionComponentChangeEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref ActionComponentChangeEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RelayedActionComponentChangeEvent target1 = (RelayedActionComponentChangeEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (ActionComponentChangeEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RelayedActionComponentChangeEvent target1 = (RelayedActionComponentChangeEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual RelayedActionComponentChangeEvent ActionComponentChangeEvent.Instantiate()
  {
    return new RelayedActionComponentChangeEvent();
  }
}
