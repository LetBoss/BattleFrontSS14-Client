using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Alert;
using Content.Shared.StatusEffectNew.Components;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.StatusEffectNew;

public abstract class SharedStatusEffectsSystem : EntitySystem
{
	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private IPrototypeManager _proto;

	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private INetManager _net;

	private EntityQuery<StatusEffectContainerComponent> _containerQuery;

	private EntityQuery<StatusEffectComponent> _effectQuery;

	public override void Initialize()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		InitializeRelay();
		((EntitySystem)this).SubscribeLocalEvent<StatusEffectComponent, StatusEffectAppliedEvent>((EntityEventRefHandler<StatusEffectComponent, StatusEffectAppliedEvent>)OnStatusEffectApplied, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StatusEffectComponent, StatusEffectRemovedEvent>((EntityEventRefHandler<StatusEffectComponent, StatusEffectRemovedEvent>)OnStatusEffectRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StatusEffectContainerComponent, ComponentGetState>((EntityEventRefHandler<StatusEffectContainerComponent, ComponentGetState>)OnGetState, (Type[])null, (Type[])null);
		_containerQuery = ((EntitySystem)this).GetEntityQuery<StatusEffectContainerComponent>();
		_effectQuery = ((EntitySystem)this).GetEntityQuery<StatusEffectComponent>();
	}

	private void OnGetState(Entity<StatusEffectContainerComponent> ent, ref ComponentGetState args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new StatusEffectContainerComponentState(((EntitySystem)this).GetNetEntitySet(ent.Comp.ActiveStatusEffects));
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<StatusEffectComponent> query = ((EntitySystem)this).EntityQueryEnumerator<StatusEffectComponent>();
		EntityUid ent = default(EntityUid);
		StatusEffectComponent effect = default(StatusEffectComponent);
		while (query.MoveNext(ref ent, ref effect))
		{
			TimeSpan? endEffectTime = effect.EndEffectTime;
			if (!endEffectTime.HasValue)
			{
				continue;
			}
			TimeSpan curTime = _timing.CurTime;
			endEffectTime = effect.EndEffectTime;
			if (!(curTime >= endEffectTime))
			{
				continue;
			}
			EntityUid? appliedTo = effect.AppliedTo;
			if (appliedTo.HasValue)
			{
				MetaDataComponent meta = ((EntitySystem)this).MetaData(ent);
				if (meta.EntityPrototype != null)
				{
					TryRemoveStatusEffect(effect.AppliedTo.Value, EntProtoId.op_Implicit(meta.EntityPrototype));
				}
			}
		}
	}

	private void AddStatusEffectTime(EntityUid effect, TimeSpan delta)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		StatusEffectComponent effectComp = default(StatusEffectComponent);
		if (_effectQuery.TryComp(effect, ref effectComp))
		{
			StatusEffectComponent statusEffectComponent = effectComp;
			statusEffectComponent.EndEffectTime += delta;
			((EntitySystem)this).Dirty(effect, (IComponent)(object)effectComp, (MetaDataComponent)null);
			ShowAlertIfNeeded(effectComp);
		}
	}

	private void SetStatusEffectTime(EntityUid effect, TimeSpan? duration)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		StatusEffectComponent effectComp = default(StatusEffectComponent);
		if (!_effectQuery.TryComp(effect, ref effectComp))
		{
			return;
		}
		if (!duration.HasValue)
		{
			TimeSpan? endEffectTime = effectComp.EndEffectTime;
			if (!endEffectTime.HasValue)
			{
				return;
			}
			effectComp.EndEffectTime = null;
		}
		else
		{
			StatusEffectComponent statusEffectComponent = effectComp;
			TimeSpan curTime = _timing.CurTime;
			TimeSpan? endEffectTime = duration;
			statusEffectComponent.EndEffectTime = curTime + endEffectTime;
		}
		((EntitySystem)this).Dirty(effect, (IComponent)(object)effectComp, (MetaDataComponent)null);
		ShowAlertIfNeeded(effectComp);
	}

	private void UpdateStatusEffectTime(EntityUid effect, TimeSpan? duration)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		StatusEffectComponent effectComp = default(StatusEffectComponent);
		if (!_effectQuery.TryComp(effect, ref effectComp))
		{
			return;
		}
		TimeSpan? endEffectTime = effectComp.EndEffectTime;
		if (!endEffectTime.HasValue)
		{
			return;
		}
		if (!duration.HasValue)
		{
			effectComp.EndEffectTime = null;
		}
		else
		{
			TimeSpan curTime = _timing.CurTime;
			endEffectTime = duration;
			TimeSpan? newEndTime = curTime + endEffectTime;
			if (effectComp.EndEffectTime >= newEndTime)
			{
				return;
			}
			effectComp.EndEffectTime = newEndTime;
		}
		((EntitySystem)this).Dirty(effect, (IComponent)(object)effectComp, (MetaDataComponent)null);
		ShowAlertIfNeeded(effectComp);
	}

	private void OnStatusEffectApplied(Entity<StatusEffectComponent> ent, ref StatusEffectAppliedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		StatusEffectComponent statusEffect = Entity<StatusEffectComponent>.op_Implicit(ent);
		ShowAlertIfNeeded(statusEffect);
	}

	private void OnStatusEffectRemoved(Entity<StatusEffectComponent> ent, ref StatusEffectRemovedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? appliedTo = ent.Comp.AppliedTo;
		if (!appliedTo.HasValue)
		{
			return;
		}
		StatusEffectComponent comp = ent.Comp;
		if (comp == null)
		{
			return;
		}
		appliedTo = comp.AppliedTo;
		if (appliedTo.HasValue)
		{
			ProtoId<AlertPrototype>? alert = comp.Alert;
			if (alert.HasValue)
			{
				_alerts.ClearAlert(ent.Comp.AppliedTo.Value, ent.Comp.Alert.Value);
			}
		}
	}

	private bool CanAddStatusEffect(EntityUid uid, EntProtoId effectProto)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		EntityPrototype effectProtoData = default(EntityPrototype);
		if (!_proto.TryIndex(effectProto, ref effectProtoData))
		{
			return false;
		}
		StatusEffectComponent effectProtoComp = default(StatusEffectComponent);
		if (!effectProtoData.TryGetComponent<StatusEffectComponent>(ref effectProtoComp, _compFactory))
		{
			return false;
		}
		if (!_whitelist.CheckBoth(uid, effectProtoComp.Blacklist, effectProtoComp.Whitelist))
		{
			return false;
		}
		BeforeStatusEffectAddedEvent ev = new BeforeStatusEffectAddedEvent(effectProto);
		((EntitySystem)this).RaiseLocalEvent<BeforeStatusEffectAddedEvent>(uid, ref ev, false);
		if (ev.Cancelled)
		{
			return false;
		}
		return true;
	}

	private bool TryAddStatusEffect(EntityUid target, EntProtoId effectProto, [NotNullWhen(true)] out EntityUid? statusEffect, TimeSpan? duration = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		statusEffect = null;
		if (!CanAddStatusEffect(target, effectProto))
		{
			return false;
		}
		StatusEffectContainerComponent container = ((EntitySystem)this).EnsureComp<StatusEffectContainerComponent>(target);
		EntityUid effect = ((EntitySystem)this).PredictedSpawnAttachedTo(EntProtoId.op_Implicit(effectProto), ((EntitySystem)this).Transform(target).Coordinates, (ComponentRegistry)null, default(Angle));
		_transform.SetParent(effect, target);
		StatusEffectComponent effectComp = default(StatusEffectComponent);
		if (!_effectQuery.TryComp(effect, ref effectComp))
		{
			return false;
		}
		statusEffect = effect;
		if (duration.HasValue)
		{
			StatusEffectComponent statusEffectComponent = effectComp;
			TimeSpan curTime = _timing.CurTime;
			TimeSpan? timeSpan = duration;
			statusEffectComponent.EndEffectTime = curTime + timeSpan;
		}
		container.ActiveStatusEffects.Add(effect);
		effectComp.AppliedTo = target;
		((EntitySystem)this).Dirty(target, (IComponent)(object)container, (MetaDataComponent)null);
		((EntitySystem)this).Dirty(effect, (IComponent)(object)effectComp, (MetaDataComponent)null);
		StatusEffectAppliedEvent ev = new StatusEffectAppliedEvent(target);
		((EntitySystem)this).RaiseLocalEvent<StatusEffectAppliedEvent>(effect, ref ev, false);
		return true;
	}

	private void ShowAlertIfNeeded(StatusEffectComponent effectComp)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (effectComp == null)
		{
			return;
		}
		EntityUid? appliedTo = effectComp.AppliedTo;
		if (appliedTo.HasValue)
		{
			ProtoId<AlertPrototype>? alert = effectComp.Alert;
			if (alert.HasValue)
			{
				TimeSpan? endEffectTime = effectComp.EndEffectTime;
				(TimeSpan, TimeSpan)? cooldown = ((!endEffectTime.HasValue) ? (((TimeSpan, TimeSpan)?)null) : new(TimeSpan, TimeSpan)?((_timing.CurTime, effectComp.EndEffectTime.Value)));
				AlertsSystem alerts = _alerts;
				EntityUid value = effectComp.AppliedTo.Value;
				ProtoId<AlertPrototype> value2 = effectComp.Alert.Value;
				(TimeSpan, TimeSpan)? cooldown2 = cooldown;
				alerts.ShowAlert(value, value2, null, cooldown2);
			}
		}
	}

	public bool TryAddStatusEffectDuration(EntityUid target, EntProtoId effectProto, [NotNullWhen(true)] out EntityUid? statusEffect, TimeSpan duration)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetStatusEffect(target, effectProto, out statusEffect))
		{
			return TryAddStatusEffect(target, effectProto, out statusEffect, duration);
		}
		AddStatusEffectTime(statusEffect.Value, duration);
		return true;
	}

	public bool TryAddStatusEffectDuration(EntityUid target, EntProtoId effectProto, TimeSpan duration)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? statusEffect;
		return TryAddStatusEffectDuration(target, effectProto, out statusEffect, duration);
	}

	public bool TrySetStatusEffectDuration(EntityUid target, EntProtoId effectProto, [NotNullWhen(true)] out EntityUid? statusEffect, TimeSpan? duration = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetStatusEffect(target, effectProto, out statusEffect))
		{
			return TryAddStatusEffect(target, effectProto, out statusEffect, duration);
		}
		SetStatusEffectTime(statusEffect.Value, duration);
		return true;
	}

	public bool TrySetStatusEffectDuration(EntityUid target, EntProtoId effectProto, TimeSpan? duration = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? statusEffect;
		return TrySetStatusEffectDuration(target, effectProto, out statusEffect, duration);
	}

	public bool TryUpdateStatusEffectDuration(EntityUid target, EntProtoId effectProto, [NotNullWhen(true)] out EntityUid? statusEffect, TimeSpan? duration = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetStatusEffect(target, effectProto, out statusEffect))
		{
			return TryAddStatusEffect(target, effectProto, out statusEffect, duration);
		}
		UpdateStatusEffectTime(statusEffect.Value, duration);
		return true;
	}

	public bool TryUpdateStatusEffectDuration(EntityUid target, EntProtoId effectProto, TimeSpan? duration = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? statusEffect;
		return TryUpdateStatusEffectDuration(target, effectProto, out statusEffect, duration);
	}

	public bool TryRemoveStatusEffect(EntityUid target, EntProtoId effectProto)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return false;
		}
		StatusEffectContainerComponent container = default(StatusEffectContainerComponent);
		if (!_containerQuery.TryComp(target, ref container))
		{
			return false;
		}
		StatusEffectComponent effectComp = default(StatusEffectComponent);
		foreach (EntityUid effect in container.ActiveStatusEffects)
		{
			MetaDataComponent meta = ((EntitySystem)this).MetaData(effect);
			if (meta.EntityPrototype != null && EntProtoId.op_Implicit(meta.EntityPrototype) == effectProto)
			{
				if (!_effectQuery.TryComp(effect, ref effectComp))
				{
					return false;
				}
				StatusEffectRemovedEvent ev = new StatusEffectRemovedEvent(target);
				((EntitySystem)this).RaiseLocalEvent<StatusEffectRemovedEvent>(effect, ref ev, false);
				((EntitySystem)this).QueueDel((EntityUid?)effect);
				container.ActiveStatusEffects.Remove(effect);
				((EntitySystem)this).Dirty(target, (IComponent)(object)container, (MetaDataComponent)null);
				return true;
			}
		}
		return false;
	}

	public bool HasStatusEffect(EntityUid target, EntProtoId effectProto)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		StatusEffectContainerComponent container = default(StatusEffectContainerComponent);
		if (!_containerQuery.TryComp(target, ref container))
		{
			return false;
		}
		foreach (EntityUid effect in container.ActiveStatusEffects)
		{
			MetaDataComponent meta = ((EntitySystem)this).MetaData(effect);
			if (meta.EntityPrototype != null && EntProtoId.op_Implicit(meta.EntityPrototype) == effectProto)
			{
				return true;
			}
		}
		return false;
	}

	public bool TryGetStatusEffect(EntityUid target, EntProtoId effectProto, [NotNullWhen(true)] out EntityUid? effect)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		effect = null;
		StatusEffectContainerComponent container = default(StatusEffectContainerComponent);
		if (!_containerQuery.TryComp(target, ref container))
		{
			return false;
		}
		MetaDataComponent meta = default(MetaDataComponent);
		foreach (EntityUid e in container.ActiveStatusEffects)
		{
			if (((EntitySystem)this).TryComp(e, ref meta) && meta.EntityPrototype != null && EntProtoId.op_Implicit(meta.EntityPrototype) == effectProto)
			{
				effect = e;
				return true;
			}
		}
		return false;
	}

	public bool TryGetTime(EntityUid uid, EntProtoId effectProto, out (EntityUid EffectEnt, TimeSpan? EndEffectTime) time, StatusEffectContainerComponent? container = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		time = default((EntityUid, TimeSpan?));
		if (!((EntitySystem)this).Resolve<StatusEffectContainerComponent>(uid, ref container, true))
		{
			return false;
		}
		StatusEffectComponent effectComp = default(StatusEffectComponent);
		foreach (EntityUid effect in container.ActiveStatusEffects)
		{
			MetaDataComponent meta = ((EntitySystem)this).MetaData(effect);
			if (meta.EntityPrototype != null && EntProtoId.op_Implicit(meta.EntityPrototype) == effectProto)
			{
				if (!_effectQuery.TryComp(effect, ref effectComp))
				{
					return false;
				}
				time = (EffectEnt: effect, EndEffectTime: effectComp.EndEffectTime);
				return true;
			}
		}
		return false;
	}

	public bool TryGetMaxTime<T>(EntityUid uid, out (EntityUid EffectEnt, TimeSpan? EndEffectTime) time) where T : IComponent
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		time = default((EntityUid, TimeSpan?));
		if (!TryEffectsWithComp<T>(uid, out HashSet<Entity<T, StatusEffectComponent>> status))
		{
			return false;
		}
		time.EndEffectTime = TimeSpan.Zero;
		foreach (Entity<T, StatusEffectComponent> effect in status)
		{
			if (!effect.Comp2.EndEffectTime.HasValue)
			{
				time = (EffectEnt: effect.Owner, EndEffectTime: null);
				return true;
			}
			if (effect.Comp2.EndEffectTime > time.EndEffectTime)
			{
				time = (EffectEnt: effect.Owner, EndEffectTime: effect.Comp2.EndEffectTime);
			}
		}
		return true;
	}

	public bool TryAddTime(EntityUid uid, EntProtoId effectProto, TimeSpan time)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		StatusEffectContainerComponent container = default(StatusEffectContainerComponent);
		if (!_containerQuery.TryComp(uid, ref container))
		{
			return false;
		}
		foreach (EntityUid effect in container.ActiveStatusEffects)
		{
			MetaDataComponent meta = ((EntitySystem)this).MetaData(effect);
			if (meta.EntityPrototype != null && EntProtoId.op_Implicit(meta.EntityPrototype) == effectProto)
			{
				AddStatusEffectTime(effect, time);
				return true;
			}
		}
		return false;
	}

	public bool TrySetTime(EntityUid uid, EntProtoId effectProto, TimeSpan time)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		StatusEffectContainerComponent container = default(StatusEffectContainerComponent);
		if (!_containerQuery.TryComp(uid, ref container))
		{
			return false;
		}
		foreach (EntityUid effect in container.ActiveStatusEffects)
		{
			MetaDataComponent meta = ((EntitySystem)this).MetaData(effect);
			if (meta.EntityPrototype != null && EntProtoId.op_Implicit(meta.EntityPrototype) == effectProto)
			{
				SetStatusEffectTime(effect, time);
				return true;
			}
		}
		return false;
	}

	public bool HasEffectComp<T>(EntityUid? target) where T : IComponent
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		StatusEffectContainerComponent container = default(StatusEffectContainerComponent);
		if (!_containerQuery.TryComp(target, ref container))
		{
			return false;
		}
		foreach (EntityUid effect in container.ActiveStatusEffects)
		{
			if (((EntitySystem)this).HasComp<T>(effect))
			{
				return true;
			}
		}
		return false;
	}

	public bool TryEffectsWithComp<T>(EntityUid? target, [NotNullWhen(true)] out HashSet<Entity<T, StatusEffectComponent>>? effects) where T : IComponent
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		effects = null;
		StatusEffectContainerComponent container = default(StatusEffectContainerComponent);
		if (!_containerQuery.TryComp(target, ref container))
		{
			return false;
		}
		StatusEffectComponent statusComp = default(StatusEffectComponent);
		T comp = default(T);
		foreach (EntityUid effect in container.ActiveStatusEffects)
		{
			if (_effectQuery.TryComp(effect, ref statusComp) && ((EntitySystem)this).TryComp<T>(effect, ref comp))
			{
				if (effects == null)
				{
					effects = new HashSet<Entity<T, StatusEffectComponent>>();
				}
				effects.Add(Entity<T, StatusEffectComponent>.op_Implicit((effect, comp, statusComp)));
			}
		}
		return effects != null;
	}

	public bool TryGetEffectsEndTimeWithComp<T>(EntityUid? target, out TimeSpan? endTime) where T : IComponent
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		endTime = _timing.CurTime;
		StatusEffectContainerComponent container = default(StatusEffectContainerComponent);
		if (!_containerQuery.TryComp(target, ref container))
		{
			return false;
		}
		StatusEffectComponent statusComp = default(StatusEffectComponent);
		foreach (EntityUid effect in container.ActiveStatusEffects)
		{
			if (((EntitySystem)this).HasComp<T>(effect) && _effectQuery.TryComp(effect, ref statusComp))
			{
				TimeSpan? endEffectTime = statusComp.EndEffectTime;
				if (!endEffectTime.HasValue)
				{
					endTime = null;
					return true;
				}
				if (statusComp.EndEffectTime > endTime)
				{
					endTime = statusComp.EndEffectTime;
				}
			}
		}
		TimeSpan? timeSpan = endTime;
		return timeSpan.HasValue;
	}

	protected void InitializeRelay()
	{
		((EntitySystem)this).SubscribeLocalEvent<StatusEffectContainerComponent, LocalPlayerAttachedEvent>((ComponentEventHandler<StatusEffectContainerComponent, LocalPlayerAttachedEvent>)RelayStatusEffectEvent<LocalPlayerAttachedEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StatusEffectContainerComponent, LocalPlayerDetachedEvent>((ComponentEventHandler<StatusEffectContainerComponent, LocalPlayerDetachedEvent>)RelayStatusEffectEvent<LocalPlayerDetachedEvent>, (Type[])null, (Type[])null);
	}

	protected void RefRelayStatusEffectEvent<T>(EntityUid uid, StatusEffectContainerComponent component, ref T args) where T : struct
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		RelayEvent(Entity<StatusEffectContainerComponent>.op_Implicit((uid, component)), ref args);
	}

	protected void RelayStatusEffectEvent<T>(EntityUid uid, StatusEffectContainerComponent component, T args) where T : class
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		RelayEvent(Entity<StatusEffectContainerComponent>.op_Implicit((uid, component)), args);
	}

	public void RelayEvent<T>(Entity<StatusEffectContainerComponent> statusEffect, ref T args) where T : struct
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		StatusEffectRelayedEvent<T> ev = new StatusEffectRelayedEvent<T>(args);
		foreach (EntityUid activeEffect in statusEffect.Comp.ActiveStatusEffects)
		{
			((EntitySystem)this).RaiseLocalEvent<StatusEffectRelayedEvent<T>>(activeEffect, ref ev, false);
		}
		args = ev.Args;
	}

	public void RelayEvent<T>(Entity<StatusEffectContainerComponent> statusEffect, T args) where T : class
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		StatusEffectRelayedEvent<T> ev = new StatusEffectRelayedEvent<T>(args);
		foreach (EntityUid activeEffect in statusEffect.Comp.ActiveStatusEffects)
		{
			((EntitySystem)this).RaiseLocalEvent<StatusEffectRelayedEvent<T>>(activeEffect, ref ev, false);
		}
	}
}
