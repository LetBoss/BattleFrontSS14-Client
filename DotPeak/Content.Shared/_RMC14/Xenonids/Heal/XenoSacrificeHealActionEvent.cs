// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Heal.XenoSacrificeHealActionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.FixedPoint;
using Content.Shared.StatusEffect;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Heal;

public sealed class XenoSacrificeHealActionEvent : 
  EntityTargetActionEvent,
  ISerializationGenerated<XenoSacrificeHealActionEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Range = 2f;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 TransferProportion = (FixedPoint2) 0.75;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan RespawnDelay = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId HealEffect = (EntProtoId) "RMCEffectHealSacrifice";
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<StatusEffectPrototype>[] AilmentsRemove = new ProtoId<StatusEffectPrototype>[4]
  {
    (ProtoId<StatusEffectPrototype>) "KnockedDown",
    (ProtoId<StatusEffectPrototype>) "Stun",
    (ProtoId<StatusEffectPrototype>) "Dazed",
    (ProtoId<StatusEffectPrototype>) "Unconscious"
  };
  [DataField(null, false, 1, false, false, null)]
  public ComponentRegistry ComponentsRemove;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoSacrificeHealActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityTargetActionEvent target1 = (EntityTargetActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoSacrificeHealActionEvent) target1;
    if (serialization.TryCustomCopy<XenoSacrificeHealActionEvent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target2, hookCtx, false, context))
      target2 = this.Range;
    target.Range = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.TransferProportion, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.TransferProportion, hookCtx, context);
    target.TransferProportion = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RespawnDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.RespawnDelay, hookCtx, context);
    target.RespawnDelay = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.HealEffect, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.HealEffect, hookCtx, context);
    target.HealEffect = target5;
    ProtoId<StatusEffectPrototype>[] target6 = (ProtoId<StatusEffectPrototype>[]) null;
    if (this.AilmentsRemove == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ProtoId<StatusEffectPrototype>[]>(this.AilmentsRemove, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<ProtoId<StatusEffectPrototype>[]>(this.AilmentsRemove, hookCtx, context);
    target.AilmentsRemove = target6;
    ComponentRegistry target7 = (ComponentRegistry) null;
    if (this.ComponentsRemove == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.ComponentsRemove, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<ComponentRegistry>(this.ComponentsRemove, hookCtx, context);
    target.ComponentsRemove = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoSacrificeHealActionEvent target,
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
    XenoSacrificeHealActionEvent target1 = (XenoSacrificeHealActionEvent) target;
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
    XenoSacrificeHealActionEvent target1 = (XenoSacrificeHealActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoSacrificeHealActionEvent EntityTargetActionEvent.Instantiate()
  {
    return new XenoSacrificeHealActionEvent();
  }
}
