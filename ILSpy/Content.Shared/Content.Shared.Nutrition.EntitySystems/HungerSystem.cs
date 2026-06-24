using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Alert;
using Content.Shared.Damage;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Rejuvenate;
using Content.Shared.StatusIcon;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Nutrition.EntitySystems;

public sealed class HungerSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeedModifier;

	[Dependency]
	private SharedJetpackSystem _jetpack;

	private static readonly ProtoId<SatiationIconPrototype> HungerIconOverfedId = ProtoId<SatiationIconPrototype>.op_Implicit("HungerIconOverfed");

	private static readonly ProtoId<SatiationIconPrototype> HungerIconPeckishId = ProtoId<SatiationIconPrototype>.op_Implicit("HungerIconPeckish");

	private static readonly ProtoId<SatiationIconPrototype> HungerIconStarvingId = ProtoId<SatiationIconPrototype>.op_Implicit("HungerIconStarving");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<HungerComponent, MapInitEvent>((ComponentEventHandler<HungerComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HungerComponent, ComponentShutdown>((ComponentEventHandler<HungerComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HungerComponent, RefreshMovementSpeedModifiersEvent>((ComponentEventHandler<HungerComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshMovespeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HungerComponent, RejuvenateEvent>((ComponentEventHandler<HungerComponent, RejuvenateEvent>)OnRejuvenate, (Type[])null, (Type[])null);
	}

	private void OnMapInit(EntityUid uid, HungerComponent component, MapInitEvent args)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		int amount = _random.Next((int)component.Thresholds[HungerThreshold.Peckish] + 10, (int)component.Thresholds[HungerThreshold.Okay]);
		SetHunger(uid, amount, component);
	}

	private void OnShutdown(EntityUid uid, HungerComponent component, ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_alerts.ClearAlertCategory(uid, component.HungerAlertCategory);
	}

	private void OnRefreshMovespeed(EntityUid uid, HungerComponent component, RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if ((int)component.CurrentThreshold <= 1 && !_jetpack.IsUserFlying(uid))
		{
			args.ModifySpeed(component.StarvingSlowdownModifier, component.StarvingSlowdownModifier);
		}
	}

	private void OnRejuvenate(EntityUid uid, HungerComponent component, RejuvenateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetHunger(uid, component.Thresholds[HungerThreshold.Okay], component);
	}

	public float GetHunger(HungerComponent component)
	{
		TimeSpan dt = _timing.CurTime - component.LastAuthoritativeHungerChangeTime;
		float value = component.LastAuthoritativeHungerValue - (float)dt.TotalSeconds * component.ActualDecayRate;
		return ClampHungerWithinThresholds(component, value);
	}

	public void ModifyHunger(EntityUid uid, float amount, HungerComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HungerComponent>(uid, ref component, true))
		{
			SetHunger(uid, GetHunger(component) + amount, component);
		}
	}

	public void SetHunger(EntityUid uid, float amount, HungerComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HungerComponent>(uid, ref component, true))
		{
			SetAuthoritativeHungerValue(Entity<HungerComponent>.op_Implicit((uid, component)), amount);
			UpdateCurrentThreshold(uid, component);
		}
	}

	private void SetAuthoritativeHungerValue(Entity<HungerComponent> entity, float value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		entity.Comp.LastAuthoritativeHungerChangeTime = _timing.CurTime;
		entity.Comp.LastAuthoritativeHungerValue = ClampHungerWithinThresholds(entity.Comp, value);
		((EntitySystem)this).DirtyField<HungerComponent>(entity.Owner, entity.Comp, "LastAuthoritativeHungerChangeTime", (MetaDataComponent)null);
		((EntitySystem)this).DirtyField<HungerComponent>(entity.Owner, entity.Comp, "LastAuthoritativeHungerValue", (MetaDataComponent)null);
	}

	private void UpdateCurrentThreshold(EntityUid uid, HungerComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HungerComponent>(uid, ref component, true))
		{
			HungerThreshold calculatedHungerThreshold = GetHungerThreshold(component);
			if (calculatedHungerThreshold != component.CurrentThreshold)
			{
				component.CurrentThreshold = calculatedHungerThreshold;
				((EntitySystem)this).DirtyField<HungerComponent>(uid, component, "CurrentThreshold", (MetaDataComponent)null);
				DoHungerThresholdEffects(uid, component);
			}
		}
	}

	private void DoHungerThresholdEffects(EntityUid uid, HungerComponent? component = null, bool force = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HungerComponent>(uid, ref component, true) && (component.CurrentThreshold != component.LastThreshold || force))
		{
			if (GetMovementThreshold(component.CurrentThreshold) != GetMovementThreshold(component.LastThreshold))
			{
				_movementSpeedModifier.RefreshMovementSpeedModifiers(uid);
			}
			if (component.HungerThresholdAlerts.TryGetValue(component.CurrentThreshold, out ProtoId<AlertPrototype> alertId))
			{
				_alerts.ShowAlert(uid, alertId);
			}
			else
			{
				_alerts.ClearAlertCategory(uid, component.HungerAlertCategory);
			}
			if (component.HungerThresholdDecayModifiers.TryGetValue(component.CurrentThreshold, out var modifier))
			{
				component.ActualDecayRate = component.BaseDecayRate * modifier;
				((EntitySystem)this).DirtyField<HungerComponent>(uid, component, "ActualDecayRate", (MetaDataComponent)null);
				SetAuthoritativeHungerValue(Entity<HungerComponent>.op_Implicit((uid, component)), GetHunger(component));
			}
			component.LastThreshold = component.CurrentThreshold;
			((EntitySystem)this).DirtyField<HungerComponent>(uid, component, "LastThreshold", (MetaDataComponent)null);
		}
	}

	private void DoContinuousHungerEffects(EntityUid uid, HungerComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HungerComponent>(uid, ref component, true) && (int)component.CurrentThreshold <= 1)
		{
			DamageSpecifier damage = component.StarvationDamage;
			if (damage != null && !_mobState.IsDead(uid))
			{
				_damageable.TryChangeDamage(uid, damage, ignoreResistances: true, interruptsDoAfters: false);
			}
		}
	}

	public HungerThreshold GetHungerThreshold(HungerComponent component, float? food = null)
	{
		float valueOrDefault = food.GetValueOrDefault();
		if (!food.HasValue)
		{
			valueOrDefault = GetHunger(component);
			food = valueOrDefault;
		}
		HungerThreshold result = HungerThreshold.Dead;
		float value = component.Thresholds[HungerThreshold.Overfed];
		foreach (KeyValuePair<HungerThreshold, float> threshold in component.Thresholds)
		{
			if (threshold.Value <= value && threshold.Value >= food)
			{
				result = threshold.Key;
				value = threshold.Value;
			}
		}
		return result;
	}

	public bool IsHungerBelowState(EntityUid uid, HungerThreshold threshold, float? food = null, HungerComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HungerComponent>(uid, ref comp, true))
		{
			return false;
		}
		return (int)GetHungerThreshold(comp, food) < (int)threshold;
	}

	private bool GetMovementThreshold(HungerThreshold threshold)
	{
		switch (threshold)
		{
		case HungerThreshold.Okay:
		case HungerThreshold.Overfed:
			return true;
		case HungerThreshold.Dead:
		case HungerThreshold.Starving:
		case HungerThreshold.Peckish:
			return false;
		default:
			throw new ArgumentOutOfRangeException("threshold", threshold, null);
		}
	}

	public bool TryGetStatusIconPrototype(HungerComponent component, [NotNullWhen(true)] out SatiationIconPrototype? prototype)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		switch (component.CurrentThreshold)
		{
		case HungerThreshold.Overfed:
			_prototype.TryIndex<SatiationIconPrototype>(HungerIconOverfedId, ref prototype);
			break;
		case HungerThreshold.Peckish:
			_prototype.TryIndex<SatiationIconPrototype>(HungerIconPeckishId, ref prototype);
			break;
		case HungerThreshold.Starving:
			_prototype.TryIndex<SatiationIconPrototype>(HungerIconStarvingId, ref prototype);
			break;
		default:
			prototype = null;
			break;
		}
		return prototype != null;
	}

	private static float ClampHungerWithinThresholds(HungerComponent component, float hungerValue)
	{
		return Math.Clamp(hungerValue, component.Thresholds[HungerThreshold.Dead], component.Thresholds[HungerThreshold.Overfed]);
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<HungerComponent> query = ((EntitySystem)this).EntityQueryEnumerator<HungerComponent>();
		EntityUid uid = default(EntityUid);
		HungerComponent hunger = default(HungerComponent);
		while (query.MoveNext(ref uid, ref hunger))
		{
			if (!(_timing.CurTime < hunger.NextThresholdUpdateTime))
			{
				hunger.NextThresholdUpdateTime = _timing.CurTime + hunger.ThresholdUpdateRate;
				UpdateCurrentThreshold(uid, hunger);
				DoContinuousHungerEffects(uid, hunger);
			}
		}
	}
}
