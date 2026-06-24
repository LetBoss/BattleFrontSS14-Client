// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Power.RMCFusionReactorRepairDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Power;

[NetSerializable]
[Serializable]
public sealed class RMCFusionReactorRepairDoAfterEvent : 
  SimpleDoAfterEvent,
  ISerializationGenerated<RMCFusionReactorRepairDoAfterEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public RMCFusionReactorState State;

  public RMCFusionReactorRepairDoAfterEvent(RMCFusionReactorState state) => this.State = state;

  public RMCFusionReactorRepairDoAfterEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCFusionReactorRepairDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCFusionReactorRepairDoAfterEvent) target1;
    if (serialization.TryCustomCopy<RMCFusionReactorRepairDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    RMCFusionReactorState target2 = RMCFusionReactorState.Working;
    if (!serialization.TryCustomCopy<RMCFusionReactorState>(this.State, ref target2, hookCtx, false, context))
      target2 = this.State;
    target.State = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCFusionReactorRepairDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref SimpleDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCFusionReactorRepairDoAfterEvent target1 = (RMCFusionReactorRepairDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (SimpleDoAfterEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCFusionReactorRepairDoAfterEvent target1 = (RMCFusionReactorRepairDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual RMCFusionReactorRepairDoAfterEvent SimpleDoAfterEvent.Instantiate()
  {
    return new RMCFusionReactorRepairDoAfterEvent();
  }
}
