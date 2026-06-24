using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._RMC14.Xenonids.CriticalGrace;
using Content.Shared.Alert;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Shared.Mobs.Systems;

public sealed class MobThresholdSystem : EntitySystem
{
	[Dependency]
	private MobStateSystem _mobStateSystem;

	[Dependency]
	private AlertsSystem _alerts;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MobThresholdsComponent, ComponentGetState>((ComponentEventRefHandler<MobThresholdsComponent, ComponentGetState>)OnGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobThresholdsComponent, ComponentHandleState>((ComponentEventRefHandler<MobThresholdsComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobThresholdsComponent, ComponentShutdown>((ComponentEventHandler<MobThresholdsComponent, ComponentShutdown>)MobThresholdShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobThresholdsComponent, ComponentStartup>((ComponentEventHandler<MobThresholdsComponent, ComponentStartup>)MobThresholdStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobThresholdsComponent, DamageChangedEvent>((ComponentEventHandler<MobThresholdsComponent, DamageChangedEvent>)OnDamaged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobThresholdsComponent, UpdateMobStateEvent>((ComponentEventRefHandler<MobThresholdsComponent, UpdateMobStateEvent>)OnUpdateMobState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobThresholdsComponent, MobStateChangedEvent>((EntityEventRefHandler<MobThresholdsComponent, MobStateChangedEvent>)OnThresholdsMobState, (Type[])null, (Type[])null);
	}

	private void OnGetState(EntityUid uid, MobThresholdsComponent component, ref ComponentGetState args)
	{
		Dictionary<FixedPoint2, MobState> thresholds = new Dictionary<FixedPoint2, MobState>();
		foreach (var (key, value) in component.Thresholds)
		{
			thresholds.Add(key, value);
		}
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new MobThresholdsComponentState(thresholds, component.TriggersAlerts, component.CurrentThresholdState, component.StateAlertDict, component.ShowOverlays, component.AllowRevives, component.DisplayDamageInAlert);
	}

	private void OnHandleState(EntityUid uid, MobThresholdsComponent component, ref ComponentHandleState args)
	{
		if (((ComponentHandleState)(ref args)).Current is MobThresholdsComponentState state)
		{
			component.Thresholds = new SortedDictionary<FixedPoint2, MobState>(state.UnsortedThresholds);
			component.TriggersAlerts = state.TriggersAlerts;
			component.CurrentThresholdState = state.CurrentThresholdState;
			component.AllowRevives = state.AllowRevives;
		}
	}

	public bool TryGetNextState(EntityUid target, MobState mobState, [NotNullWhen(true)] out MobState? nextState, MobThresholdsComponent? thresholdsComponent = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		nextState = null;
		if (!((EntitySystem)this).Resolve<MobThresholdsComponent>(target, ref thresholdsComponent, true))
		{
			return false;
		}
		MobState? min = null;
		foreach (MobState state in thresholdsComponent.Thresholds.Values)
		{
			if ((int)state > (int)mobState && (!min.HasValue || (int?)state < (int?)min))
			{
				min = state;
			}
		}
		nextState = min;
		return nextState.HasValue;
	}

	public FixedPoint2 GetThresholdForState(EntityUid target, MobState mobState, MobThresholdsComponent? thresholdComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MobThresholdsComponent>(target, ref thresholdComponent, true))
		{
			return FixedPoint2.Zero;
		}
		foreach (KeyValuePair<FixedPoint2, MobState> pair in thresholdComponent.Thresholds)
		{
			if (pair.Value == mobState)
			{
				return pair.Key;
			}
		}
		return FixedPoint2.Zero;
	}

	public bool TryGetThresholdForState(EntityUid target, MobState mobState, [NotNullWhen(true)] out FixedPoint2? threshold, MobThresholdsComponent? thresholdComponent = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		threshold = null;
		if (!((EntitySystem)this).Resolve<MobThresholdsComponent>(target, ref thresholdComponent, true))
		{
			return false;
		}
		foreach (KeyValuePair<FixedPoint2, MobState> pair in thresholdComponent.Thresholds)
		{
			if (pair.Value == mobState)
			{
				threshold = pair.Key;
				return true;
			}
		}
		return false;
	}

	public bool TryGetPercentageForState(EntityUid target, MobState mobState, FixedPoint2 damage, [NotNullWhen(true)] out FixedPoint2? percentage, MobThresholdsComponent? thresholdComponent = null)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		percentage = null;
		if (!TryGetThresholdForState(target, mobState, out var threshold, thresholdComponent))
		{
			return false;
		}
		FixedPoint2? fixedPoint = threshold;
		percentage = damage / fixedPoint;
		return true;
	}

	public bool TryGetIncapThreshold(EntityUid target, [NotNullWhen(true)] out FixedPoint2? threshold, MobThresholdsComponent? thresholdComponent = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		threshold = null;
		if (!((EntitySystem)this).Resolve<MobThresholdsComponent>(target, ref thresholdComponent, true))
		{
			return false;
		}
		if (!TryGetThresholdForState(target, MobState.Critical, out threshold, thresholdComponent))
		{
			return TryGetThresholdForState(target, MobState.Dead, out threshold, thresholdComponent);
		}
		return true;
	}

	public bool TryGetIncapPercentage(EntityUid target, FixedPoint2 damage, [NotNullWhen(true)] out FixedPoint2? percentage, MobThresholdsComponent? thresholdComponent = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		percentage = null;
		if (!TryGetIncapThreshold(target, out var threshold, thresholdComponent))
		{
			return false;
		}
		if (damage == 0)
		{
			percentage = 0;
			return true;
		}
		percentage = FixedPoint2.Min(1f, damage / threshold.Value);
		return true;
	}

	public bool TryGetDeadThreshold(EntityUid target, [NotNullWhen(true)] out FixedPoint2? threshold, MobThresholdsComponent? thresholdComponent = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		threshold = null;
		if (!((EntitySystem)this).Resolve<MobThresholdsComponent>(target, ref thresholdComponent, false))
		{
			return false;
		}
		return TryGetThresholdForState(target, MobState.Dead, out threshold, thresholdComponent);
	}

	public bool TryGetDeadPercentage(EntityUid target, FixedPoint2 damage, [NotNullWhen(true)] out FixedPoint2? percentage, MobThresholdsComponent? thresholdComponent = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		percentage = null;
		if (!TryGetDeadThreshold(target, out var threshold, thresholdComponent))
		{
			return false;
		}
		if (damage == 0)
		{
			percentage = 0;
			return true;
		}
		percentage = FixedPoint2.Min(1f, damage / threshold.Value);
		return true;
	}

	public bool GetScaledDamage(EntityUid target1, EntityUid target2, out DamageSpecifier? damage)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		damage = null;
		DamageableComponent oldDamage = default(DamageableComponent);
		if (!((EntitySystem)this).TryComp<DamageableComponent>(target1, ref oldDamage))
		{
			return false;
		}
		MobThresholdsComponent threshold1 = default(MobThresholdsComponent);
		MobThresholdsComponent threshold2 = default(MobThresholdsComponent);
		if (!((EntitySystem)this).TryComp<MobThresholdsComponent>(target1, ref threshold1) || !((EntitySystem)this).TryComp<MobThresholdsComponent>(target2, ref threshold2))
		{
			return false;
		}
		if (!TryGetThresholdForState(target1, MobState.Dead, out var ent1DeadThreshold, threshold1))
		{
			ent1DeadThreshold = 0;
		}
		if (!TryGetThresholdForState(target2, MobState.Dead, out var ent2DeadThreshold, threshold2))
		{
			ent2DeadThreshold = 0;
		}
		damage = oldDamage.Damage / ent1DeadThreshold.Value * ent2DeadThreshold.Value;
		return true;
	}

	public void SetMobStateThreshold(EntityUid target, FixedPoint2 damage, MobState mobState, MobThresholdsComponent? threshold = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MobThresholdsComponent>(target, ref threshold, true))
		{
			return;
		}
		foreach (var (damageThreshold, mobState3) in new Dictionary<FixedPoint2, MobState>(threshold.Thresholds))
		{
			if (mobState3 == mobState)
			{
				threshold.Thresholds.Remove(damageThreshold);
			}
		}
		threshold.Thresholds[damage] = mobState;
		((EntitySystem)this).Dirty(target, (IComponent)(object)threshold, (MetaDataComponent)null);
		VerifyThresholds(target, threshold);
	}

	public void VerifyThresholds(EntityUid target, MobThresholdsComponent? threshold = null, MobStateComponent? mobState = null, DamageableComponent? damageable = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<MobStateComponent, MobThresholdsComponent, DamageableComponent>(target, ref mobState, ref threshold, ref damageable, true))
		{
			CheckThresholds(target, mobState, threshold, damageable);
			MobThresholdChecked ev = new MobThresholdChecked(target, mobState, threshold, damageable);
			((EntitySystem)this).RaiseLocalEvent<MobThresholdChecked>(target, ref ev, true);
			UpdateAlerts(target, mobState.CurrentState, threshold, damageable);
		}
	}

	public void SetAllowRevives(EntityUid uid, bool val, MobThresholdsComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<MobThresholdsComponent>(uid, ref component, false))
		{
			component.AllowRevives = val;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			VerifyThresholds(uid, component);
		}
	}

	private void CheckThresholds(EntityUid target, MobStateComponent mobStateComponent, MobThresholdsComponent thresholdsComponent, DamageableComponent damageableComponent, EntityUid? origin = null)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		foreach (var (threshold, mobState2) in thresholdsComponent.Thresholds.Reverse())
		{
			if (!(damageableComponent.TotalDamage < threshold))
			{
				TriggerThreshold(target, mobState2, mobStateComponent, thresholdsComponent, origin);
				break;
			}
		}
	}

	private void TriggerThreshold(EntityUid target, MobState newState, MobStateComponent? mobState = null, MobThresholdsComponent? thresholds = null, EntityUid? origin = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<MobStateComponent, MobThresholdsComponent>(target, ref mobState, ref thresholds, true) && mobState.CurrentState != newState)
		{
			if (mobState.CurrentState != MobState.Dead || thresholds.AllowRevives)
			{
				thresholds.CurrentThresholdState = newState;
				((EntitySystem)this).Dirty(target, (IComponent)(object)thresholds, (MetaDataComponent)null);
			}
			_mobStateSystem.UpdateMobState(target, mobState, origin);
		}
	}

	private void UpdateAlerts(EntityUid target, MobState currentMobState, MobThresholdsComponent? threshold = null, DamageableComponent? damageable = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MobThresholdsComponent, DamageableComponent>(target, ref threshold, ref damageable, true) || !threshold.TriggersAlerts)
		{
			return;
		}
		FixedPoint2? healthMax;
		bool hasIncap = TryGetIncapThreshold(target, out healthMax, threshold);
		MobState state = currentMobState;
		if (hasIncap && ((EntitySystem)this).HasComp<InCriticalGraceComponent>(target))
		{
			FixedPoint2 totalDamage = damageable.TotalDamage;
			FixedPoint2? fixedPoint = healthMax;
			if (totalDamage > fixedPoint)
			{
				state = MobState.Critical;
			}
		}
		if (!threshold.StateAlertDict.TryGetValue(state, out ProtoId<AlertPrototype> currentAlert))
		{
			((EntitySystem)this).Log.Error($"No alert alert for mob state {state} for entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target))}");
			return;
		}
		if (!_alerts.TryGet(currentAlert, out AlertPrototype alertPrototype))
		{
			((EntitySystem)this).Log.Error($"Invalid alert type {currentAlert}");
			return;
		}
		string healthMessage = null;
		if (threshold.DisplayDamageInAlert && hasIncap && healthMax.HasValue)
		{
			string text = ((int)healthMax.Value - (int)damageable.TotalDamage).ToString();
			FixedPoint2? fixedPoint = healthMax;
			healthMessage = text + " / " + fixedPoint.ToString();
		}
		if (alertPrototype.SupportsSeverity)
		{
			short severity = _alerts.GetMinSeverity(currentAlert);
			BeforeAlertSeverityCheckEvent ev = new BeforeAlertSeverityCheckEvent(currentAlert, severity);
			((EntitySystem)this).RaiseLocalEvent<BeforeAlertSeverityCheckEvent>(target, ev, false);
			if (ev.CancelUpdate)
			{
				_alerts.ShowAlert(target, ev.CurrentAlert, ev.Severity);
				return;
			}
			if (TryGetNextState(target, currentMobState, out var nextState, threshold) && TryGetPercentageForState(target, nextState.Value, damageable.TotalDamage, out var percentage))
			{
				percentage = FixedPoint2.Clamp(percentage.Value, 0, 1);
				severity = (short)MathF.Round(MathHelper.Lerp((float)_alerts.GetMinSeverity(currentAlert), (float)_alerts.GetMaxSeverity(currentAlert), percentage.Value.Float()));
			}
			AlertsSystem alerts = _alerts;
			ProtoId<AlertPrototype> alertType = currentAlert;
			short? severity2 = severity;
			string dynamicMessage = healthMessage;
			alerts.ShowAlert(target, alertType, severity2, null, autoRemove: false, showCooldown: true, dynamicMessage);
		}
		else
		{
			AlertsSystem alerts2 = _alerts;
			ProtoId<AlertPrototype> alertType2 = currentAlert;
			string dynamicMessage = healthMessage;
			alerts2.ShowAlert(target, alertType2, null, null, autoRemove: false, showCooldown: true, dynamicMessage);
		}
	}

	private void OnDamaged(EntityUid target, MobThresholdsComponent thresholds, DamageChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		MobStateComponent mobState = default(MobStateComponent);
		if (((EntitySystem)this).TryComp<MobStateComponent>(target, ref mobState))
		{
			CheckThresholds(target, mobState, thresholds, args.Damageable, args.Origin);
			MobThresholdChecked ev = new MobThresholdChecked(target, mobState, thresholds, args.Damageable);
			((EntitySystem)this).RaiseLocalEvent<MobThresholdChecked>(target, ref ev, true);
			UpdateAlerts(target, mobState.CurrentState, thresholds, args.Damageable);
		}
	}

	private void MobThresholdStartup(EntityUid target, MobThresholdsComponent thresholds, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		MobStateComponent mobState = default(MobStateComponent);
		DamageableComponent damageable = default(DamageableComponent);
		if (((EntitySystem)this).TryComp<MobStateComponent>(target, ref mobState) && ((EntitySystem)this).TryComp<DamageableComponent>(target, ref damageable))
		{
			CheckThresholds(target, mobState, thresholds, damageable);
			UpdateAllEffects(Entity<MobThresholdsComponent, MobStateComponent, DamageableComponent>.op_Implicit((target, thresholds, mobState, damageable)), mobState.CurrentState);
		}
	}

	private void MobThresholdShutdown(EntityUid target, MobThresholdsComponent component, ComponentShutdown args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (component.TriggersAlerts)
		{
			_alerts.ClearAlertCategory(target, component.HealthAlertCategory);
		}
	}

	private void OnUpdateMobState(EntityUid target, MobThresholdsComponent component, ref UpdateMobStateEvent args)
	{
		if (!component.AllowRevives && component.CurrentThresholdState == MobState.Dead)
		{
			args.State = MobState.Dead;
		}
		else if (component.CurrentThresholdState != MobState.Invalid)
		{
			args.State = component.CurrentThresholdState;
		}
	}

	private void UpdateAllEffects(Entity<MobThresholdsComponent, MobStateComponent?, DamageableComponent?> ent, MobState currentState)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		Entity<MobThresholdsComponent, MobStateComponent, DamageableComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		MobThresholdsComponent mobThresholdsComponent = default(MobThresholdsComponent);
		MobStateComponent mobStateComponent = default(MobStateComponent);
		DamageableComponent damageableComponent = default(DamageableComponent);
		val.Deconstruct(ref val2, ref mobThresholdsComponent, ref mobStateComponent, ref damageableComponent);
		MobThresholdsComponent thresholds = mobThresholdsComponent;
		MobStateComponent mobState = mobStateComponent;
		DamageableComponent damageable = damageableComponent;
		if (((EntitySystem)this).Resolve<MobThresholdsComponent, MobStateComponent, DamageableComponent>(Entity<MobThresholdsComponent, MobStateComponent, DamageableComponent>.op_Implicit(ent), ref thresholds, ref mobState, ref damageable, true))
		{
			MobThresholdChecked ev = new MobThresholdChecked(Entity<MobThresholdsComponent, MobStateComponent, DamageableComponent>.op_Implicit(ent), mobState, thresholds, damageable);
			((EntitySystem)this).RaiseLocalEvent<MobThresholdChecked>(Entity<MobThresholdsComponent, MobStateComponent, DamageableComponent>.op_Implicit(ent), ref ev, true);
		}
		UpdateAlerts(Entity<MobThresholdsComponent, MobStateComponent, DamageableComponent>.op_Implicit(ent), currentState, thresholds, damageable);
	}

	private void OnThresholdsMobState(Entity<MobThresholdsComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		UpdateAllEffects(Entity<MobThresholdsComponent, MobStateComponent, DamageableComponent>.op_Implicit((ValueTuple<EntityUid, MobThresholdsComponent, MobStateComponent, DamageableComponent>)(Entity<MobThresholdsComponent>.op_Implicit(ent), Entity<MobThresholdsComponent>.op_Implicit(ent), null, null)), args.NewMobState);
	}
}
