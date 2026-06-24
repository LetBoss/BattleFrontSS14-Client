using System;
using Content.Shared.Clothing.Components;
using Content.Shared.Foldable;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Clothing.EntitySystems;

public sealed class FoldableClothingSystem : EntitySystem
{
	[Dependency]
	private ClothingSystem _clothingSystem;

	[Dependency]
	private InventorySystem _inventorySystem;

	[Dependency]
	private SharedItemSystem _itemSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FoldableClothingComponent, FoldAttemptEvent>((EntityEventRefHandler<FoldableClothingComponent, FoldAttemptEvent>)OnFoldAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FoldableClothingComponent, FoldedEvent>((EntityEventRefHandler<FoldableClothingComponent, FoldedEvent>)OnFolded, (Type[])null, new Type[1] { typeof(MaskSystem) });
	}

	private void OnFoldAttempt(Entity<FoldableClothingComponent> ent, ref FoldAttemptEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && _inventorySystem.TryGetContainingSlot(Entity<TransformComponent, MetaDataComponent>.op_Implicit(ent.Owner), out SlotDefinition slot))
		{
			SlotFlags? newSlots = (args.Comp.IsFolded ? ent.Comp.UnfoldedSlots : ent.Comp.FoldedSlots);
			if (newSlots.HasValue && (newSlots.Value & slot.SlotFlags) != slot.SlotFlags)
			{
				args.Cancelled = true;
			}
			else if (ent.Comp.FoldedHideLayers.Count != 0 || ent.Comp.UnfoldedHideLayers.Count != 0)
			{
				args.Cancelled = true;
			}
		}
	}

	private void OnFolded(Entity<FoldableClothingComponent> ent, ref FoldedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		ClothingComponent clothingComp = default(ClothingComponent);
		ItemComponent itemComp = default(ItemComponent);
		if (!((EntitySystem)this).TryComp<ClothingComponent>(ent.Owner, ref clothingComp) || !((EntitySystem)this).TryComp<ItemComponent>(ent.Owner, ref itemComp))
		{
			return;
		}
		if (args.IsFolded)
		{
			if (ent.Comp.FoldedSlots.HasValue)
			{
				_clothingSystem.SetSlots(ent.Owner, ent.Comp.FoldedSlots.Value, clothingComp);
			}
			if (ent.Comp.FoldedEquippedPrefix != null)
			{
				_clothingSystem.SetEquippedPrefix(ent.Owner, ent.Comp.FoldedEquippedPrefix, clothingComp);
			}
			if (ent.Comp.FoldedHeldPrefix != null)
			{
				_itemSystem.SetHeldPrefix(ent.Owner, ent.Comp.FoldedHeldPrefix, force: false, itemComp);
			}
			HideLayerClothingComponent hideLayerComp = default(HideLayerClothingComponent);
			if (ent.Comp.FoldedHideLayers.Count != 0 && ((EntitySystem)this).TryComp<HideLayerClothingComponent>(ent.Owner, ref hideLayerComp))
			{
				hideLayerComp.Slots = ent.Comp.FoldedHideLayers;
			}
		}
		else
		{
			if (ent.Comp.UnfoldedSlots.HasValue)
			{
				_clothingSystem.SetSlots(ent.Owner, ent.Comp.UnfoldedSlots.Value, clothingComp);
			}
			if (ent.Comp.FoldedEquippedPrefix != null)
			{
				_clothingSystem.SetEquippedPrefix(ent.Owner, null, clothingComp);
			}
			if (ent.Comp.FoldedHeldPrefix != null)
			{
				_itemSystem.SetHeldPrefix(ent.Owner, null, force: false, itemComp);
			}
			HideLayerClothingComponent hideLayerComp2 = default(HideLayerClothingComponent);
			if (ent.Comp.UnfoldedHideLayers.Count != 0 && ((EntitySystem)this).TryComp<HideLayerClothingComponent>(ent.Owner, ref hideLayerComp2))
			{
				hideLayerComp2.Slots = ent.Comp.UnfoldedHideLayers;
			}
		}
	}
}
