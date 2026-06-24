// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.RMCVehicleDamageScaleRule
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[DataDefinition]
public sealed class RMCVehicleDamageScaleRule : 
  ISerializationGenerated<RMCVehicleDamageScaleRule>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public List<string> DamageTypes = new List<string>();
  [DataField(null, false, 1, true, false, null)]
  public FixedPoint2 MaxDamage;
  [DataField(null, false, 1, true, false, null)]
  public float Multiplier = 1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCVehicleDamageScaleRule target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<RMCVehicleDamageScaleRule>(this, ref target, hookCtx, false, context))
      return;
    List<string> target1 = (List<string>) null;
    if (this.DamageTypes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.DamageTypes, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<List<string>>(this.DamageTypes, hookCtx, context);
    target.DamageTypes = target1;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MaxDamage, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.MaxDamage, hookCtx, context);
    target.MaxDamage = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Multiplier, ref target3, hookCtx, false, context))
      target3 = this.Multiplier;
    target.Multiplier = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCVehicleDamageScaleRule target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCVehicleDamageScaleRule target1 = (RMCVehicleDamageScaleRule) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RMCVehicleDamageScaleRule Instantiate() => new RMCVehicleDamageScaleRule();
}
