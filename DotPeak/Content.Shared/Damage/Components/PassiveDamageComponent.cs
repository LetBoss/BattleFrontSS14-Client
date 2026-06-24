// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Components.PassiveDamageComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Damage.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class PassiveDamageComponent : 
  Component,
  ISerializationGenerated<PassiveDamageComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public List<MobState> AllowedStates = new List<MobState>();
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float Interval = 1f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public FixedPoint2 DamageCap = (FixedPoint2) 0;
  [DataField("nextDamage", false, 1, false, false, typeof (TimeOffsetSerializer))]
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan NextDamage = TimeSpan.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PassiveDamageComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (PassiveDamageComponent) component;
    if (serialization.TryCustomCopy<PassiveDamageComponent>(this, ref target, hookCtx, false, context))
      return;
    List<MobState> mobStateList = (List<MobState>) null;
    if (this.AllowedStates == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<MobState>>(this.AllowedStates, ref mobStateList, hookCtx, true, context))
      mobStateList = serialization.CreateCopy<List<MobState>>(this.AllowedStates, hookCtx, context, false);
    target.AllowedStates = mobStateList;
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
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Interval, ref num, hookCtx, false, context))
      num = this.Interval;
    target.Interval = num;
    FixedPoint2 fixedPoint2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.DamageCap, ref fixedPoint2, hookCtx, false, context))
      fixedPoint2 = serialization.CreateCopy<FixedPoint2>(this.DamageCap, hookCtx, context, false);
    target.DamageCap = fixedPoint2;
    TimeSpan timeSpan = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextDamage, ref timeSpan, hookCtx, false, context))
      timeSpan = serialization.CreateCopy<TimeSpan>(this.NextDamage, hookCtx, context, false);
    target.NextDamage = timeSpan;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PassiveDamageComponent target,
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
    PassiveDamageComponent target1 = (PassiveDamageComponent) target;
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
    PassiveDamageComponent target1 = (PassiveDamageComponent) target;
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
    PassiveDamageComponent target1 = (PassiveDamageComponent) target;
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
  virtual PassiveDamageComponent Component.Instantiate() => new PassiveDamageComponent();
}
