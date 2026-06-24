using System;
using Content.Shared.Beeper.Components;
using Content.Shared.ProximityDetection;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Beeper.Systems;

public sealed class ProximityBeeperSystem : EntitySystem
{
	[Dependency]
	private BeeperSystem _beeper;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<ProximityBeeperComponent, NewProximityTargetEvent>((ComponentEventRefHandler<ProximityBeeperComponent, NewProximityTargetEvent>)OnNewProximityTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ProximityBeeperComponent, ProximityTargetUpdatedEvent>((ComponentEventRefHandler<ProximityBeeperComponent, ProximityTargetUpdatedEvent>)OnProximityTargetUpdate, (Type[])null, (Type[])null);
	}

	private void OnProximityTargetUpdate(EntityUid owner, ProximityBeeperComponent proxBeeper, ref ProximityTargetUpdatedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		BeeperComponent beeper = default(BeeperComponent);
		if (((EntitySystem)this).TryComp<BeeperComponent>(owner, ref beeper))
		{
			_beeper.SetIntervalScaling(owner, args.Distance / args.Detector.Comp.Range, beeper);
		}
	}

	private void OnNewProximityTarget(EntityUid owner, ProximityBeeperComponent proxBeeper, ref NewProximityTargetEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_beeper.SetMute(owner, !args.Target.HasValue);
	}
}
