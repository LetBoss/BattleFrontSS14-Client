using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._RMC14.Input;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Xenonids.Devour;
using Content.Shared.Administration.Logs;
using Content.Shared.Clothing.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Strip.Components;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Inventory;

public abstract class SharedCMInventorySystem : EntitySystem
{
	private readonly record struct HolsterSlot(int Priority, bool IsHolster, ContainerSlot? Slot, EntityUid Ent, ItemSlot? ItemSlot) : IComparable<HolsterSlot>
	{
		public int CompareTo(HolsterSlot other)
		{
			if (IsHolster && other.IsHolster)
			{
				return Priority.CompareTo(other.Priority);
			}
			if (IsHolster)
			{
				return -1;
			}
			return 1;
		}
	}

	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private ItemSlotsSystem _itemSlots;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedStorageSystem _storage;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedItemSystem _item;

	[Dependency]
	private SkillsSystem _skills;

	private readonly SlotFlags[] _order = new SlotFlags[6]
	{
		SlotFlags.SUITSTORAGE,
		SlotFlags.BELT,
		SlotFlags.BACK,
		SlotFlags.POCKET,
		SlotFlags.INNERCLOTHING,
		SlotFlags.FEET
	};

	private readonly SlotFlags[] _quickEquipOrder = new SlotFlags[15]
	{
		SlotFlags.BACK,
		SlotFlags.IDCARD,
		SlotFlags.INNERCLOTHING,
		SlotFlags.OUTERCLOTHING,
		SlotFlags.HEAD,
		SlotFlags.FEET,
		SlotFlags.MASK,
		SlotFlags.GLOVES,
		SlotFlags.EARS,
		SlotFlags.EYES,
		SlotFlags.BELT,
		SlotFlags.SUITSTORAGE,
		SlotFlags.NECK,
		SlotFlags.POCKET,
		SlotFlags.LEGS
	};

	private EntityQuery<RMCPickupDroppedItemsComponent> _pickupDroppedItemsQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Expected O, but got Unknown
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Expected O, but got Unknown
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Expected O, but got Unknown
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Expected O, but got Unknown
		_pickupDroppedItemsQuery = ((EntitySystem)this).GetEntityQuery<RMCPickupDroppedItemsComponent>();
		((EntitySystem)this).SubscribeLocalEvent<GunComponent, IsUnholsterableEvent>((EntityEventRefHandler<GunComponent, IsUnholsterableEvent>)AllowUnholster<GunComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MeleeWeaponComponent, IsUnholsterableEvent>((EntityEventRefHandler<MeleeWeaponComponent, IsUnholsterableEvent>)AllowUnholster<MeleeWeaponComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMItemSlotsComponent, MapInitEvent>((EntityEventRefHandler<CMItemSlotsComponent, MapInitEvent>)OnSlotsFillMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMItemSlotsComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<CMItemSlotsComponent, AfterAutoHandleStateEvent>)OnSlotsComponentHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMItemSlotsComponent, ActivateInWorldEvent>((EntityEventRefHandler<CMItemSlotsComponent, ActivateInWorldEvent>)OnSlotsActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMItemSlotsComponent, ItemSlotEjectAttemptEvent>((EntityEventRefHandler<CMItemSlotsComponent, ItemSlotEjectAttemptEvent>)OnSlotsEjectAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMItemSlotsComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<CMItemSlotsComponent, EntInsertedIntoContainerMessage>)OnSlotsEntInsertedIntoContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMItemSlotsComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<CMItemSlotsComponent, EntRemovedFromContainerMessage>)OnSlotsEntRemovedFromContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMItemSlotsComponent, InteractUsingEvent>((EntityEventRefHandler<CMItemSlotsComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMHolsterComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<CMHolsterComponent, GetVerbsEvent<AlternativeVerb>>)OnHolsterGetAltVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMHolsterComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<CMHolsterComponent, AfterAutoHandleStateEvent>)OnHolsterComponentHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMHolsterComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<CMHolsterComponent, EntInsertedIntoContainerMessage>)OnHolsterEntInsertedIntoContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMHolsterComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<CMHolsterComponent, EntRemovedFromContainerMessage>)OnHolsterEntRemovedFromContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCItemPickupComponent, DroppedEvent>((EntityEventRefHandler<RMCItemPickupComponent, DroppedEvent>)OnItemDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCItemPickupComponent, RMCDroppedEvent>((EntityEventRefHandler<RMCItemPickupComponent, RMCDroppedEvent>)OnItemDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCStripTimeSkillComponent, BeforeStripEvent>((EntityEventRefHandler<RMCStripTimeSkillComponent, BeforeStripEvent>)OnSkilledBeforeStrip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMVirtualItemComponent, BeforeRangedInteractEvent>((EntityEventRefHandler<CMVirtualItemComponent, BeforeRangedInteractEvent>)OnVirtualBeforeRangedInteract, (Type[])null, new Type[1] { typeof(SharedVirtualItemSystem) });
		CommandBinds.Builder.Bind(CMKeyFunctions.CMHolsterPrimary, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate(ICommonSession? session)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
			if (val.HasValue)
			{
				EntityUid valueOrDefault = val.GetValueOrDefault();
				OnHolster(valueOrDefault, 0);
			}
		}, (StateInputCmdDelegate)null, false, true)).Bind(CMKeyFunctions.CMHolsterSecondary, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate(ICommonSession? session)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
			if (val.HasValue)
			{
				EntityUid valueOrDefault = val.GetValueOrDefault();
				OnHolster(valueOrDefault, 1);
			}
		}, (StateInputCmdDelegate)null, false, true)).Bind(CMKeyFunctions.CMHolsterTertiary, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate(ICommonSession? session)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
			if (val.HasValue)
			{
				EntityUid valueOrDefault = val.GetValueOrDefault();
				OnHolster(valueOrDefault, 2);
			}
		}, (StateInputCmdDelegate)null, false, true))
			.Bind(CMKeyFunctions.CMHolsterQuaternary, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate(ICommonSession? session)
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
				if (val.HasValue)
				{
					EntityUid valueOrDefault = val.GetValueOrDefault();
					OnHolster(valueOrDefault, 3, CMHolsterChoose.Last);
				}
			}, (StateInputCmdDelegate)null, false, true))
			.Bind(CMKeyFunctions.RMCPickUpDroppedItems, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate(ICommonSession? session)
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
				if (val.HasValue)
				{
					EntityUid valueOrDefault = val.GetValueOrDefault();
					TryPickupDroppedItems(valueOrDefault);
				}
			}, (StateInputCmdDelegate)null, false, true))
			.Register<SharedCMInventorySystem>();
	}

	private void OnHolsterGetAltVerbs(EntityUid holster, CMHolsterComponent comp, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && comp.Contents.Count != 0 && !((EntitySystem)this).HasComp<CMItemSlotsComponent>(holster))
		{
			AlternativeVerb holsterVerb = new AlternativeVerb
			{
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					Unholster(args.User, holster, out var _);
				},
				Text = base.Loc.GetString("rmc-storage-holster-eject-verb"),
				IconEntity = ((EntitySystem)this).GetNetEntity(comp.Contents[0], (MetaDataComponent)null),
				Priority = 5
			};
			args.Verbs.Add(holsterVerb);
		}
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<SharedCMInventorySystem>();
	}

	private void AllowUnholster<T>(Entity<T> ent, ref IsUnholsterableEvent args) where T : IComponent
	{
		args.Unholsterable = true;
	}

	private void OnSlotsFillMapInit(Entity<CMItemSlotsComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		ItemSlot slot = ent.Comp.Slot;
		if (slot == null)
		{
			return;
		}
		int? count = ent.Comp.Count;
		if (!count.HasValue)
		{
			return;
		}
		int count2 = count.GetValueOrDefault();
		List<EntProtoId> items = new List<EntProtoId>();
		EntProtoId? startingItem = ent.Comp.StartingItem;
		if (startingItem.HasValue)
		{
			EntProtoId id = startingItem.GetValueOrDefault();
			items = Enumerable.Repeat<EntProtoId>(id, count2).ToList();
		}
		else
		{
			List<EntProtoId> idList = ent.Comp.StartingItems;
			if (idList != null)
			{
				items = idList;
			}
		}
		ItemSlotsComponent slots = ((EntitySystem)this).EnsureComp<ItemSlotsComponent>(Entity<CMItemSlotsComponent>.op_Implicit(ent));
		EntityCoordinates coordinates = ((EntitySystem)this).Transform(Entity<CMItemSlotsComponent>.op_Implicit(ent)).Coordinates;
		for (int i = 0; i < count2; i++)
		{
			int n = i + 1;
			ItemSlot copy = new ItemSlot(slot);
			copy.Name = $"{copy.Name} {n}";
			_itemSlots.AddItemSlot(Entity<CMItemSlotsComponent>.op_Implicit(ent), $"{slot.Name}{n}", copy);
			if (items.Count > i)
			{
				EntProtoId itemId = items[i];
				ContainerSlot containerSlot = copy.ContainerSlot;
				if (containerSlot != null)
				{
					EntityUid item = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(itemId), coordinates);
					_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(item), (BaseContainer)(object)containerSlot, (TransformComponent)null, false);
				}
				else
				{
					copy.StartingItem = EntProtoId.op_Implicit(itemId);
				}
			}
		}
		ContentsUpdated(ent);
		((EntitySystem)this).Dirty(Entity<CMItemSlotsComponent>.op_Implicit(ent), (IComponent)(object)slots, (MetaDataComponent)null);
	}

	private void OnSlotsComponentHandleState(Entity<CMItemSlotsComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ContentsUpdated(ent);
	}

	private void OnHolsterComponentHandleState(Entity<CMHolsterComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ContentsUpdated(ent);
	}

	private void OnSlotsActivateInWorld(Entity<CMItemSlotsComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<StorageComponent>(Entity<CMItemSlotsComponent>.op_Implicit(ent)))
		{
			PickupSlot(args.User, Entity<CMItemSlotsComponent>.op_Implicit(ent));
		}
	}

	private void OnSlotsEjectAttempt(Entity<CMItemSlotsComponent> ent, ref ItemSlotEjectAttemptEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			return;
		}
		TimeSpan? cooldown = ent.Comp.Cooldown;
		if (cooldown.HasValue)
		{
			TimeSpan cooldown2 = cooldown.GetValueOrDefault();
			if (_timing.CurTime < ent.Comp.LastEjectAt + cooldown2)
			{
				args.Cancelled = true;
			}
		}
	}

	private void OnInteractUsing(Entity<CMItemSlotsComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		ItemSlotsComponent usedStorage = default(ItemSlotsComponent);
		ItemSlotsComponent storage = default(ItemSlotsComponent);
		if (!((EntitySystem)this).TryComp<ItemSlotsComponent>(args.Used, ref usedStorage) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(Entity<CMItemSlotsComponent>.op_Implicit(ent), ref storage) || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		SoundSpecifier insertSound = null;
		BaseContainer usedContainer = default(BaseContainer);
		BaseContainer container = default(BaseContainer);
		foreach (KeyValuePair<string, ItemSlot> usedStorageItemSlot in usedStorage.Slots)
		{
			if (!_container.TryGetContainer(args.Used, usedStorageItemSlot.Key, ref usedContainer, (ContainerManagerComponent)null))
			{
				continue;
			}
			foreach (KeyValuePair<string, ItemSlot> itemSlot in storage.Slots)
			{
				if (!_container.TryGetContainer(Entity<CMItemSlotsComponent>.op_Implicit(ent), itemSlot.Key, ref container, (ContainerManagerComponent)null))
				{
					continue;
				}
				if (_itemSlots.CanInsert(ent.Owner, args.Used, args.User, itemSlot.Value) && _container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(args.Used), container, (TransformComponent)null, false))
				{
					insertSound = itemSlot.Value.InsertSound;
					((HandledEntityEventArgs)args).Handled = true;
					break;
				}
				foreach (EntityUid entity in usedContainer.ContainedEntities)
				{
					if (_itemSlots.CanInsert(ent.Owner, entity, args.User, itemSlot.Value) && _container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(entity), container, (TransformComponent)null, false))
					{
						insertSound = itemSlot.Value.InsertSound;
						((HandledEntityEventArgs)args).Handled = true;
					}
				}
			}
		}
		_audio.PlayPredicted(insertSound, Entity<CMItemSlotsComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
	}

	protected void OnSlotsEntInsertedIntoContainer(Entity<CMItemSlotsComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ContentsUpdated(ent);
	}

	protected void OnSlotsEntRemovedFromContainer(Entity<CMItemSlotsComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			ent.Comp.LastEjectAt = _timing.CurTime;
			((EntitySystem)this).Dirty<CMItemSlotsComponent>(ent, (MetaDataComponent)null);
		}
		ContentsUpdated(ent);
	}

	protected void OnHolsterEntInsertedIntoContainer(Entity<CMHolsterComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid item = ((ContainerModifiedMessage)args).Entity;
		IsUnholsterableEvent ev = default(IsUnholsterableEvent);
		((EntitySystem)this).RaiseLocalEvent<IsUnholsterableEvent>(item, ref ev, false);
		if (ev.Unholsterable && !ent.Comp.Contents.Contains(item))
		{
			EntityWhitelist whitelist = ent.Comp.Whitelist;
			if (whitelist == null || _whitelist.IsWhitelistPass(whitelist, item))
			{
				ent.Comp.Contents.Add(item);
			}
		}
		ContentsUpdated(ent);
	}

	protected void OnHolsterEntRemovedFromContainer(Entity<CMHolsterComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			ent.Comp.LastEjectAt = _timing.CurTime;
			((EntitySystem)this).Dirty<CMHolsterComponent>(ent, (MetaDataComponent)null);
		}
		EntityUid item = ((ContainerModifiedMessage)args).Entity;
		ent.Comp.Contents.Remove(item);
		ContentsUpdated(ent);
	}

	protected void OnItemDropped(Entity<RMCItemPickupComponent> ent, ref DroppedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		HandleDroppedItem(ent, args.User);
	}

	protected void OnItemDropped(Entity<RMCItemPickupComponent> ent, ref RMCDroppedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		HandleDroppedItem(ent, args.User);
	}

	protected void HandleDroppedItem(Entity<RMCItemPickupComponent> item, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		RMCPickupDroppedItemsComponent pickupDroppedItems = default(RMCPickupDroppedItemsComponent);
		if (_pickupDroppedItemsQuery.TryComp(user, ref pickupDroppedItems))
		{
			pickupDroppedItems.DroppedItems.Add(item.Owner);
		}
	}

	protected void TryPickupDroppedItems(EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		RMCPickupDroppedItemsComponent pickupDroppedItems = default(RMCPickupDroppedItemsComponent);
		if (!_pickupDroppedItemsQuery.TryComp(user, ref pickupDroppedItems) || ((EntitySystem)this).HasComp<DevouredComponent>(user))
		{
			return;
		}
		foreach (EntityUid item in (from val in pickupDroppedItems.DroppedItems
			orderby ((EntitySystem)this).HasComp<GunComponent>(val) descending, ((EntitySystem)this).HasComp<MeleeWeaponComponent>(val) descending
			select val).ToList().Distinct())
		{
			if (!_container.IsEntityInContainer(item, (MetaDataComponent)null) && _interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(item)) && _hands.TryPickupAnyHand(user, item))
			{
				pickupDroppedItems.DroppedItems.Remove(item);
				break;
			}
		}
	}

	protected void OnSkilledBeforeStrip(Entity<RMCStripTimeSkillComponent> ent, ref BeforeStripEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		args.Multiplier = _skills.GetSkillDelayMultiplier(Entity<SkillsComponent>.op_Implicit(ent.Owner), ent.Comp.Skill);
	}

	private void OnVirtualBeforeRangedInteract(Entity<CMVirtualItemComponent> ent, ref BeforeRangedInteractEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		VirtualItemComponent comp = default(VirtualItemComponent);
		if (((EntitySystem)this).TryComp<VirtualItemComponent>(Entity<CMVirtualItemComponent>.op_Implicit(ent), ref comp))
		{
			ShouldHandleVirtualItemInteractEvent ev = new ShouldHandleVirtualItemInteractEvent(args);
			((EntitySystem)this).RaiseLocalEvent<ShouldHandleVirtualItemInteractEvent>(comp.BlockingEntity, ref ev, false);
			if (ev.Handle)
			{
				((EntitySystem)this).RaiseLocalEvent<BeforeRangedInteractEvent>(comp.BlockingEntity, args, false);
			}
		}
	}

	protected virtual void ContentsUpdated(Entity<CMItemSlotsComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		(int Filled, int Total) itemSlotsFilled = GetItemSlotsFilled(Entity<ItemSlotsComponent>.op_Implicit(ent.Owner));
		int filled = itemSlotsFilled.Filled;
		int total = itemSlotsFilled.Total;
		CMItemSlotsVisuals visuals = ((total != 0) ? ((filled >= total) ? CMItemSlotsVisuals.Full : (((float)filled >= (float)total * 0.666f) ? CMItemSlotsVisuals.High : (((float)filled >= (float)total * 0.333f) ? CMItemSlotsVisuals.Medium : ((filled > 0) ? CMItemSlotsVisuals.Low : CMItemSlotsVisuals.Empty)))) : CMItemSlotsVisuals.Empty);
		_appearance.SetData(Entity<CMItemSlotsComponent>.op_Implicit(ent), (Enum)CMItemSlotsLayers.Fill, (object)visuals, (AppearanceComponent)null);
	}

	protected virtual void ContentsUpdated(Entity<CMHolsterComponent> ent)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		CMHolsterVisuals visuals = CMHolsterVisuals.Empty;
		int size = 0;
		if (ent.Comp.Contents.Count != 0)
		{
			visuals = CMHolsterVisuals.Full;
			ItemComponent itemComp = default(ItemComponent);
			foreach (EntityUid item in ent.Comp.Contents)
			{
				if (((EntitySystem)this).TryComp<ItemComponent>(item, ref itemComp))
				{
					size += _item.GetItemShape(itemComp).GetArea();
				}
			}
		}
		_appearance.SetData(Entity<CMHolsterComponent>.op_Implicit(ent), (Enum)CMHolsterLayers.Fill, (object)visuals, (AppearanceComponent)null);
	}

	private bool SlotCanInteract(EntityUid user, EntityUid holster, [NotNullWhen(true)] out ItemSlotsComponent? itemSlots)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TryComp<ItemSlotsComponent>(holster, ref itemSlots))
		{
			return false;
		}
		BaseContainer container = default(BaseContainer);
		if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(holster, null)), ref container) && container.Owner != user && _inventory.HasSlot(container.Owner, container.ID))
		{
			itemSlots = null;
			return false;
		}
		return true;
	}

	private bool PickupSlot(EntityUid user, EntityUid holster, EntityWhitelist? whitelist = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		if (!SlotCanInteract(user, holster, out ItemSlotsComponent itemSlots))
		{
			return false;
		}
		foreach (ItemSlot slot in itemSlots.Slots.Values.OrderBy((ItemSlot s) => s.Priority))
		{
			ContainerSlot? containerSlot = slot.ContainerSlot;
			EntityUid? item = ((containerSlot != null) ? containerSlot.ContainedEntity : ((EntityUid?)null));
			if ((!item.HasValue || !_whitelist.IsWhitelistFail(whitelist, item.Value)) && _itemSlots.TryEjectToHands(holster, slot, user, excludeUserAudio: true))
			{
				if (item.HasValue)
				{
					ISharedAdminLogManager adminLog = _adminLog;
					LogStringHandler handler = new LogStringHandler(13, 2);
					handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
					handler.AppendLiteral(" unholstered ");
					handler.AppendFormatted(((EntitySystem)this).ToPrettyString(item, (MetaDataComponent)null), "ToPrettyString(item)");
					adminLog.Add(LogType.RMCHolster, ref handler);
				}
				return true;
			}
		}
		return false;
	}

	private void OnHolster(EntityUid user, int startIndex, CMHolsterChoose choose = CMHolsterChoose.First)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(user), out var active))
		{
			Holster(user, active.Value);
		}
		else
		{
			Unholster(user, startIndex, choose);
		}
	}

	public bool Holster(EntityUid user, EntityUid item, bool act = true)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		List<HolsterSlot> validSlots = new List<HolsterSlot>();
		int priority = 0;
		SlotFlags[] quickEquipOrder = _quickEquipOrder;
		EntityUid? stackedEntity;
		CMHolsterComponent holster = default(CMHolsterComponent);
		string reason;
		foreach (SlotFlags flag in quickEquipOrder)
		{
			InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(user), flag);
			ContainerSlot slot;
			while (slots.MoveNext(out slot))
			{
				stackedEntity = slot.ContainedEntity;
				if (stackedEntity.HasValue)
				{
					EntityUid clothing = stackedEntity.GetValueOrDefault();
					if (!((EntitySystem)this).TryComp<CMHolsterComponent>(clothing, ref holster))
					{
						continue;
					}
					EntityWhitelist whitelist = holster.Whitelist;
					if (whitelist == null || _whitelist.IsWhitelistPass(whitelist, item))
					{
						if (((EntitySystem)this).HasComp<CMItemSlotsComponent>(clothing) && SlotCanInteract(user, clothing, out ItemSlotsComponent slotComp) && TryGetAvailableSlot(Entity<ItemSlotsComponent>.op_Implicit((clothing, slotComp)), item, Entity<HandsComponent>.op_Implicit(user), out ItemSlot itemSlot, emptyOnly: true) && itemSlot.ContainerSlot != null)
						{
							validSlots.Add(new HolsterSlot(priority, IsHolster: true, null, clothing, itemSlot));
						}
						else if (((EntitySystem)this).HasComp<StorageComponent>(clothing) && _storage.CanInsert(clothing, item, user, out reason))
						{
							validSlots.Add(new HolsterSlot(priority, IsHolster: true, null, clothing, null));
						}
					}
				}
				else if (_inventory.CanEquip(user, item, ((BaseContainer)slot).ID, out reason))
				{
					validSlots.Add(new HolsterSlot(priority, IsHolster: false, slot, user, null));
				}
			}
			priority++;
		}
		validSlots.Sort();
		StorageComponent storage = default(StorageComponent);
		CMHolsterComponent holster2 = default(CMHolsterComponent);
		foreach (HolsterSlot slot2 in validSlots)
		{
			if (!slot2.IsHolster && slot2.Slot != null && _inventory.CanEquip(user, item, ((BaseContainer)slot2.Slot).ID, out reason))
			{
				if (act)
				{
					_inventory.TryEquip(user, item, ((BaseContainer)slot2.Slot).ID, silent: true, force: false, predicted: false, null, null, checkDoafter: true);
				}
				return true;
			}
			if (slot2.ItemSlot != null && _itemSlots.CanInsert(slot2.Ent, item, user, slot2.ItemSlot))
			{
				if (act)
				{
					_itemSlots.TryInsert(slot2.Ent, slot2.ItemSlot, item, user, excludeUserAudio: true);
					ISharedAdminLogManager adminLog = _adminLog;
					LogStringHandler handler = new LogStringHandler(11, 2);
					handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
					handler.AppendLiteral(" holstered ");
					handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(item)), "ToPrettyString(item)");
					adminLog.Add(LogType.RMCHolster, ref handler);
				}
				return true;
			}
			if (slot2.ItemSlot == null && ((EntitySystem)this).TryComp<StorageComponent>(slot2.Ent, ref storage) && ((EntitySystem)this).TryComp<CMHolsterComponent>(slot2.Ent, ref holster2) && !holster2.Contents.Contains(item) && _hands.CanDrop(Entity<HandsComponent>.op_Implicit(user), item) && _storage.CanInsert(slot2.Ent, item, user, out reason, storage))
			{
				if (act && _hands.TryDrop(Entity<HandsComponent>.op_Implicit(user), item))
				{
					_storage.Insert(slot2.Ent, item, out stackedEntity, user, storage, playSound: false);
					_audio.PlayPredicted(holster2.InsertSound, item, (EntityUid?)user, (AudioParams?)null);
					ISharedAdminLogManager adminLog2 = _adminLog;
					LogStringHandler handler2 = new LogStringHandler(11, 2);
					handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
					handler2.AppendLiteral(" holstered ");
					handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(item)), "ToPrettyString(item)");
					adminLog2.Add(LogType.RMCHolster, ref handler2);
				}
				return true;
			}
		}
		_popup.PopupClient(base.Loc.GetString("cm-inventory-unable-equip"), user, user, PopupType.SmallCaution);
		return false;
	}

	public bool CanHolster(EntityUid user, EntityUid item)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return Holster(user, item, act: false);
	}

	private bool TryGetAvailableSlot(Entity<ItemSlotsComponent?> ent, EntityUid item, Entity<HandsComponent?>? userEnt, [NotNullWhen(true)] out ItemSlot? itemSlot, bool emptyOnly = false)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		itemSlot = null;
		if (userEnt.HasValue)
		{
			Entity<HandsComponent> user = userEnt.GetValueOrDefault();
			if (((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(user), ref user.Comp, true) && _hands.IsHolding(user, item) && !_hands.CanDrop(user, item))
			{
				return false;
			}
		}
		if (!((EntitySystem)this).Resolve<ItemSlotsComponent>(Entity<ItemSlotsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		List<ItemSlot> slots = new List<ItemSlot>();
		foreach (ItemSlot slot in ent.Comp.Slots.Values)
		{
			if (emptyOnly)
			{
				ContainerSlot? containerSlot = slot.ContainerSlot;
				if (containerSlot != null && containerSlot.ContainedEntity.HasValue)
				{
					continue;
				}
			}
			ItemSlotsSystem itemSlots = _itemSlots;
			EntityUid uid = Entity<ItemSlotsComponent>.op_Implicit(ent);
			Entity<HandsComponent>? val = userEnt;
			if (itemSlots.CanInsert(uid, item, val.HasValue ? new EntityUid?(Entity<HandsComponent>.op_Implicit(val.GetValueOrDefault())) : ((EntityUid?)null), slot))
			{
				slots.Add(slot);
			}
		}
		if (slots.Count == 0)
		{
			return false;
		}
		slots.Sort(ItemSlotsSystem.SortEmpty);
		itemSlot = slots[0];
		return true;
	}

	private bool TryGetLastInserted(Entity<CMHolsterComponent?> holster, out EntityUid item)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		item = default(EntityUid);
		if (!((EntitySystem)this).Resolve<CMHolsterComponent>(Entity<CMHolsterComponent>.op_Implicit(holster), ref holster.Comp, true))
		{
			return false;
		}
		List<EntityUid> contents = holster.Comp.Contents;
		if (contents.Count == 0)
		{
			return false;
		}
		item = contents[contents.Count - 1];
		return true;
	}

	private void Unholster(EntityUid user, int startIndex, CMHolsterChoose choose)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (_order.Length == 0)
		{
			return;
		}
		if (startIndex >= _order.Length)
		{
			startIndex = _order.Length - 1;
		}
		for (int i = startIndex; i < _order.Length; i++)
		{
			if (Unholster(user, _order[i], choose, out var stop) || stop)
			{
				return;
			}
		}
		bool stop2;
		for (int j = 0; j < startIndex && !(Unholster(user, _order[j], choose, out stop2) || stop2); j++)
		{
		}
	}

	private bool Unholster(EntityUid user, SlotFlags flag, CMHolsterChoose choose, out bool stop)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		stop = false;
		InventorySystem.InventorySlotEnumerator enumerator = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(user), flag);
		if (choose == CMHolsterChoose.Last)
		{
			List<EntityUid> items = new List<EntityUid>();
			EntityUid next;
			while (enumerator.NextItem(out next))
			{
				items.Add(next);
			}
			items.Reverse();
			foreach (EntityUid item in items)
			{
				if (Unholster(user, item, out stop))
				{
					return true;
				}
			}
		}
		EntityUid item2;
		while (enumerator.NextItem(out item2))
		{
			if (Unholster(user, item2, out stop))
			{
				return true;
			}
		}
		return false;
	}

	private bool Unholster(EntityUid user, EntityUid item, out bool stop)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		stop = false;
		CMHolsterComponent holster = default(CMHolsterComponent);
		if (((EntitySystem)this).TryComp<CMHolsterComponent>(item, ref holster))
		{
			TimeSpan? cooldown = holster.Cooldown;
			if (cooldown.HasValue)
			{
				TimeSpan cooldown2 = cooldown.GetValueOrDefault();
				if (_timing.CurTime < holster.LastEjectAt + cooldown2)
				{
					stop = true;
					_popup.PopupPredicted(holster.CooldownPopup, user, user, PopupType.SmallCaution);
					return false;
				}
			}
			StorageComponent storage = default(StorageComponent);
			if (((EntitySystem)this).TryComp<StorageComponent>(item, ref storage) && TryGetLastInserted(Entity<CMHolsterComponent>.op_Implicit((item, holster)), out var weapon))
			{
				if (!_hands.TryPickup(user, weapon))
				{
					return false;
				}
				holster.Contents.Remove(weapon);
				_audio.PlayPredicted(holster.EjectSound, item, (EntityUid?)user, (AudioParams?)null);
				stop = true;
				return true;
			}
			if (PickupSlot(user, item, holster.Whitelist))
			{
				ISharedAdminLogManager adminLog = _adminLog;
				LogStringHandler handler = new LogStringHandler(13, 2);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
				handler.AppendLiteral(" unholstered ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(item)), "ToPrettyString(item)");
				adminLog.Add(LogType.RMCHolster, ref handler);
				return true;
			}
		}
		IsUnholsterableEvent ev = default(IsUnholsterableEvent);
		((EntitySystem)this).RaiseLocalEvent<IsUnholsterableEvent>(item, ref ev, false);
		if (!ev.Unholsterable)
		{
			return false;
		}
		ISharedAdminLogManager adminLog2 = _adminLog;
		LogStringHandler handler2 = new LogStringHandler(13, 2);
		handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
		handler2.AppendLiteral(" unholstered ");
		handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(item)), "ToPrettyString(item)");
		adminLog2.Add(LogType.RMCHolster, ref handler2);
		return _hands.TryPickup(user, item);
	}

	public bool TryEquipClothing(EntityUid user, Entity<ClothingComponent> clothing, bool doRangeCheck = true)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		SlotFlags[] quickEquipOrder = _quickEquipOrder;
		foreach (SlotFlags order in quickEquipOrder)
		{
			if ((clothing.Comp.Slots & order) == 0 || !_inventory.TryGetContainerSlotEnumerator(Entity<InventoryComponent>.op_Implicit(user), out var slots, clothing.Comp.Slots))
			{
				continue;
			}
			ContainerSlot slot;
			while (slots.MoveNext(out slot))
			{
				if (_inventory.TryEquip(user, Entity<ClothingComponent>.op_Implicit(clothing), ((BaseContainer)slot).ID, silent: false, force: false, predicted: false, null, null, checkDoafter: false, triggerHandContact: false, doRangeCheck))
				{
					return true;
				}
			}
		}
		return false;
	}

	public (int Filled, int Total) GetItemSlotsFilled(Entity<ItemSlotsComponent?> slots)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ItemSlotsComponent>(Entity<ItemSlotsComponent>.op_Implicit(slots), ref slots.Comp, false))
		{
			return (Filled: 0, Total: 0);
		}
		if (slots.Comp.Slots.Count == 0)
		{
			return (Filled: 0, Total: 0);
		}
		int filled = 0;
		foreach (KeyValuePair<string, ItemSlot> slot in slots.Comp.Slots)
		{
			slot.Deconstruct(out var _, out var value);
			ContainerSlot? containerSlot = value.ContainerSlot;
			EntityUid? val = ((containerSlot != null) ? containerSlot.ContainedEntity : ((EntityUid?)null));
			if (val.HasValue)
			{
				EntityUid contained = val.GetValueOrDefault();
				if (!((EntitySystem)this).TerminatingOrDeleted(contained, (MetaDataComponent)null))
				{
					filled++;
				}
			}
		}
		return (Filled: filled, Total: slots.Comp.Slots.Count);
	}

	public bool TryGetUserHoldingOrStoringItem(EntityUid item, out EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		user = default(EntityUid);
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(item, null)), ref container))
		{
			return false;
		}
		if (IsUser(this, container.Owner))
		{
			user = container.Owner;
			return true;
		}
		StorageComponent storage = default(StorageComponent);
		if (!((EntitySystem)this).TryComp<StorageComponent>(container.Owner, ref storage) || (object)storage.Container != container || !_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(container.Owner, null)), ref container))
		{
			return false;
		}
		if (!IsUser(this, container.Owner))
		{
			return false;
		}
		user = container.Owner;
		return true;
		static bool IsUser(SharedCMInventorySystem system, EntityUid val)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			if (!((EntitySystem)system).HasComp<InventoryComponent>(val))
			{
				return ((EntitySystem)system).HasComp<HandsComponent>(val);
			}
			return true;
		}
	}
}
