using System;
using Content.Shared.Power.Components;
using Content.Shared.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Shared.Power.EntitySystems;

public abstract class SharedActivatableUIRequiresPowerSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIRequiresPowerComponent, ActivatableUIOpenAttemptEvent>((EntityEventRefHandler<ActivatableUIRequiresPowerComponent, ActivatableUIOpenAttemptEvent>)OnActivate, (Type[])null, (Type[])null);
	}

	protected abstract void OnActivate(Entity<ActivatableUIRequiresPowerComponent> ent, ref ActivatableUIOpenAttemptEvent args);
}
