using System;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Effect;

public abstract class SharedRMCEffectSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<EffectAlphaAnimationComponent, MapInitEvent>((EntityEventRefHandler<EffectAlphaAnimationComponent, MapInitEvent>)OnAlphaAnimationMapInit, (Type[])null, (Type[])null);
	}

	private void OnAlphaAnimationMapInit(Entity<EffectAlphaAnimationComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.SpawnedAt = _timing.CurTime;
		((EntitySystem)this).Dirty<EffectAlphaAnimationComponent>(ent, (MetaDataComponent)null);
	}
}
