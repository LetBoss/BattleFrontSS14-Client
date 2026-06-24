using System;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tools;
using Content.Shared.Tools.Systems;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Vehicle;

public sealed class HardpointSlotSystem : EntitySystem
{
	[Dependency]
	private readonly SharedDoAfterSystem _doAfter;

	[Dependency]
	private readonly HardpointSystem _hardpoints;

	[Dependency]
	private readonly ItemSlotsSystem _itemSlots;

	[Dependency]
	private readonly SharedPopupSystem _popup;

	[Dependency]
	private readonly SharedToolSystem _tool;

	[Dependency]
	private readonly SharedUserInterfaceSystem _ui;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<HardpointSlotsComponent, ItemSlotInsertAttemptEvent>((EntityEventRefHandler<HardpointSlotsComponent, ItemSlotInsertAttemptEvent>)OnInsertAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointSlotsComponent, HardpointInsertDoAfterEvent>((EntityEventRefHandler<HardpointSlotsComponent, HardpointInsertDoAfterEvent>)OnInsertDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointSlotsComponent, GetVerbsEvent<InteractionVerb>>((EntityEventRefHandler<HardpointSlotsComponent, GetVerbsEvent<InteractionVerb>>)OnGetRemoveVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointSlotsComponent, InteractUsingEvent>((EntityEventRefHandler<HardpointSlotsComponent, InteractUsingEvent>)OnSlotsInteractUsing, new Type[1] { typeof(ItemSlotsSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointSlotsComponent, BoundUIOpenedEvent>((EntityEventRefHandler<HardpointSlotsComponent, BoundUIOpenedEvent>)OnHardpointUiOpened, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointSlotsComponent, BoundUIClosedEvent>((EntityEventRefHandler<HardpointSlotsComponent, BoundUIClosedEvent>)OnHardpointUiClosed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointSlotsComponent, HardpointRemoveMessage>((EntityEventRefHandler<HardpointSlotsComponent, HardpointRemoveMessage>)OnHardpointRemoveMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointSlotsComponent, HardpointRemoveDoAfterEvent>((EntityEventRefHandler<HardpointSlotsComponent, HardpointRemoveDoAfterEvent>)OnHardpointRemoveDoAfter, (Type[])null, (Type[])null);
	}

	public void DisableEjectForAllSlots(Entity<HardpointSlotsComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
		if (!((EntitySystem)this).TryComp<ItemSlotsComponent>(ent.Owner, ref itemSlots))
		{
			return;
		}
		foreach (HardpointSlot slot in ent.Comp.Slots)
		{
			if (_itemSlots.TryGetSlot(ent.Owner, slot.Id, out ItemSlot itemSlot, itemSlots))
			{
				_itemSlots.SetEjectFlags(ent.Owner, itemSlot, disableEject: true, itemSlot.EjectOnInteract, itemSlot.EjectOnUse, itemSlots);
			}
		}
	}

	private void OnInsertAttempt(Entity<HardpointSlotsComponent> ent, ref ItemSlotInsertAttemptEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		if (!args.User.HasValue || !_hardpoints.TryGetSlot(ent.Comp, args.Slot.ID, out HardpointSlot slot) || ent.Comp.CompletingInserts.Contains(slot.Id))
		{
			return;
		}
		if (!_hardpoints.IsValidHardpoint(args.Item, ent.Comp, slot))
		{
			args.Cancelled = true;
		}
		else
		{
			if (slot.InsertDelay <= 0f)
			{
				return;
			}
			if (ent.Comp.PendingInsertUsers.Contains(args.User.Value))
			{
				args.Cancelled = true;
				return;
			}
			if (!ent.Comp.PendingInserts.Add(slot.Id))
			{
				args.Cancelled = true;
				return;
			}
			args.Cancelled = true;
			ent.Comp.PendingInsertUsers.Add(args.User.Value);
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User.Value, slot.InsertDelay, new HardpointInsertDoAfterEvent(slot.Id), ent.Owner, ent.Owner, args.Item)
			{
				BreakOnMove = true,
				BreakOnDamage = true,
				BreakOnHandChange = true,
				BreakOnDropItem = true,
				BreakOnWeightlessMove = true,
				NeedHand = true,
				RequireCanInteract = true,
				DuplicateCondition = DuplicateConditions.SameEvent
			};
			if (!_doAfter.TryStartDoAfter(doAfter))
			{
				ent.Comp.PendingInserts.Remove(slot.Id);
				ent.Comp.PendingInsertUsers.Remove(args.User.Value);
			}
		}
	}

	private void OnInsertDoAfter(Entity<HardpointSlotsComponent> ent, ref HardpointInsertDoAfterEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.PendingInserts.Remove(args.SlotId);
		ent.Comp.PendingInsertUsers.Remove(args.User);
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid? used = args.Used;
		if (used.HasValue)
		{
			EntityUid item = used.GetValueOrDefault();
			ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
			if (!string.IsNullOrEmpty(args.SlotId) && ((EntitySystem)this).TryComp<ItemSlotsComponent>(ent.Owner, ref itemSlots) && _hardpoints.TryGetSlot(ent.Comp, args.SlotId, out HardpointSlot hardpointSlot) && _itemSlots.TryGetSlot(ent.Owner, args.SlotId, out ItemSlot slot, itemSlots) && _hardpoints.IsValidHardpoint(item, ent.Comp, hardpointSlot))
			{
				ent.Comp.CompletingInserts.Add(args.SlotId);
				_itemSlots.TryInsertFromHand(ent.Owner, slot, args.User);
				ent.Comp.CompletingInserts.Remove(args.SlotId);
			}
		}
	}

	private void OnGetRemoveVerbs(Entity<HardpointSlotsComponent> ent, ref GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
		if (!args.CanAccess || !args.CanInteract || !((EntitySystem)this).TryComp<ItemSlotsComponent>(ent.Owner, ref itemSlots))
		{
			return;
		}
		foreach (HardpointSlot slot in ent.Comp.Slots)
		{
			if (_itemSlots.TryGetSlot(ent.Owner, slot.Id, out ItemSlot itemSlot, itemSlots) && itemSlot.HasItem && !((EntitySystem)this).HasComp<HardpointNoRemoveComponent>(itemSlot.Item.Value))
			{
				EntityUid user = args.User;
				string slotId = slot.Id;
				InteractionVerb verb = new InteractionVerb
				{
					Act = delegate
					{
						//IL_0016: Unknown result type (might be due to invalid IL or missing references)
						//IL_002c: Unknown result type (might be due to invalid IL or missing references)
						TryStartHardpointRemoval(ent.Owner, ent.Comp, user, slotId);
					},
					Category = VerbCategory.Eject,
					Text = base.Loc.GetString("rmc-hardpoint-remove-verb", (ValueTuple<string, object>)("slot", ((EntitySystem)this).Name(itemSlot.Item.Value, (MetaDataComponent)null))),
					Priority = itemSlot.Priority,
					IconEntity = ((EntitySystem)this).GetNetEntity(itemSlot.Item, (MetaDataComponent)null)
				};
				args.Verbs.Add(verb);
			}
		}
		AddTurretRemoveVerbs(ent, ref args, itemSlots);
	}

	private void OnSlotsInteractUsing(Entity<HardpointSlotsComponent> ent, ref InteractUsingEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			_ = args.User;
			if (TryInsertTurretAttachment(ent, args.User, args.Used))
			{
				((HandledEntityEventArgs)args).Handled = true;
			}
			else if (_tool.HasQuality(args.Used, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.RemoveToolQuality)) && _ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)HardpointUiKey.Key, args.User, false))
			{
				_hardpoints.UpdateHardpointUi(ent.Owner, ent.Comp);
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void OnHardpointUiOpened(Entity<HardpointSlotsComponent> ent, ref BoundUIOpenedEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (object.Equals(((BaseBoundUserInterfaceEvent)args).UiKey, HardpointUiKey.Key))
		{
			ent.Comp.LastUiError = null;
			_hardpoints.UpdateHardpointUi(ent.Owner, ent.Comp);
		}
	}

	private void OnHardpointUiClosed(Entity<HardpointSlotsComponent> ent, ref BoundUIClosedEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (object.Equals(((BaseBoundUserInterfaceEvent)args).UiKey, HardpointUiKey.Key))
		{
			ent.Comp.PendingRemovals.Clear();
			ent.Comp.LastUiError = null;
		}
	}

	private void OnHardpointRemoveMessage(Entity<HardpointSlotsComponent> ent, ref HardpointRemoveMessage args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (object.Equals(((BaseBoundUserInterfaceEvent)args).UiKey, HardpointUiKey.Key) && !(((BaseBoundUserInterfaceEvent)args).Actor == default(EntityUid)) && ((EntitySystem)this).Exists(((BaseBoundUserInterfaceEvent)args).Actor))
		{
			TryStartHardpointRemoval(ent.Owner, ent.Comp, ((BaseBoundUserInterfaceEvent)args).Actor, args.SlotId);
		}
	}

	private void OnHardpointRemoveDoAfter(Entity<HardpointSlotsComponent> ent, ref HardpointRemoveDoAfterEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.PendingRemovals.Remove(args.SlotId);
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			if (args.Cancelled)
			{
				ent.Comp.LastUiError = "Hardpoint removal cancelled.";
				_hardpoints.SetContainingVehicleUiError(ent.Owner, ent.Comp.LastUiError);
			}
			_hardpoints.UpdateHardpointUi(ent.Owner, ent.Comp);
			_hardpoints.UpdateContainingVehicleUi(ent.Owner);
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
		HardpointSlot slot;
		ItemSlot itemSlot;
		if (!((EntitySystem)this).TryComp<ItemSlotsComponent>(ent.Owner, ref itemSlots))
		{
			ent.Comp.LastUiError = "Unable to access hardpoint slots.";
			_hardpoints.SetContainingVehicleUiError(ent.Owner, ent.Comp.LastUiError);
			_hardpoints.UpdateHardpointUi(ent.Owner, ent.Comp);
			_hardpoints.UpdateContainingVehicleUi(ent.Owner);
		}
		else if (!_hardpoints.TryGetSlot(ent.Comp, args.SlotId, out slot))
		{
			ent.Comp.LastUiError = "That hardpoint slot is no longer available.";
			_hardpoints.SetContainingVehicleUiError(ent.Owner, ent.Comp.LastUiError);
			_hardpoints.UpdateHardpointUi(ent.Owner, ent.Comp, itemSlots);
			_hardpoints.UpdateContainingVehicleUi(ent.Owner);
		}
		else if (!_itemSlots.TryGetSlot(ent.Owner, args.SlotId, out itemSlot, itemSlots) || !itemSlot.HasItem)
		{
			ent.Comp.LastUiError = "No hardpoint is installed in that slot.";
			_hardpoints.SetContainingVehicleUiError(ent.Owner, ent.Comp.LastUiError);
			_hardpoints.UpdateHardpointUi(ent.Owner, ent.Comp, itemSlots);
			_hardpoints.UpdateContainingVehicleUi(ent.Owner);
		}
		else if (!_itemSlots.TryEjectToHands(ent.Owner, itemSlot, args.User, excludeUserAudio: true))
		{
			ent.Comp.LastUiError = "Couldn't remove the hardpoint. Free a hand and try again.";
			_hardpoints.SetContainingVehicleUiError(ent.Owner, ent.Comp.LastUiError);
			_hardpoints.UpdateHardpointUi(ent.Owner, ent.Comp, itemSlots);
			_hardpoints.UpdateContainingVehicleUi(ent.Owner);
		}
		else
		{
			ent.Comp.LastUiError = null;
			_hardpoints.SetContainingVehicleUiError(ent.Owner, null);
			_hardpoints.UpdateHardpointUi(ent.Owner, ent.Comp, itemSlots);
			_hardpoints.UpdateContainingVehicleUi(ent.Owner);
			_hardpoints.RefreshCanRun(ent.Owner);
		}
	}

	private void TryStartHardpointRemoval(EntityUid uid, HardpointSlotsComponent component, EntityUid user, string? slotId, EntityUid? uiOwnerUid = null, HardpointSlotsComponent? uiOwnerComp = null)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		bool num = !uiOwnerUid.HasValue || uiOwnerComp == null;
		EntityUid valueOrDefault = uiOwnerUid.GetValueOrDefault();
		if (!uiOwnerUid.HasValue)
		{
			valueOrDefault = uid;
			uiOwnerUid = valueOrDefault;
		}
		if (uiOwnerComp == null)
		{
			uiOwnerComp = component;
		}
		if (num)
		{
			uiOwnerComp.LastUiError = null;
		}
		if (string.IsNullOrWhiteSpace(slotId))
		{
			SetError("Invalid hardpoint slot.");
			RefreshUi();
			return;
		}
		if (VehicleTurretSlotIds.TryParse(slotId, out string parentSlotId, out string childSlotId))
		{
			ItemSlotsComponent parentItemSlots = default(ItemSlotsComponent);
			if (!((EntitySystem)this).TryComp<ItemSlotsComponent>(uid, ref parentItemSlots) || !_hardpoints.TryGetSlot(component, parentSlotId, out HardpointSlot _))
			{
				SetError("Unable to find that turret slot.");
				RefreshUi(parentItemSlots);
				return;
			}
			if (!_itemSlots.TryGetSlot(uid, parentSlotId, out ItemSlot parentSlot, parentItemSlots) || !parentSlot.HasItem)
			{
				SetError("Install a turret before removing turret hardpoints.");
				RefreshUi(parentItemSlots);
				return;
			}
			EntityUid turretUid = parentSlot.Item.Value;
			HardpointSlotsComponent parentTurretSlots = default(HardpointSlotsComponent);
			if (!((EntitySystem)this).TryComp<HardpointSlotsComponent>(turretUid, ref parentTurretSlots))
			{
				SetError("Turret hardpoint slots are unavailable.");
				RefreshUi(parentItemSlots);
			}
			else
			{
				TryStartHardpointRemoval(turretUid, parentTurretSlots, user, childSlotId, uiOwnerUid, uiOwnerComp);
				RefreshUi(parentItemSlots);
			}
			return;
		}
		ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
		if (!((EntitySystem)this).TryComp<ItemSlotsComponent>(uid, ref itemSlots))
		{
			SetError("Unable to access hardpoint slots.");
			RefreshUi();
			return;
		}
		if (!_hardpoints.TryGetSlot(component, slotId, out HardpointSlot slot2))
		{
			SetError("That hardpoint slot does not exist.");
			RefreshUi(itemSlots);
			return;
		}
		if (!_itemSlots.TryGetSlot(uid, slotId, out ItemSlot itemSlot, itemSlots) || !itemSlot.HasItem)
		{
			SetError("No hardpoint is installed in that slot.");
			RefreshUi(itemSlots);
			return;
		}
		HardpointSlotsComponent attachedSlots = default(HardpointSlotsComponent);
		ItemSlotsComponent attachedItemSlots = default(ItemSlotsComponent);
		if (((EntitySystem)this).TryComp<HardpointSlotsComponent>(itemSlot.Item.Value, ref attachedSlots) && ((EntitySystem)this).TryComp<ItemSlotsComponent>(itemSlot.Item.Value, ref attachedItemSlots) && _hardpoints.HasAttachedHardpoints(itemSlot.Item.Value, attachedSlots, attachedItemSlots))
		{
			_popup.PopupEntity("Remove the turret attachments before removing the turret.", uid, user);
			SetError("Remove the turret attachments before removing the turret.");
			RefreshUi(itemSlots);
			return;
		}
		if (((EntitySystem)this).HasComp<HardpointNoRemoveComponent>(itemSlot.Item.Value))
		{
			string error = base.Loc.GetString("rmc-hardpoint-remove-blocked");
			_popup.PopupEntity(error, uid, user);
			SetError(error);
			RefreshUi(itemSlots);
			return;
		}
		if (component.PendingInserts.Contains(slotId) || component.CompletingInserts.Contains(slotId))
		{
			_popup.PopupEntity("Finish installing that hardpoint before removing it.", user, user);
			SetError("Finish installing that hardpoint before removing it.");
			RefreshUi(itemSlots);
			return;
		}
		if (!_hardpoints.TryGetPryingTool(user, component.RemoveToolQuality, out var tool))
		{
			string error2 = base.Loc.GetString("rmc-hardpoint-remove-need-tool");
			_popup.PopupEntity(error2, user, user);
			SetError(error2);
			RefreshUi(itemSlots);
			return;
		}
		if (!component.PendingRemovals.Add(slotId))
		{
			SetError("That hardpoint is already being removed.");
			RefreshUi(itemSlots);
			return;
		}
		float delay = ((slot2.RemoveDelay > 0f) ? slot2.RemoveDelay : slot2.InsertDelay);
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay, new HardpointRemoveDoAfterEvent(slotId), uid, uid, tool)
		{
			BreakOnMove = true,
			BreakOnDamage = true,
			BreakOnHandChange = true,
			BreakOnDropItem = true,
			BreakOnWeightlessMove = true,
			NeedHand = true,
			RequireCanInteract = true,
			DuplicateCondition = DuplicateConditions.SameEvent
		};
		if (!_doAfter.TryStartDoAfter(doAfter))
		{
			component.PendingRemovals.Remove(slotId);
			SetError("Couldn't start hardpoint removal.");
			RefreshUi(itemSlots);
		}
		else
		{
			uiOwnerComp.LastUiError = null;
			RefreshUi(itemSlots);
		}
		void RefreshUi(ItemSlotsComponent? currentItemSlots = null)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			_hardpoints.UpdateHardpointUi(uid, component, currentItemSlots);
			if (uiOwnerUid.Value != uid || uiOwnerComp != component)
			{
				_hardpoints.UpdateHardpointUi(uiOwnerUid.Value, uiOwnerComp);
			}
		}
		void SetError(string lastUiError)
		{
			uiOwnerComp.LastUiError = lastUiError;
		}
	}

	private bool TryInsertTurretAttachment(Entity<HardpointSlotsComponent> ent, EntityUid user, EntityUid used)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<HardpointItemComponent>(used))
		{
			return false;
		}
		ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
		if (!((EntitySystem)this).TryComp<ItemSlotsComponent>(ent.Owner, ref itemSlots))
		{
			return false;
		}
		bool requiresTurret = ((EntitySystem)this).HasComp<VehicleTurretAttachmentComponent>(used);
		bool hasMatchingEmptySlot = false;
		foreach (HardpointSlot slot in ent.Comp.Slots)
		{
			if (_hardpoints.IsValidHardpoint(used, ent.Comp, slot) && _itemSlots.TryGetSlot(ent.Owner, slot.Id, out ItemSlot vehicleSlot, itemSlots) && !vehicleSlot.HasItem)
			{
				hasMatchingEmptySlot = true;
				break;
			}
		}
		if (!requiresTurret && hasMatchingEmptySlot)
		{
			return false;
		}
		HardpointSlotsComponent turretSlots = default(HardpointSlotsComponent);
		ItemSlotsComponent turretItemSlots = default(ItemSlotsComponent);
		foreach (HardpointSlot slot2 in ent.Comp.Slots)
		{
			if (!_itemSlots.TryGetSlot(ent.Owner, slot2.Id, out ItemSlot vehicleSlot2, itemSlots) || !vehicleSlot2.HasItem)
			{
				continue;
			}
			EntityUid turretUid = vehicleSlot2.Item.Value;
			if (!((EntitySystem)this).TryComp<HardpointSlotsComponent>(turretUid, ref turretSlots) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(turretUid, ref turretItemSlots))
			{
				continue;
			}
			foreach (HardpointSlot turretSlot in turretSlots.Slots)
			{
				if (_hardpoints.IsValidHardpoint(used, turretSlots, turretSlot) && _itemSlots.TryGetSlot(turretUid, turretSlot.Id, out ItemSlot turretItemSlot, turretItemSlots) && !turretItemSlot.HasItem)
				{
					_itemSlots.TryInsertFromHand(turretUid, turretItemSlot, user);
					return true;
				}
			}
		}
		if (requiresTurret)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-vehicle-turret-no-base"), ent.Owner, user);
			return true;
		}
		return false;
	}

	private void AddTurretRemoveVerbs(Entity<HardpointSlotsComponent> ent, ref GetVerbsEvent<InteractionVerb> args, ItemSlotsComponent itemSlots)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		HardpointSlotsComponent turretSlots = default(HardpointSlotsComponent);
		ItemSlotsComponent turretItemSlots = default(ItemSlotsComponent);
		foreach (HardpointSlot slot in ent.Comp.Slots)
		{
			if (!_itemSlots.TryGetSlot(ent.Owner, slot.Id, out ItemSlot vehicleSlot, itemSlots) || !vehicleSlot.HasItem)
			{
				continue;
			}
			EntityUid turretUid = vehicleSlot.Item.Value;
			if (!((EntitySystem)this).TryComp<HardpointSlotsComponent>(turretUid, ref turretSlots) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(turretUid, ref turretItemSlots))
			{
				continue;
			}
			foreach (HardpointSlot turretSlot in turretSlots.Slots)
			{
				if (_itemSlots.TryGetSlot(turretUid, turretSlot.Id, out ItemSlot turretItemSlot, turretItemSlots) && turretItemSlot.HasItem && !((EntitySystem)this).HasComp<HardpointNoRemoveComponent>(turretItemSlot.Item.Value))
				{
					EntityUid user = args.User;
					string slotId = turretSlot.Id;
					InteractionVerb verb = new InteractionVerb
					{
						Act = delegate
						{
							//IL_0011: Unknown result type (might be due to invalid IL or missing references)
							//IL_0022: Unknown result type (might be due to invalid IL or missing references)
							TryStartHardpointRemoval(turretUid, turretSlots, user, slotId);
						},
						Category = VerbCategory.Eject,
						Text = base.Loc.GetString("rmc-hardpoint-remove-verb", (ValueTuple<string, object>)("slot", ((EntitySystem)this).Name(turretItemSlot.Item.Value, (MetaDataComponent)null))),
						Priority = turretItemSlot.Priority,
						IconEntity = ((EntitySystem)this).GetNetEntity(turretItemSlot.Item, (MetaDataComponent)null)
					};
					args.Verbs.Add(verb);
				}
			}
		}
	}
}
