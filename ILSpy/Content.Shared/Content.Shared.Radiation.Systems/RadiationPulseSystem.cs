using System;
using Content.Shared.Radiation.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Spawners;
using Robust.Shared.Timing;

namespace Content.Shared.Radiation.Systems;

public sealed class RadiationPulseSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RadiationPulseComponent, ComponentStartup>((ComponentEventHandler<RadiationPulseComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
	}

	private void OnStartup(EntityUid uid, RadiationPulseComponent component, ComponentStartup args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		component.StartTime = _timing.RealTime;
		TimedDespawnComponent despawn = default(TimedDespawnComponent);
		if (((EntitySystem)this).TryComp<TimedDespawnComponent>(uid, ref despawn))
		{
			component.VisualDuration = despawn.Lifetime;
		}
		RadiationSourceComponent radSource = default(RadiationSourceComponent);
		if (((EntitySystem)this).TryComp<RadiationSourceComponent>(uid, ref radSource))
		{
			component.VisualRange = radSource.Intensity / radSource.Slope;
		}
	}
}
