using System;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory;
using Content.Shared.Movement.Events;
using Robust.Shared.GameObjects;

namespace Content.Shared.Clothing.EntitySystems;

public sealed class SpeedModifierContactCapClothingSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SpeedModifierContactCapClothingComponent, InventoryRelayedEvent<GetSpeedModifierContactCapEvent>>((EntityEventRefHandler<SpeedModifierContactCapClothingComponent, InventoryRelayedEvent<GetSpeedModifierContactCapEvent>>)OnGetMaxSlow, (Type[])null, (Type[])null);
	}

	private void OnGetMaxSlow(Entity<SpeedModifierContactCapClothingComponent> ent, ref InventoryRelayedEvent<GetSpeedModifierContactCapEvent> args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		args.Args.SetIfMax(ent.Comp.MaxContactSprintSlowdown, ent.Comp.MaxContactWalkSlowdown);
	}
}
