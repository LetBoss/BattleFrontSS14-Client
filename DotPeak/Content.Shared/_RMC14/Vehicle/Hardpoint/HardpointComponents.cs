// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.HardpointItemComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (HardpointSystem), typeof (HardpointSlotSystem)})]
public sealed class HardpointItemComponent : 
  Component,
  ISerializationGenerated<HardpointItemComponent>,
  ISerializationGenerated
{
  public const string ComponentId = "HardpointItem";
  [DataField(null, false, 1, true, false, null)]
  public string HardpointType = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<HardpointVehicleFamilyPrototype>? VehicleFamily;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<HardpointSlotTypePrototype>? SlotType;
  [DataField(null, false, 1, false, false, null)]
  public string? CompatibilityId;
  [DataField(null, false, 1, false, false, null)]
  public float DamageMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float RepairRate = 0.01f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HardpointItemComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HardpointItemComponent) target1;
    if (serialization.TryCustomCopy<HardpointItemComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.HardpointType == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.HardpointType, ref target2, hookCtx, false, context))
      target2 = this.HardpointType;
    target.HardpointType = target2;
    ProtoId<HardpointVehicleFamilyPrototype>? target3 = new ProtoId<HardpointVehicleFamilyPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<HardpointVehicleFamilyPrototype>?>(this.VehicleFamily, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<HardpointVehicleFamilyPrototype>?>(this.VehicleFamily, hookCtx, context);
    target.VehicleFamily = target3;
    ProtoId<HardpointSlotTypePrototype>? target4 = new ProtoId<HardpointSlotTypePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<HardpointSlotTypePrototype>?>(this.SlotType, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ProtoId<HardpointSlotTypePrototype>?>(this.SlotType, hookCtx, context);
    target.SlotType = target4;
    string target5 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.CompatibilityId, ref target5, hookCtx, false, context))
      target5 = this.CompatibilityId;
    target.CompatibilityId = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DamageMultiplier, ref target6, hookCtx, false, context))
      target6 = this.DamageMultiplier;
    target.DamageMultiplier = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RepairRate, ref target7, hookCtx, false, context))
      target7 = this.RepairRate;
    target.RepairRate = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HardpointItemComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    HardpointItemComponent target1 = (HardpointItemComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    HardpointItemComponent target1 = (HardpointItemComponent) target;
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
    HardpointItemComponent target1 = (HardpointItemComponent) target;
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
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual HardpointItemComponent Component.Instantiate() => new HardpointItemComponent();
}
