// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.ForceSay.DamageForceSayComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Prototypes;
using Content.Shared.Dataset;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Damage.ForceSay;

[RegisterComponent]
[NetworkedComponent]
public sealed class DamageForceSayComponent : 
  Component,
  ISerializationGenerated<DamageForceSayComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public LocId ForceSayMessageWrap = LocId.op_Implicit("damage-force-say-message-wrap");
  [DataField(null, false, 1, false, false, null)]
  public LocId ForceSayMessageWrapNoSuffix = LocId.op_Implicit("damage-force-say-message-wrap-no-suffix");
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<LocalizedDatasetPrototype> ForceSayStringDataset = ProtoId<LocalizedDatasetPrototype>.op_Implicit(nameof (ForceSayStringDataset));
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 DamageThreshold = FixedPoint2.New(5);
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<DamageGroupPrototype>>? ValidDamageGroups = new HashSet<ProtoId<DamageGroupPrototype>>()
  {
    ProtoId<DamageGroupPrototype>.op_Implicit("Brute"),
    ProtoId<DamageGroupPrototype>.op_Implicit("Burn")
  };
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(5.0);
  public TimeSpan? NextAllowedTime;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageForceSayComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DamageForceSayComponent) component;
    if (serialization.TryCustomCopy<DamageForceSayComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId locId1 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ForceSayMessageWrap, ref locId1, hookCtx, false, context))
      locId1 = serialization.CreateCopy<LocId>(this.ForceSayMessageWrap, hookCtx, context, false);
    target.ForceSayMessageWrap = locId1;
    LocId locId2 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ForceSayMessageWrapNoSuffix, ref locId2, hookCtx, false, context))
      locId2 = serialization.CreateCopy<LocId>(this.ForceSayMessageWrapNoSuffix, hookCtx, context, false);
    target.ForceSayMessageWrapNoSuffix = locId2;
    ProtoId<LocalizedDatasetPrototype> protoId = new ProtoId<LocalizedDatasetPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<LocalizedDatasetPrototype>>(this.ForceSayStringDataset, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<LocalizedDatasetPrototype>>(this.ForceSayStringDataset, hookCtx, context, false);
    target.ForceSayStringDataset = protoId;
    FixedPoint2 fixedPoint2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.DamageThreshold, ref fixedPoint2, hookCtx, false, context))
      fixedPoint2 = serialization.CreateCopy<FixedPoint2>(this.DamageThreshold, hookCtx, context, false);
    target.DamageThreshold = fixedPoint2;
    HashSet<ProtoId<DamageGroupPrototype>> protoIdSet = (HashSet<ProtoId<DamageGroupPrototype>>) null;
    if (!serialization.TryCustomCopy<HashSet<ProtoId<DamageGroupPrototype>>>(this.ValidDamageGroups, ref protoIdSet, hookCtx, true, context))
      protoIdSet = serialization.CreateCopy<HashSet<ProtoId<DamageGroupPrototype>>>(this.ValidDamageGroups, hookCtx, context, false);
    target.ValidDamageGroups = protoIdSet;
    TimeSpan timeSpan = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref timeSpan, hookCtx, false, context))
      timeSpan = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context, false);
    target.Cooldown = timeSpan;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageForceSayComponent target,
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
    DamageForceSayComponent target1 = (DamageForceSayComponent) target;
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
    DamageForceSayComponent target1 = (DamageForceSayComponent) target;
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
    DamageForceSayComponent target1 = (DamageForceSayComponent) target;
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
  virtual DamageForceSayComponent Component.Instantiate() => new DamageForceSayComponent();
}
