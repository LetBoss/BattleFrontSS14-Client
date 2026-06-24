using System;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Popups;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Shared.Inventory.VirtualItem;

public abstract class SharedVirtualItemSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _containerSystem;

	[Dependency]
	private SharedItemSystem _itemSystem;

	[Dependency]
	private InventorySystem _inventorySystem;

	[Dependency]
	private SharedHandsSystem _handsSystem;

	[Dependency]
	private SharedPopupSystem _popup;

	private static readonly EntProtoId VirtualItem = EntProtoId.op_Implicit("VirtualItem");

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<VirtualItemComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<VirtualItemComponent, AfterAutoHandleStateEvent>)OnAfterAutoHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VirtualItemComponent, BeingEquippedAttemptEvent>((EntityEventRefHandler<VirtualItemComponent, BeingEquippedAttemptEvent>)OnBeingEquippedAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VirtualItemComponent, BeingUnequippedAttemptEvent>((EntityEventRefHandler<VirtualItemComponent, BeingUnequippedAttemptEvent>)OnBeingUnequippedAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VirtualItemComponent, BeforeRangedInteractEvent>((EntityEventRefHandler<VirtualItemComponent, BeforeRangedInteractEvent>)OnBeforeRangedInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VirtualItemComponent, GettingInteractedWithAttemptEvent>((EntityEventRefHandler<VirtualItemComponent, GettingInteractedWithAttemptEvent>)OnGettingInteractedWithAttemptEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VirtualItemComponent, GetUsedEntityEvent>((EntityEventRefHandler<VirtualItemComponent, GetUsedEntityEvent>)OnGetUsedEntity, (Type[])null, (Type[])null);
	}

	private void OnAfterAutoHandleState(Entity<VirtualItemComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (_containerSystem.IsEntityInContainer(Entity<VirtualItemComponent>.op_Implicit(ent), (MetaDataComponent)null))
		{
			_itemSystem.VisualsChanged(Entity<VirtualItemComponent>.op_Implicit(ent));
		}
	}

	private void OnBeingEquippedAttempt(Entity<VirtualItemComponent> ent, ref BeingEquippedAttemptEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}

	private void OnBeingUnequippedAttempt(Entity<VirtualItemComponent> ent, ref BeingUnequippedAttemptEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}

	private void OnBeforeRangedInteract(Entity<VirtualItemComponent> ent, ref BeforeRangedInteractEvent args)
	{
		((HandledEntityEventArgs)args).Handled = true;
	}

	private void OnGettingInteractedWithAttemptEvent(Entity<VirtualItemComponent> ent, ref GettingInteractedWithAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnGetUsedEntity(Entity<VirtualItemComponent> ent, ref GetUsedEntityEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (args.Handled)
		{
			return;
		}
		foreach (EntityUid item in _handsSystem.EnumerateHeld(Entity<HandsComponent>.op_Implicit(args.User)))
		{
			if (item == ent.Comp.BlockingEntity)
			{
				args.Used = ent.Comp.BlockingEntity;
				break;
			}
		}
	}

	public bool TrySpawnVirtualItemInHand(EntityUid blockingEnt, EntityUid user, bool dropOthers = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? virtualItem;
		return TrySpawnVirtualItemInHand(blockingEnt, user, out virtualItem, dropOthers);
	}

	public bool TrySpawnVirtualItemInHand(EntityUid blockingEnt, EntityUid user, [NotNullWhen(true)] out EntityUid? virtualItem, bool dropOthers = false, string? empty = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		virtualItem = null;
		if (empty == null && !_handsSystem.TryGetEmptyHand(Entity<HandsComponent>.op_Implicit(user), out empty))
		{
			if (!dropOthers)
			{
				return false;
			}
			foreach (string hand in _handsSystem.EnumerateHands(Entity<HandsComponent>.op_Implicit(user)))
			{
				if (!_handsSystem.TryGetHeldItem(Entity<HandsComponent>.op_Implicit(user), hand, out var held))
				{
					continue;
				}
				EntityUid? val = held;
				if ((!val.HasValue || !(val.GetValueOrDefault() == blockingEnt)) && _handsSystem.TryDrop(Entity<HandsComponent>.op_Implicit(user), hand))
				{
					if (!((EntitySystem)this).TerminatingOrDeleted(held, (MetaDataComponent)null))
					{
						_popup.PopupClient(base.Loc.GetString("virtual-item-dropped-other", (ValueTuple<string, object>)("dropped", held)), user, user);
					}
					empty = hand;
					break;
				}
			}
		}
		if (empty == null)
		{
			return false;
		}
		if (!TrySpawnVirtualItem(blockingEnt, user, out virtualItem))
		{
			return false;
		}
		_handsSystem.DoPickup(user, empty, virtualItem.Value);
		return true;
	}

	public void DeleteInHandsMatching(EntityUid user, EntityUid matching)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		VirtualItemComponent virt = default(VirtualItemComponent);
		foreach (EntityUid held in _handsSystem.EnumerateHeld(Entity<HandsComponent>.op_Implicit(user)))
		{
			if (((EntitySystem)this).TryComp<VirtualItemComponent>(held, ref virt) && virt.BlockingEntity == matching)
			{
				DeleteVirtualItem(Entity<VirtualItemComponent>.op_Implicit((held, virt)), user);
			}
		}
	}

	public bool TrySpawnVirtualItemInInventory(EntityUid blockingEnt, EntityUid user, string slot, bool force = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? virtualItem;
		return TrySpawnVirtualItemInInventory(blockingEnt, user, slot, force, out virtualItem);
	}

	public bool TrySpawnVirtualItemInInventory(EntityUid blockingEnt, EntityUid user, string slot, bool force, [NotNullWhen(true)] out EntityUid? virtualItem)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!TrySpawnVirtualItem(blockingEnt, user, out virtualItem))
		{
			return false;
		}
		_inventorySystem.TryEquip(user, virtualItem.Value, slot, silent: false, force);
		return true;
	}

	public void DeleteInSlotMatching(EntityUid user, EntityUid matching, string? slotName = null)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (slotName != null)
		{
			VirtualItemComponent virt = default(VirtualItemComponent);
			if (_inventorySystem.TryGetSlotEntity(user, slotName, out var slotEnt) && ((EntitySystem)this).TryComp<VirtualItemComponent>(slotEnt, ref virt) && virt.BlockingEntity == matching)
			{
				DeleteVirtualItem(Entity<VirtualItemComponent>.op_Implicit((slotEnt.Value, virt)), user);
			}
		}
		else
		{
			if (!_inventorySystem.TryGetSlots(user, out SlotDefinition[] slotDefinitions))
			{
				return;
			}
			SlotDefinition[] array = slotDefinitions;
			VirtualItemComponent virt2 = default(VirtualItemComponent);
			foreach (SlotDefinition slot in array)
			{
				if (_inventorySystem.TryGetSlotEntity(user, slot.Name, out var slotEnt2) && ((EntitySystem)this).TryComp<VirtualItemComponent>(slotEnt2, ref virt2) && virt2.BlockingEntity == matching)
				{
					DeleteVirtualItem(Entity<VirtualItemComponent>.op_Implicit((slotEnt2.Value, virt2)), user);
				}
			}
		}
	}

	public bool TrySpawnVirtualItem(EntityUid blockingEnt, EntityUid user, [NotNullWhen(true)] out EntityUid? virtualItem)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates pos = ((EntitySystem)this).Transform(user).Coordinates;
		virtualItem = ((EntitySystem)this).PredictedSpawnAttachedTo(EntProtoId.op_Implicit(VirtualItem), pos, (ComponentRegistry)null, default(Angle));
		VirtualItemComponent virtualItemComp = ((EntitySystem)this).Comp<VirtualItemComponent>(virtualItem.Value);
		virtualItemComp.BlockingEntity = blockingEnt;
		((EntitySystem)this).Dirty(virtualItem.Value, (IComponent)(object)virtualItemComp, (MetaDataComponent)null);
		return true;
	}

	public void DeleteVirtualItem(Entity<VirtualItemComponent> item, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		VirtualItemDeletedEvent userEv = new VirtualItemDeletedEvent(item.Comp.BlockingEntity, user);
		((EntitySystem)this).RaiseLocalEvent<VirtualItemDeletedEvent>(user, userEv, false);
		VirtualItemDeletedEvent targEv = new VirtualItemDeletedEvent(item.Comp.BlockingEntity, user);
		((EntitySystem)this).RaiseLocalEvent<VirtualItemDeletedEvent>(item.Comp.BlockingEntity, targEv, false);
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<VirtualItemComponent>.op_Implicit(item), (MetaDataComponent)null))
		{
			((EntitySystem)this).PredictedQueueDel(item.Owner);
		}
	}
}
