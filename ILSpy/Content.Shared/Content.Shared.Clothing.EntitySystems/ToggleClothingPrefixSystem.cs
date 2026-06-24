using System;
using Content.Shared.Clothing.Components;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Clothing.EntitySystems;

public sealed class ToggleClothingPrefixSystem : EntitySystem
{
	[Dependency]
	private ClothingSystem _clothing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ToggleClothingPrefixComponent, ItemToggledEvent>((EntityEventRefHandler<ToggleClothingPrefixComponent, ItemToggledEvent>)OnToggled, (Type[])null, (Type[])null);
	}

	private void OnToggled(Entity<ToggleClothingPrefixComponent> ent, ref ItemToggledEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		_clothing.SetEquippedPrefix(Entity<ToggleClothingPrefixComponent>.op_Implicit(ent), args.Activated ? ent.Comp.PrefixOn : ent.Comp.PrefixOff);
	}
}
