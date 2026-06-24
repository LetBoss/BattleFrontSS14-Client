using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Inventory;
using Content.Shared.Containers.ItemSlots;
using Robust.Client.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Inventory;

public sealed class CMInventorySystem : SharedCMInventorySystem
{
	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CMItemSlotsComponent, AppearanceChangeEvent>((EntityEventRefHandler<CMItemSlotsComponent, AppearanceChangeEvent>)OnItemSlotsAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnItemSlotsAppearanceChange(Entity<CMItemSlotsComponent> ent, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ContentsUpdated(ent);
	}

	protected override void ContentsUpdated(Entity<CMItemSlotsComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		base.ContentsUpdated(ent);
		SpriteComponent item = default(SpriteComponent);
		int num = default(int);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(Entity<CMItemSlotsComponent>.op_Implicit(ent), ref item) || !_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), (Enum)CMItemSlotsLayers.Fill, ref num, false))
		{
			return;
		}
		ItemSlotsComponent itemSlotsComponent = default(ItemSlotsComponent);
		if (!((EntitySystem)this).TryComp<ItemSlotsComponent>(Entity<CMItemSlotsComponent>.op_Implicit(ent), ref itemSlotsComponent))
		{
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), num, false);
			return;
		}
		foreach (KeyValuePair<string, ItemSlot> slot in itemSlotsComponent.Slots)
		{
			slot.Deconstruct(out var _, out var value);
			ContainerSlot? containerSlot = value.ContainerSlot;
			EntityUid? val = ((containerSlot != null) ? containerSlot.ContainedEntity : ((EntityUid?)null));
			if (val.HasValue)
			{
				EntityUid valueOrDefault = val.GetValueOrDefault();
				if (!((EntitySystem)this).TerminatingOrDeleted(valueOrDefault, (MetaDataComponent)null))
				{
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), num, true);
					return;
				}
			}
		}
		_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), num, false);
	}
}
