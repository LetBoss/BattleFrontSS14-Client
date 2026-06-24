// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Heal.XenoApplySalveActionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Heal;

public sealed class XenoApplySalveActionEvent : 
  EntityTargetActionEvent,
  ISerializationGenerated<XenoApplySalveActionEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Range = 2f;
  [DataField(null, false, 1, false, false, null)]
  public float PlasmaCostModifier = 2f;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 DamageTakenModifier = (FixedPoint2) 0.75f;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 StandardHealAmount = (FixedPoint2) 100f;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 SmallHealAmount = (FixedPoint2) 15f;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan TimeBetweenHeals = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan TotalHealDuration = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId HealEffect = (EntProtoId) "RMCEffectHealHealer";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier HealSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_drool1.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoApplySalveActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityTargetActionEvent target1 = (EntityTargetActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoApplySalveActionEvent) target1;
    if (serialization.TryCustomCopy<XenoApplySalveActionEvent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target2, hookCtx, false, context))
      target2 = this.Range;
    target.Range = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PlasmaCostModifier, ref target3, hookCtx, false, context))
      target3 = this.PlasmaCostModifier;
    target.PlasmaCostModifier = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.DamageTakenModifier, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.DamageTakenModifier, hookCtx, context);
    target.DamageTakenModifier = target4;
    FixedPoint2 target5 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.StandardHealAmount, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<FixedPoint2>(this.StandardHealAmount, hookCtx, context);
    target.StandardHealAmount = target5;
    FixedPoint2 target6 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.SmallHealAmount, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<FixedPoint2>(this.SmallHealAmount, hookCtx, context);
    target.SmallHealAmount = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TimeBetweenHeals, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.TimeBetweenHeals, hookCtx, context);
    target.TimeBetweenHeals = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TotalHealDuration, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.TotalHealDuration, hookCtx, context);
    target.TotalHealDuration = target8;
    EntProtoId target9 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.HealEffect, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntProtoId>(this.HealEffect, hookCtx, context);
    target.HealEffect = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (this.HealSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.HealSound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.HealSound, hookCtx, context);
    target.HealSound = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoApplySalveActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityTargetActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    XenoApplySalveActionEvent target1 = (XenoApplySalveActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityTargetActionEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    XenoApplySalveActionEvent target1 = (XenoApplySalveActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoApplySalveActionEvent EntityTargetActionEvent.Instantiate()
  {
    return new XenoApplySalveActionEvent();
  }
}
