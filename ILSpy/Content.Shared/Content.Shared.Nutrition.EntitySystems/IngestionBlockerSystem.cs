using System;
using Content.Shared.Clothing;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Nutrition.EntitySystems;

public sealed class IngestionBlockerSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<IngestionBlockerComponent, ItemMaskToggledEvent>((EntityEventRefHandler<IngestionBlockerComponent, ItemMaskToggledEvent>)OnBlockerMaskToggled, (Type[])null, (Type[])null);
	}

	private void OnBlockerMaskToggled(Entity<IngestionBlockerComponent> ent, ref ItemMaskToggledEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Enabled = !args.Mask.Comp.IsToggled;
	}
}
