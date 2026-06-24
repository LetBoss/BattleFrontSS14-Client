// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleArmorHardpointComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Prototypes;
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
public sealed class VehicleArmorHardpointComponent : 
  Component,
  ISerializationGenerated<VehicleArmorHardpointComponent>,
  ISerializationGenerated
{
  [DataField("modifierSets", false, 1, false, false, null)]
  public List<ProtoId<DamageModifierSetPrototype>> ModifierSets = new List<ProtoId<DamageModifierSetPrototype>>();
  [DataField("explosionCoefficient", false, 1, false, false, null)]
  public float? ExplosionCoefficient;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleArmorHardpointComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleArmorHardpointComponent) target1;
    if (serialization.TryCustomCopy<VehicleArmorHardpointComponent>(this, ref target, hookCtx, false, context))
      return;
    List<ProtoId<DamageModifierSetPrototype>> target2 = (List<ProtoId<DamageModifierSetPrototype>>) null;
    if (this.ModifierSets == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<DamageModifierSetPrototype>>>(this.ModifierSets, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<ProtoId<DamageModifierSetPrototype>>>(this.ModifierSets, hookCtx, context);
    target.ModifierSets = target2;
    float? target3 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.ExplosionCoefficient, ref target3, hookCtx, false, context))
      target3 = this.ExplosionCoefficient;
    target.ExplosionCoefficient = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleArmorHardpointComponent target,
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
    VehicleArmorHardpointComponent target1 = (VehicleArmorHardpointComponent) target;
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
    VehicleArmorHardpointComponent target1 = (VehicleArmorHardpointComponent) target;
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
    VehicleArmorHardpointComponent target1 = (VehicleArmorHardpointComponent) target;
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
  virtual VehicleArmorHardpointComponent Component.Instantiate()
  {
    return new VehicleArmorHardpointComponent();
  }
}
