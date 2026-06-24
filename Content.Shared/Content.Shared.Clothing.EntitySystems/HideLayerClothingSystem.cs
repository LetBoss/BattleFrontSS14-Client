using System;
using System.Collections.Generic;
using Content.Shared.Clothing.Components;
using Content.Shared.Humanoid;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Clothing.EntitySystems;

public sealed class HideLayerClothingSystem : EntitySystem
{
	[Dependency]
	private SharedHumanoidAppearanceSystem _humanoid;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<HideLayerClothingComponent, ClothingGotUnequippedEvent>((EntityEventRefHandler<HideLayerClothingComponent, ClothingGotUnequippedEvent>)OnHideGotUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HideLayerClothingComponent, ClothingGotEquippedEvent>((EntityEventRefHandler<HideLayerClothingComponent, ClothingGotEquippedEvent>)OnHideGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HideLayerClothingComponent, ItemMaskToggledEvent>((EntityEventRefHandler<HideLayerClothingComponent, ItemMaskToggledEvent>)OnHideToggled, (Type[])null, (Type[])null);
	}

	private void OnHideToggled(Entity<HideLayerClothingComponent> ent, ref ItemMaskToggledEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (args.Wearer.HasValue)
		{
			SetLayerVisibility(Entity<HideLayerClothingComponent, ClothingComponent>.op_Implicit(ent), Entity<HumanoidAppearanceComponent>.op_Implicit(args.Wearer.Value), hideLayers: true);
		}
	}

	private void OnHideGotEquipped(Entity<HideLayerClothingComponent> ent, ref ClothingGotEquippedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SetLayerVisibility(Entity<HideLayerClothingComponent, ClothingComponent>.op_Implicit(ent), Entity<HumanoidAppearanceComponent>.op_Implicit(args.Wearer), hideLayers: true);
	}

	private void OnHideGotUnequipped(Entity<HideLayerClothingComponent> ent, ref ClothingGotUnequippedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SetLayerVisibility(Entity<HideLayerClothingComponent, ClothingComponent>.op_Implicit(ent), Entity<HumanoidAppearanceComponent>.op_Implicit(args.Wearer), hideLayers: false);
	}

	private void SetLayerVisibility(Entity<HideLayerClothingComponent?, ClothingComponent?> clothing, Entity<HumanoidAppearanceComponent?> user, bool hideLayers)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.ApplyingState || !((EntitySystem)this).Resolve<HideLayerClothingComponent, ClothingComponent>(clothing.Owner, ref clothing.Comp1, ref clothing.Comp2, true) || !((EntitySystem)this).Resolve<HumanoidAppearanceComponent>(user.Owner, ref user.Comp, false))
		{
			return;
		}
		hideLayers &= IsEnabled(clothing);
		HashSet<HumanoidVisualLayers> hideable = user.Comp.HideLayersOnEquip;
		SlotFlags inSlot = clothing.Comp2.InSlotFlag.GetValueOrDefault();
		bool dirty = false;
		foreach (var (layer, validSlots) in clothing.Comp1.Layers)
		{
			if (hideable.Contains(layer) && validSlots.HasFlag(inSlot))
			{
				_humanoid.SetLayerVisibility(user, layer, !hideLayers, inSlot, ref dirty);
			}
		}
		HashSet<HumanoidVisualLayers> slots = clothing.Comp1.Slots;
		if (slots != null && clothing.Comp2.Slots.HasFlag(inSlot))
		{
			foreach (HumanoidVisualLayers layer2 in slots)
			{
				if (hideable.Contains(layer2))
				{
					_humanoid.SetLayerVisibility(user, layer2, !hideLayers, inSlot, ref dirty);
				}
			}
		}
		if (dirty)
		{
			((EntitySystem)this).Dirty<HumanoidAppearanceComponent>(user, (MetaDataComponent)null);
		}
	}

	private bool IsEnabled(Entity<HideLayerClothingComponent, ClothingComponent> clothing)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!clothing.Comp1.HideOnToggle)
		{
			return true;
		}
		MaskComponent mask = default(MaskComponent);
		if (!((EntitySystem)this).TryComp<MaskComponent>(Entity<HideLayerClothingComponent, ClothingComponent>.op_Implicit(clothing), ref mask))
		{
			return true;
		}
		return !mask.IsToggled;
	}
}
