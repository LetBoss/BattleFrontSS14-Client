// Decompiled with JetBrains decompiler
// Type: Content.Shared.Explosion.Components.ExplosiveComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Explosion.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Explosion.Components;

[RegisterComponent]
[Access(new Type[] {typeof (SharedExplosionSystem)})]
public sealed class ExplosiveComponent : 
  Component,
  ISerializationGenerated<ExplosiveComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<ExplosionPrototype> ExplosionType;
  [DataField(null, false, 1, false, false, null)]
  public float MaxIntensity = 4f;
  [DataField(null, false, 1, false, false, null)]
  public float IntensitySlope = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float TotalIntensity = 10f;
  [DataField(null, false, 1, false, false, null)]
  public float TileBreakScale = 1f;
  [DataField(null, false, 1, false, false, null)]
  public int MaxTileBreak = int.MaxValue;
  [DataField(null, false, 1, false, false, null)]
  public bool CanCreateVacuum = true;
  [DataField(null, false, 1, false, false, null)]
  public bool? DeleteAfterExplosion;
  [DataField(null, false, 1, false, false, null)]
  public bool Repeatable;
  public bool Exploded;
  [DataField(null, false, 1, false, false, null)]
  public float? VehicleLightDamage;
  [DataField(null, false, 1, false, false, null)]
  public float? VehicleHeavyDamage;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ExplosiveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ExplosiveComponent) target1;
    if (serialization.TryCustomCopy<ExplosiveComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ExplosionPrototype> target2 = new ProtoId<ExplosionPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ExplosionPrototype>>(this.ExplosionType, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<ExplosionPrototype>>(this.ExplosionType, hookCtx, context);
    target.ExplosionType = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxIntensity, ref target3, hookCtx, false, context))
      target3 = this.MaxIntensity;
    target.MaxIntensity = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.IntensitySlope, ref target4, hookCtx, false, context))
      target4 = this.IntensitySlope;
    target.IntensitySlope = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TotalIntensity, ref target5, hookCtx, false, context))
      target5 = this.TotalIntensity;
    target.TotalIntensity = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TileBreakScale, ref target6, hookCtx, false, context))
      target6 = this.TileBreakScale;
    target.TileBreakScale = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxTileBreak, ref target7, hookCtx, false, context))
      target7 = this.MaxTileBreak;
    target.MaxTileBreak = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanCreateVacuum, ref target8, hookCtx, false, context))
      target8 = this.CanCreateVacuum;
    target.CanCreateVacuum = target8;
    bool? target9 = new bool?();
    if (!serialization.TryCustomCopy<bool?>(this.DeleteAfterExplosion, ref target9, hookCtx, false, context))
      target9 = this.DeleteAfterExplosion;
    target.DeleteAfterExplosion = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.Repeatable, ref target10, hookCtx, false, context))
      target10 = this.Repeatable;
    target.Repeatable = target10;
    float? target11 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.VehicleLightDamage, ref target11, hookCtx, false, context))
      target11 = this.VehicleLightDamage;
    target.VehicleLightDamage = target11;
    float? target12 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.VehicleHeavyDamage, ref target12, hookCtx, false, context))
      target12 = this.VehicleHeavyDamage;
    target.VehicleHeavyDamage = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ExplosiveComponent target,
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
    ExplosiveComponent target1 = (ExplosiveComponent) target;
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
    ExplosiveComponent target1 = (ExplosiveComponent) target;
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
    ExplosiveComponent target1 = (ExplosiveComponent) target;
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
  virtual ExplosiveComponent Component.Instantiate() => new ExplosiveComponent();
}
