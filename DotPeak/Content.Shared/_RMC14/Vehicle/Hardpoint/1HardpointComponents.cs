// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.HardpointSlotsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Tools;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (HardpointSystem), typeof (HardpointSlotSystem)})]
public sealed class HardpointSlotsComponent : 
  Component,
  ISerializationGenerated<HardpointSlotsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<HardpointVehicleFamilyPrototype>? VehicleFamily;
  [DataField(null, false, 1, true, false, null)]
  public List<HardpointSlot> Slots = new List<HardpointSlot>();
  [DataField(null, false, 1, false, false, null)]
  public float FrameDamageFractionWhileIntact = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ToolQualityPrototype> RemoveToolQuality = (ProtoId<ToolQualityPrototype>) "Prying";
  [NonSerialized]
  public HashSet<string> PendingInserts = new HashSet<string>();
  [NonSerialized]
  public HashSet<string> CompletingInserts = new HashSet<string>();
  [NonSerialized]
  public HashSet<string> PendingRemovals = new HashSet<string>();
  [NonSerialized]
  public HashSet<EntityUid> PendingInsertUsers = new HashSet<EntityUid>();
  [NonSerialized]
  public string? LastUiError;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HardpointSlotsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HardpointSlotsComponent) target1;
    if (serialization.TryCustomCopy<HardpointSlotsComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<HardpointVehicleFamilyPrototype>? target2 = new ProtoId<HardpointVehicleFamilyPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<HardpointVehicleFamilyPrototype>?>(this.VehicleFamily, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<HardpointVehicleFamilyPrototype>?>(this.VehicleFamily, hookCtx, context);
    target.VehicleFamily = target2;
    List<HardpointSlot> target3 = (List<HardpointSlot>) null;
    if (this.Slots == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<HardpointSlot>>(this.Slots, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<HardpointSlot>>(this.Slots, hookCtx, context);
    target.Slots = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FrameDamageFractionWhileIntact, ref target4, hookCtx, false, context))
      target4 = this.FrameDamageFractionWhileIntact;
    target.FrameDamageFractionWhileIntact = target4;
    ProtoId<ToolQualityPrototype> target5 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.RemoveToolQuality, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.RemoveToolQuality, hookCtx, context);
    target.RemoveToolQuality = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HardpointSlotsComponent target,
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
    HardpointSlotsComponent target1 = (HardpointSlotsComponent) target;
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
    HardpointSlotsComponent target1 = (HardpointSlotsComponent) target;
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
    HardpointSlotsComponent target1 = (HardpointSlotsComponent) target;
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
  virtual HardpointSlotsComponent Component.Instantiate() => new HardpointSlotsComponent();
}
