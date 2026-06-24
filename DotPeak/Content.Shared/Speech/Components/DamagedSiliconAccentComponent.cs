// Decompiled with JetBrains decompiler
// Type: Content.Shared.Speech.Components.DamagedSiliconAccentComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Speech.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class DamagedSiliconAccentComponent : 
  Component,
  ISerializationGenerated<DamagedSiliconAccentComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool EnableDamageCorruption = true;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2? OverrideTotalDamage;
  [DataField(null, false, 1, false, false, null)]
  public float MaxDamageCorruption = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2? DamageAtMaxCorruption;
  [DataField(null, false, 1, false, false, null)]
  public bool EnableChargeCorruption = true;
  [DataField(null, false, 1, false, false, null)]
  public float? OverrideChargeLevel;
  [DataField(null, false, 1, false, false, null)]
  public float ChargeThresholdForPowerCorruption = 0.15f;
  [DataField(null, false, 1, false, false, null)]
  public int StartPowerCorruptionAtCharIdx = 8;
  [DataField(null, false, 1, false, false, null)]
  public int MaxPowerCorruptionAtCharIdx = 40;
  [DataField(null, false, 1, false, false, null)]
  public float MaxDropProbFromPower = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  public float ProbToCorruptDotFromPower = 0.6f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamagedSiliconAccentComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DamagedSiliconAccentComponent) target1;
    if (serialization.TryCustomCopy<DamagedSiliconAccentComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.EnableDamageCorruption, ref target2, hookCtx, false, context))
      target2 = this.EnableDamageCorruption;
    target.EnableDamageCorruption = target2;
    FixedPoint2? target3 = new FixedPoint2?();
    if (!serialization.TryCustomCopy<FixedPoint2?>(this.OverrideTotalDamage, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2?>(this.OverrideTotalDamage, hookCtx, context);
    target.OverrideTotalDamage = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxDamageCorruption, ref target4, hookCtx, false, context))
      target4 = this.MaxDamageCorruption;
    target.MaxDamageCorruption = target4;
    FixedPoint2? target5 = new FixedPoint2?();
    if (!serialization.TryCustomCopy<FixedPoint2?>(this.DamageAtMaxCorruption, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<FixedPoint2?>(this.DamageAtMaxCorruption, hookCtx, context);
    target.DamageAtMaxCorruption = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.EnableChargeCorruption, ref target6, hookCtx, false, context))
      target6 = this.EnableChargeCorruption;
    target.EnableChargeCorruption = target6;
    float? target7 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.OverrideChargeLevel, ref target7, hookCtx, false, context))
      target7 = this.OverrideChargeLevel;
    target.OverrideChargeLevel = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ChargeThresholdForPowerCorruption, ref target8, hookCtx, false, context))
      target8 = this.ChargeThresholdForPowerCorruption;
    target.ChargeThresholdForPowerCorruption = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.StartPowerCorruptionAtCharIdx, ref target9, hookCtx, false, context))
      target9 = this.StartPowerCorruptionAtCharIdx;
    target.StartPowerCorruptionAtCharIdx = target9;
    int target10 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxPowerCorruptionAtCharIdx, ref target10, hookCtx, false, context))
      target10 = this.MaxPowerCorruptionAtCharIdx;
    target.MaxPowerCorruptionAtCharIdx = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxDropProbFromPower, ref target11, hookCtx, false, context))
      target11 = this.MaxDropProbFromPower;
    target.MaxDropProbFromPower = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ProbToCorruptDotFromPower, ref target12, hookCtx, false, context))
      target12 = this.ProbToCorruptDotFromPower;
    target.ProbToCorruptDotFromPower = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamagedSiliconAccentComponent target,
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
    DamagedSiliconAccentComponent target1 = (DamagedSiliconAccentComponent) target;
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
    DamagedSiliconAccentComponent target1 = (DamagedSiliconAccentComponent) target;
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
    DamagedSiliconAccentComponent target1 = (DamagedSiliconAccentComponent) target;
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
  virtual DamagedSiliconAccentComponent Component.Instantiate()
  {
    return new DamagedSiliconAccentComponent();
  }
}
