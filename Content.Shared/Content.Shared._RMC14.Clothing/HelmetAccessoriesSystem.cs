using System;
using Content.Shared.Clothing;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Storage;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Clothing;

public sealed class HelmetAccessoriesSystem : EntitySystem
{
	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedItemSystem _item;

	[Dependency]
	private ItemToggleSystem _itemToggle;

	private EntityQuery<StorageComponent> _storageQuery;

	private EntityQuery<HelmetAccessoryComponent> _accessoryQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_storageQuery = ((EntitySystem)this).GetEntityQuery<StorageComponent>();
		_accessoryQuery = ((EntitySystem)this).GetEntityQuery<HelmetAccessoryComponent>();
		((EntitySystem)this).SubscribeLocalEvent<HelmetAccessoryHolderComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<HelmetAccessoryHolderComponent, EntInsertedIntoContainerMessage>)OnEntInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HelmetAccessoryHolderComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<HelmetAccessoryHolderComponent, EntRemovedFromContainerMessage>)OnEntRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HelmetAccessoryHolderComponent, GetEquipmentVisualsEvent>((EntityEventRefHandler<HelmetAccessoryHolderComponent, GetEquipmentVisualsEvent>)OnGetEquipmentVisuals, (Type[])null, new Type[1] { typeof(ClothingSystem) });
		((EntitySystem)this).SubscribeLocalEvent<HelmetAccessoryComponent, ItemToggledEvent>((EntityEventRefHandler<HelmetAccessoryComponent, ItemToggledEvent>)OnToggled, (Type[])null, (Type[])null);
	}

	private void OnEntInserted(Entity<HelmetAccessoryHolderComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_item.VisualsChanged(Entity<HelmetAccessoryHolderComponent>.op_Implicit(ent));
	}

	private void OnEntRemoved(Entity<HelmetAccessoryHolderComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_item.VisualsChanged(Entity<HelmetAccessoryHolderComponent>.op_Implicit(ent));
	}

	private void OnToggled(Entity<HelmetAccessoryComponent> ent, ref ItemToggledEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		if (((EntitySystem)this).TryComp(Entity<HelmetAccessoryComponent>.op_Implicit(ent), ref xform) && !((EntitySystem)this).TerminatingOrDeleted(xform.ParentUid, (MetaDataComponent)null))
		{
			_item.VisualsChanged(xform.ParentUid);
		}
	}

	private void OnGetEquipmentVisuals(Entity<HelmetAccessoryHolderComponent> ent, ref GetEquipmentVisualsEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Expected O, but got Unknown
		StorageComponent storage = default(StorageComponent);
		if ((_inventory.TryGetSlot(args.Equipee, args.Slot, out SlotDefinition slot) && (slot.SlotFlags & ent.Comp.Slot) == 0) || !_storageQuery.TryComp(ent.Owner, ref storage) || storage.Container == null)
		{
			return;
		}
		int index = 0;
		HelmetAccessoryComponent accessoryComp = default(HelmetAccessoryComponent);
		foreach (EntityUid item in ((BaseContainer)storage.Container).ContainedEntities)
		{
			string layer = $"enum.{"HelmetAccessoryLayers"}.{HelmetAccessoryLayers.Helmet}{index}_{((EntitySystem)this).Name(ent.Owner, (MetaDataComponent)null)}";
			if (_accessoryQuery.TryComp(item, ref accessoryComp))
			{
				Rsi rsi = (Rsi)((_itemToggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(item)) && accessoryComp.ToggledRsi != null) ? ((ent.Comp.IsHat && accessoryComp.HatToggledRsi != null) ? accessoryComp.HatToggledRsi : accessoryComp.ToggledRsi) : ((ent.Comp.IsHat && accessoryComp.HatRsi != null) ? ((object)accessoryComp.HatRsi) : ((object)accessoryComp.Rsi)));
				args.Layers.Add((layer, new PrototypeLayerData
				{
					RsiPath = ((object)rsi.RsiPath/*cast due to constrained. prefix*/).ToString(),
					State = rsi.RsiState,
					Visible = true
				}));
				index++;
			}
		}
	}
}
