using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Lock;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Utility;

namespace Content.Shared.Containers.ItemSlots;

public sealed class ItemSlotsSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private ActionBlockerSystem _actionBlockerSystem;

	[Dependency]
	private SharedContainerSystem _containers;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedHandsSystem _handsSystem;

	[Dependency]
	private SharedAudioSystem _audioSystem;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		InitializeLock();
		((EntitySystem)this).SubscribeLocalEvent<ItemSlotsComponent, MapInitEvent>((ComponentEventHandler<ItemSlotsComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemSlotsComponent, ComponentInit>((ComponentEventHandler<ItemSlotsComponent, ComponentInit>)Oninitialize, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemSlotsComponent, InteractUsingEvent>((ComponentEventHandler<ItemSlotsComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemSlotsComponent, InteractHandEvent>((ComponentEventHandler<ItemSlotsComponent, InteractHandEvent>)OnInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemSlotsComponent, UseInHandEvent>((ComponentEventHandler<ItemSlotsComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemSlotsComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<ItemSlotsComponent, GetVerbsEvent<AlternativeVerb>>)AddAlternativeVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemSlotsComponent, GetVerbsEvent<InteractionVerb>>((ComponentEventHandler<ItemSlotsComponent, GetVerbsEvent<InteractionVerb>>)AddInteractionVerbsVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemSlotsComponent, BreakageEventArgs>((ComponentEventHandler<ItemSlotsComponent, BreakageEventArgs>)OnBreak, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemSlotsComponent, DestructionEventArgs>((ComponentEventHandler<ItemSlotsComponent, DestructionEventArgs>)OnBreak, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemSlotsComponent, ComponentGetState>((ComponentEventRefHandler<ItemSlotsComponent, ComponentGetState>)GetItemSlotsState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemSlotsComponent, ComponentHandleState>((ComponentEventRefHandler<ItemSlotsComponent, ComponentHandleState>)HandleItemSlotsState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemSlotsComponent, ItemSlotButtonPressedEvent>((ComponentEventHandler<ItemSlotsComponent, ItemSlotButtonPressedEvent>)HandleButtonPressed, (Type[])null, (Type[])null);
	}

	private void OnMapInit(EntityUid uid, ItemSlotsComponent itemSlots, MapInitEvent args)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		foreach (ItemSlot slot in itemSlots.Slots.Values)
		{
			if (!slot.HasItem && !string.IsNullOrEmpty(slot.StartingItem))
			{
				EntityUid item = ((EntitySystem)this).Spawn(slot.StartingItem, ((EntitySystem)this).Transform(uid).Coordinates);
				if (slot.ContainerSlot != null)
				{
					_containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(item), (BaseContainer)(object)slot.ContainerSlot, (TransformComponent)null, false);
				}
			}
		}
	}

	private void Oninitialize(EntityUid uid, ItemSlotsComponent itemSlots, ComponentInit args)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<string, ItemSlot> slot in itemSlots.Slots)
		{
			slot.Deconstruct(out var key, out var value);
			string id = key;
			value.ContainerSlot = _containers.EnsureContainer<ContainerSlot>(uid, id, (ContainerManagerComponent)null);
		}
	}

	public void AddItemSlot(EntityUid uid, string id, ItemSlot slot, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (itemSlots == null)
		{
			itemSlots = ((EntitySystem)this).EnsureComp<ItemSlotsComponent>(uid);
		}
		if (itemSlots.Slots.TryGetValue(id, out ItemSlot existing))
		{
			if (existing.Local)
			{
				((EntitySystem)this).Log.Error($"Duplicate item slot key. Entity: {((EntitySystem)this).Comp<MetaDataComponent>(uid).EntityName} ({uid}), key: {id}");
			}
			else
			{
				slot.CopyFrom(existing);
			}
		}
		slot.ContainerSlot = _containers.EnsureContainer<ContainerSlot>(uid, id, (ContainerManagerComponent)null);
		itemSlots.Slots[id] = slot;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)itemSlots, (MetaDataComponent)null);
	}

	public void RemoveItemSlot(EntityUid uid, ItemSlot slot, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Terminating(uid, (MetaDataComponent)null) || slot.ContainerSlot == null)
		{
			return;
		}
		_containers.ShutdownContainer((BaseContainer)(object)slot.ContainerSlot);
		if (((EntitySystem)this).Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
		{
			itemSlots.Slots.Remove(((BaseContainer)slot.ContainerSlot).ID);
			if (itemSlots.Slots.Count == 0)
			{
				((EntitySystem)this).RemComp(uid, (IComponent)(object)itemSlots);
			}
			else
			{
				((EntitySystem)this).Dirty(uid, (IComponent)(object)itemSlots, (MetaDataComponent)null);
			}
		}
	}

	public bool TryGetSlot(EntityUid uid, string slotId, [NotNullWhen(true)] out ItemSlot? itemSlot, ItemSlotsComponent? component = null)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		itemSlot = null;
		if (!((EntitySystem)this).Resolve<ItemSlotsComponent>(uid, ref component, true))
		{
			return false;
		}
		return component.Slots.TryGetValue(slotId, out itemSlot);
	}

	public void SetEjectFlags(EntityUid uid, ItemSlot slot, bool disableEject, bool ejectOnInteract = false, bool ejectOnUse = false, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
		{
			slot.DisableEject = disableEject;
			slot.EjectOnInteract = ejectOnInteract;
			slot.EjectOnUse = ejectOnUse;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)itemSlots, (MetaDataComponent)null);
		}
	}

	private void OnInteractHand(EntityUid uid, ItemSlotsComponent itemSlots, InteractHandEvent args)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		foreach (ItemSlot slot in itemSlots.Slots.Values)
		{
			if (slot.EjectOnInteract && slot.Item.HasValue && CanEject(uid, args.User, slot, args.User))
			{
				((HandledEntityEventArgs)args).Handled = true;
				TryEjectToHands(uid, slot, args.User, excludeUserAudio: true);
				break;
			}
		}
	}

	private void OnUseInHand(EntityUid uid, ItemSlotsComponent itemSlots, UseInHandEvent args)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		foreach (ItemSlot slot in itemSlots.Slots.Values)
		{
			if (slot.EjectOnUse && slot.Item.HasValue && CanEject(uid, args.User, slot, args.User))
			{
				((HandledEntityEventArgs)args).Handled = true;
				TryEjectToHands(uid, slot, args.User, excludeUserAudio: true);
				break;
			}
		}
	}

	private void OnInteractUsing(EntityUid uid, ItemSlotsComponent itemSlots, InteractUsingEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent hands = default(HandsComponent);
		if (((HandledEntityEventArgs)args).Handled || !((EntitySystem)this).TryComp<HandsComponent>(args.User, ref hands) || itemSlots.Slots.Count == 0)
		{
			return;
		}
		List<ItemSlot> slots = new List<ItemSlot>();
		string whitelistFailPopup = null;
		string lockedFailPopup = null;
		foreach (ItemSlot slot in itemSlots.Slots.Values)
		{
			if (!slot.InsertOnInteract)
			{
				continue;
			}
			if (CanInsert(uid, args.Used, args.User, slot, slot.Swap))
			{
				slots.Add(slot);
				continue;
			}
			bool allowed = CanInsertWhitelist(args.Used, slot);
			if (lockedFailPopup == null && slot.LockedFailPopup.HasValue && allowed && slot.Locked)
			{
				LocId? lockedFailPopup2 = slot.LockedFailPopup;
				lockedFailPopup = (lockedFailPopup2.HasValue ? LocId.op_Implicit(lockedFailPopup2.GetValueOrDefault()) : null);
			}
			if (whitelistFailPopup == null && slot.WhitelistFailPopup.HasValue)
			{
				LocId? lockedFailPopup2 = slot.WhitelistFailPopup;
				whitelistFailPopup = (lockedFailPopup2.HasValue ? LocId.op_Implicit(lockedFailPopup2.GetValueOrDefault()) : null);
			}
		}
		if (slots.Count == 0)
		{
			if (lockedFailPopup != null)
			{
				_popupSystem.PopupClient(base.Loc.GetString(lockedFailPopup), uid, args.User);
			}
			else if (whitelistFailPopup != null)
			{
				_popupSystem.PopupClient(base.Loc.GetString(whitelistFailPopup), uid, args.User);
			}
		}
		else
		{
			if (!_handsSystem.TryDrop(Entity<HandsComponent>.op_Implicit(args.User), args.Used))
			{
				return;
			}
			slots.Sort(SortEmpty);
			using List<ItemSlot>.Enumerator enumerator2 = slots.GetEnumerator();
			if (enumerator2.MoveNext())
			{
				ItemSlot slot2 = enumerator2.Current;
				if (slot2.Item.HasValue)
				{
					_handsSystem.TryPickupAnyHand(args.User, slot2.Item.Value, checkActionBlocker: true, animateUser: false, animate: true, hands);
				}
				Insert(uid, slot2, args.Used, args.User, excludeUserAudio: true);
				if (slot2.InsertSuccessPopup.HasValue)
				{
					SharedPopupSystem popupSystem = _popupSystem;
					ILocalizationManager loc = base.Loc;
					LocId? lockedFailPopup2 = slot2.InsertSuccessPopup;
					popupSystem.PopupClient(loc.GetString(lockedFailPopup2.HasValue ? LocId.op_Implicit(lockedFailPopup2.GetValueOrDefault()) : null), uid, args.User);
				}
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void Insert(EntityUid uid, ItemSlot slot, EntityUid item, EntityUid? user, bool excludeUserAudio = false)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		bool? inserted = ((slot.ContainerSlot != null) ? new bool?(_containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(item), (BaseContainer)(object)slot.ContainerSlot, (TransformComponent)null, false)) : ((bool?)null));
		if (inserted.HasValue && inserted.Value && user.HasValue)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(16, 4);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user.Value)), "ToPrettyString(user.Value)");
			handler.AppendLiteral(" inserted ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(item)), "ToPrettyString(item)");
			handler.AppendLiteral(" into ");
			handler.AppendFormatted(((BaseContainer)(slot.ContainerSlot?)).ID + " slot of ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
			adminLogger.Add(LogType.Action, LogImpact.Low, ref handler);
		}
		_audioSystem.PlayPredicted(slot.InsertSound, uid, excludeUserAudio ? user : ((EntityUid?)null), (AudioParams?)null);
	}

	public bool CanInsert(EntityUid uid, EntityUid usedUid, EntityUid? user, ItemSlot slot, bool swap = false)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (slot.ContainerSlot == null)
		{
			return false;
		}
		if (slot.HasItem && (!swap || (swap && !CanEject(uid, user, slot))))
		{
			return false;
		}
		if (!CanInsertWhitelist(usedUid, slot))
		{
			return false;
		}
		if (slot.Locked)
		{
			return false;
		}
		ItemSlotInsertAttemptEvent ev = new ItemSlotInsertAttemptEvent(uid, usedUid, user, slot);
		((EntitySystem)this).RaiseLocalEvent<ItemSlotInsertAttemptEvent>(uid, ref ev, false);
		((EntitySystem)this).RaiseLocalEvent<ItemSlotInsertAttemptEvent>(usedUid, ref ev, false);
		if (ev.Cancelled)
		{
			return false;
		}
		return _containers.CanInsert(usedUid, (BaseContainer)(object)slot.ContainerSlot, swap, (TransformComponent)null);
	}

	private bool CanInsertWhitelist(EntityUid usedUid, ItemSlot slot)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (_whitelistSystem.IsWhitelistFail(slot.Whitelist, usedUid) || _whitelistSystem.IsBlacklistPass(slot.Blacklist, usedUid))
		{
			return false;
		}
		return true;
	}

	public bool TryInsert(EntityUid uid, string id, EntityUid item, EntityUid? user, ItemSlotsComponent? itemSlots = null, bool excludeUserAudio = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ItemSlotsComponent>(uid, ref itemSlots, true))
		{
			return false;
		}
		if (!itemSlots.Slots.TryGetValue(id, out ItemSlot slot))
		{
			return false;
		}
		return TryInsert(uid, slot, item, user, excludeUserAudio);
	}

	public bool TryInsert(EntityUid uid, ItemSlot slot, EntityUid item, EntityUid? user, bool excludeUserAudio = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!CanInsert(uid, item, user, slot))
		{
			return false;
		}
		Insert(uid, slot, item, user, excludeUserAudio);
		return true;
	}

	public bool TryInsertFromHand(EntityUid uid, ItemSlot slot, EntityUid user, HandsComponent? hands = null, bool excludeUserAudio = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(user, ref hands, false))
		{
			return false;
		}
		if (!_handsSystem.TryGetActiveItem(Entity<HandsComponent>.op_Implicit((user, hands)), out var held))
		{
			return false;
		}
		if (!CanInsert(uid, held.Value, user, slot))
		{
			return false;
		}
		if (!_handsSystem.TryDrop(Entity<HandsComponent>.op_Implicit(user), hands.ActiveHandId))
		{
			return false;
		}
		Insert(uid, slot, held.Value, user, excludeUserAudio);
		return true;
	}

	public bool TryInsertEmpty(Entity<ItemSlotsComponent?> ent, EntityUid item, EntityUid? user, bool excludeUserAudio = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ItemSlotsComponent>(Entity<ItemSlotsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		Entity<ItemSlotsComponent?> ent2 = ent;
		EntityUid? val = user;
		if (!TryGetAvailableSlot(ent2, item, val.HasValue ? new Entity<HandsComponent>?(Entity<HandsComponent>.op_Implicit(val.GetValueOrDefault())) : ((Entity<HandsComponent>?)null), out ItemSlot itemSlot, emptyOnly: true))
		{
			return false;
		}
		if (user.HasValue && !_handsSystem.TryDrop(Entity<HandsComponent>.op_Implicit(user.Value), item))
		{
			return false;
		}
		Insert(Entity<ItemSlotsComponent>.op_Implicit(ent), itemSlot, item, user, excludeUserAudio);
		return true;
	}

	public bool TryGetAvailableSlot(Entity<ItemSlotsComponent?> ent, EntityUid item, Entity<HandsComponent?>? userEnt, [NotNullWhen(true)] out ItemSlot? itemSlot, bool emptyOnly = false)
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
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		itemSlot = null;
		if (userEnt.HasValue)
		{
			Entity<HandsComponent> user = userEnt.GetValueOrDefault();
			if (((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(user), ref user.Comp, true) && _handsSystem.IsHolding(user, item) && !_handsSystem.CanDrop(user, item))
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
			EntityUid uid = Entity<ItemSlotsComponent>.op_Implicit(ent);
			Entity<HandsComponent>? val = userEnt;
			if (CanInsert(uid, item, val.HasValue ? new EntityUid?(Entity<HandsComponent>.op_Implicit(val.GetValueOrDefault())) : ((EntityUid?)null), slot))
			{
				slots.Add(slot);
			}
		}
		if (slots.Count == 0)
		{
			return false;
		}
		slots.Sort(SortEmpty);
		itemSlot = slots[0];
		return true;
	}

	public static int SortEmpty(ItemSlot a, ItemSlot b)
	{
		ContainerSlot? containerSlot = a.ContainerSlot;
		EntityUid? aEnt = ((containerSlot != null) ? containerSlot.ContainedEntity : ((EntityUid?)null));
		ContainerSlot? containerSlot2 = b.ContainerSlot;
		EntityUid? bEnt = ((containerSlot2 != null) ? containerSlot2.ContainedEntity : ((EntityUid?)null));
		if (!aEnt.HasValue && !bEnt.HasValue)
		{
			return a.Priority.CompareTo(b.Priority);
		}
		if (!aEnt.HasValue)
		{
			return -1;
		}
		return 1;
	}

	public bool CanEject(EntityUid uid, EntityUid? user, ItemSlot slot, EntityUid? popup = null)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		if (slot.Locked)
		{
			if (popup.HasValue && slot.LockedFailPopup.HasValue)
			{
				SharedPopupSystem popupSystem = _popupSystem;
				ILocalizationManager loc = base.Loc;
				LocId? lockedFailPopup = slot.LockedFailPopup;
				popupSystem.PopupClient(loc.GetString(lockedFailPopup.HasValue ? LocId.op_Implicit(lockedFailPopup.GetValueOrDefault()) : null), uid, popup.Value);
			}
			return false;
		}
		ContainerSlot? containerSlot = slot.ContainerSlot;
		EntityUid? val = ((containerSlot != null) ? containerSlot.ContainedEntity : ((EntityUid?)null));
		if (val.HasValue)
		{
			EntityUid item = val.GetValueOrDefault();
			ItemSlotEjectAttemptEvent ev = new ItemSlotEjectAttemptEvent(uid, item, user, slot);
			((EntitySystem)this).RaiseLocalEvent<ItemSlotEjectAttemptEvent>(uid, ref ev, false);
			((EntitySystem)this).RaiseLocalEvent<ItemSlotEjectAttemptEvent>(item, ref ev, false);
			if (ev.Cancelled)
			{
				return false;
			}
			return _containers.CanRemove(item, (BaseContainer)(object)slot.ContainerSlot);
		}
		return false;
	}

	private void Eject(EntityUid uid, ItemSlot slot, EntityUid item, EntityUid? user, bool excludeUserAudio = false)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		bool? ejected = ((slot.ContainerSlot != null) ? new bool?(_containers.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(item), (BaseContainer)(object)slot.ContainerSlot, true, false, (EntityCoordinates?)null, (Angle?)null)) : ((bool?)null));
		if (ejected.HasValue && ejected.Value && user.HasValue)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(15, 4);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user.Value)), "ToPrettyString(user.Value)");
			handler.AppendLiteral(" ejected ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(item)), "ToPrettyString(item)");
			handler.AppendLiteral(" from ");
			handler.AppendFormatted(((BaseContainer)(slot.ContainerSlot?)).ID + " slot of ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
			adminLogger.Add(LogType.Action, LogImpact.Low, ref handler);
		}
		_audioSystem.PlayPredicted(slot.EjectSound, uid, excludeUserAudio ? user : ((EntityUid?)null), (AudioParams?)null);
	}

	public bool TryEject(EntityUid uid, ItemSlot slot, EntityUid? user, [NotNullWhen(true)] out EntityUid? item, bool excludeUserAudio = false)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		item = null;
		if (!CanEject(uid, user, slot))
		{
			return false;
		}
		item = slot.Item;
		if (user.HasValue && item.HasValue && !_actionBlockerSystem.CanPickup(user.Value, item.Value))
		{
			return false;
		}
		Eject(uid, slot, item.Value, user, excludeUserAudio);
		return true;
	}

	public bool TryEject(EntityUid uid, string id, EntityUid? user, [NotNullWhen(true)] out EntityUid? item, ItemSlotsComponent? itemSlots = null, bool excludeUserAudio = false)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		item = null;
		if (!((EntitySystem)this).Resolve<ItemSlotsComponent>(uid, ref itemSlots, true))
		{
			return false;
		}
		if (!itemSlots.Slots.TryGetValue(id, out ItemSlot slot))
		{
			return false;
		}
		return TryEject(uid, slot, user, out item, excludeUserAudio);
	}

	public bool TryEjectToHands(EntityUid uid, ItemSlot slot, EntityUid? user, bool excludeUserAudio = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!TryEject(uid, slot, user, out var item, excludeUserAudio))
		{
			return false;
		}
		if (user.HasValue)
		{
			_handsSystem.PickupOrDrop(user.Value, item.Value);
		}
		return true;
	}

	private void AddAlternativeVerbs(EntityUid uid, ItemSlotsComponent itemSlots, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Expected O, but got Unknown
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Expected O, but got Unknown
		if (args.Hands == null || !args.CanAccess || !args.CanInteract)
		{
			return;
		}
		if (args.Using.HasValue && _actionBlockerSystem.CanDrop(args.User))
		{
			bool canInsertAny = false;
			foreach (ItemSlot slot in itemSlots.Slots.Values)
			{
				if (!slot.InsertOnInteract && CanInsert(uid, args.Using.Value, args.User, slot))
				{
					string verbSubject = ((slot.Name != string.Empty) ? base.Loc.GetString(slot.Name) : ((EntitySystem)this).Name(args.Using.Value, (MetaDataComponent)null));
					AlternativeVerb verb = new AlternativeVerb
					{
						IconEntity = ((EntitySystem)this).GetNetEntity(args.Using, (MetaDataComponent)null),
						Act = delegate
						{
							//IL_0011: Unknown result type (might be due to invalid IL or missing references)
							//IL_002c: Unknown result type (might be due to invalid IL or missing references)
							//IL_003c: Unknown result type (might be due to invalid IL or missing references)
							Insert(uid, slot, args.Using.Value, args.User, excludeUserAudio: true);
						}
					};
					if (slot.InsertVerbText != null)
					{
						verb.Text = base.Loc.GetString(slot.InsertVerbText);
						verb.Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/insert.svg.192dpi.png"));
					}
					else if (slot.EjectOnInteract)
					{
						verb.Text = base.Loc.GetString("place-item-verb-text", (ValueTuple<string, object>)("subject", verbSubject));
						verb.Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/drop.svg.192dpi.png"));
					}
					else
					{
						verb.Category = VerbCategory.Insert;
						verb.Text = verbSubject;
					}
					verb.Priority = slot.Priority;
					args.Verbs.Add(verb);
					canInsertAny = true;
				}
			}
			if (canInsertAny)
			{
				return;
			}
		}
		foreach (ItemSlot slot2 in itemSlots.Slots.Values)
		{
			if (!slot2.EjectOnInteract && !slot2.DisableEject && CanEject(uid, args.User, slot2) && _actionBlockerSystem.CanPickup(args.User, slot2.Item.Value))
			{
				string verbSubject2 = ((slot2.Name != string.Empty) ? base.Loc.GetString(slot2.Name) : (((EntitySystem)this).Comp<MetaDataComponent>(slot2.Item.Value).EntityName ?? string.Empty));
				AlternativeVerb verb2 = new AlternativeVerb
				{
					IconEntity = ((EntitySystem)this).GetNetEntity(slot2.Item, (MetaDataComponent)null),
					Act = delegate
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						//IL_0027: Unknown result type (might be due to invalid IL or missing references)
						TryEjectToHands(uid, slot2, args.User, excludeUserAudio: true);
					}
				};
				if (slot2.EjectVerbText == null)
				{
					verb2.Text = verbSubject2;
					verb2.Category = VerbCategory.Eject;
				}
				else
				{
					verb2.Text = base.Loc.GetString(slot2.EjectVerbText);
				}
				verb2.Priority = slot2.Priority;
				args.Verbs.Add(verb2);
			}
		}
	}

	private void AddInteractionVerbsVerbs(EntityUid uid, ItemSlotsComponent itemSlots, GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Expected O, but got Unknown
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Expected O, but got Unknown
		if (args.Hands == null || !args.CanAccess || !args.CanInteract)
		{
			return;
		}
		foreach (ItemSlot slot in itemSlots.Slots.Values)
		{
			if (slot.EjectOnInteract && CanEject(uid, args.User, slot) && _actionBlockerSystem.CanPickup(args.User, slot.Item.Value))
			{
				string verbSubject = ((slot.Name != string.Empty) ? base.Loc.GetString(slot.Name) : ((EntitySystem)this).Name(slot.Item.Value, (MetaDataComponent)null));
				InteractionVerb takeVerb = new InteractionVerb
				{
					IconEntity = ((EntitySystem)this).GetNetEntity(slot.Item, (MetaDataComponent)null),
					Act = delegate
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						//IL_0027: Unknown result type (might be due to invalid IL or missing references)
						TryEjectToHands(uid, slot, args.User, excludeUserAudio: true);
					}
				};
				if (slot.EjectVerbText == null)
				{
					takeVerb.Text = base.Loc.GetString("take-item-verb-text", (ValueTuple<string, object>)("subject", verbSubject));
				}
				else
				{
					takeVerb.Text = base.Loc.GetString(slot.EjectVerbText);
				}
				takeVerb.Priority = slot.Priority;
				args.Verbs.Add(takeVerb);
			}
		}
		if (!args.Using.HasValue || !_actionBlockerSystem.CanDrop(args.User))
		{
			return;
		}
		foreach (ItemSlot slot2 in itemSlots.Slots.Values)
		{
			if (slot2.InsertOnInteract && CanInsert(uid, args.Using.Value, args.User, slot2))
			{
				string verbSubject2 = ((slot2.Name != string.Empty) ? base.Loc.GetString(slot2.Name) : ((EntitySystem)this).Name(args.Using.Value, (MetaDataComponent)null));
				InteractionVerb insertVerb = new InteractionVerb
				{
					IconEntity = ((EntitySystem)this).GetNetEntity(args.Using, (MetaDataComponent)null),
					Act = delegate
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						//IL_002c: Unknown result type (might be due to invalid IL or missing references)
						//IL_003c: Unknown result type (might be due to invalid IL or missing references)
						Insert(uid, slot2, args.Using.Value, args.User, excludeUserAudio: true);
					}
				};
				if (slot2.InsertVerbText != null)
				{
					insertVerb.Text = base.Loc.GetString(slot2.InsertVerbText);
					insertVerb.Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/insert.svg.192dpi.png"));
				}
				else if (slot2.EjectOnInteract)
				{
					insertVerb.Text = base.Loc.GetString("place-item-verb-text", (ValueTuple<string, object>)("subject", verbSubject2));
					insertVerb.Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/drop.svg.192dpi.png"));
				}
				else
				{
					insertVerb.Category = VerbCategory.Insert;
					insertVerb.Text = verbSubject2;
				}
				insertVerb.Priority = slot2.Priority;
				args.Verbs.Add(insertVerb);
			}
		}
	}

	private void HandleButtonPressed(EntityUid uid, ItemSlotsComponent component, ItemSlotButtonPressedEvent args)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (component.Slots.TryGetValue(args.SlotId, out ItemSlot slot))
		{
			if (args.TryEject && slot.HasItem)
			{
				TryEjectToHands(uid, slot, ((BaseBoundUserInterfaceEvent)args).Actor, excludeUserAudio: true);
			}
			else if (args.TryInsert && !slot.HasItem)
			{
				TryInsertFromHand(uid, slot, ((BaseBoundUserInterfaceEvent)args).Actor);
			}
		}
	}

	private void OnBreak(EntityUid uid, ItemSlotsComponent component, EntityEventArgs args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		foreach (ItemSlot slot in component.Slots.Values)
		{
			if (slot.EjectOnBreak && slot.HasItem)
			{
				SetLock(uid, slot, locked: false, component);
				TryEject(uid, slot, null, out var _);
			}
		}
	}

	public EntityUid? GetItemOrNull(EntityUid uid, string id, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
		{
			return null;
		}
		return itemSlots.Slots.GetValueOrDefault(id)?.Item;
	}

	public void SetLock(EntityUid uid, string id, bool locked, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ItemSlotsComponent>(uid, ref itemSlots, true) && itemSlots.Slots.TryGetValue(id, out ItemSlot slot))
		{
			SetLock(uid, slot, locked, itemSlots);
		}
	}

	public void SetLock(EntityUid uid, ItemSlot slot, bool locked, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ItemSlotsComponent>(uid, ref itemSlots, true))
		{
			slot.Locked = locked;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)itemSlots, (MetaDataComponent)null);
		}
	}

	private void HandleItemSlotsState(EntityUid uid, ItemSlotsComponent component, ref ComponentHandleState args)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ComponentHandleState)(ref args)).Current is ItemSlotsComponentState state))
		{
			return;
		}
		string key;
		ItemSlot value;
		foreach (KeyValuePair<string, ItemSlot> slot3 in component.Slots)
		{
			slot3.Deconstruct(out key, out value);
			string key2 = key;
			ItemSlot slot = value;
			if (!state.Slots.ContainsKey(key2))
			{
				RemoveItemSlot(uid, slot, component);
			}
		}
		foreach (KeyValuePair<string, ItemSlot> slot4 in state.Slots)
		{
			slot4.Deconstruct(out key, out value);
			string serverKey = key;
			ItemSlot serverSlot = value;
			if (component.Slots.TryGetValue(serverKey, out ItemSlot itemSlot))
			{
				itemSlot.CopyFrom(serverSlot);
				itemSlot.ContainerSlot = _containers.EnsureContainer<ContainerSlot>(uid, serverKey, (ContainerManagerComponent)null);
			}
			else
			{
				ItemSlot slot2 = new ItemSlot(serverSlot);
				slot2.Local = false;
				AddItemSlot(uid, serverKey, slot2);
			}
		}
	}

	private void GetItemSlotsState(EntityUid uid, ItemSlotsComponent component, ref ComponentGetState args)
	{
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new ItemSlotsComponentState(component.Slots);
	}

	private void InitializeLock()
	{
		((EntitySystem)this).SubscribeLocalEvent<ItemSlotsLockComponent, MapInitEvent>((EntityEventRefHandler<ItemSlotsLockComponent, MapInitEvent>)OnLockMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemSlotsLockComponent, LockToggledEvent>((EntityEventRefHandler<ItemSlotsLockComponent, LockToggledEvent>)OnLockToggled, (Type[])null, (Type[])null);
	}

	private void OnLockMapInit(Entity<ItemSlotsLockComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		LockComponent lockComp = default(LockComponent);
		if (((EntitySystem)this).TryComp<LockComponent>(ent.Owner, ref lockComp))
		{
			UpdateLocks(ent, lockComp.Locked);
		}
	}

	private void OnLockToggled(Entity<ItemSlotsLockComponent> ent, ref LockToggledEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateLocks(ent, args.Locked);
	}

	private void UpdateLocks(Entity<ItemSlotsLockComponent> ent, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		foreach (string slot in ent.Comp.Slots)
		{
			if (TryGetSlot(ent.Owner, slot, out ItemSlot itemSlot))
			{
				SetLock(ent.Owner, itemSlot, value);
			}
		}
	}
}
