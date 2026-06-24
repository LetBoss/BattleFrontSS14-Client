// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Acid.XenoCorrosiveAcidEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Acid;

public sealed class XenoCorrosiveAcidEvent : 
  EntityTargetActionEvent,
  ISerializationGenerated<XenoCorrosiveAcidEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId AcidId = (EntProtoId) "XenoAcidNormal";
  [DataField(null, false, 1, false, false, null)]
  public XenoAcidStrength Strength = XenoAcidStrength.Normal;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 PlasmaCost = (FixedPoint2) 100;
  [DataField(null, false, 1, false, false, null)]
  public int EnergyCost;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Time = TimeSpan.FromSeconds(225L);
  [DataField(null, false, 1, false, false, null)]
  public float Dps = 8f;
  [DataField(null, false, 1, false, false, null)]
  public float ExpendableLightDps = 2.5f;
  [DataField(null, false, 1, false, false, null)]
  public float ApplyTimeMultiplier = 1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoCorrosiveAcidEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityTargetActionEvent target1 = (EntityTargetActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoCorrosiveAcidEvent) target1;
    if (serialization.TryCustomCopy<XenoCorrosiveAcidEvent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.AcidId, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.AcidId, hookCtx, context);
    target.AcidId = target2;
    XenoAcidStrength target3 = (XenoAcidStrength) 0;
    if (!serialization.TryCustomCopy<XenoAcidStrength>(this.Strength, ref target3, hookCtx, false, context))
      target3 = this.Strength;
    target.Strength = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaCost, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.PlasmaCost, hookCtx, context);
    target.PlasmaCost = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.EnergyCost, ref target5, hookCtx, false, context))
      target5 = this.EnergyCost;
    target.EnergyCost = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Time, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.Time, hookCtx, context);
    target.Time = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Dps, ref target7, hookCtx, false, context))
      target7 = this.Dps;
    target.Dps = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ExpendableLightDps, ref target8, hookCtx, false, context))
      target8 = this.ExpendableLightDps;
    target.ExpendableLightDps = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ApplyTimeMultiplier, ref target9, hookCtx, false, context))
      target9 = this.ApplyTimeMultiplier;
    target.ApplyTimeMultiplier = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoCorrosiveAcidEvent target,
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
    XenoCorrosiveAcidEvent target1 = (XenoCorrosiveAcidEvent) target;
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
    XenoCorrosiveAcidEvent target1 = (XenoCorrosiveAcidEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoCorrosiveAcidEvent EntityTargetActionEvent.Instantiate()
  {
    return new XenoCorrosiveAcidEvent();
  }
}
