using System;
using System.Collections.Generic;
using System.Linq;
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

	public override void Initialize()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).SubscribeLocalEvent<DamageableComponent, ComponentInit>((ComponentEventHandler<DamageableComponent, ComponentInit>)DamageableInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DamageableComponent, ComponentHandleState>((ComponentEventRefHandler<DamageableComponent, ComponentHandleState>)DamageableHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DamageableComponent, ComponentGetState>((ComponentEventRefHandler<DamageableComponent, ComponentGetState>)DamageableGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DamageableComponent, OnIrradiatedEvent>((ComponentEventHandler<DamageableComponent, OnIrradiatedEvent>)OnIrradiated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DamageableComponent, RejuvenateEvent>((ComponentEventHandler<DamageableComponent, RejuvenateEvent>)OnRejuvenate, (Type[])null, (Type[])null);
		_appearanceQuery = ((EntitySystem)this).GetEntityQuery<AppearanceComponent>();
		_damageableQuery = ((EntitySystem)this).GetEntityQuery<DamageableComponent>();
		_mindContainerQuery = ((EntitySystem)this).GetEntityQuery<MindContainerComponent>();
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, CCVars.PlaytestAllDamageModifier, (Action<float>)delegate(float value)
		{
			UniversalAllDamageModifier = value;
			_chemistryGuideData.ReloadAllReagentPrototypes();
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, CCVars.PlaytestAllHealModifier, (Action<float>)delegate(float value)
		{
			UniversalAllHealModifier = value;
			_chemistryGuideData.ReloadAllReagentPrototypes();
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, CCVars.PlaytestProjectileDamageModifier, (Action<float>)delegate(float value)
		{
			UniversalProjectileDamageModifier = value;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, CCVars.PlaytestMeleeDamageModifier, (Action<float>)delegate(float value)
		{
			UniversalMeleeDamageModifier = value;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, CCVars.PlaytestProjectileDamageModifier, (Action<float>)delegate(float value)
		{
			UniversalProjectileDamageModifier = value;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, CCVars.PlaytestHitscanDamageModifier, (Action<float>)delegate(float value)
		{
			UniversalHitscanDamageModifier = value;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, CCVars.PlaytestReagentDamageModifier, (Action<float>)delegate(float value)
		{
			UniversalReagentDamageModifier = value;
			_chemistryGuideData.ReloadAllReagentPrototypes();
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, CCVars.PlaytestReagentHealModifier, (Action<float>)delegate(float value)
		{
			UniversalReagentHealModifier = value;
			_chemistryGuideData.ReloadAllReagentPrototypes();
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, CCVars.PlaytestExplosionDamageModifier, (Action<float>)delegate(float value)
		{
			UniversalExplosionDamageModifier = value;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, CCVars.PlaytestThrownDamageModifier, (Action<float>)delegate(float value)
		{
			UniversalThrownDamageModifier = value;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, CCVars.PlaytestTopicalsHealModifier, (Action<float>)delegate(float value)
		{
			UniversalTopicalsHealModifier = value;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, CCVars.PlaytestMobDamageModifier, (Action<float>)delegate(float value)
		{
			UniversalMobDamageModifier = value;
		}, true);
	}

	private void DamageableInit(EntityUid uid, DamageableComponent component, ComponentInit _)
	{
		DamageContainerPrototype damageContainerPrototype = default(DamageContainerPrototype);
		if (component.DamageContainerID.HasValue && _prototypeManager.TryIndex<DamageContainerPrototype>(component.DamageContainerID, ref damageContainerPrototype))
		{
			foreach (string type in damageContainerPrototype.SupportedTypes)
			{
				component.Damage.DamageDict.TryAdd(type, FixedPoint2.Zero);
			}
			foreach (string groupId in damageContainerPrototype.SupportedGroups)
			{
				foreach (string type2 in _prototypeManager.Index<DamageGroupPrototype>(groupId).DamageTypes)
				{
					component.Damage.DamageDict.TryAdd(type2, FixedPoint2.Zero);
				}
			}
		}
		else
		{
			foreach (DamageTypePrototype type3 in _prototypeManager.EnumeratePrototypes<DamageTypePrototype>())
			{
				component.Damage.DamageDict.TryAdd(type3.ID, FixedPoint2.Zero);
			}
		}
		component.Damage.GetDamagePerGroup(_prototypeManager, component.DamagePerGroup);
		component.TotalDamage = component.Damage.GetTotal();
	}

	public void SetDamage(EntityUid uid, DamageableComponent damageable, DamageSpecifier damage)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		damageable.Damage = damage;
		DamageChanged(uid, damageable);
	}

	public void AddDamage(EntityUid uid, DamageableComponent damageable, DamageSpecifier damage)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		damageable.Damage += damage;
		DamageChanged(uid, damageable, null, interruptsDoAfters: false);
	}

	public void DamageChanged(EntityUid uid, DamageableComponent component, DamageSpecifier? damageDelta = null, bool interruptsDoAfters = true, EntityUid? origin = null, EntityUid? tool = null)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		component.Damage.GetDamagePerGroup(_prototypeManager, component.DamagePerGroup);
		component.TotalDamage = component.Damage.GetTotal();
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		AppearanceComponent appearance = default(AppearanceComponent);
		if (_appearanceQuery.TryGetComponent(uid, ref appearance) && damageDelta != null)
		{
			DamageVisualizerGroupData data = new DamageVisualizerGroupData(component.DamagePerGroup.Keys.ToList());
			_appearance.SetData(uid, (Enum)DamageVisualizerKeys.DamageUpdateGroups, (object)data, appearance);
		}
		((EntitySystem)this).RaiseLocalEvent<DamageChangedEvent>(uid, new DamageChangedEvent(component, damageDelta, interruptsDoAfters, origin, tool), false);
	}

	public DamageSpecifier? TryChangeDamage(EntityUid? uid, DamageSpecifier damage, bool ignoreResistances = false, bool interruptsDoAfters = true, DamageableComponent? damageable = null, EntityUid? origin = null, EntityUid? tool = null, int armorPiercing = 0)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		if (!uid.HasValue || !_damageableQuery.Resolve(uid.Value, ref damageable, false))
		{
			return null;
		}
		if (damage.Empty)
		{
			return damage;
		}
		BeforeDamageChangedEvent before = new BeforeDamageChangedEvent(damage, origin);
		((EntitySystem)this).RaiseLocalEvent<BeforeDamageChangedEvent>(uid.Value, ref before, false);
		if (before.Cancelled)
		{
			return null;
		}
		if (!ignoreResistances)
		{
			DamageModifierSetPrototype modifierSet = default(DamageModifierSetPrototype);
			if (damageable.DamageModifierSetId.HasValue && _prototypeManager.TryIndex<DamageModifierSetPrototype>(damageable.DamageModifierSetId, ref modifierSet))
			{
				damage = DamageSpecifier.ApplyModifierSet(damage, modifierSet);
			}
			DamageModifyEvent ev = new DamageModifyEvent(damage, origin, tool, armorPiercing);
			((EntitySystem)this).RaiseLocalEvent<DamageModifyEvent>(uid.Value, ev, false);
			damage = ev.Damage;
			if (damage.Empty)
			{
				return damage;
			}
		}
		DamageModifyAfterResistEvent evd = new DamageModifyAfterResistEvent(damage, origin, tool);
		((EntitySystem)this).RaiseLocalEvent<DamageModifyAfterResistEvent>(uid.Value, evd, false);
		damage = evd.Damage;
		if (damage.Empty)
		{
			return damage;
		}
		damage = ApplyUniversalAllModifiers(damage);
		DamageSpecifier delta = new DamageSpecifier();
		delta.DamageDict.EnsureCapacity(damage.DamageDict.Count);
		Dictionary<string, FixedPoint2> dict = damageable.Damage.DamageDict;
		foreach (var (type, value) in damage.DamageDict)
		{
			if (dict.TryGetValue(type, out var oldValue))
			{
				FixedPoint2 newValue = FixedPoint2.Max(FixedPoint2.Zero, oldValue + value);
				if (!(newValue == oldValue))
				{
					dict[type] = newValue;
					delta.DamageDict[type] = newValue - oldValue;
				}
			}
		}
		if (delta.DamageDict.Count > 0)
		{
			DamageChanged(uid.Value, damageable, delta, interruptsDoAfters, origin, tool);
		}
		return delta;
	}

	public DamageSpecifier ApplyUniversalAllModifiers(DamageSpecifier damage)
	{
		if (UniversalAllDamageModifier == 1f && UniversalAllHealModifier == 1f)
		{
			return damage;
		}
		foreach (KeyValuePair<string, FixedPoint2> item in damage.DamageDict)
		{
			item.Deconstruct(out var key, out var value);
			string key2 = key;
			FixedPoint2 value2 = value;
			if (!(value2 == 0))
			{
				if (value2 > 0)
				{
					Dictionary<string, FixedPoint2> damageDict = damage.DamageDict;
					key = key2;
					damageDict[key] *= UniversalAllDamageModifier;
				}
				else if (value2 < 0)
				{
					Dictionary<string, FixedPoint2> damageDict = damage.DamageDict;
					key = key2;
					damageDict[key] *= UniversalAllHealModifier;
				}
			}
		}
		return damage;
	}

	public void SetAllDamage(EntityUid uid, DamageableComponent component, FixedPoint2 newValue)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (newValue < 0)
		{
			return;
		}
		foreach (string type in component.Damage.DamageDict.Keys)
		{
			component.Damage.DamageDict[type] = newValue;
		}
		DamageChanged(uid, component, new DamageSpecifier());
	}

	public void SetDamageModifierSetId(EntityUid uid, string? damageModifierSetId, DamageableComponent? comp = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (_damageableQuery.Resolve(uid, ref comp, true))
		{
			comp.DamageModifierSetId = ProtoId<DamageModifierSetPrototype>.op_Implicit(damageModifierSetId);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
		}
	}

	private void DamageableGetState(EntityUid uid, DamageableComponent component, ref ComponentGetState args)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (_netMan.IsServer)
		{
			Dictionary<string, FixedPoint2> damageDict = component.Damage.DamageDict;
			ProtoId<DamageContainerPrototype>? damageContainerID = component.DamageContainerID;
			string damageContainerId = (damageContainerID.HasValue ? ProtoId<DamageContainerPrototype>.op_Implicit(damageContainerID.GetValueOrDefault()) : null);
			ProtoId<DamageModifierSetPrototype>? damageModifierSetId = component.DamageModifierSetId;
			((ComponentGetState)(ref args)).State = (IComponentState)(object)new DamageableComponentState(damageDict, damageContainerId, damageModifierSetId.HasValue ? ProtoId<DamageModifierSetPrototype>.op_Implicit(damageModifierSetId.GetValueOrDefault()) : null, component.HealthBarThreshold);
		}
		else
		{
			Dictionary<string, FixedPoint2> damageDict2 = Extensions.ShallowClone<string, FixedPoint2>(component.Damage.DamageDict);
			ProtoId<DamageContainerPrototype>? damageContainerID = component.DamageContainerID;
			string damageContainerId2 = (damageContainerID.HasValue ? ProtoId<DamageContainerPrototype>.op_Implicit(damageContainerID.GetValueOrDefault()) : null);
			ProtoId<DamageModifierSetPrototype>? damageModifierSetId = component.DamageModifierSetId;
			((ComponentGetState)(ref args)).State = (IComponentState)(object)new DamageableComponentState(damageDict2, damageContainerId2, damageModifierSetId.HasValue ? ProtoId<DamageModifierSetPrototype>.op_Implicit(damageModifierSetId.GetValueOrDefault()) : null, component.HealthBarThreshold);
		}
	}

	private void OnIrradiated(EntityUid uid, DamageableComponent component, OnIrradiatedEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		FixedPoint2 damageValue = FixedPoint2.New(args.TotalRads);
		DamageSpecifier damage = new DamageSpecifier();
		foreach (ProtoId<DamageTypePrototype> typeId in component.RadiationDamageTypeIDs)
		{
			damage.DamageDict.Add(ProtoId<DamageTypePrototype>.op_Implicit(typeId), damageValue);
		}
		TryChangeDamage(uid, damage, ignoreResistances: false, interruptsDoAfters: false, null, args.Origin);
	}

	private void OnRejuvenate(EntityUid uid, DamageableComponent component, RejuvenateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		MobThresholdsComponent thresholds = default(MobThresholdsComponent);
		((EntitySystem)this).TryComp<MobThresholdsComponent>(uid, ref thresholds);
		_mobThreshold.SetAllowRevives(uid, val: true, thresholds);
		SetAllDamage(uid, component, 0);
		_mobThreshold.SetAllowRevives(uid, val: false, thresholds);
	}

	private void DamageableHandleState(EntityUid uid, DamageableComponent component, ref ComponentHandleState args)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is DamageableComponentState state)
		{
			component.DamageContainerID = ProtoId<DamageContainerPrototype>.op_Implicit(state.DamageContainerId);
			component.DamageModifierSetId = ProtoId<DamageModifierSetPrototype>.op_Implicit(state.ModifierSetId);
			component.HealthBarThreshold = state.HealthBarThreshold;
			DamageSpecifier newDamage = new DamageSpecifier
			{
				DamageDict = new Dictionary<string, FixedPoint2>(state.DamageDict)
			};
			DamageSpecifier delta = newDamage - component.Damage;
			delta.TrimZeros();
			if (!delta.Empty)
			{
				component.Damage = newDamage;
				DamageChanged(uid, component, delta);
			}
		}
	}
}
