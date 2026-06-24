// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Melee.Components.BonusMeleeDamageComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.FixedPoint;
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
public sealed class BonusMeleeDamageComponent : 
  Component,
  ISerializationGenerated<BonusMeleeDamageComponent>,
  ISerializationGenerated
{
  [DataField("bonusDamage", false, 1, false, false, null)]
  public DamageSpecifier? BonusDamage;
  [DataField("damageModifierSet", false, 1, false, false, null)]
  public DamageModifierSet? DamageModifierSet;
  [DataField("heavyDamageFlatModifier", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public FixedPoint2 HeavyDamageFlatModifier;
  [DataField("heavyDamageMultiplier", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float HeavyDamageMultiplier = 1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BonusMeleeDamageComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BonusMeleeDamageComponent) target1;
    if (serialization.TryCustomCopy<BonusMeleeDamageComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier target2 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.BonusDamage, ref target2, hookCtx, false, context))
    {
      if (this.BonusDamage == null)
        target2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.BonusDamage, ref target2, hookCtx, context);
    }
    target.BonusDamage = target2;
    DamageModifierSet target3 = (DamageModifierSet) null;
    if (!serialization.TryCustomCopy<DamageModifierSet>(this.DamageModifierSet, ref target3, hookCtx, true, context))
    {
      if (this.DamageModifierSet == null)
        target3 = (DamageModifierSet) null;
      else
        serialization.CopyTo<DamageModifierSet>(this.DamageModifierSet, ref target3, hookCtx, context);
    }
    target.DamageModifierSet = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.HeavyDamageFlatModifier, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.HeavyDamageFlatModifier, hookCtx, context);
    target.HeavyDamageFlatModifier = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HeavyDamageMultiplier, ref target5, hookCtx, false, context))
      target5 = this.HeavyDamageMultiplier;
    target.HeavyDamageMultiplier = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BonusMeleeDamageComponent target,
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
    BonusMeleeDamageComponent target1 = (BonusMeleeDamageComponent) target;
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
    BonusMeleeDamageComponent target1 = (BonusMeleeDamageComponent) target;
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
    BonusMeleeDamageComponent target1 = (BonusMeleeDamageComponent) target;
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
  virtual BonusMeleeDamageComponent Component.Instantiate() => new BonusMeleeDamageComponent();
}
