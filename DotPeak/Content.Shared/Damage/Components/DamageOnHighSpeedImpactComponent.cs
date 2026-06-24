// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Components.DamageOnHighSpeedImpactComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Damage.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (DamageOnHighSpeedImpactSystem)})]
public sealed class DamageOnHighSpeedImpactComponent : 
  Component,
  ISerializationGenerated<DamageOnHighSpeedImpactComponent>,
  ISerializationGenerated
{
  [DataField("minimumSpeed", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float MinimumSpeed = 20f;
  [DataField("speedDamageFactor", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float SpeedDamageFactor = 0.5f;
  [DataField("soundHit", false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public SoundSpecifier SoundHit;
  [DataField("stunChance", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float StunChance = 0.25f;
  [DataField("stunMinimumDamage", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public int StunMinimumDamage = 10;
  [DataField("stunSeconds", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float StunSeconds = 1f;
  [DataField("damageCooldown", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float DamageCooldown = 2f;
  [DataField("lastHit", false, 1, false, false, typeof (TimeOffsetSerializer))]
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan? LastHit;
  [DataField("damage", false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public DamageSpecifier Damage;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageOnHighSpeedImpactComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DamageOnHighSpeedImpactComponent) component;
    if (serialization.TryCustomCopy<DamageOnHighSpeedImpactComponent>(this, ref target, hookCtx, false, context))
      return;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinimumSpeed, ref num1, hookCtx, false, context))
      num1 = this.MinimumSpeed;
    target.MinimumSpeed = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedDamageFactor, ref num2, hookCtx, false, context))
      num2 = this.SpeedDamageFactor;
    target.SpeedDamageFactor = num2;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (this.SoundHit == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundHit, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.SoundHit, hookCtx, context, false);
    target.SoundHit = soundSpecifier;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StunChance, ref num3, hookCtx, false, context))
      num3 = this.StunChance;
    target.StunChance = num3;
    int num4 = 0;
    if (!serialization.TryCustomCopy<int>(this.StunMinimumDamage, ref num4, hookCtx, false, context))
      num4 = this.StunMinimumDamage;
    target.StunMinimumDamage = num4;
    float num5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StunSeconds, ref num5, hookCtx, false, context))
      num5 = this.StunSeconds;
    target.StunSeconds = num5;
    float num6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DamageCooldown, ref num6, hookCtx, false, context))
      num6 = this.DamageCooldown;
    target.DamageCooldown = num6;
    TimeSpan? nullable = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.LastHit, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<TimeSpan?>(this.LastHit, hookCtx, context, false);
    target.LastHit = nullable;
    DamageSpecifier damageSpecifier = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref damageSpecifier, hookCtx, false, context))
    {
      if (this.Damage == null)
        damageSpecifier = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref damageSpecifier, hookCtx, context, true);
    }
    target.Damage = damageSpecifier;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageOnHighSpeedImpactComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DamageOnHighSpeedImpactComponent target1 = (DamageOnHighSpeedImpactComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DamageOnHighSpeedImpactComponent target1 = (DamageOnHighSpeedImpactComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DamageOnHighSpeedImpactComponent target1 = (DamageOnHighSpeedImpactComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual DamageOnHighSpeedImpactComponent Component.Instantiate()
  {
    return new DamageOnHighSpeedImpactComponent();
  }
}
