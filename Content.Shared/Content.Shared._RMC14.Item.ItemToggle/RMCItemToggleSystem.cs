using System;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Item;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Item.ItemToggle;

public sealed class RMCItemToggleSystem : EntitySystem
{
	[Dependency]
	private ClothingSystem _clothing;

	[Dependency]
	private SharedItemSystem _item;

	private EntityQuery<ItemToggleComponent> _query;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_query = ((EntitySystem)this).GetEntityQuery<ItemToggleComponent>();
		((EntitySystem)this).SubscribeLocalEvent<RMCItemToggleClothingVisualsComponent, ItemToggledEvent>((EntityEventRefHandler<RMCItemToggleClothingVisualsComponent, ItemToggledEvent>)OnToggled, (Type[])null, (Type[])null);
	}

	private void OnToggled(Entity<RMCItemToggleClothingVisualsComponent> ent, ref ItemToggledEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		string prefix = (args.Activated ? ent.Comp.Prefix : null);
		_item.SetHeldPrefix(Entity<RMCItemToggleClothingVisualsComponent>.op_Implicit(ent), prefix);
		_clothing.SetEquippedPrefix(Entity<RMCItemToggleClothingVisualsComponent>.op_Implicit(ent), prefix);
	}
}
