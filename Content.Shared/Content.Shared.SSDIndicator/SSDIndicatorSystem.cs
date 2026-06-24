using System;
using Content.Shared.CCVar;
using Content.Shared.StatusEffectNew;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.SSDIndicator;

public sealed class SSDIndicatorSystem : EntitySystem
{
	public static readonly EntProtoId StatusEffectSSDSleeping = EntProtoId.op_Implicit("StatusEffectSSDSleeping");

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedStatusEffectsSystem _statusEffects;

	private bool _icSsdSleep;

	private float _icSsdSleepTime;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<SSDIndicatorComponent, PlayerAttachedEvent>((ComponentEventHandler<SSDIndicatorComponent, PlayerAttachedEvent>)OnPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SSDIndicatorComponent, PlayerDetachedEvent>((ComponentEventHandler<SSDIndicatorComponent, PlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SSDIndicatorComponent, MapInitEvent>((ComponentEventHandler<SSDIndicatorComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		_cfg.OnValueChanged<bool>(CCVars.ICSSDSleep, (Action<bool>)delegate(bool obj)
		{
			_icSsdSleep = obj;
		}, true);
		_cfg.OnValueChanged<float>(CCVars.ICSSDSleepTime, (Action<float>)delegate(float obj)
		{
			_icSsdSleepTime = obj;
		}, true);
	}

	private void OnPlayerAttached(EntityUid uid, SSDIndicatorComponent component, PlayerAttachedEvent args)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		component.IsSSD = false;
		if (_icSsdSleep)
		{
			component.FallAsleepTime = TimeSpan.Zero;
			_statusEffects.TryRemoveStatusEffect(uid, StatusEffectSSDSleeping);
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void OnPlayerDetached(EntityUid uid, SSDIndicatorComponent component, PlayerDetachedEvent args)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		component.IsSSD = true;
		if (_icSsdSleep)
		{
			component.FallAsleepTime = _timing.CurTime + TimeSpan.FromSeconds(_icSsdSleepTime);
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void OnMapInit(EntityUid uid, SSDIndicatorComponent component, MapInitEvent args)
	{
		if (_icSsdSleep && component.IsSSD && component.FallAsleepTime == TimeSpan.Zero)
		{
			component.FallAsleepTime = _timing.CurTime + TimeSpan.FromSeconds(_icSsdSleepTime);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (!_icSsdSleep)
		{
			return;
		}
		EntityQueryEnumerator<SSDIndicatorComponent> query = ((EntitySystem)this).EntityQueryEnumerator<SSDIndicatorComponent>();
		EntityUid uid = default(EntityUid);
		SSDIndicatorComponent ssd = default(SSDIndicatorComponent);
		while (query.MoveNext(ref uid, ref ssd))
		{
			if (ssd.IsSSD && ssd.FallAsleepTime <= _timing.CurTime && !((EntitySystem)this).TerminatingOrDeleted(uid, (MetaDataComponent)null))
			{
				_statusEffects.TrySetStatusEffectDuration(uid, StatusEffectSSDSleeping);
			}
		}
	}
}
