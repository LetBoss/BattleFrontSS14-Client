using System;
using Content.Shared._RMC14.Inventory;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Hands;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;

namespace Content.Shared._RMC14.Armor.Magnetic;

public sealed class RMCMagneticSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private ItemSlotsSystem _slots;

	[Dependency]
	private ThrownItemSystem _thrownItem;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCMagneticItemComponent, DroppedEvent>((EntityEventRefHandler<RMCMagneticItemComponent, DroppedEvent>)OnMagneticItemDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCMagneticItemComponent, RMCDroppedEvent>((EntityEventRefHandler<RMCMagneticItemComponent, RMCDroppedEvent>)OnMagneticItemRMCDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCMagneticItemComponent, ThrownEvent>((EntityEventRefHandler<RMCMagneticItemComponent, ThrownEvent>)OnMagneticItemThrown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCMagneticItemComponent, DropAttemptEvent>((EntityEventRefHandler<RMCMagneticItemComponent, DropAttemptEvent>)OnMagneticItemDropAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCMagneticArmorComponent, InventoryRelayedEvent<RMCMagnetizeItemEvent>>((EntityEventRefHandler<RMCMagneticArmorComponent, InventoryRelayedEvent<RMCMagnetizeItemEvent>>)OnMagnetizeItem, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, RMCMagnetizeItemEvent>((EntityEventRefHandler<InventoryComponent, RMCMagnetizeItemEvent>)_inventory.RelayEvent<RMCMagnetizeItemEvent>, (Type[])null, (Type[])null);
	}

	private void OnMagneticItemDropped(Entity<RMCMagneticItemComponent> ent, ref DroppedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		TryReturn(ent, args.User);
	}

	private void OnMagneticItemRMCDropped(Entity<RMCMagneticItemComponent> ent, ref RMCDroppedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		TryReturn(ent, args.User);
	}

	private void OnMagneticItemThrown(Entity<RMCMagneticItemComponent> ent, ref ThrownEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? user = args.User;
		if (user.HasValue)
		{
			EntityUid user2 = user.GetValueOrDefault();
			ThrownItemComponent thrown = default(ThrownItemComponent);
			if (TryReturn(ent, user2) && ((EntitySystem)this).TryComp<ThrownItemComponent>(Entity<RMCMagneticItemComponent>.op_Implicit(ent), ref thrown))
			{
				_thrownItem.StopThrow(Entity<RMCMagneticItemComponent>.op_Implicit(ent), thrown);
			}
		}
	}

	private void OnMagneticItemDropAttempt(Entity<RMCMagneticItemComponent> ent, ref DropAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		if (CanReturn(ent, args.Uid, out EntityUid _, out EntityUid? _, out string _))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private unsafe void OnMagnetizeItem(Entity<RMCMagneticArmorComponent> ent, ref InventoryRelayedEvent<RMCMagnetizeItemEvent> args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		InventorySystem.InventorySlotEnumerator everySlotEnumerator = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(args.Args.User));
		ContainerSlot slot;
		ItemSlotsComponent itemSlotsComp = default(ItemSlotsComponent);
		while (everySlotEnumerator.MoveNext(out slot))
		{
			EntityUid? containedEntity = slot.ContainedEntity;
			if (!containedEntity.HasValue)
			{
				continue;
			}
			EntityUid slotItem = containedEntity.GetValueOrDefault();
			if (!((EntitySystem)this).HasComp<RMCMagneticItemReceiverComponent>(slotItem) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(slotItem, ref itemSlotsComp))
			{
				continue;
			}
			AllContainersEnumerable allContainers = _container.GetAllContainers(slotItem, (ContainerManagerComponent)null);
			AllContainersEnumerator enumerator = ((AllContainersEnumerable)(ref allContainers)).GetEnumerator();
			try
			{
				while (((AllContainersEnumerator)(ref enumerator)).MoveNext())
				{
					BaseContainer slotContainer = ((AllContainersEnumerator)(ref enumerator)).Current;
					if (_slots.TryGetSlot(slotItem, slotContainer.ID, out ItemSlot itemSlot, itemSlotsComp) && _slots.CanInsert(Entity<RMCMagneticArmorComponent>.op_Implicit(ent), args.Args.Item, args.Args.User, itemSlot))
					{
						args.Args.Magnetizer = Entity<RMCMagneticArmorComponent>.op_Implicit(ent);
						args.Args.ReceivingItem = slotItem;
						args.Args.ReceivingContainer = slotContainer.ID;
						return;
					}
				}
			}
			finally
			{
				((IDisposable)(*(AllContainersEnumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
			}
		}
		if ((ent.Comp.AllowMagnetizeToSlots & args.Args.MagnetizeToSlots) == 0)
		{
			return;
		}
		InventorySystem.InventorySlotEnumerator slotEnumerator = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(args.Args.User), ent.Comp.AllowMagnetizeToSlots & args.Args.MagnetizeToSlots);
		ContainerSlot container;
		while (slotEnumerator.MoveNext(out container))
		{
			if (((BaseContainer)container).Count <= 0)
			{
				args.Args.Magnetizer = Entity<RMCMagneticArmorComponent>.op_Implicit(ent);
				break;
			}
		}
	}

	private bool CanReturn(Entity<RMCMagneticItemComponent> ent, EntityUid user, out EntityUid magnetizer, out EntityUid? receivingItem, out string receivingContainer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		RMCMagnetizeItemEvent ev = new RMCMagnetizeItemEvent(user, ent.Owner, ent.Comp.MagnetizeToSlots, SlotFlags.OUTERCLOTHING);
		((EntitySystem)this).RaiseLocalEvent<RMCMagnetizeItemEvent>(user, ref ev, false);
		magnetizer = ev.Magnetizer.GetValueOrDefault();
		receivingItem = ev.ReceivingItem;
		receivingContainer = ev.ReceivingContainer;
		return magnetizer != default(EntityUid);
	}

	private bool TryReturn(Entity<RMCMagneticItemComponent> ent, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (!CanReturn(ent, user, out EntityUid magnetizer, out EntityUid? receivingItem, out string receivingContainer))
		{
			return false;
		}
		RMCReturnToInventoryComponent returnComp = ((EntitySystem)this).EnsureComp<RMCReturnToInventoryComponent>(Entity<RMCMagneticItemComponent>.op_Implicit(ent));
		returnComp.User = user;
		returnComp.Magnetizer = magnetizer;
		returnComp.ReceivingItem = receivingItem;
		returnComp.ReceivingContainer = receivingContainer;
		((EntitySystem)this).Dirty(Entity<RMCMagneticItemComponent>.op_Implicit(ent), (IComponent)(object)returnComp, (MetaDataComponent)null);
		return true;
	}

	public void SetMagnetizeToSlots(Entity<RMCMagneticItemComponent> ent, SlotFlags slots)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.MagnetizeToSlots = slots;
		((EntitySystem)this).Dirty<RMCMagneticItemComponent>(ent, (MetaDataComponent)null);
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<RMCReturnToInventoryComponent> query = ((EntitySystem)this).EntityQueryEnumerator<RMCReturnToInventoryComponent>();
		EntityUid uid = default(EntityUid);
		RMCReturnToInventoryComponent comp = default(RMCReturnToInventoryComponent);
		BaseContainer container = default(BaseContainer);
		while (query.MoveNext(ref uid, ref comp))
		{
			if (comp.Returned)
			{
				continue;
			}
			EntityUid user = comp.User;
			EntityUid magnetizer = comp.Magnetizer;
			if (!((EntitySystem)this).TerminatingOrDeleted(user, (MetaDataComponent)null) && !((EntitySystem)this).TerminatingOrDeleted(magnetizer, (MetaDataComponent)null))
			{
				EntityUid? receivingItem = comp.ReceivingItem;
				if (receivingItem.HasValue)
				{
					EntityUid insertInto = receivingItem.GetValueOrDefault();
					if (_container.TryGetContainer(insertInto, comp.ReceivingContainer, ref container, (ContainerManagerComponent)null) && _container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(uid), container, (TransformComponent)null, true))
					{
						string popup = base.Loc.GetString("rmc-magnetize-return", (ValueTuple<string, object>)("item", uid), (ValueTuple<string, object>)("magnetizer", insertInto));
						_popup.PopupClient(popup, user, user, PopupType.Medium);
						comp.Returned = true;
						((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
					}
				}
				else
				{
					InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(user), SlotFlags.SUITSTORAGE);
					ContainerSlot slot;
					while (slots.MoveNext(out slot))
					{
						if (_inventory.TryEquip(user, uid, ((BaseContainer)slot).ID, silent: false, force: true))
						{
							string popup2 = base.Loc.GetString("rmc-magnetize-return", (ValueTuple<string, object>)("item", uid), (ValueTuple<string, object>)("magnetizer", magnetizer));
							_popup.PopupClient(popup2, user, user, PopupType.Medium);
							comp.Returned = true;
							((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
							break;
						}
					}
				}
			}
			((EntitySystem)this).RemCompDeferred<RMCReturnToInventoryComponent>(uid);
		}
	}
}
