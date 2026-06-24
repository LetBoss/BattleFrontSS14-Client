using System;
using Content.Shared.Clothing.Components;
using Content.Shared.Gravity;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Clothing.EntitySystems;

public sealed class AntiGravityClothingSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<AntiGravityClothingComponent, InventoryRelayedEvent<IsWeightlessEvent>>((EntityEventRefHandler<AntiGravityClothingComponent, InventoryRelayedEvent<IsWeightlessEvent>>)OnIsWeightless, (Type[])null, (Type[])null);
	}

	private void OnIsWeightless(Entity<AntiGravityClothingComponent> ent, ref InventoryRelayedEvent<IsWeightlessEvent> args)
	{
		if (!args.Args.Handled)
		{
			args.Args.Handled = true;
			args.Args.IsWeightless = true;
		}
	}
}
