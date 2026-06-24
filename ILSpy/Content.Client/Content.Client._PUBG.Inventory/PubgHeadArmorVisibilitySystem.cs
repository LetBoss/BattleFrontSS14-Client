using System;
using System.Collections.Generic;
using Content.Client.Clothing;
using Content.Client.Inventory;
using Content.Shared._PUBG.Armor;
using Content.Shared.Armor;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._PUBG.Inventory;

public sealed class PubgHeadArmorVisibilitySystem : EntitySystem
{
	private const string HeadSlot = "head";

	private const string HeadArmorSlot = "headArmor";

	private const string OuterClothingSlot = "outerClothing";

	private const string OuterArmorSlot = "outerArmor";

	private const string OuterArmorLayerAnchor = "enum.SquadArmorLayers.Armor";

	private const string OuterArmorFallbackAnchor = "pubg-outer-armor-anchor";

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private ClientClothingSystem _clothing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, DidEquipEvent>((ComponentEventHandler<InventoryComponent, DidEquipEvent>)OnDidEquip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, DidUnequipEvent>((ComponentEventHandler<InventoryComponent, DidUnequipEvent>)OnDidUnequip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, InventoryTemplateUpdated>((ComponentEventRefHandler<InventoryComponent, InventoryTemplateUpdated>)OnInventoryTemplateUpdated, (Type[])null, new Type[1] { typeof(ClientClothingSystem) });
	}

	private void OnDidEquip(EntityUid uid, InventoryComponent component, DidEquipEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (IsTrackedSlot(args.Slot))
		{
			UpdateLayerVisibility(uid, component);
		}
	}

	private void OnDidUnequip(EntityUid uid, InventoryComponent component, DidUnequipEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (IsTrackedSlot(args.Slot))
		{
			UpdateLayerVisibility(uid, component);
		}
	}

	private void OnInventoryTemplateUpdated(EntityUid uid, InventoryComponent component, ref InventoryTemplateUpdated args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateLayerVisibility(uid, component);
	}

	private void UpdateLayerVisibility(EntityUid uid, InventoryComponent inventory)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = default(SpriteComponent);
		InventorySlotsComponent inventorySlots = default(InventorySlotsComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref sprite) && ((EntitySystem)this).TryComp<InventorySlotsComponent>(uid, ref inventorySlots))
		{
			EnsureOuterArmorLayerAlias(uid, sprite, inventory);
			_clothing.InitClothing(uid, inventory);
			if (HasArmorHelmet(uid, inventory))
			{
				HideHeadSlotLayers(uid, sprite, inventorySlots);
			}
		}
	}

	private bool HasArmorHelmet(EntityUid uid, InventoryComponent inventory)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!_inventory.TryGetSlotEntity(uid, "headArmor", out var entityUid, inventory))
		{
			return false;
		}
		PubgArmorComponent pubgArmorComponent = default(PubgArmorComponent);
		ArmorComponent armorComponent = default(ArmorComponent);
		if (!((EntitySystem)this).TryComp<PubgArmorComponent>(entityUid.Value, ref pubgArmorComponent))
		{
			return ((EntitySystem)this).TryComp<ArmorComponent>(entityUid.Value, ref armorComponent);
		}
		return true;
	}

	private bool IsTrackedSlot(string slot)
	{
		switch (slot)
		{
		default:
			return slot == "outerArmor";
		case "head":
		case "headArmor":
		case "outerClothing":
			return true;
		}
	}

	private void EnsureOuterArmorLayerAlias(EntityUid uid, SpriteComponent sprite, InventoryComponent inventory)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		if (HasSlot(inventory, "outerArmor") && TryGetOuterArmorAnchor(uid, sprite, out var armorLayer) && (!_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), "outerArmor", ref num, false) || num != armorLayer))
		{
			_sprite.LayerMapSet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), "outerArmor", armorLayer);
		}
	}

	private bool TryGetOuterArmorAnchor(EntityUid uid, SpriteComponent sprite, out int armorLayer)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), "enum.SquadArmorLayers.Armor", ref armorLayer, false))
		{
			return true;
		}
		if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), "pubg-outer-armor-anchor", ref armorLayer, false))
		{
			return true;
		}
		int num = default(int);
		if (!_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), "outerClothing", ref num, false))
		{
			armorLayer = 0;
			return false;
		}
		armorLayer = num + 1;
		_sprite.AddBlankLayer(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (int?)armorLayer);
		_sprite.LayerMapSet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), "pubg-outer-armor-anchor", armorLayer);
		return true;
	}

	private static bool HasSlot(InventoryComponent inventory, string slotName)
	{
		SlotDefinition[] slots = inventory.Slots;
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].Name == slotName)
			{
				return true;
			}
		}
		return false;
	}

	private void HideHeadSlotLayers(EntityUid uid, SpriteComponent sprite, InventorySlotsComponent inventorySlots)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!inventorySlots.VisualLayerKeys.TryGetValue("head", out HashSet<string> value) || value.Count == 0)
		{
			return;
		}
		foreach (string item in value)
		{
			try
			{
				_sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((uid, sprite)), item, true);
			}
			catch
			{
			}
		}
		value.Clear();
	}
}
