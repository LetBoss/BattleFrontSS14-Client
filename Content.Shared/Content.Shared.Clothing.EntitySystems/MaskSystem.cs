using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Clothing.Components;
using Content.Shared.Foldable;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Clothing.EntitySystems;

public sealed class MaskSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actionSystem;

	[Dependency]
	private InventorySystem _inventorySystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private ClothingSystem _clothing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MaskComponent, ToggleMaskEvent>((EntityEventRefHandler<MaskComponent, ToggleMaskEvent>)OnToggleMask, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MaskComponent, GetItemActionsEvent>((ComponentEventHandler<MaskComponent, GetItemActionsEvent>)OnGetActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MaskComponent, GotUnequippedEvent>((ComponentEventHandler<MaskComponent, GotUnequippedEvent>)OnGotUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MaskComponent, FoldedEvent>((EntityEventRefHandler<MaskComponent, FoldedEvent>)OnFolded, (Type[])null, (Type[])null);
	}

	private void OnGetActions(EntityUid uid, MaskComponent component, GetItemActionsEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (_inventorySystem.InSlotWithFlags(Entity<TransformComponent, MetaDataComponent>.op_Implicit(uid), SlotFlags.MASK))
		{
			args.AddAction(ref component.ToggleActionEntity, EntProtoId.op_Implicit(component.ToggleAction));
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	private void OnToggleMask(Entity<MaskComponent> ent, ref ToggleMaskEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		Entity<MaskComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		MaskComponent maskComponent = default(MaskComponent);
		val.Deconstruct(ref val2, ref maskComponent);
		EntityUid uid = val2;
		MaskComponent mask = maskComponent;
		ClothingComponent clothing = default(ClothingComponent);
		if (!mask.ToggleActionEntity.HasValue || !mask.IsToggleable || !((EntitySystem)this).TryComp<ClothingComponent>(Entity<MaskComponent>.op_Implicit(ent), ref clothing))
		{
			return;
		}
		SlotFlags? inSlotFlag = clothing.InSlotFlag;
		if (inSlotFlag.HasValue)
		{
			SlotFlags slotFlag = inSlotFlag.GetValueOrDefault();
			if (clothing.Slots.HasFlag(slotFlag))
			{
				SetToggled(Entity<MaskComponent>.op_Implicit((uid, mask)), !mask.IsToggled);
				string dir = (mask.IsToggled ? "down" : "up");
				string msg = "action-mask-pull-" + dir + "-popup-message";
				_popupSystem.PopupClient(base.Loc.GetString(msg, (ValueTuple<string, object>)("mask", uid)), args.Performer, args.Performer);
			}
		}
	}

	private void OnGotUnequipped(EntityUid uid, MaskComponent mask, GotUnequippedEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (mask.IsToggled && mask.IsToggleable)
		{
			mask.IsToggled = false;
			ToggleMaskComponents(uid, mask, args.Equipee, mask.EquippedPrefix, isEquip: true);
		}
	}

	private void ToggleMaskComponents(EntityUid uid, MaskComponent mask, EntityUid wearer, string? equippedPrefix = null, bool isEquip = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Dirty(uid, (IComponent)(object)mask, (MetaDataComponent)null);
		EntityUid? toggleActionEntity = mask.ToggleActionEntity;
		if (toggleActionEntity.HasValue)
		{
			EntityUid action = toggleActionEntity.GetValueOrDefault();
			_actionSystem.SetToggled(Entity<ActionComponent>.op_Implicit(action), mask.IsToggled);
		}
		ItemMaskToggledEvent maskEv = new ItemMaskToggledEvent(Entity<MaskComponent>.op_Implicit((uid, mask)), wearer);
		((EntitySystem)this).RaiseLocalEvent<ItemMaskToggledEvent>(uid, ref maskEv, false);
		WearerMaskToggledEvent wearerEv = new WearerMaskToggledEvent(Entity<MaskComponent>.op_Implicit((uid, mask)));
		((EntitySystem)this).RaiseLocalEvent<WearerMaskToggledEvent>(wearer, ref wearerEv, false);
	}

	private void OnFolded(Entity<MaskComponent> ent, ref FoldedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.DisableOnFolded)
		{
			SetToggled(ent, args.IsFolded, force: true);
			SetToggleable(ent, !args.IsFolded);
		}
	}

	public void SetToggled(Entity<MaskComponent?> mask, bool toggled, bool force = false)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.ApplyingState || !((EntitySystem)this).Resolve<MaskComponent>(mask.Owner, ref mask.Comp, true) || (!force && !mask.Comp.IsToggleable) || mask.Comp.IsToggled == toggled)
		{
			return;
		}
		mask.Comp.IsToggled = toggled;
		EntityUid? toggleActionEntity = mask.Comp.ToggleActionEntity;
		if (toggleActionEntity.HasValue)
		{
			EntityUid action = toggleActionEntity.GetValueOrDefault();
			_actionSystem.SetToggled(Entity<ActionComponent>.op_Implicit(action), mask.Comp.IsToggled);
		}
		string prefix = (mask.Comp.IsToggled ? mask.Comp.EquippedPrefix : null);
		_clothing.SetEquippedPrefix(Entity<MaskComponent>.op_Implicit(mask), prefix);
		EntityUid? wearer = null;
		ClothingComponent clothing = default(ClothingComponent);
		if (((EntitySystem)this).TryComp<ClothingComponent>(Entity<MaskComponent>.op_Implicit(mask), ref clothing))
		{
			SlotFlags? inSlotFlag = clothing.InSlotFlag;
			if (inSlotFlag.HasValue)
			{
				SlotFlags slotFlag = inSlotFlag.GetValueOrDefault();
				if (clothing.Slots.HasFlag(slotFlag))
				{
					wearer = ((EntitySystem)this).Transform(Entity<MaskComponent>.op_Implicit(mask)).ParentUid;
				}
			}
		}
		ItemMaskToggledEvent maskEv = new ItemMaskToggledEvent(mask, wearer);
		((EntitySystem)this).RaiseLocalEvent<ItemMaskToggledEvent>(Entity<MaskComponent>.op_Implicit(mask), ref maskEv, false);
		if (wearer.HasValue)
		{
			WearerMaskToggledEvent wearerEv = new WearerMaskToggledEvent(mask);
			((EntitySystem)this).RaiseLocalEvent<WearerMaskToggledEvent>(wearer.Value, ref wearerEv, false);
		}
		((EntitySystem)this).Dirty<MaskComponent>(mask, (MetaDataComponent)null);
	}

	public void SetToggleable(Entity<MaskComponent?> mask, bool toggleable)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && ((EntitySystem)this).Resolve<MaskComponent>(mask.Owner, ref mask.Comp, true) && mask.Comp.IsToggleable != toggleable)
		{
			EntityUid? toggleActionEntity = mask.Comp.ToggleActionEntity;
			if (toggleActionEntity.HasValue)
			{
				EntityUid action = toggleActionEntity.GetValueOrDefault();
				_actionSystem.SetEnabled(Entity<ActionComponent>.op_Implicit(action), mask.Comp.IsToggleable);
			}
			mask.Comp.IsToggleable = toggleable;
			((EntitySystem)this).Dirty<MaskComponent>(mask, (MetaDataComponent)null);
		}
	}
}
