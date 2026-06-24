using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Client.DisplacementMap;
using Content.Client.Inventory;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.DisplacementMap;
using Content.Shared.Humanoid;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Client.Clothing;

public sealed class ClientClothingSystem : ClothingSystem
{
	public const string Jumpsuit = "jumpsuit";

	private static readonly Dictionary<string, string> TemporarySlotMap = new Dictionary<string, string>
	{
		{ "head", "HELMET" },
		{ "headArmor", "HELMET" },
		{ "eyes", "EYES" },
		{ "ears", "EARS" },
		{ "mask", "MASK" },
		{ "outerClothing", "OUTERCLOTHING" },
		{ "outerArmor", "OUTERCLOTHING" },
		{ "jumpsuit", "INNERCLOTHING" },
		{ "neck", "NECK" },
		{ "back", "BACKPACK" },
		{ "belt", "BELT" },
		{ "gloves", "HAND" },
		{ "shoes", "FEET" },
		{ "id", "IDCARD" },
		{ "pocket1", "POCKET1" },
		{ "pocket2", "POCKET2" },
		{ "suitstorage", "SUITSTORAGE" }
	};

	[Dependency]
	private IResourceCache _cache;

	[Dependency]
	private InventorySystem _inventorySystem;

	[Dependency]
	private DisplacementMapSystem _displacement;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ClothingComponent, GetEquipmentVisualsEvent>((ComponentEventHandler<ClothingComponent, GetEquipmentVisualsEvent>)OnGetVisuals, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingComponent, InventoryTemplateUpdated>((EntityEventRefHandler<ClothingComponent, InventoryTemplateUpdated>)OnInventoryTemplateUpdated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, VisualsChangedEvent>((ComponentEventHandler<InventoryComponent, VisualsChangedEvent>)OnVisualsChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpriteComponent, DidUnequipEvent>((EntityEventRefHandler<SpriteComponent, DidUnequipEvent>)OnDidUnequip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, AppearanceChangeEvent>((ComponentEventRefHandler<InventoryComponent, AppearanceChangeEvent>)OnAppearanceUpdate, (Type[])null, (Type[])null);
	}

	private void OnAppearanceUpdate(EntityUid uid, InventoryComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			UpdateAllSlots(uid, component);
			int num = default(int);
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)HumanoidVisualLayers.StencilMask, ref num, false))
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, false);
			}
		}
	}

	private void OnInventoryTemplateUpdated(Entity<ClothingComponent> ent, ref InventoryTemplateUpdated args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UpdateAllSlots(ent.Owner, null, ent.Comp);
	}

	private void UpdateAllSlots(EntityUid uid, InventoryComponent? inventoryComponent = null, ClothingComponent? clothing = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		InventorySystem.InventorySlotEnumerator slotEnumerator = _inventorySystem.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit((uid, inventoryComponent)));
		EntityUid item;
		SlotDefinition slot;
		while (slotEnumerator.NextItem(out item, out slot))
		{
			RenderEquipment(uid, item, slot.Name, inventoryComponent, null, clothing);
		}
	}

	private void OnGetVisuals(EntityUid uid, ClothingComponent item, GetEquipmentVisualsEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		InventoryComponent inventoryComponent = default(InventoryComponent);
		if (!((EntitySystem)this).TryComp<InventoryComponent>(args.Equipee, ref inventoryComponent))
		{
			return;
		}
		List<PrototypeLayerData> value = null;
		if (inventoryComponent.SpeciesId != null)
		{
			item.ClothingVisuals.TryGetValue(args.Slot + "-" + inventoryComponent.SpeciesId, out value);
		}
		if (value == null && !item.ClothingVisuals.TryGetValue(args.Slot, out value) && !TryGetDefaultVisuals(uid, item, args.Slot, inventoryComponent.SpeciesId, out value))
		{
			return;
		}
		int num = 0;
		foreach (PrototypeLayerData item2 in value)
		{
			string text = item2.MapKeys?.FirstOrDefault();
			if (text == null)
			{
				text = $"{args.Slot}-{num}";
				num++;
			}
			item.MappedLayer = text;
			args.Layers.Add((text, item2));
		}
	}

	private bool TryGetDefaultVisuals(EntityUid uid, ClothingComponent clothing, string slot, string? speciesId, [NotNullWhen(true)] out List<PrototypeLayerData>? layers)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		layers = null;
		RSI val = null;
		SpriteComponent val2 = default(SpriteComponent);
		if (clothing.RsiPath != null)
		{
			val = _cache.GetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / clothing.RsiPath, true).RSI;
		}
		else if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val2))
		{
			val = val2.BaseRSI;
		}
		if (val == null)
		{
			return false;
		}
		string value = slot;
		TemporarySlotMap.TryGetValue(value, out value);
		string text = "equipped-" + value;
		if (!string.IsNullOrEmpty(clothing.EquippedPrefix))
		{
			text = clothing.EquippedPrefix + "-equipped-" + value;
		}
		if (clothing.EquippedState != null)
		{
			text = clothing.EquippedState ?? "";
		}
		State val3 = default(State);
		if (speciesId != null && val.TryGetState(StateId.op_Implicit(text + "-" + speciesId), ref val3))
		{
			text = text + "-" + speciesId;
		}
		else if (!val.TryGetState(StateId.op_Implicit(text), ref val3))
		{
			return false;
		}
		PrototypeLayerData val4 = new PrototypeLayerData();
		val4.RsiPath = ((object)val.Path/*cast due to constrained. prefix*/).ToString();
		val4.State = text;
		layers = new List<PrototypeLayerData> { val4 };
		return true;
	}

	private void OnVisualsChanged(EntityUid uid, InventoryComponent component, VisualsChangedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity = ((EntitySystem)this).GetEntity(args.Item);
		ClothingComponent clothingComponent = default(ClothingComponent);
		if (((EntitySystem)this).TryComp<ClothingComponent>(entity, ref clothingComponent) && clothingComponent.InSlot != null)
		{
			RenderEquipment(uid, entity, clothingComponent.InSlot, component, null, clothingComponent);
		}
	}

	private void OnDidUnequip(Entity<SpriteComponent> entity, ref DidUnequipEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		InventorySlotsComponent inventorySlotsComponent = default(InventorySlotsComponent);
		if (!((EntitySystem)this).TryComp<InventorySlotsComponent>(Entity<SpriteComponent>.op_Implicit(entity), ref inventorySlotsComponent) || !inventorySlotsComponent.VisualLayerKeys.TryGetValue(args.Slot, out HashSet<string> value))
		{
			return;
		}
		foreach (string item in value)
		{
			try
			{
				_sprite.RemoveLayer(entity.AsNullable(), item, true);
			}
			catch (Exception value2)
			{
				((EntitySystem)this).Log.Error($"Error removing layer:\n{value2}");
			}
		}
		value.Clear();
	}

	public void InitClothing(EntityUid uid, InventoryComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref sprite))
		{
			InventorySystem.InventorySlotEnumerator slotEnumerator = _inventorySystem.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit((uid, component)));
			EntityUid item;
			SlotDefinition slot;
			while (slotEnumerator.NextItem(out item, out slot))
			{
				RenderEquipment(uid, item, slot.Name, component, sprite);
			}
		}
	}

	protected override void OnGotEquipped(EntityUid uid, ClothingComponent component, GotEquippedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		base.OnGotEquipped(uid, component, args);
		RenderEquipment(args.Equipee, uid, args.Slot, null, null, component);
	}

	private void RenderEquipment(EntityUid equipee, EntityUid equipment, string slot, InventoryComponent? inventory = null, SpriteComponent? sprite = null, ClothingComponent? clothingComponent = null, InventorySlotsComponent? inventorySlots = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<InventoryComponent, SpriteComponent, InventorySlotsComponent>(equipee, ref inventory, ref sprite, ref inventorySlots, true) || !((EntitySystem)this).Resolve<ClothingComponent>(equipment, ref clothingComponent, false) || !_inventorySystem.TryGetSlot(equipee, slot, out SlotDefinition slotDefinition, inventory))
		{
			return;
		}
		if (inventorySlots.VisualLayerKeys.TryGetValue(slot, out HashSet<string> value))
		{
			foreach (string item in value)
			{
				_sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((equipee, sprite)), item, true);
			}
			value.Clear();
		}
		else
		{
			value = new HashSet<string>();
			inventorySlots.VisualLayerKeys[slot] = value;
		}
		GetEquipmentVisualsEvent getEquipmentVisualsEvent = new GetEquipmentVisualsEvent(equipee, slot);
		((EntitySystem)this).RaiseLocalEvent<GetEquipmentVisualsEvent>(equipment, getEquipmentVisualsEvent, false);
		if (getEquipmentVisualsEvent.Layers.Count == 0)
		{
			((EntitySystem)this).RaiseLocalEvent<EquipmentVisualsUpdatedEvent>(equipment, new EquipmentVisualsUpdatedEvent(equipee, slot, value), true);
			return;
		}
		int num = default(int);
		bool flag = _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((equipee, sprite)), slot, ref num, false);
		DisplacementData valueOrDefault = inventory.Displacements.GetValueOrDefault(slot);
		Sex? sex = ((EntitySystem)this).CompOrNull<HumanoidAppearanceComponent>(equipee)?.Sex;
		if (sex.HasValue)
		{
			switch (sex)
			{
			case Sex.Male:
				if (inventory.MaleDisplacements.Count > 0)
				{
					valueOrDefault = inventory.MaleDisplacements.GetValueOrDefault(slot);
				}
				break;
			case Sex.Female:
				if (inventory.FemaleDisplacements.Count > 0)
				{
					valueOrDefault = inventory.FemaleDisplacements.GetValueOrDefault(slot);
				}
				break;
			}
		}
		SpriteComponent val3 = default(SpriteComponent);
		foreach (var (text, val) in getEquipmentVisualsEvent.Layers)
		{
			if (!value.Add(text))
			{
				((EntitySystem)this).Log.Warning($"Duplicate key for clothing visuals: {text}. Are multiple components attempting to modify the same layer? Equipment: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(equipment))}");
				continue;
			}
			if (flag)
			{
				num++;
				_sprite.AddBlankLayer(Entity<SpriteComponent>.op_Implicit((equipee, sprite)), (int?)num);
				_sprite.LayerMapRemove(Entity<SpriteComponent>.op_Implicit((equipee, sprite)), text);
				_sprite.LayerMapSet(Entity<SpriteComponent>.op_Implicit((equipee, sprite)), text, num);
				if (val.Color.HasValue)
				{
					_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((equipee, sprite)), text, val.Color.Value);
				}
				if (val.Scale.HasValue)
				{
					_sprite.LayerSetScale(Entity<SpriteComponent>.op_Implicit((equipee, sprite)), text, val.Scale.Value);
				}
			}
			else
			{
				num = _sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((equipee, sprite)), text);
			}
			ISpriteLayer obj = sprite[num];
			Layer val2 = (Layer)(object)((obj is Layer) ? obj : null);
			if (val2 != null)
			{
				if (val.RsiPath == null && val.TexturePath == null && val2.RSI == null && ((EntitySystem)this).TryComp<SpriteComponent>(equipment, ref val3))
				{
					_sprite.LayerSetRsi(val2, val3.BaseRSI, (StateId?)null);
				}
				_sprite.LayerSetData(Entity<SpriteComponent>.op_Implicit((equipee, sprite)), num, val);
				_sprite.LayerSetOffset(val2, val2.Offset + slotDefinition.Offset);
				if (valueOrDefault != null && (val.State == null || inventory.SpeciesId == null || !val.State.EndsWith(inventory.SpeciesId)) && _displacement.TryAddDisplacement(valueOrDefault, Entity<SpriteComponent>.op_Implicit((equipee, sprite)), num, text, out string displacementKey))
				{
					value.Add(displacementKey);
					num++;
				}
			}
		}
		((EntitySystem)this).RaiseLocalEvent<EquipmentVisualsUpdatedEvent>(equipment, new EquipmentVisualsUpdatedEvent(equipee, slot, value), true);
	}
}
