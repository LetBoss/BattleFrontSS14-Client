// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.DamageableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Damage;
using Content.Shared.CCVar;
using Content.Shared.Chemistry;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Radiation.Events;
using Content.Shared.Rejuvenate;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Damage;

public sealed class DamageableSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private INetManager _netMan;
  [Dependency]
  private MobThresholdSystem _mobThreshold;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private SharedChemistryGuideDataSystem _chemistryGuideData;
  private EntityQuery<AppearanceComponent> _appearanceQuery;
  private EntityQuery<DamageableComponent> _damageableQuery;
  private EntityQuery<MindContainerComponent> _mindContainerQuery;

  public float UniversalAllDamageModifier { get; private set; } = 1f;

  public float UniversalAllHealModifier { get; private set; } = 1f;

  public float UniversalMeleeDamageModifier { get; private set; } = 1f;

  public float UniversalProjectileDamageModifier { get; private set; } = 1f;

  public float UniversalHitscanDamageModifier { get; private set; } = 1f;

  public float UniversalReagentDamageModifier { get; private set; } = 1f;

  public float UniversalReagentHealModifier { get; private set; } = 1f;

  public float UniversalExplosionDamageModifier { get; private set; } = 1f;

  public float UniversalThrownDamageModifier { get; private set; } = 1f;

  public float UniversalTopicalsHealModifier { get; private set; } = 1f;

  public float UniversalMobDamageModifier { get; private set; } = 1f;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DamageableComponent, ComponentInit>(new ComponentEventHandler<DamageableComponent, ComponentInit>((object) this, __methodptr(DamageableInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DamageableComponent, ComponentHandleState>(new ComponentEventRefHandler<DamageableComponent, ComponentHandleState>((object) this, __methodptr(DamageableHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DamageableComponent, ComponentGetState>(new ComponentEventRefHandler<DamageableComponent, ComponentGetState>((object) this, __methodptr(DamageableGetState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DamageableComponent, OnIrradiatedEvent>(new ComponentEventHandler<DamageableComponent, OnIrradiatedEvent>((object) this, __methodptr(OnIrradiated)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DamageableComponent, RejuvenateEvent>(new ComponentEventHandler<DamageableComponent, RejuvenateEvent>((object) this, __methodptr(OnRejuvenate)), (Type[]) null, (Type[]) null);
    this._appearanceQuery = this.GetEntityQuery<AppearanceComponent>();
    this._damageableQuery = this.GetEntityQuery<DamageableComponent>();
    this._mindContainerQuery = this.GetEntityQuery<MindContainerComponent>();
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._config, CCVars.PlaytestAllDamageModifier, (Action<float>) (value =>
    {
      this.UniversalAllDamageModifier = value;
      this._chemistryGuideData.ReloadAllReagentPrototypes();
    }), true);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._config, CCVars.PlaytestAllHealModifier, (Action<float>) (value =>
    {
      this.UniversalAllHealModifier = value;
      this._chemistryGuideData.ReloadAllReagentPrototypes();
    }), true);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._config, CCVars.PlaytestProjectileDamageModifier, (Action<float>) (value => this.UniversalProjectileDamageModifier = value), true);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._config, CCVars.PlaytestMeleeDamageModifier, (Action<float>) (value => this.UniversalMeleeDamageModifier = value), true);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._config, CCVars.PlaytestProjectileDamageModifier, (Action<float>) (value => this.UniversalProjectileDamageModifier = value), true);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._config, CCVars.PlaytestHitscanDamageModifier, (Action<float>) (value => this.UniversalHitscanDamageModifier = value), true);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._config, CCVars.PlaytestReagentDamageModifier, (Action<float>) (value =>
    {
      this.UniversalReagentDamageModifier = value;
      this._chemistryGuideData.ReloadAllReagentPrototypes();
    }), true);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._config, CCVars.PlaytestReagentHealModifier, (Action<float>) (value =>
    {
      this.UniversalReagentHealModifier = value;
      this._chemistryGuideData.ReloadAllReagentPrototypes();
    }), true);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._config, CCVars.PlaytestExplosionDamageModifier, (Action<float>) (value => this.UniversalExplosionDamageModifier = value), true);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._config, CCVars.PlaytestThrownDamageModifier, (Action<float>) (value => this.UniversalThrownDamageModifier = value), true);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._config, CCVars.PlaytestTopicalsHealModifier, (Action<float>) (value => this.UniversalTopicalsHealModifier = value), true);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._config, CCVars.PlaytestMobDamageModifier, (Action<float>) (value => this.UniversalMobDamageModifier = value), true);
  }

  private void DamageableInit(EntityUid uid, DamageableComponent component, ComponentInit _)
  {
    DamageContainerPrototype containerPrototype;
    if (component.DamageContainerID.HasValue && this._prototypeManager.TryIndex<DamageContainerPrototype>(component.DamageContainerID, ref containerPrototype))
    {
      foreach (string supportedType in containerPrototype.SupportedTypes)
        component.Damage.DamageDict.TryAdd(supportedType, FixedPoint2.Zero);
      foreach (string supportedGroup in containerPrototype.SupportedGroups)
      {
        foreach (string damageType in this._prototypeManager.Index<DamageGroupPrototype>(supportedGroup).DamageTypes)
          component.Damage.DamageDict.TryAdd(damageType, FixedPoint2.Zero);
      }
    }
    else
    {
      foreach (DamageTypePrototype enumeratePrototype in this._prototypeManager.EnumeratePrototypes<DamageTypePrototype>())
        component.Damage.DamageDict.TryAdd(enumeratePrototype.ID, FixedPoint2.Zero);
    }
    component.Damage.GetDamagePerGroup(this._prototypeManager, component.DamagePerGroup);
    component.TotalDamage = component.Damage.GetTotal();
  }

  public void SetDamage(EntityUid uid, DamageableComponent damageable, DamageSpecifier damage)
  {
    damageable.Damage = damage;
    this.DamageChanged(uid, damageable);
  }

  public void AddDamage(EntityUid uid, DamageableComponent damageable, DamageSpecifier damage)
  {
    damageable.Damage += damage;
    this.DamageChanged(uid, damageable, interruptsDoAfters: false);
  }

  public void DamageChanged(
    EntityUid uid,
    DamageableComponent component,
    DamageSpecifier? damageDelta = null,
    bool interruptsDoAfters = true,
    EntityUid? origin = null,
    EntityUid? tool = null)
  {
    component.Damage.GetDamagePerGroup(this._prototypeManager, component.DamagePerGroup);
    component.TotalDamage = component.Damage.GetTotal();
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    AppearanceComponent appearanceComponent;
    if (this._appearanceQuery.TryGetComponent(uid, ref appearanceComponent) && damageDelta != null)
    {
      DamageVisualizerGroupData visualizerGroupData = new DamageVisualizerGroupData(component.DamagePerGroup.Keys.ToList<string>());
      this._appearance.SetData(uid, (Enum) DamageVisualizerKeys.DamageUpdateGroups, (object) visualizerGroupData, appearanceComponent);
    }
    this.RaiseLocalEvent<DamageChangedEvent>(uid, new DamageChangedEvent(component, damageDelta, interruptsDoAfters, origin, tool), false);
  }

  public DamageSpecifier? TryChangeDamage(
    EntityUid? uid,
    DamageSpecifier damage,
    bool ignoreResistances = false,
    bool interruptsDoAfters = true,
    DamageableComponent? damageable = null,
    EntityUid? origin = null,
    EntityUid? tool = null,
    int armorPiercing = 0)
  {
    if (!uid.HasValue || !this._damageableQuery.Resolve(uid.Value, ref damageable, false))
      return (DamageSpecifier) null;
    if (damage.Empty)
      return damage;
    BeforeDamageChangedEvent damageChangedEvent = new BeforeDamageChangedEvent(damage, origin);
    this.RaiseLocalEvent<BeforeDamageChangedEvent>(uid.Value, ref damageChangedEvent, false);
    if (damageChangedEvent.Cancelled)
      return (DamageSpecifier) null;
    if (!ignoreResistances)
    {
      DamageModifierSetPrototype modifierSet;
      if (damageable.DamageModifierSetId.HasValue && this._prototypeManager.TryIndex<DamageModifierSetPrototype>(damageable.DamageModifierSetId, ref modifierSet))
        damage = DamageSpecifier.ApplyModifierSet(damage, (DamageModifierSet) modifierSet);
      DamageModifyEvent damageModifyEvent = new DamageModifyEvent(damage, origin, tool, armorPiercing);
      this.RaiseLocalEvent<DamageModifyEvent>(uid.Value, damageModifyEvent, false);
      damage = damageModifyEvent.Damage;
      if (damage.Empty)
        return damage;
    }
    DamageModifyAfterResistEvent afterResistEvent = new DamageModifyAfterResistEvent(damage, origin, tool);
    this.RaiseLocalEvent<DamageModifyAfterResistEvent>(uid.Value, afterResistEvent, false);
    damage = afterResistEvent.Damage;
    if (damage.Empty)
      return damage;
    damage = this.ApplyUniversalAllModifiers(damage);
    DamageSpecifier damageDelta = new DamageSpecifier();
    damageDelta.DamageDict.EnsureCapacity(damage.DamageDict.Count);
    Dictionary<string, FixedPoint2> damageDict = damageable.Damage.DamageDict;
    foreach ((string key, FixedPoint2 fixedPoint2_1) in damage.DamageDict)
    {
      FixedPoint2 fixedPoint2_2;
      if (damageDict.TryGetValue(key, out fixedPoint2_2))
      {
        FixedPoint2 fixedPoint2_3 = FixedPoint2.Max(FixedPoint2.Zero, fixedPoint2_2 + fixedPoint2_1);
        if (!(fixedPoint2_3 == fixedPoint2_2))
        {
          damageDict[key] = fixedPoint2_3;
          damageDelta.DamageDict[key] = fixedPoint2_3 - fixedPoint2_2;
        }
      }
    }
    if (damageDelta.DamageDict.Count > 0)
      this.DamageChanged(uid.Value, damageable, damageDelta, interruptsDoAfters, origin, tool);
    return damageDelta;
  }

  public DamageSpecifier ApplyUniversalAllModifiers(DamageSpecifier damage)
  {
    if ((double) this.UniversalAllDamageModifier == 1.0 && (double) this.UniversalAllHealModifier == 1.0)
      return damage;
    foreach ((string key, FixedPoint2 fixedPoint2_1) in damage.DamageDict)
    {
      string str = key;
      FixedPoint2 fixedPoint2_2 = fixedPoint2_1;
      if (!(fixedPoint2_2 == 0))
      {
        if (fixedPoint2_2 > 0)
        {
          Dictionary<string, FixedPoint2> damageDict = damage.DamageDict;
          key = str;
          damageDict[key] *= this.UniversalAllDamageModifier;
        }
        else if (fixedPoint2_2 < 0)
        {
          Dictionary<string, FixedPoint2> damageDict = damage.DamageDict;
          key = str;
          damageDict[key] *= this.UniversalAllHealModifier;
        }
      }
    }
    return damage;
  }

  public void SetAllDamage(EntityUid uid, DamageableComponent component, FixedPoint2 newValue)
  {
    if (newValue < 0)
      return;
    foreach (string key in component.Damage.DamageDict.Keys)
      component.Damage.DamageDict[key] = newValue;
    this.DamageChanged(uid, component, new DamageSpecifier());
  }

  public void SetDamageModifierSetId(
    EntityUid uid,
    string? damageModifierSetId,
    DamageableComponent? comp = null)
  {
    if (!this._damageableQuery.Resolve(uid, ref comp, true))
      return;
    comp.DamageModifierSetId = ProtoId<DamageModifierSetPrototype>.op_Implicit(damageModifierSetId);
    this.Dirty(uid, (IComponent) comp, (MetaDataComponent) null);
  }

  private void DamageableGetState(
    EntityUid uid,
    DamageableComponent component,
    ref ComponentGetState args)
  {
    if (this._netMan.IsServer)
    {
      ref ComponentGetState local = ref args;
      Dictionary<string, FixedPoint2> damageDict = component.Damage.DamageDict;
      ProtoId<DamageContainerPrototype>? damageContainerId1 = component.DamageContainerID;
      string damageContainerId2 = damageContainerId1.HasValue ? ProtoId<DamageContainerPrototype>.op_Implicit(damageContainerId1.GetValueOrDefault()) : (string) null;
      ProtoId<DamageModifierSetPrototype>? damageModifierSetId = component.DamageModifierSetId;
      string modifierSetId = damageModifierSetId.HasValue ? ProtoId<DamageModifierSetPrototype>.op_Implicit(damageModifierSetId.GetValueOrDefault()) : (string) null;
      FixedPoint2? healthBarThreshold = component.HealthBarThreshold;
      DamageableComponentState damageableComponentState = new DamageableComponentState(damageDict, damageContainerId2, modifierSetId, healthBarThreshold);
      ((ComponentGetState) ref local).State = (IComponentState) damageableComponentState;
    }
    else
    {
      ref ComponentGetState local = ref args;
      Dictionary<string, FixedPoint2> damageDict = Extensions.ShallowClone<string, FixedPoint2>(component.Damage.DamageDict);
      ProtoId<DamageContainerPrototype>? damageContainerId3 = component.DamageContainerID;
      string damageContainerId4 = damageContainerId3.HasValue ? ProtoId<DamageContainerPrototype>.op_Implicit(damageContainerId3.GetValueOrDefault()) : (string) null;
      ProtoId<DamageModifierSetPrototype>? damageModifierSetId = component.DamageModifierSetId;
      string modifierSetId = damageModifierSetId.HasValue ? ProtoId<DamageModifierSetPrototype>.op_Implicit(damageModifierSetId.GetValueOrDefault()) : (string) null;
      FixedPoint2? healthBarThreshold = component.HealthBarThreshold;
      DamageableComponentState damageableComponentState = new DamageableComponentState(damageDict, damageContainerId4, modifierSetId, healthBarThreshold);
      ((ComponentGetState) ref local).State = (IComponentState) damageableComponentState;
    }
  }

  private void OnIrradiated(EntityUid uid, DamageableComponent component, OnIrradiatedEvent args)
  {
    FixedPoint2 fixedPoint2 = FixedPoint2.New(args.TotalRads);
    DamageSpecifier damage = new DamageSpecifier();
    foreach (ProtoId<DamageTypePrototype> radiationDamageTypeId in component.RadiationDamageTypeIDs)
      damage.DamageDict.Add(ProtoId<DamageTypePrototype>.op_Implicit(radiationDamageTypeId), fixedPoint2);
    this.TryChangeDamage(new EntityUid?(uid), damage, interruptsDoAfters: false, origin: new EntityUid?(args.Origin));
  }

  private void OnRejuvenate(EntityUid uid, DamageableComponent component, RejuvenateEvent args)
  {
    MobThresholdsComponent component1;
    this.TryComp<MobThresholdsComponent>(uid, ref component1);
    this._mobThreshold.SetAllowRevives(uid, true, component1);
    this.SetAllDamage(uid, component, (FixedPoint2) 0);
    this._mobThreshold.SetAllowRevives(uid, false, component1);
  }

  private void DamageableHandleState(
    EntityUid uid,
    DamageableComponent component,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is DamageableComponentState current))
      return;
    component.DamageContainerID = ProtoId<DamageContainerPrototype>.op_Implicit(current.DamageContainerId);
    component.DamageModifierSetId = ProtoId<DamageModifierSetPrototype>.op_Implicit(current.ModifierSetId);
    component.HealthBarThreshold = current.HealthBarThreshold;
    DamageSpecifier damageSpecifier = new DamageSpecifier()
    {
      DamageDict = new Dictionary<string, FixedPoint2>((IDictionary<string, FixedPoint2>) current.DamageDict)
    };
    DamageSpecifier damageDelta = damageSpecifier - component.Damage;
    damageDelta.TrimZeros();
    if (damageDelta.Empty)
      return;
    component.Damage = damageSpecifier;
    this.DamageChanged(uid, component, damageDelta);
  }
}
