using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Temperature.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Temperature.Systems;

public sealed class SharedTemperatureSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeedModifier;

	private static readonly TimeSpan SlowdownApplicationDelay = TimeSpan.FromSeconds(1.0);

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<TemperatureSpeedComponent, OnTemperatureChangeEvent>((EntityEventRefHandler<TemperatureSpeedComponent, OnTemperatureChangeEvent>)OnTemperatureChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TemperatureSpeedComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<TemperatureSpeedComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshMovementSpeedModifiers, (Type[])null, (Type[])null);
	}

	private void OnTemperatureChanged(Entity<TemperatureSpeedComponent> ent, ref OnTemperatureChangeEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		foreach (var (threshold, modifier) in ent.Comp.Thresholds)
		{
			if ((args.CurrentTemperature < threshold && args.LastTemperature > threshold) || (args.CurrentTemperature > threshold && args.LastTemperature < threshold))
			{
				ent.Comp.NextSlowdownUpdate = _timing.CurTime + SlowdownApplicationDelay;
				ent.Comp.CurrentSpeedModifier = modifier;
				((EntitySystem)this).Dirty<TemperatureSpeedComponent>(ent, (MetaDataComponent)null);
				break;
			}
		}
		float maxThreshold = ent.Comp.Thresholds.Max((KeyValuePair<float, float> p) => p.Key);
		if (args.CurrentTemperature > maxThreshold && args.LastTemperature < maxThreshold)
		{
			ent.Comp.NextSlowdownUpdate = _timing.CurTime + SlowdownApplicationDelay;
			ent.Comp.CurrentSpeedModifier = null;
			((EntitySystem)this).Dirty<TemperatureSpeedComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnRefreshMovementSpeedModifiers(Entity<TemperatureSpeedComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.NextSlowdownUpdate.HasValue && ent.Comp.CurrentSpeedModifier.HasValue)
		{
			args.ModifySpeed(ent.Comp.CurrentSpeedModifier.Value, ent.Comp.CurrentSpeedModifier.Value);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<TemperatureSpeedComponent, MovementSpeedModifierComponent> query = ((EntitySystem)this).EntityQueryEnumerator<TemperatureSpeedComponent, MovementSpeedModifierComponent>();
		EntityUid uid = default(EntityUid);
		TemperatureSpeedComponent temp = default(TemperatureSpeedComponent);
		MovementSpeedModifierComponent movement = default(MovementSpeedModifierComponent);
		while (query.MoveNext(ref uid, ref temp, ref movement))
		{
			if (temp.NextSlowdownUpdate.HasValue)
			{
				TimeSpan curTime = _timing.CurTime;
				TimeSpan? nextSlowdownUpdate = temp.NextSlowdownUpdate;
				if (!(curTime < nextSlowdownUpdate))
				{
					temp.NextSlowdownUpdate = null;
					_movementSpeedModifier.RefreshMovementSpeedModifiers(uid, movement);
					((EntitySystem)this).Dirty(uid, (IComponent)(object)temp, (MetaDataComponent)null);
				}
			}
		}
	}
}
