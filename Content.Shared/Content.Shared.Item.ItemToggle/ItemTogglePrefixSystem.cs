using System;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Item.ItemToggle;

public sealed class ItemTogglePrefixSystem : EntitySystem
{
	[Dependency]
	private SharedItemSystem _item;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ItemTogglePrefixComponent, ItemToggledEvent>((EntityEventRefHandler<ItemTogglePrefixComponent, ItemToggledEvent>)OnToggled, (Type[])null, (Type[])null);
	}

	private void OnToggled(Entity<ItemTogglePrefixComponent> ent, ref ItemToggledEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		_item.SetHeldPrefix(ent.Owner, args.Activated ? ent.Comp.PrefixOn : ent.Comp.PrefixOff);
	}
}
