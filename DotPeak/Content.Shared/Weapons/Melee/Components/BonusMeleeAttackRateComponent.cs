// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Melee.Components.BonusMeleeAttackRateComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Melee.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedMeleeWeaponSystem)})]
public sealed class BonusMeleeAttackRateComponent : 
  Component,
  ISerializationGenerated<BonusMeleeAttackRateComponent>,
  ISerializationGenerated
{
  [DataField("flatModifier", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float FlatModifier;
  [DataField("multiplier", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float Multiplier = 1f;
  [DataField("heavyWindupFlatModifier", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float HeavyWindupFlatModifier;
  [DataField("heavyWindupMultiplier", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float HeavyWindupMultiplier = 1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BonusMeleeAttackRateComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BonusMeleeAttackRateComponent) target1;
    if (serialization.TryCustomCopy<BonusMeleeAttackRateComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FlatModifier, ref target2, hookCtx, false, context))
      target2 = this.FlatModifier;
    target.FlatModifier = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Multiplier, ref target3, hookCtx, false, context))
      target3 = this.Multiplier;
    target.Multiplier = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HeavyWindupFlatModifier, ref target4, hookCtx, false, context))
      target4 = this.HeavyWindupFlatModifier;
    target.HeavyWindupFlatModifier = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HeavyWindupMultiplier, ref target5, hookCtx, false, context))
      target5 = this.HeavyWindupMultiplier;
    target.HeavyWindupMultiplier = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BonusMeleeAttackRateComponent target,
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
    BonusMeleeAttackRateComponent target1 = (BonusMeleeAttackRateComponent) target;
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
    BonusMeleeAttackRateComponent target1 = (BonusMeleeAttackRateComponent) target;
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
    BonusMeleeAttackRateComponent target1 = (BonusMeleeAttackRateComponent) target;
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
  virtual BonusMeleeAttackRateComponent Component.Instantiate()
  {
    return new BonusMeleeAttackRateComponent();
  }
}
