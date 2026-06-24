// Decompiled with JetBrains decompiler
// Type: Content.Shared.Zombies.PendingZombieComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Zombies;

[RegisterComponent]
[NetworkedComponent]
public sealed class PendingZombieComponent : 
  Component,
  ISerializationGenerated<PendingZombieComponent>,
  ISerializationGenerated
{
  [DataField("damage", false, 1, false, false, null)]
  public DamageSpecifier Damage = new DamageSpecifier()
  {
    DamageDict = new Dictionary<string, FixedPoint2>()
    {
      {
        "Poison",
        (FixedPoint2) 0.4
      }
    }
  };
  [DataField("critDamageMultiplier", false, 1, false, false, null)]
  public float CritDamageMultiplier = 10f;
  [DataField("nextTick", false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan NextTick;
  [DataField("gracePeriod", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan GracePeriod = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan MinInitialInfectedGrace = TimeSpan.FromMinutes(12.5);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan MaxInitialInfectedGrace = TimeSpan.FromMinutes(15.0);
  [DataField("infectionWarningChance", false, 1, false, false, null)]
  public float InfectionWarningChance = 0.0166f;
  [DataField("infectionWarnings", false, 1, false, false, null)]
  public List<string> InfectionWarnings = new List<string>()
  {
    "zombie-infection-warning",
    "zombie-infection-underway"
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PendingZombieComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PendingZombieComponent) target1;
    if (serialization.TryCustomCopy<PendingZombieComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier target2 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target2, hookCtx, false, context))
    {
      if (this.Damage == null)
        target2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target2, hookCtx, context, true);
    }
    target.Damage = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CritDamageMultiplier, ref target3, hookCtx, false, context))
      target3 = this.CritDamageMultiplier;
    target.CritDamageMultiplier = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextTick, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.NextTick, hookCtx, context);
    target.NextTick = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.GracePeriod, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.GracePeriod, hookCtx, context);
    target.GracePeriod = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MinInitialInfectedGrace, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.MinInitialInfectedGrace, hookCtx, context);
    target.MinInitialInfectedGrace = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MaxInitialInfectedGrace, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.MaxInitialInfectedGrace, hookCtx, context);
    target.MaxInitialInfectedGrace = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InfectionWarningChance, ref target8, hookCtx, false, context))
      target8 = this.InfectionWarningChance;
    target.InfectionWarningChance = target8;
    List<string> target9 = (List<string>) null;
    if (this.InfectionWarnings == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.InfectionWarnings, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<List<string>>(this.InfectionWarnings, hookCtx, context);
    target.InfectionWarnings = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PendingZombieComponent target,
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
    PendingZombieComponent target1 = (PendingZombieComponent) target;
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
    PendingZombieComponent target1 = (PendingZombieComponent) target;
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
    PendingZombieComponent target1 = (PendingZombieComponent) target;
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
  virtual PendingZombieComponent Component.Instantiate() => new PendingZombieComponent();
}
