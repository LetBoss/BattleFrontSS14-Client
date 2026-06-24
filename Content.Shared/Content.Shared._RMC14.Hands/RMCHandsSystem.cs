using System;
using System.Linq;
using Content.Shared._RMC14.Storage;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Hands;

public abstract class RMCHandsSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private RMCStorageSystem _rmcStorage;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private SharedStorageSystem _storage;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<GiveHandsComponent, MapInitEvent>((EntityEventRefHandler<GiveHandsComponent, MapInitEvent>)OnXenoHandsMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WhitelistPickupByComponent, GettingPickedUpAttemptEvent>((EntityEventRefHandler<WhitelistPickupByComponent, GettingPickedUpAttemptEvent>)OnWhitelistGettingPickedUpAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CartridgeAmmoComponent, GettingPickedUpAttemptEvent>((EntityEventRefHandler<CartridgeAmmoComponent, GettingPickedUpAttemptEvent>)OnSpentCasingPickup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WhitelistPickupComponent, PickupAttemptEvent>((EntityEventRefHandler<WhitelistPickupComponent, PickupAttemptEvent>)OnWhitelistPickUpAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropHeldOnIncapacitateComponent, MobStateChangedEvent>((EntityEventRefHandler<DropHeldOnIncapacitateComponent, MobStateChangedEvent>)OnDropMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCStorageEjectHandComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<RMCStorageEjectHandComponent, GetVerbsEvent<AlternativeVerb>>)OnStorageEjectHandVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropOnUseInHandComponent, UseInHandEvent>((EntityEventRefHandler<DropOnUseInHandComponent, UseInHandEvent>)OnDropOnUseInHand, (Type[])null, (Type[])null);
	}

	private void OnXenoHandsMapInit(Entity<GiveHandsComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		foreach (GivenHand hand in ent.Comp.Hands)
		{
			_hands.AddHand(Entity<HandsComponent>.op_Implicit(ent.Owner), hand.Name, hand.Location);
		}
	}

	private void OnWhitelistGettingPickedUpAttempt(Entity<WhitelistPickupByComponent> ent, ref GettingPickedUpAttemptEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && !_whitelist.IsValid(ent.Comp.Whitelist, args.User))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnSpentCasingPickup(Entity<CartridgeAmmoComponent> ent, ref GettingPickedUpAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && ent.Comp.Spent)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnWhitelistPickUpAttempt(Entity<WhitelistPickupComponent> ent, ref PickupAttemptEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled)
		{
			if (!_whitelist.IsValid(ent.Comp.Whitelist, args.Item))
			{
				((CancellableEntityEventArgs)args).Cancel();
			}
			else if (!ent.Comp.AllowDead && _mobState.IsDead(args.Item))
			{
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
	}

	private void OnDropMobStateChanged(Entity<DropHeldOnIncapacitateComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent handsComp = default(HandsComponent);
		if (args.OldMobState != MobState.Alive || (int)args.NewMobState <= 1 || !((EntitySystem)this).TryComp<HandsComponent>(Entity<DropHeldOnIncapacitateComponent>.op_Implicit(ent), ref handsComp))
		{
			return;
		}
		foreach (string hand in handsComp.Hands.Keys)
		{
			_hands.TryDrop(Entity<HandsComponent>.op_Implicit((Entity<DropHeldOnIncapacitateComponent>.op_Implicit(ent), handsComp)), hand, null, checkActionBlocker: false);
		}
	}

	private void OnStorageEjectHandVerbs(Entity<RMCStorageEjectHandComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanInteract)
		{
			return;
		}
		EntityUid user = args.User;
		if (!ent.Comp.CanToggleStorage || _container.GetContainingContainers(Entity<TransformComponent>.op_Implicit(ent.Owner)).All((BaseContainer c) => c.Owner != user))
		{
			return;
		}
		AlternativeVerb switchStorageVerb = new AlternativeVerb
		{
			Text = base.Loc.GetString("rmc-storage-hand-switch"),
			Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/flip.svg.192dpi.png")),
			Priority = -2,
			Act = delegate
			{
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				ent.Comp.State = GetNextState(ent.Comp.State);
				((EntitySystem)this).Dirty<RMCStorageEjectHandComponent>(ent, (MetaDataComponent)null);
				string text = ent.Comp.State switch
				{
					RMCStorageEjectState.Last => "rmc-storage-hand-eject-last-item", 
					RMCStorageEjectState.First => "rmc-storage-hand-eject-first-item", 
					RMCStorageEjectState.Unequip => "rmc-storage-hand-eject-unequips", 
					RMCStorageEjectState.Open => "rmc-storage-hand-eject-open", 
					_ => string.Empty, 
				};
				_popup.PopupClient(base.Loc.GetString(text, (ValueTuple<string, object>)("storage", ent.Owner)), user, user, PopupType.Medium);
			}
		};
		args.Verbs.Add(switchStorageVerb);
		BaseContainer containing = default(BaseContainer);
		if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(Entity<RMCStorageEjectHandComponent>.op_Implicit(ent), null)), ref containing) || containing.Owner != user || !_inventory.TryGetContainingSlot(Entity<TransformComponent, MetaDataComponent>.op_Implicit(ent.Owner), out SlotDefinition slot))
		{
			return;
		}
		AlternativeVerb unequipVerb = new AlternativeVerb
		{
			Text = "Unequip",
			Act = delegate
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				if (_inventory.TryGetContainingSlot(Entity<TransformComponent, MetaDataComponent>.op_Implicit(ent.Owner), out slot) && _inventory.TryUnequip(user, user, slot.Name, silent: false, force: false, predicted: false, null, null, reparent: true, checkDoafter: true))
				{
					_hands.TryPickupAnyHand(user, ent.Owner);
				}
			}
		};
		args.Verbs.Add(unequipVerb);
	}

	private static RMCStorageEjectState GetNextState(RMCStorageEjectState current)
	{
		return (RMCStorageEjectState)((int)(current + 1) % Enum.GetValues<RMCStorageEjectState>().Length);
	}

	private void OnDropOnUseInHand(Entity<DropOnUseInHandComponent> ent, ref UseInHandEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_hands.TryDrop(Entity<HandsComponent>.op_Implicit(args.User), Entity<DropOnUseInHandComponent>.op_Implicit(ent));
	}

	public bool IsPickupByAllowed(Entity<WhitelistPickupByComponent?> item, Entity<WhitelistPickupComponent?> user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Resolve<WhitelistPickupByComponent>(Entity<WhitelistPickupByComponent>.op_Implicit(item), ref item.Comp, false);
		((EntitySystem)this).Resolve<WhitelistPickupComponent>(Entity<WhitelistPickupComponent>.op_Implicit(user), ref user.Comp, false);
		if (item.Comp != null && !_whitelist.IsValid(item.Comp.Whitelist, Entity<WhitelistPickupComponent>.op_Implicit(user)))
		{
			return false;
		}
		if (user.Comp != null && !_whitelist.IsValid(user.Comp.Whitelist, item.Owner))
		{
			return false;
		}
		WhitelistPickupComponent comp = user.Comp;
		if (comp != null && !comp.AllowDead && _mobState.IsDead(Entity<WhitelistPickupByComponent>.op_Implicit(item)))
		{
			return false;
		}
		return true;
	}

	public bool TryGetHolder(EntityUid item, out EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		user = default(EntityUid);
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(item, null)), ref container))
		{
			return false;
		}
		if (!_hands.IsHolding(Entity<HandsComponent>.op_Implicit(container.Owner), item))
		{
			return false;
		}
		user = container.Owner;
		return true;
	}

	public bool TryGetNestedStorageParent(EntityUid item, out EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		user = default(EntityUid);
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(item, null)), ref container))
		{
			return false;
		}
		StorageComponent storage = default(StorageComponent);
		if (!((EntitySystem)this).TryComp<StorageComponent>(container.Owner, ref storage) || !storage.StoredItems.ContainsKey(item))
		{
			return false;
		}
		user = container.Owner;
		return true;
	}

	public bool TryStorageEjectHand(EntityUid user, string handName)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (_hands.TryGetHand(Entity<HandsComponent>.op_Implicit(user), handName, out var _))
		{
			EntityUid? heldItem = _hands.GetHeldItem(Entity<HandsComponent>.op_Implicit(user), handName);
			if (heldItem.HasValue)
			{
				EntityUid held = heldItem.GetValueOrDefault();
				if (((EntitySystem)this).HasComp<InteractionActivateOnClickComponent>(held) && _interaction.InteractionActivate(user, held))
				{
					return true;
				}
				return TryStorageEjectHand(user, held);
			}
		}
		return false;
	}

	public bool TryStorageEjectHand(EntityUid user, EntityUid item)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		RMCStorageEjectHandItemEvent ev = new RMCStorageEjectHandItemEvent(user);
		((EntitySystem)this).RaiseLocalEvent<RMCStorageEjectHandItemEvent>(item, ref ev, false);
		if (ev.Handled)
		{
			return true;
		}
		RMCStorageEjectHandComponent eject = default(RMCStorageEjectHandComponent);
		StorageComponent storage = default(StorageComponent);
		if (!((EntitySystem)this).TryComp<RMCStorageEjectHandComponent>(item, ref eject) || !((EntitySystem)this).TryComp<StorageComponent>(item, ref storage))
		{
			return false;
		}
		if (eject.NestedWhitelist != null && (!TryGetNestedStorageParent(item, out var parent) || !_whitelist.IsWhitelistPass(eject.NestedWhitelist, parent)))
		{
			return false;
		}
		switch (eject.State)
		{
		case RMCStorageEjectState.Unequip:
			return false;
		case RMCStorageEjectState.Open:
			_storage.OpenStorageUI(item, user, storage, silent: false);
			return true;
		default:
		{
			if (!_rmcStorage.CanEject(item, user, out var popup))
			{
				_popup.PopupClient(LocId.op_Implicit(popup), user, user, PopupType.SmallCaution);
				return false;
			}
			if (eject.Whitelist != null)
			{
				foreach (EntityUid contained in ((BaseContainer)storage.Container).ContainedEntities)
				{
					if (_whitelist.IsWhitelistPass(eject.Whitelist, contained))
					{
						_hands.TryPickupAnyHand(user, contained);
						return true;
					}
				}
			}
			EntityUid? pickUpItem = null;
			switch (eject.State)
			{
			case RMCStorageEjectState.Last:
			{
				if (_rmcStorage.TryGetLastItem(Entity<StorageComponent>.op_Implicit((item, storage)), out var last))
				{
					pickUpItem = last;
					break;
				}
				if (eject.EjectWhenEmpty)
				{
					return false;
				}
				_popup.PopupClient(base.Loc.GetString("rmc-storage-nothing-left", (ValueTuple<string, object>)("storage", item)), user, user);
				return true;
			}
			case RMCStorageEjectState.First:
			{
				if (_rmcStorage.TryGetFirstItem(Entity<StorageComponent>.op_Implicit((item, storage)), out var first))
				{
					pickUpItem = first;
					break;
				}
				if (eject.EjectWhenEmpty)
				{
					return false;
				}
				_popup.PopupClient(base.Loc.GetString("rmc-storage-nothing-left", (ValueTuple<string, object>)("storage", item)), user, user);
				return true;
			}
			}
			if (!pickUpItem.HasValue)
			{
				return false;
			}
			_hands.TryPickupAnyHand(user, pickUpItem.Value);
			return true;
		}
		}
	}

	public virtual void ThrowHeldItem(EntityUid player, EntityCoordinates coordinates, float minDistance = 0.1f)
	{
	}
}
