// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.DamageableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.StatusIcon;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Damage;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (DamageableSystem)})]
public sealed class DamageableComponent : 
  Component,
  ISerializationGenerated<DamageableComponent>,
  ISerializationGenerated
{
  [DataField("damageContainer", false, 1, false, false, null)]
  public ProtoId<DamageContainerPrototype>? DamageContainerID;
  [DataField("damageModifierSet", false, 1, false, false, null)]
  public ProtoId<DamageModifierSetPrototype>? DamageModifierSetId;
  [DataField(null, true, 1, false, false, null)]
  public DamageSpecifier Damage = new DamageSpecifier();
  [Robust.Shared.ViewVariables.ViewVariables]
  public Dictionary<string, FixedPoint2> DamagePerGroup = new Dictionary<string, FixedPoint2>();
  [Robust.Shared.ViewVariables.ViewVariables]
  public FixedPoint2 TotalDamage;
  [DataField("radiationDamageTypes", false, 1, false, false, null)]
  public List<ProtoId<DamageTypePrototype>> RadiationDamageTypeIDs = new List<ProtoId<DamageTypePrototype>>()
  {
    ProtoId<DamageTypePrototype>.op_Implicit("Radiation")
  };
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<DamageGroupPrototype>> PainDamageGroups = new List<ProtoId<DamageGroupPrototype>>()
  {
    ProtoId<DamageGroupPrototype>.op_Implicit("Brute"),
    ProtoId<DamageGroupPrototype>.op_Implicit("Burn")
  };
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<MobState, ProtoId<HealthIconPrototype>> HealthIcons = new Dictionary<MobState, ProtoId<HealthIconPrototype>>()
  {
    {
      MobState.Alive,
      ProtoId<HealthIconPrototype>.op_Implicit("HealthIconFine")
    },
    {
      MobState.Critical,
      ProtoId<HealthIconPrototype>.op_Implicit("HealthIconCritical")
    },
    {
      MobState.Dead,
      ProtoId<HealthIconPrototype>.op_Implicit("HealthIconDead")
    }
  };
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<HealthIconPrototype> RottingIcon = ProtoId<HealthIconPrototype>.op_Implicit("HealthIconRotting");
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2? HealthBarThreshold;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DamageableComponent) component;
    if (serialization.TryCustomCopy<DamageableComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<DamageContainerPrototype>? nullable1 = new ProtoId<DamageContainerPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<DamageContainerPrototype>?>(this.DamageContainerID, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<ProtoId<DamageContainerPrototype>?>(this.DamageContainerID, hookCtx, context, false);
    target.DamageContainerID = nullable1;
    ProtoId<DamageModifierSetPrototype>? nullable2 = new ProtoId<DamageModifierSetPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<DamageModifierSetPrototype>?>(this.DamageModifierSetId, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<ProtoId<DamageModifierSetPrototype>?>(this.DamageModifierSetId, hookCtx, context, false);
    target.DamageModifierSetId = nullable2;
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
    List<ProtoId<DamageTypePrototype>> protoIdList1 = (List<ProtoId<DamageTypePrototype>>) null;
    if (this.RadiationDamageTypeIDs == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<DamageTypePrototype>>>(this.RadiationDamageTypeIDs, ref protoIdList1, hookCtx, true, context))
      protoIdList1 = serialization.CreateCopy<List<ProtoId<DamageTypePrototype>>>(this.RadiationDamageTypeIDs, hookCtx, context, false);
    target.RadiationDamageTypeIDs = protoIdList1;
    List<ProtoId<DamageGroupPrototype>> protoIdList2 = (List<ProtoId<DamageGroupPrototype>>) null;
    if (this.PainDamageGroups == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<DamageGroupPrototype>>>(this.PainDamageGroups, ref protoIdList2, hookCtx, true, context))
      protoIdList2 = serialization.CreateCopy<List<ProtoId<DamageGroupPrototype>>>(this.PainDamageGroups, hookCtx, context, false);
    target.PainDamageGroups = protoIdList2;
    Dictionary<MobState, ProtoId<HealthIconPrototype>> dictionary = (Dictionary<MobState, ProtoId<HealthIconPrototype>>) null;
    if (this.HealthIcons == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<MobState, ProtoId<HealthIconPrototype>>>(this.HealthIcons, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<MobState, ProtoId<HealthIconPrototype>>>(this.HealthIcons, hookCtx, context, false);
    target.HealthIcons = dictionary;
    ProtoId<HealthIconPrototype> protoId = new ProtoId<HealthIconPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<HealthIconPrototype>>(this.RottingIcon, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<HealthIconPrototype>>(this.RottingIcon, hookCtx, context, false);
    target.RottingIcon = protoId;
    FixedPoint2? nullable3 = new FixedPoint2?();
    if (!serialization.TryCustomCopy<FixedPoint2?>(this.HealthBarThreshold, ref nullable3, hookCtx, false, context))
      nullable3 = serialization.CreateCopy<FixedPoint2?>(this.HealthBarThreshold, hookCtx, context, false);
    target.HealthBarThreshold = nullable3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageableComponent target,
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
    DamageableComponent target1 = (DamageableComponent) target;
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
    DamageableComponent target1 = (DamageableComponent) target;
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
    DamageableComponent target1 = (DamageableComponent) target;
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
  virtual DamageableComponent Component.Instantiate() => new DamageableComponent();
}
