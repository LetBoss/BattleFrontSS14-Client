using System;
using Content.Shared.Temperature.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Temperature.Systems;

public sealed class AlwaysHotSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AlwaysHotComponent, IsHotEvent>((EntityEventRefHandler<AlwaysHotComponent, IsHotEvent>)OnIsHot, (Type[])null, (Type[])null);
	}

	private void OnIsHot(Entity<AlwaysHotComponent> ent, ref IsHotEvent args)
	{
		args.IsHot = true;
	}
}
