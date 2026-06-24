// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleSmashableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Tag;
using Robust.Shared.Audio;
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
public sealed class VehicleSmashableComponent : 
  Component,
  ISerializationGenerated<VehicleSmashableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool DeleteOnHit = true;
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<TagPrototype>> RequiredVehicleTags = new HashSet<ProtoId<TagPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public double DamageOnHit = 1000.0;
  [DataField(null, false, 1, false, false, null)]
  public float SlowdownMultiplier = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  public float SlowdownDuration;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SmashSound;
  [DataField(null, false, 1, false, false, null)]
  public bool RequiresDoorUnpowered;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleSmashableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleSmashableComponent) target1;
    if (serialization.TryCustomCopy<VehicleSmashableComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.DeleteOnHit, ref target2, hookCtx, false, context))
      target2 = this.DeleteOnHit;
    target.DeleteOnHit = target2;
    HashSet<ProtoId<TagPrototype>> target3 = (HashSet<ProtoId<TagPrototype>>) null;
    if (this.RequiredVehicleTags == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<TagPrototype>>>(this.RequiredVehicleTags, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<ProtoId<TagPrototype>>>(this.RequiredVehicleTags, hookCtx, context);
    target.RequiredVehicleTags = target3;
    double target4 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.DamageOnHit, ref target4, hookCtx, false, context))
      target4 = this.DamageOnHit;
    target.DamageOnHit = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SlowdownMultiplier, ref target5, hookCtx, false, context))
      target5 = this.SlowdownMultiplier;
    target.SlowdownMultiplier = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SlowdownDuration, ref target6, hookCtx, false, context))
      target6 = this.SlowdownDuration;
    target.SlowdownDuration = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SmashSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.SmashSound, hookCtx, context);
    target.SmashSound = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequiresDoorUnpowered, ref target8, hookCtx, false, context))
      target8 = this.RequiresDoorUnpowered;
    target.RequiresDoorUnpowered = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleSmashableComponent target,
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
    VehicleSmashableComponent target1 = (VehicleSmashableComponent) target;
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
    VehicleSmashableComponent target1 = (VehicleSmashableComponent) target;
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
    VehicleSmashableComponent target1 = (VehicleSmashableComponent) target;
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
  virtual VehicleSmashableComponent Component.Instantiate() => new VehicleSmashableComponent();
}
