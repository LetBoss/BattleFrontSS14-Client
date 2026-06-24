using System;
using Content.Shared._RMC14.Communications;
using Content.Shared._RMC14.Power;
using Content.Shared._RMC14.Sensor;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Tools;

public sealed class RMCDeviceBreakerSystem : EntitySystem
{
	[Dependency]
	private SharedDoAfterSystem _doafter;

	[Dependency]
	private SharedRMCPowerSystem _power;

	[Dependency]
	private SharedDestructibleSystem _destroy;

	[Dependency]
	private SensorTowerSystem _sensor;

	[Dependency]
	private SharedAudioSystem _audio;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCDeviceBreakerComponent, RMCDeviceBreakerDoAfterEvent>((EntityEventRefHandler<RMCDeviceBreakerComponent, RMCDeviceBreakerDoAfterEvent>)OnDeviceBreakerDoafter, (Type[])null, (Type[])null);
	}

	private void OnDeviceBreakerDoafter(Entity<RMCDeviceBreakerComponent> breaker, ref RMCDeviceBreakerDoAfterEvent args)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && !args.Cancelled && args.Target.HasValue && CanBreak(args.Target.Value))
		{
			((HandledEntityEventArgs)args).Handled = true;
			Break(args.Target.Value, args.User);
			_audio.PlayPredicted(breaker.Comp.UseSound, Entity<RMCDeviceBreakerComponent>.op_Implicit(breaker), (EntityUid?)args.User, (AudioParams?)null);
			if (breaker.Comp.Repeat && CanBreak(args.Target.Value))
			{
				DoAfterArgs doafter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, breaker.Comp.DoAfterTime, new RMCDeviceBreakerDoAfterEvent(), args.Used, args.Target, args.Used)
				{
					BreakOnMove = true,
					RequireCanInteract = true,
					BreakOnHandChange = true,
					DuplicateCondition = DuplicateConditions.SameTool
				};
				_doafter.TryStartDoAfter(doafter);
			}
		}
	}

	private bool CanBreak(EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		RMCFusionReactorComponent reactor = default(RMCFusionReactorComponent);
		if (((EntitySystem)this).TryComp<RMCFusionReactorComponent>(target, ref reactor) && reactor.State != RMCFusionReactorState.Weld)
		{
			return true;
		}
		CommunicationsTowerComponent comms = default(CommunicationsTowerComponent);
		if (((EntitySystem)this).TryComp<CommunicationsTowerComponent>(target, ref comms) && comms.State != CommunicationsTowerState.Broken)
		{
			return true;
		}
		SensorTowerComponent sensors = default(SensorTowerComponent);
		if (((EntitySystem)this).TryComp<SensorTowerComponent>(target, ref sensors) && sensors.State != SensorTowerState.Weld)
		{
			return true;
		}
		return false;
	}

	private void Break(EntityUid target, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		RMCFusionReactorComponent reactor = default(RMCFusionReactorComponent);
		if (((EntitySystem)this).TryComp<RMCFusionReactorComponent>(target, ref reactor) && reactor.State != RMCFusionReactorState.Weld)
		{
			_power.DestroyReactor(Entity<RMCFusionReactorComponent>.op_Implicit((target, reactor)), user);
		}
		CommunicationsTowerComponent comms = default(CommunicationsTowerComponent);
		if (((EntitySystem)this).TryComp<CommunicationsTowerComponent>(target, ref comms) && comms.State != CommunicationsTowerState.Broken)
		{
			_destroy.BreakEntity(target);
		}
		SensorTowerComponent sensors = default(SensorTowerComponent);
		if (((EntitySystem)this).TryComp<SensorTowerComponent>(target, ref sensors) && sensors.State != SensorTowerState.Weld)
		{
			_sensor.SensorTowerIncrementalDestroy(Entity<SensorTowerComponent>.op_Implicit((target, sensors)));
		}
	}
}
