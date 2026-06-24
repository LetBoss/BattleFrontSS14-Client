using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Webbing;

public abstract class SharedWebbingSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedItemSystem _item;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private MobStateSystem _mob;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedStorageSystem _storage;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<ClothingBlockWebbingComponent, BeingEquippedAttemptEvent>((EntityEventRefHandler<ClothingBlockWebbingComponent, BeingEquippedAttemptEvent>)OnBlockWebbingBeingEquippedAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WebbingClothingComponent, MapInitEvent>((EntityEventRefHandler<WebbingClothingComponent, MapInitEvent>)OnWebbingClothingMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WebbingClothingComponent, InteractUsingEvent>((EntityEventRefHandler<WebbingClothingComponent, InteractUsingEvent>)OnWebbingClothingInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WebbingClothingComponent, InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>>>((ComponentEventHandler<WebbingClothingComponent, InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>>>)GetRelayedVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WebbingClothingComponent, GetVerbsEvent<EquipmentVerb>>((EntityEventRefHandler<WebbingClothingComponent, GetVerbsEvent<EquipmentVerb>>)OnWebbingClothingGetEquipmentVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WebbingClothingComponent, GetVerbsEvent<InteractionVerb>>((EntityEventRefHandler<WebbingClothingComponent, GetVerbsEvent<InteractionVerb>>)OnWebbingClothingGetInteractionVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WebbingClothingComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<WebbingClothingComponent, EntInsertedIntoContainerMessage>)OnClothingInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WebbingClothingComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<WebbingClothingComponent, EntRemovedFromContainerMessage>)OnClothingRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WebbingClothingComponent, BeingEquippedAttemptEvent>((EntityEventRefHandler<WebbingClothingComponent, BeingEquippedAttemptEvent>)OnClothingBeingEquippedAttempt, (Type[])null, (Type[])null);
	}

	private void OnBlockWebbingBeingEquippedAttempt(Entity<ClothingBlockWebbingComponent> ent, ref BeingEquippedAttemptEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (((CancellableEntityEventArgs)args).Cancelled)
		{
			return;
		}
		InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(args.EquipTarget));
		ContainerSlot slot;
		WebbingClothingComponent clothing = default(WebbingClothingComponent);
		while (slots.MoveNext(out slot))
		{
			if (((EntitySystem)this).TryComp<WebbingClothingComponent>(slot.ContainedEntity, ref clothing) && clothing.Webbing.HasValue)
			{
				args.Reason = "rmc-webbing-cannot-wear-with-webbing";
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
	}

	private void OnWebbingClothingMapInit(Entity<WebbingClothingComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		EntProtoId<WebbingComponent>? startingWebbing = ent.Comp.StartingWebbing;
		if (startingWebbing.HasValue)
		{
			EntProtoId<WebbingComponent> starting = startingWebbing.GetValueOrDefault();
			EntityUid webbing = ((EntitySystem)this).Spawn(EntProtoId<WebbingComponent>.op_Implicit(starting), MapCoordinates.Nullspace, (ComponentRegistry)null, default(Angle));
			Attach(ent, webbing, null, out var _);
		}
	}

	private void OnWebbingClothingInteractUsing(Entity<WebbingClothingComponent> clothing, ref InteractUsingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Attach(clothing, args.Used, args.User, out var handled);
		((HandledEntityEventArgs)args).Handled = handled;
	}

	private void OnWebbingClothingGetInteractionVerbs(Entity<WebbingClothingComponent> clothing, ref GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && !((EntitySystem)this).HasComp<XenoComponent>(args.User) && HasWebbing(Entity<WebbingClothingComponent>.op_Implicit((Entity<WebbingClothingComponent>.op_Implicit(clothing), Entity<WebbingClothingComponent>.op_Implicit(clothing))), out Entity<WebbingComponent> _))
		{
			EntityUid user = args.User;
			args.Verbs.Add(new InteractionVerb
			{
				Text = base.Loc.GetString("rmc-storage-webbing-remove-verb"),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					Detach(clothing, user);
				},
				IconEntity = ((EntitySystem)this).GetNetEntity(clothing.Owner, (MetaDataComponent)null)
			});
		}
	}

	private void GetRelayedVerbs(EntityUid uid, WebbingClothingComponent component, InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>> args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		OnWebbingClothingGetEquipmentVerbs(Entity<WebbingClothingComponent>.op_Implicit((uid, component)), ref args.Args);
	}

	private void OnWebbingClothingGetEquipmentVerbs(Entity<WebbingClothingComponent> clothing, ref GetVerbsEvent<EquipmentVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanAccess || !args.CanInteract || ((EntitySystem)this).HasComp<XenoComponent>(args.User) || !HasWebbing(Entity<WebbingClothingComponent>.op_Implicit((Entity<WebbingClothingComponent>.op_Implicit(clothing), Entity<WebbingClothingComponent>.op_Implicit(clothing))), out Entity<WebbingComponent> _))
		{
			return;
		}
		EntityUid wearer = ((EntitySystem)this).Transform(Entity<WebbingClothingComponent>.op_Implicit(clothing)).ParentUid;
		EntityUid user = args.User;
		if (!(user == wearer) && _mob.IsDead(wearer))
		{
			args.Verbs.Add(new EquipmentVerb
			{
				Text = base.Loc.GetString("rmc-storage-webbing-remove-verb"),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					Detach(clothing, user);
				},
				IconEntity = ((EntitySystem)this).GetNetEntity(clothing.Owner, (MetaDataComponent)null)
			});
		}
	}

	public bool HasWebbing(Entity<WebbingClothingComponent?> clothing, out Entity<WebbingComponent> webbing)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		webbing = default(Entity<WebbingComponent>);
		if (!((EntitySystem)this).Resolve<WebbingClothingComponent>(Entity<WebbingClothingComponent>.op_Implicit(clothing), ref clothing.Comp, false))
		{
			return false;
		}
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<WebbingClothingComponent>.op_Implicit(clothing), clothing.Comp.Container, ref container, (ContainerManagerComponent)null) || container.Count <= 0)
		{
			return false;
		}
		EntityUid ent = container.ContainedEntities[0];
		WebbingComponent webbingComp = default(WebbingComponent);
		if (!((EntitySystem)this).TryComp<WebbingComponent>(ent, ref webbingComp))
		{
			return false;
		}
		webbing = Entity<WebbingComponent>.op_Implicit((ent, webbingComp));
		return true;
	}

	protected virtual void OnClothingInserted(Entity<WebbingClothingComponent> clothing, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (!(clothing.Comp.Container != ((ContainerModifiedMessage)args).Container.ID))
		{
			clothing.Comp.Webbing = ((ContainerModifiedMessage)args).Entity;
			((EntitySystem)this).Dirty<WebbingClothingComponent>(clothing, (MetaDataComponent)null);
			_item.VisualsChanged(Entity<WebbingClothingComponent>.op_Implicit(clothing));
		}
	}

	protected virtual void OnClothingRemoved(Entity<WebbingClothingComponent> clothing, ref EntRemovedFromContainerMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (!(clothing.Comp.Container != ((ContainerModifiedMessage)args).Container.ID))
		{
			clothing.Comp.Webbing = null;
			((EntitySystem)this).Dirty<WebbingClothingComponent>(clothing, (MetaDataComponent)null);
			_item.VisualsChanged(Entity<WebbingClothingComponent>.op_Implicit(clothing));
		}
	}

	private void OnClothingBeingEquippedAttempt(Entity<WebbingClothingComponent> ent, ref BeingEquippedAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Webbing.HasValue)
		{
			return;
		}
		InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(args.EquipTarget));
		ContainerSlot slot;
		while (slots.MoveNext(out slot))
		{
			if (((EntitySystem)this).HasComp<ClothingBlockWebbingComponent>(slot.ContainedEntity))
			{
				args.Reason = "rmc-webbing-cannot-wear-with-webbing";
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
	}

	public bool Attach(Entity<WebbingClothingComponent> clothing, EntityUid webbing, EntityUid? user, out bool handled)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		handled = false;
		WebbingComponent webbingComp = default(WebbingComponent);
		ItemComponent clothingItem = default(ItemComponent);
		ItemComponent webbingItem = default(ItemComponent);
		if (!((EntitySystem)this).TryComp<WebbingComponent>(webbing, ref webbingComp) || ((EntitySystem)this).HasComp<StorageComponent>(Entity<WebbingClothingComponent>.op_Implicit(clothing)) || !((EntitySystem)this).HasComp<StorageComponent>(webbing) || !((EntitySystem)this).TryComp<ItemComponent>(Entity<WebbingClothingComponent>.op_Implicit(clothing), ref clothingItem) || !((EntitySystem)this).TryComp<ItemComponent>(webbing, ref webbingItem))
		{
			return false;
		}
		BaseContainer containing = default(BaseContainer);
		if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(Entity<WebbingClothingComponent>.op_Implicit(clothing), null)), ref containing))
		{
			StorageComponent storage = default(StorageComponent);
			if (((EntitySystem)this).TryComp<StorageComponent>(containing.Owner, ref storage) && storage.StoredItems.ContainsKey(Entity<WebbingClothingComponent>.op_Implicit(clothing)))
			{
				handled = true;
				if (user.HasValue)
				{
					_popup.PopupClient(base.Loc.GetString("rmc-webbing-cannot-in-storage"), user, PopupType.LargeCaution);
				}
				return false;
			}
			InventoryComponent inventory = default(InventoryComponent);
			if (((EntitySystem)this).TryComp<InventoryComponent>(containing.Owner, ref inventory))
			{
				InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit((containing.Owner, inventory)));
				ContainerSlot slot;
				while (slots.MoveNext(out slot))
				{
					if (((EntitySystem)this).HasComp<ClothingBlockWebbingComponent>(slot.ContainedEntity))
					{
						handled = true;
						if (user.HasValue)
						{
							_popup.PopupClient(base.Loc.GetString("rmc-webbing-cannot-wear-with-webbing"), webbing, user, PopupType.SmallCaution);
						}
						return false;
					}
				}
			}
		}
		ContainerSlot container = _container.EnsureContainer<ContainerSlot>(Entity<WebbingClothingComponent>.op_Implicit(clothing), clothing.Comp.Container, (ContainerManagerComponent)null);
		if (((BaseContainer)container).Count > 0 || !_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(webbing), (BaseContainer)(object)container, (TransformComponent)null, false))
		{
			return false;
		}
		base.EntityManager.AddComponents(Entity<WebbingClothingComponent>.op_Implicit(clothing), webbingComp.Components, true);
		WebbingTransferComponent comp = ((EntitySystem)this).EnsureComp<WebbingTransferComponent>(webbing);
		comp.Clothing = Entity<WebbingClothingComponent>.op_Implicit(clothing);
		comp.Transfer = WebbingTransferComponent.TransferType.ToClothing;
		((EntitySystem)this).Dirty(webbing, (IComponent)(object)comp, (MetaDataComponent)null);
		clothing.Comp.UnequippedSize = clothingItem.Size;
		_item.SetSize(Entity<WebbingClothingComponent>.op_Implicit(clothing), webbingItem.Size);
		handled = true;
		return true;
	}

	private void Detach(Entity<WebbingClothingComponent> clothing, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<WebbingClothingComponent>.op_Implicit(clothing), (MetaDataComponent)null) && ((Component)clothing.Comp).Running && HasWebbing(Entity<WebbingClothingComponent>.op_Implicit((Entity<WebbingClothingComponent>.op_Implicit(clothing), Entity<WebbingClothingComponent>.op_Implicit(clothing))), out Entity<WebbingComponent> webbing))
		{
			_container.TryRemoveFromContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(webbing.Owner), false);
			_hands.TryPickupAnyHand(user, Entity<WebbingComponent>.op_Implicit(webbing));
			base.EntityManager.AddComponents(Entity<WebbingComponent>.op_Implicit(webbing), webbing.Comp.Components, true);
			WebbingTransferComponent comp = ((EntitySystem)this).EnsureComp<WebbingTransferComponent>(Entity<WebbingComponent>.op_Implicit(webbing));
			comp.Clothing = Entity<WebbingClothingComponent>.op_Implicit(clothing);
			comp.Transfer = WebbingTransferComponent.TransferType.ToWebbing;
			((EntitySystem)this).Dirty(Entity<WebbingComponent>.op_Implicit(webbing), (IComponent)(object)comp, (MetaDataComponent)null);
			ProtoId<ItemSizePrototype>? unequippedSize = clothing.Comp.UnequippedSize;
			if (unequippedSize.HasValue)
			{
				ProtoId<ItemSizePrototype> size = unequippedSize.GetValueOrDefault();
				clothing.Comp.UnequippedSize = null;
				_item.SetSize(Entity<WebbingClothingComponent>.op_Implicit(clothing), size);
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<WebbingTransferComponent, WebbingComponent> query = ((EntitySystem)this).EntityQueryEnumerator<WebbingTransferComponent, WebbingComponent>();
		EntityUid uid = default(EntityUid);
		WebbingTransferComponent transfer = default(WebbingTransferComponent);
		WebbingComponent webbing = default(WebbingComponent);
		StorageComponent storage2 = default(StorageComponent);
		StorageComponent storage = default(StorageComponent);
		while (query.MoveNext(ref uid, ref transfer, ref webbing))
		{
			if (transfer.Defer)
			{
				transfer.Defer = false;
				continue;
			}
			((EntitySystem)this).RemCompDeferred<WebbingTransferComponent>(uid);
			switch (transfer.Transfer)
			{
			case WebbingTransferComponent.TransferType.ToClothing:
			{
				if (!((EntitySystem)this).TryComp<StorageComponent>(uid, ref storage2))
				{
					break;
				}
				EntityUid? stackedEntity = transfer.Clothing;
				if (stackedEntity.HasValue)
				{
					EntityUid clothing2 = stackedEntity.GetValueOrDefault();
					EntityUid[] array = ((BaseContainer)storage2.Container).ContainedEntities.ToArray();
					foreach (EntityUid stored2 in array)
					{
						_storage.Insert(clothing2, stored2, out stackedEntity, null, null, playSound: false);
					}
				}
				break;
			}
			case WebbingTransferComponent.TransferType.ToWebbing:
			{
				EntityUid? stackedEntity = transfer.Clothing;
				if (!stackedEntity.HasValue)
				{
					break;
				}
				EntityUid clothing = stackedEntity.GetValueOrDefault();
				if (((EntitySystem)this).TryComp<StorageComponent>(clothing, ref storage))
				{
					EntityUid[] array = ((BaseContainer)storage.Container).ContainedEntities.ToArray();
					foreach (EntityUid stored in array)
					{
						_storage.Insert(uid, stored, out stackedEntity, null, null, playSound: false);
					}
				}
				foreach (ComponentRegistryEntry entry in ((Dictionary<string, ComponentRegistryEntry>)(object)webbing.Components).Values)
				{
					((EntitySystem)this).RemComp(clothing, ((object)entry.Component).GetType());
				}
				break;
			}
			}
		}
	}
}
