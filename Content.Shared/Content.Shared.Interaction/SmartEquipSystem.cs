using System;
using System.Collections.Generic;
using Content.Shared.ActionBlocker;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Input;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Shared.Interaction;

public sealed class SmartEquipSystem : EntitySystem
{
	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedStorageSystem _storage;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private ItemSlotsSystem _slots;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	public override void Initialize()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		CommandBinds.Builder.Bind(ContentKeyFunctions.SmartEquipBackpack, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(HandleSmartEquipBackpack), (StateInputCmdDelegate)null, false, false)).Bind(ContentKeyFunctions.SmartEquipBelt, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(HandleSmartEquipBelt), (StateInputCmdDelegate)null, false, false)).Register<SmartEquipSystem>();
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<SmartEquipSystem>();
	}

	private void HandleSmartEquipBackpack(ICommonSession? session)
	{
		HandleSmartEquip(session, "back");
	}

	private void HandleSmartEquipBelt(ICommonSession? session)
	{
		HandleSmartEquip(session, "belt");
	}

	private void HandleSmartEquip(ICommonSession? session, string equipmentSlot)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		if (session == null)
		{
			return;
		}
		EntityUid? attachedEntity = session.AttachedEntity;
		if (!attachedEntity.HasValue)
		{
			return;
		}
		EntityUid uid = attachedEntity.GetValueOrDefault();
		HandsComponent hands = default(HandsComponent);
		if (!((EntityUid)(ref uid)).Valid || !((EntitySystem)this).Exists(uid) || !((EntitySystem)this).TryComp<HandsComponent>(uid, ref hands) || hands.ActiveHandId == null)
		{
			return;
		}
		EntityUid? handItem = _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit((uid, hands)));
		if (!_actionBlocker.CanInteract(uid, handItem))
		{
			return;
		}
		InventoryComponent inventory = default(InventoryComponent);
		if (!((EntitySystem)this).TryComp<InventoryComponent>(uid, ref inventory) || !_inventory.HasSlot(uid, equipmentSlot, inventory))
		{
			_popup.PopupClient(base.Loc.GetString("smart-equip-missing-equipment-slot", (ValueTuple<string, object>)("slotName", equipmentSlot)), uid, uid);
			return;
		}
		if (handItem.HasValue && !_hands.CanDropHeld(uid, hands.ActiveHandId))
		{
			_popup.PopupClient(base.Loc.GetString("smart-equip-cant-drop"), uid, uid);
			return;
		}
		_inventory.TryGetSlotEntity(uid, equipmentSlot, out var slotEntity);
		string emptyEquipmentSlotString = base.Loc.GetString("smart-equip-empty-equipment-slot", (ValueTuple<string, object>)("slotName", equipmentSlot));
		string reason3;
		if (slotEntity.HasValue)
		{
			EntityUid slotItem = slotEntity.GetValueOrDefault();
			StorageComponent storage = default(StorageComponent);
			ItemSlotsComponent slots = default(ItemSlotsComponent);
			if (((EntitySystem)this).TryComp<StorageComponent>(slotItem, ref storage))
			{
				attachedEntity = handItem;
				string reason;
				if (!attachedEntity.HasValue)
				{
					if (((BaseContainer)storage.Container).ContainedEntities.Count == 0)
					{
						_popup.PopupClient(emptyEquipmentSlotString, uid, uid);
						return;
					}
					IReadOnlyList<EntityUid> containedEntities = ((BaseContainer)storage.Container).ContainedEntities;
					EntityUid removing = containedEntities[containedEntities.Count - 1];
					_container.RemoveEntity(slotItem, removing, (ContainerManagerComponent)null, (TransformComponent)null, (MetaDataComponent)null, true, false, (EntityCoordinates?)null, (Angle?)null);
					_hands.TryPickup(uid, removing, null, checkActionBlocker: true, animateUser: false, animate: true, hands);
				}
				else if (!_storage.CanInsert(slotItem, handItem.Value, session.AttachedEntity, out reason))
				{
					if (reason != null)
					{
						_popup.PopupClient(base.Loc.GetString(reason), uid, uid);
					}
				}
				else
				{
					_hands.TryDrop(Entity<HandsComponent>.op_Implicit((uid, hands)), hands.ActiveHandId);
					_storage.Insert(slotItem, handItem.Value, out EntityUid? stacked, out string reason2, uid);
					StackComponent handStack = default(StackComponent);
					if (stacked.HasValue && !_storage.CanInsert(slotItem, handItem.Value, session.AttachedEntity, out reason2) && ((EntitySystem)this).TryComp<StackComponent>(handItem.Value, ref handStack) && handStack.Count > 0)
					{
						_hands.TryPickup(uid, handItem.Value, null, checkActionBlocker: true, animateUser: false, animate: true, hands);
					}
				}
			}
			else if (((EntitySystem)this).TryComp<ItemSlotsComponent>(slotItem, ref slots))
			{
				if (!handItem.HasValue)
				{
					ItemSlot toEjectFrom = null;
					foreach (ItemSlot slot in slots.Slots.Values)
					{
						if (slot.HasItem && slot.Priority > (toEjectFrom?.Priority ?? int.MinValue))
						{
							toEjectFrom = slot;
						}
					}
					if (toEjectFrom == null)
					{
						_popup.PopupClient(emptyEquipmentSlotString, uid, uid);
					}
					else
					{
						_slots.TryEjectToHands(slotItem, toEjectFrom, uid, excludeUserAudio: true);
					}
					return;
				}
				ItemSlot toInsertTo = null;
				foreach (ItemSlot slot2 in slots.Slots.Values)
				{
					if (!slot2.HasItem && _whitelistSystem.IsWhitelistPassOrNull(slot2.Whitelist, handItem.Value) && slot2.Priority > (toInsertTo?.Priority ?? int.MinValue))
					{
						toInsertTo = slot2;
					}
				}
				if (toInsertTo == null)
				{
					_popup.PopupClient(base.Loc.GetString("smart-equip-no-valid-item-slot-insert", (ValueTuple<string, object>)("item", handItem.Value)), uid, uid);
				}
				else
				{
					_slots.TryInsertFromHand(slotItem, toInsertTo, uid, hands, excludeUserAudio: true);
				}
			}
			else if (!handItem.HasValue)
			{
				if (!_inventory.CanUnequip(uid, equipmentSlot, out string inventoryReason))
				{
					_popup.PopupClient(base.Loc.GetString(inventoryReason), uid, uid);
					return;
				}
				_inventory.TryUnequip(uid, equipmentSlot, silent: false, force: false, predicted: true, inventory, null, reparent: true, checkDoafter: true);
				_hands.TryPickup(uid, slotItem, null, checkActionBlocker: true, animateUser: false, animate: true, hands);
			}
		}
		else if (!handItem.HasValue)
		{
			_popup.PopupClient(emptyEquipmentSlotString, uid, uid);
		}
		else if (!_inventory.CanEquip(uid, handItem.Value, equipmentSlot, out reason3))
		{
			_popup.PopupClient(base.Loc.GetString(reason3), uid, uid);
		}
		else
		{
			_hands.TryDrop(Entity<HandsComponent>.op_Implicit((uid, hands)), hands.ActiveHandId);
			_inventory.TryEquip(uid, handItem.Value, equipmentSlot, silent: false, force: false, predicted: true, null, null, checkDoafter: true);
		}
	}
}
