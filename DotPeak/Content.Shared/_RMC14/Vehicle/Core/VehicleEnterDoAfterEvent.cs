// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleEnterDoAfterEvent
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
namespace Content.Shared._RMC14.Vehicle;

[NetSerializable]
[Serializable]
public sealed class VehicleEnterDoAfterEvent : 
  SimpleDoAfterEvent,
  ISerializationGenerated<VehicleEnterDoAfterEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public int EntryIndex;

  public override DoAfterEvent Clone()
  {
    return (DoAfterEvent) new VehicleEnterDoAfterEvent()
    {
      EntryIndex = this.EntryIndex
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleEnterDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleEnterDoAfterEvent) target1;
    if (serialization.TryCustomCopy<VehicleEnterDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.EntryIndex, ref target2, hookCtx, false, context))
      target2 = this.EntryIndex;
    target.EntryIndex = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleEnterDoAfterEvent target,
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
    VehicleEnterDoAfterEvent target1 = (VehicleEnterDoAfterEvent) target;
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
    VehicleEnterDoAfterEvent target1 = (VehicleEnterDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual VehicleEnterDoAfterEvent SimpleDoAfterEvent.Instantiate()
  {
    return new VehicleEnterDoAfterEvent();
  }
}
