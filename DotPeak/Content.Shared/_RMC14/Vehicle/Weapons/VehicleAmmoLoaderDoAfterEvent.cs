// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleAmmoLoaderDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[NetSerializable]
[Serializable]
public sealed class VehicleAmmoLoaderDoAfterEvent : 
  DoAfterEvent,
  ISerializationGenerated<VehicleAmmoLoaderDoAfterEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string SlotId = string.Empty;

  public VehicleAmmoLoaderDoAfterEvent()
  {
  }

  public VehicleAmmoLoaderDoAfterEvent(string slotId) => this.SlotId = slotId;

  public override DoAfterEvent Clone()
  {
    return (DoAfterEvent) new VehicleAmmoLoaderDoAfterEvent(this.SlotId);
  }

  public override bool IsDuplicate(DoAfterEvent other)
  {
    if (other == null || !(other is VehicleAmmoLoaderDoAfterEvent loaderDoAfterEvent) || !(loaderDoAfterEvent.SlotId == this.SlotId) || !(other.User == this.User))
      return false;
    EntityUid? nullable1 = other.Target;
    EntityUid? nullable2 = this.Target;
    if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) == 0)
      return false;
    nullable2 = other.Used;
    nullable1 = this.Used;
    if (nullable2.HasValue != nullable1.HasValue)
      return false;
    return !nullable2.HasValue || nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleAmmoLoaderDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleAmmoLoaderDoAfterEvent) target1;
    if (serialization.TryCustomCopy<VehicleAmmoLoaderDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.SlotId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SlotId, ref target2, hookCtx, false, context))
      target2 = this.SlotId;
    target.SlotId = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleAmmoLoaderDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref DoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    VehicleAmmoLoaderDoAfterEvent target1 = (VehicleAmmoLoaderDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (DoAfterEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    VehicleAmmoLoaderDoAfterEvent target1 = (VehicleAmmoLoaderDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual VehicleAmmoLoaderDoAfterEvent DoAfterEvent.Instantiate()
  {
    return new VehicleAmmoLoaderDoAfterEvent();
  }
}
