using System;
using Content.Shared.Light.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Shared.Light.EntitySystems;

public abstract class SharedSunShadowSystem : EntitySystem
{
	[Dependency]
	private IRobustRandom _random;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SunShadowCycleComponent, MapInitEvent>((EntityEventRefHandler<SunShadowCycleComponent, MapInitEvent>)OnCycleMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SunShadowCycleComponent, LightCycleOffsetEvent>((EntityEventRefHandler<SunShadowCycleComponent, LightCycleOffsetEvent>)OnCycleOffset, (Type[])null, (Type[])null);
	}

	private void OnCycleOffset(Entity<SunShadowCycleComponent> ent, ref LightCycleOffsetEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Offset = args.Offset;
		((EntitySystem)this).Dirty<SunShadowCycleComponent>(ent, (MetaDataComponent)null);
	}

	private void OnCycleMapInit(Entity<SunShadowCycleComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		LightCycleComponent lightCycle = default(LightCycleComponent);
		if (((EntitySystem)this).TryComp<LightCycleComponent>(ent.Owner, ref lightCycle))
		{
			ent.Comp.Duration = lightCycle.Duration;
			ent.Comp.Offset = lightCycle.Offset;
		}
		else
		{
			ent.Comp.Offset = _random.Next(ent.Comp.Duration);
		}
		((EntitySystem)this).Dirty<SunShadowCycleComponent>(ent, (MetaDataComponent)null);
	}
}
