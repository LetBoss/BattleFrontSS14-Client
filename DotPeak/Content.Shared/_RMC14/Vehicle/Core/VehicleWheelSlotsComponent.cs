// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleWheelSlotsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
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
[Access(new Type[] {typeof (VehicleWheelSystem)})]
public sealed class VehicleWheelSlotsComponent : 
  Component,
  ISerializationGenerated<VehicleWheelSlotsComponent>,
  ISerializationGenerated
{
  public const string WheelComponentId = "VehicleWheelItem";
  public const string HardpointTypeId = "Wheel";
  [DataField(null, false, 1, false, false, null)]
  public int SlotCount = 1;
  [DataField(null, false, 1, false, false, null)]
  public List<string> Slots = new List<string>();
  [DataField(null, false, 1, false, false, null)]
  public string SlotPrefix = "wheel";
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist WheelWhitelist = new EntityWhitelist()
  {
    Components = new string[1]{ "VehicleWheelItem" }
  };
  [DataField(null, false, 1, false, false, null)]
  public float CollisionDamagePerSpeed;
  [DataField(null, false, 1, false, false, null)]
  public float MinCollisionDamage;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleWheelSlotsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleWheelSlotsComponent) target1;
    if (serialization.TryCustomCopy<VehicleWheelSlotsComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.SlotCount, ref target2, hookCtx, false, context))
      target2 = this.SlotCount;
    target.SlotCount = target2;
    List<string> target3 = (List<string>) null;
    if (this.Slots == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.Slots, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<string>>(this.Slots, hookCtx, context);
    target.Slots = target3;
    string target4 = (string) null;
    if (this.SlotPrefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SlotPrefix, ref target4, hookCtx, false, context))
      target4 = this.SlotPrefix;
    target.SlotPrefix = target4;
    EntityWhitelist target5 = (EntityWhitelist) null;
    if (this.WheelWhitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.WheelWhitelist, ref target5, hookCtx, false, context))
    {
      if (this.WheelWhitelist == null)
        target5 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.WheelWhitelist, ref target5, hookCtx, context, true);
    }
    target.WheelWhitelist = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CollisionDamagePerSpeed, ref target6, hookCtx, false, context))
      target6 = this.CollisionDamagePerSpeed;
    target.CollisionDamagePerSpeed = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinCollisionDamage, ref target7, hookCtx, false, context))
      target7 = this.MinCollisionDamage;
    target.MinCollisionDamage = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleWheelSlotsComponent target,
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
    VehicleWheelSlotsComponent target1 = (VehicleWheelSlotsComponent) target;
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
    VehicleWheelSlotsComponent target1 = (VehicleWheelSlotsComponent) target;
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
    VehicleWheelSlotsComponent target1 = (VehicleWheelSlotsComponent) target;
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
  virtual VehicleWheelSlotsComponent Component.Instantiate() => new VehicleWheelSlotsComponent();
}
