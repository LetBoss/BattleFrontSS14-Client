// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Sensor.SensorTowerRepairDoAfterEvent
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
namespace Content.Shared._RMC14.Sensor;

[NetSerializable]
[Serializable]
public sealed class SensorTowerRepairDoAfterEvent : 
  SimpleDoAfterEvent,
  ISerializationGenerated<SensorTowerRepairDoAfterEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public SensorTowerState State;

  public SensorTowerRepairDoAfterEvent(SensorTowerState state) => this.State = state;

  public SensorTowerRepairDoAfterEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SensorTowerRepairDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SensorTowerRepairDoAfterEvent) target1;
    if (serialization.TryCustomCopy<SensorTowerRepairDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    SensorTowerState target2 = SensorTowerState.Weld;
    if (!serialization.TryCustomCopy<SensorTowerState>(this.State, ref target2, hookCtx, false, context))
      target2 = this.State;
    target.State = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SensorTowerRepairDoAfterEvent target,
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
    SensorTowerRepairDoAfterEvent target1 = (SensorTowerRepairDoAfterEvent) target;
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
    SensorTowerRepairDoAfterEvent target1 = (SensorTowerRepairDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SensorTowerRepairDoAfterEvent SimpleDoAfterEvent.Instantiate()
  {
    return new SensorTowerRepairDoAfterEvent();
  }
}
