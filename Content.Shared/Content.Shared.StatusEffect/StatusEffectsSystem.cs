using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared._RMC14.StatusEffect;
using Content.Shared.Alert;
using Content.Shared.Rejuvenate;
using Content.Shared.StatusEffectNew;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.StatusEffect;

[Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
public sealed class StatusEffectsSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private AlertsSystem _alertsSystem;

	private List<EntityUid> _toRemove = new List<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).UpdatesOutsidePrediction = true;
		((EntitySystem)this).SubscribeLocalEvent<StatusEffectsComponent, ComponentGetState>((ComponentEventRefHandler<StatusEffectsComponent, ComponentGetState>)OnGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StatusEffectsComponent, ComponentHandleState>((ComponentEventRefHandler<StatusEffectsComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StatusEffectsComponent, RejuvenateEvent>((ComponentEventHandler<StatusEffectsComponent, RejuvenateEvent>)OnRejuvenate, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		TimeSpan curTime = _gameTiming.CurTime;
		EntityQueryEnumerator<ActiveStatusEffectsComponent, StatusEffectsComponent> enumerator = ((EntitySystem)this).EntityQueryEnumerator<ActiveStatusEffectsComponent, StatusEffectsComponent>();
		_toRemove.Clear();
		EntityUid uid = default(EntityUid);
		ActiveStatusEffectsComponent activeStatusEffectsComponent = default(ActiveStatusEffectsComponent);
		StatusEffectsComponent status = default(StatusEffectsComponent);
		while (enumerator.MoveNext(ref uid, ref activeStatusEffectsComponent, ref status))
		{
			if (status.ActiveEffects.Count == 0)
			{
				_toRemove.Add(uid);
				continue;
			}
			foreach (KeyValuePair<string, StatusEffectState> state in status.ActiveEffects)
			{
				if (curTime > state.Value.Cooldown.Item2)
				{
					TryRemoveStatusEffect(uid, state.Key, status);
				}
			}
		}
		foreach (EntityUid uid2 in _toRemove)
		{
			((EntitySystem)this).RemComp<ActiveStatusEffectsComponent>(uid2);
		}
	}

	private void OnGetState(EntityUid uid, StatusEffectsComponent component, ref ComponentGetState args)
	{
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new StatusEffectsComponentState(new Dictionary<string, StatusEffectState>(component.ActiveEffects), new List<string>(component.AllowedEffects));
	}

	private void OnHandleState(EntityUid uid, StatusEffectsComponent component, ref ComponentHandleState args)
	{
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ComponentHandleState)(ref args)).Current is StatusEffectsComponentState state))
		{
			return;
		}
		component.AllowedEffects.Clear();
		component.AllowedEffects.AddRange(state.AllowedEffects);
		foreach (string key in component.ActiveEffects.Keys)
		{
			if (!state.ActiveEffects.ContainsKey(key))
			{
				component.ActiveEffects.Remove(key);
			}
		}
		foreach (var (key2, effect) in state.ActiveEffects)
		{
			component.ActiveEffects[key2] = new StatusEffectState(effect);
		}
		if (component.ActiveEffects.Count == 0)
		{
			((EntitySystem)this).RemComp<ActiveStatusEffectsComponent>(uid);
		}
		else
		{
			((EntitySystem)this).EnsureComp<ActiveStatusEffectsComponent>(uid);
		}
	}

	private void OnRejuvenate(EntityUid uid, StatusEffectsComponent component, RejuvenateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		TryRemoveAllStatusEffects(uid, component);
	}

	[Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
	public bool TryAddStatusEffect<T>(EntityUid uid, string key, TimeSpan time, bool refresh, StatusEffectsComponent? status = null, bool force = false) where T : IComponent, new()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			return false;
		}
		StatusEffectsComponent? status2 = status;
		bool force2 = force;
		if (!TryAddStatusEffect(uid, key, time, refresh, status2, null, force2))
		{
			return false;
		}
		if (((EntitySystem)this).HasComp<T>(uid))
		{
			status.ActiveEffects[key].RelevantComponent = ((EntitySystem)this).Factory.GetComponentName<T>();
			return true;
		}
		((EntitySystem)this).AddComp<T>(uid);
		status.ActiveEffects[key].RelevantComponent = ((EntitySystem)this).Factory.GetComponentName<T>();
		return true;
	}

	[Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
	public bool TryAddStatusEffect(EntityUid uid, string key, TimeSpan time, bool refresh, string component, StatusEffectsComponent? status = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			return false;
		}
		if (TryAddStatusEffect(uid, key, time, refresh, status))
		{
			if (!((EntitySystem)this).HasComp(uid, ((EntitySystem)this).Factory.GetRegistration(component, false).Type))
			{
				Component newComponent = (Component)((EntitySystem)this).Factory.GetComponent(component, false);
				((EntitySystem)this).AddComp<Component>(uid, newComponent, false);
				status.ActiveEffects[key].RelevantComponent = component;
			}
			return true;
		}
		return false;
	}

	[Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
	public bool TryAddStatusEffect(EntityUid uid, string key, TimeSpan time, bool refresh, StatusEffectsComponent? status = null, TimeSpan? startTime = null, bool force = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			return false;
		}
		if (!CanApplyEffect(uid, key, status, force))
		{
			return false;
		}
		RMCStatusEffectTimeEvent ev = new RMCStatusEffectTimeEvent(key, time);
		((EntitySystem)this).RaiseLocalEvent<RMCStatusEffectTimeEvent>(uid, ref ev, false);
		time = ev.Duration;
		StatusEffectPrototype proto = _prototypeManager.Index<StatusEffectPrototype>(key);
		TimeSpan obj = startTime ?? _gameTiming.CurTime;
		(TimeSpan, TimeSpan) cooldown = (obj, obj + time);
		if (HasStatusEffect(uid, key, status))
		{
			status.ActiveEffects[key].CooldownRefresh = refresh;
			if (refresh)
			{
				if (status.ActiveEffects[key].Cooldown.Item2 - _gameTiming.CurTime < time)
				{
					status.ActiveEffects[key].Cooldown = cooldown;
				}
			}
			else
			{
				status.ActiveEffects[key].Cooldown.Item2 += time;
			}
		}
		else
		{
			status.ActiveEffects.Add(key, new StatusEffectState(cooldown, refresh));
			((EntitySystem)this).EnsureComp<ActiveStatusEffectsComponent>(uid);
		}
		if (proto.Alert.HasValue)
		{
			(TimeSpan, TimeSpan)? cooldown2 = GetAlertCooldown(uid, proto.Alert.Value, status);
			_alertsSystem.ShowAlert(uid, proto.Alert.Value, null, cooldown2);
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)status, (MetaDataComponent)null);
		((EntitySystem)this).RaiseLocalEvent<StatusEffectAddedEvent>(uid, new StatusEffectAddedEvent(uid, key), false);
		return true;
	}

	private (TimeSpan, TimeSpan)? GetAlertCooldown(EntityUid uid, ProtoId<AlertPrototype> alert, StatusEffectsComponent status)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		(TimeSpan, TimeSpan)? maxCooldown = null;
		foreach (KeyValuePair<string, StatusEffectState> kvp in status.ActiveEffects)
		{
			ProtoId<AlertPrototype>? alert2 = _prototypeManager.Index<StatusEffectPrototype>(kvp.Key).Alert;
			if (alert2.HasValue && alert2.GetValueOrDefault() == alert && (!maxCooldown.HasValue || kvp.Value.Cooldown.Item2 > maxCooldown.Value.Item2))
			{
				maxCooldown = kvp.Value.Cooldown;
			}
		}
		return maxCooldown;
	}

	[Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
	public bool TryRemoveStatusEffect(EntityUid uid, string key, StatusEffectsComponent? status = null, bool remComp = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			return false;
		}
		if (!status.ActiveEffects.ContainsKey(key))
		{
			return false;
		}
		StatusEffectPrototype proto = default(StatusEffectPrototype);
		if (!_prototypeManager.TryIndex<StatusEffectPrototype>(key, ref proto))
		{
			return false;
		}
		StatusEffectState state = status.ActiveEffects[key];
		ComponentRegistration registration = default(ComponentRegistration);
		if (remComp && state.RelevantComponent != null && ((EntitySystem)this).Factory.TryGetRegistration(state.RelevantComponent, ref registration, false))
		{
			Type type = registration.Type;
			((EntitySystem)this).RemComp(uid, type);
		}
		if (proto.Alert.HasValue)
		{
			_alertsSystem.ClearAlert(uid, proto.Alert.Value);
		}
		status.ActiveEffects.Remove(key);
		if (status.ActiveEffects.Count == 0)
		{
			((EntitySystem)this).RemComp<ActiveStatusEffectsComponent>(uid);
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)status, (MetaDataComponent)null);
		((EntitySystem)this).RaiseLocalEvent<StatusEffectEndedEvent>(uid, new StatusEffectEndedEvent(uid, key), false);
		return true;
	}

	[Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
	public bool TryRemoveAllStatusEffects(EntityUid uid, StatusEffectsComponent? status = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			return false;
		}
		bool failed = false;
		foreach (KeyValuePair<string, StatusEffectState> activeEffect in status.ActiveEffects)
		{
			if (!TryRemoveStatusEffect(uid, activeEffect.Key, status))
			{
				failed = true;
			}
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)status, (MetaDataComponent)null);
		return failed;
	}

	[Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
	public bool HasStatusEffect(EntityUid uid, string key, StatusEffectsComponent? status = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			return false;
		}
		if (!status.ActiveEffects.ContainsKey(key))
		{
			return false;
		}
		return true;
	}

	[Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
	public bool CanApplyEffect(EntityUid uid, string key, StatusEffectsComponent? status = null, bool force = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			return false;
		}
		if (!force)
		{
			BeforeStatusEffectAddedEvent ev = new BeforeStatusEffectAddedEvent(EntProtoId.op_Implicit(key));
			((EntitySystem)this).RaiseLocalEvent<BeforeStatusEffectAddedEvent>(uid, ref ev, false);
			if (ev.Cancelled)
			{
				return false;
			}
		}
		StatusEffectPrototype proto = default(StatusEffectPrototype);
		if (!_prototypeManager.TryIndex<StatusEffectPrototype>(key, ref proto))
		{
			return false;
		}
		if (!status.AllowedEffects.Contains(key) && !proto.AlwaysAllowed)
		{
			return false;
		}
		return true;
	}

	[Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
	public bool TryAddTime(EntityUid uid, string key, TimeSpan time, StatusEffectsComponent? status = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			return false;
		}
		if (!HasStatusEffect(uid, key, status))
		{
			return false;
		}
		(TimeSpan, TimeSpan) timer = status.ActiveEffects[key].Cooldown;
		timer.Item2 += time;
		status.ActiveEffects[key].Cooldown = timer;
		StatusEffectPrototype proto = default(StatusEffectPrototype);
		if (_prototypeManager.TryIndex<StatusEffectPrototype>(key, ref proto) && proto.Alert.HasValue)
		{
			(TimeSpan, TimeSpan)? cooldown = GetAlertCooldown(uid, proto.Alert.Value, status);
			_alertsSystem.ShowAlert(uid, proto.Alert.Value, null, cooldown);
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)status, (MetaDataComponent)null);
		return true;
	}

	[Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
	public bool TryRemoveTime(EntityUid uid, string key, TimeSpan time, StatusEffectsComponent? status = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			return false;
		}
		if (!HasStatusEffect(uid, key, status))
		{
			return false;
		}
		(TimeSpan, TimeSpan) timer = status.ActiveEffects[key].Cooldown;
		if (time > timer.Item2)
		{
			return false;
		}
		timer.Item2 -= time;
		status.ActiveEffects[key].Cooldown = timer;
		StatusEffectPrototype proto = default(StatusEffectPrototype);
		if (_prototypeManager.TryIndex<StatusEffectPrototype>(key, ref proto) && proto.Alert.HasValue)
		{
			(TimeSpan, TimeSpan)? cooldown = GetAlertCooldown(uid, proto.Alert.Value, status);
			_alertsSystem.ShowAlert(uid, proto.Alert.Value, null, cooldown);
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)status, (MetaDataComponent)null);
		return true;
	}

	[Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
	public bool TrySetTime(EntityUid uid, string key, TimeSpan time, StatusEffectsComponent? status = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			return false;
		}
		if (!HasStatusEffect(uid, key, status))
		{
			return false;
		}
		status.ActiveEffects[key].Cooldown = (_gameTiming.CurTime, _gameTiming.CurTime + time);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)status, (MetaDataComponent)null);
		return true;
	}

	[Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
	public bool TryGetTime(EntityUid uid, string key, [NotNullWhen(true)] out (TimeSpan, TimeSpan)? time, StatusEffectsComponent? status = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false) || !HasStatusEffect(uid, key, status))
		{
			time = null;
			return false;
		}
		time = status.ActiveEffects[key].Cooldown;
		return true;
	}
}
