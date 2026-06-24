using System;
using System.Collections.Generic;
using Content.Shared._RMC14.CrashLand;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Prototypes;
using Content.Shared._RMC14.Storage.Containers;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Lock;
using Content.Shared.ParaDrop;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Stunnable;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Storage;

public sealed class RMCStorageSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedCrashLandSystem _crashLand;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedEntityStorageSystem _entityStorage;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedItemSystem _item;

	[Dependency]
	private LockSystem _lock;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SharedStorageSystem _storage;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	private readonly List<EntityUid> _toRemove = new List<EntityUid>();

	private readonly List<EntityUid> _toClose = new List<EntityUid>();

	private EntityQuery<ItemComponent> _itemQuery;

	private EntityQuery<StorageComponent> _storageQuery;

	private readonly TimeSpan _stunStorage = TimeSpan.FromSeconds(4L);

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_itemQuery = ((EntitySystem)this).GetEntityQuery<ItemComponent>();
		_storageQuery = ((EntitySystem)this).GetEntityQuery<StorageComponent>();
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, CMStorageItemFillEvent>((EntityEventRefHandler<StorageComponent, CMStorageItemFillEvent>)OnStorageFillItem, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageOpenDoAfterComponent, OpenStorageDoAfterEvent>((EntityEventRefHandler<StorageOpenDoAfterComponent, OpenStorageDoAfterEvent>)OnStorageOpenDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageSkillRequiredComponent, StorageInteractAttemptEvent>((EntityEventRefHandler<StorageSkillRequiredComponent, StorageInteractAttemptEvent>)OnStorageSkillOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageSkillRequiredComponent, DumpableDoAfterEvent>((EntityEventRefHandler<StorageSkillRequiredComponent, DumpableDoAfterEvent>)OnDumpableDoAfter, new Type[1] { typeof(DumpableSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageCloseOnMoveComponent, GotEquippedEvent>((EntityEventRefHandler<StorageCloseOnMoveComponent, GotEquippedEvent>)OnStorageEquip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockEntityStorageComponent, InsertIntoEntityStorageAttemptEvent>((EntityEventRefHandler<BlockEntityStorageComponent, InsertIntoEntityStorageAttemptEvent>)OnBlockInsertIntoEntityStorageAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MarineComponent, EntGotRemovedFromContainerMessage>((EntityEventRefHandler<MarineComponent, EntGotRemovedFromContainerMessage>)OnRemovedMarineFromContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageNestedOpenSkillRequiredComponent, StorageInteractAttemptEvent>((EntityEventRefHandler<StorageNestedOpenSkillRequiredComponent, StorageInteractAttemptEvent>)OnNestedSkillRequiredInteractAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SkyFallingComponent, StorageInteractAttemptEvent>((EntityEventRefHandler<SkyFallingComponent, StorageInteractAttemptEvent>)OnSkyFalling, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CrashLandingComponent, StorageInteractAttemptEvent>((EntityEventRefHandler<CrashLandingComponent, StorageInteractAttemptEvent>)OnCrashLanding, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParaDroppingComponent, StorageInteractAttemptEvent>((EntityEventRefHandler<ParaDroppingComponent, StorageInteractAttemptEvent>)OnParaDropping, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCEntityStorageWhitelistComponent, ContainerIsInsertingAttemptEvent>((EntityEventRefHandler<RMCEntityStorageWhitelistComponent, ContainerIsInsertingAttemptEvent>)OnEntityStorageWhitelistAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityStorageCloseOnMapInitComponent, MapInitEvent>((EntityEventRefHandler<EntityStorageCloseOnMapInitComponent, MapInitEvent>)OnEntityStorageClose, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCContainerEmptyOnDestructionComponent, DestructionEventArgs>((EntityEventRefHandler<RMCContainerEmptyOnDestructionComponent, DestructionEventArgs>)OnContainerEmptyDestroyed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCContainerEmptyOnDestructionComponent, EntityTerminatingEvent>((EntityEventRefHandler<RMCContainerEmptyOnDestructionComponent, EntityTerminatingEvent>)OnContainerEmptyDeleted, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<StorageCloseOnMoveComponent>(((EntitySystem)this).Subs, (object)StorageComponent.StorageUiKey.Key, (BuiEventSubscriber<StorageCloseOnMoveComponent>)delegate(Subscriber<StorageCloseOnMoveComponent> subs)
		{
			subs.Event<BoundUIOpenedEvent>((EntityEventRefHandler<StorageCloseOnMoveComponent, BoundUIOpenedEvent>)OnCloseOnMoveUIOpened);
		});
		BoundUserInterfaceRegisterExt.BuiEvents<StorageOpenComponent>(((EntitySystem)this).Subs, (object)StorageComponent.StorageUiKey.Key, (BuiEventSubscriber<StorageOpenComponent>)delegate(Subscriber<StorageOpenComponent> subs)
		{
			subs.Event<BoundUIClosedEvent>((EntityEventRefHandler<StorageOpenComponent, BoundUIClosedEvent>)OnCloseOnMoveUIClosed);
		});
		((EntitySystem)this).UpdatesAfter.Add(typeof(SharedStorageSystem));
	}

	private void OnDumpableDoAfter(Entity<StorageSkillRequiredComponent> ent, ref DumpableDoAfterEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && !args.Cancelled && TryCancel(args.User, ent))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnStorageFillItem(Entity<StorageComponent> storage, ref CMStorageItemFillEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		int tries = 0;
		string reason;
		Box2i expanded = default(Box2i);
		while (!_storage.CanInsert(Entity<StorageComponent>.op_Implicit(storage), Entity<ItemComponent>.op_Implicit(args.Item), null, out reason, null, null, ignoreStacks: true) && reason == "comp-storage-insufficient-capacity" && tries < 3)
		{
			tries++;
			if (CMPrototypeExtensions.FilterCM)
			{
				((EntitySystem)this).Log.Warning($"Storage {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<StorageComponent>.op_Implicit(storage), (MetaDataComponent)null)} can't fit {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ItemComponent>.op_Implicit(args.Item), (MetaDataComponent)null)}");
			}
			foreach (Box2i item in _item.GetItemShape(Entity<StorageComponent>.op_Implicit((Entity<StorageComponent>.op_Implicit(storage), args.Storage)), Entity<ItemComponent>.op_Implicit((Entity<ItemComponent>.op_Implicit(args.Item), Entity<ItemComponent>.op_Implicit(args.Item)))))
			{
				Box2i shape = item;
				List<Box2i> grid = args.Storage.Grid;
				if (grid.Count == 0)
				{
					grid.Add(shape);
					continue;
				}
				Box2i last = grid[grid.Count - 1];
				((Box2i)(ref expanded))._002Ector(last.Left, last.Bottom, last.Right + ((Box2i)(ref shape)).Width + 1, last.Top);
				if (expanded.Top < shape.Top)
				{
					expanded.Top = shape.Top;
				}
				grid[grid.Count - 1] = expanded;
			}
		}
	}

	public bool IgnoreItemSize(Entity<StorageComponent> storage, EntityUid item)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		IgnoreContentsSizeComponent ignore = default(IgnoreContentsSizeComponent);
		if (((EntitySystem)this).TryComp<IgnoreContentsSizeComponent>(Entity<StorageComponent>.op_Implicit(storage), ref ignore))
		{
			return _entityWhitelist.IsValid(ignore.Items, item);
		}
		return false;
	}

	public bool OpenDoAfter(EntityUid uid, EntityUid entity, StorageComponent? storageComp = null, bool silent = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		StorageOpenDoAfterComponent comp = default(StorageOpenDoAfterComponent);
		if (!((EntitySystem)this).TryComp<StorageOpenDoAfterComponent>(uid, ref comp) || comp.Duration == TimeSpan.Zero)
		{
			return false;
		}
		if (comp.SkipInHand && _hands.IsHolding(Entity<HandsComponent>.op_Implicit(entity), uid))
		{
			return false;
		}
		if (comp.SkipOnGround && !_inventory.TryGetContainingSlot(Entity<TransformComponent, MetaDataComponent>.op_Implicit(uid), out SlotDefinition _))
		{
			return false;
		}
		OpenStorageDoAfterEvent ev = new OpenStorageDoAfterEvent(((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null), ((EntitySystem)this).GetNetEntity(entity, (MetaDataComponent)null), silent);
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, entity, comp.Duration, ev, uid)
		{
			BreakOnMove = true
		};
		_doAfter.TryStartDoAfter(doAfter);
		return true;
	}

	private void OnStorageOpenDoAfter(Entity<StorageOpenDoAfterComponent> ent, ref OpenStorageDoAfterEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? uid = default(EntityUid?);
		EntityUid? entity = default(EntityUid?);
		StorageComponent storage = default(StorageComponent);
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryGetEntity(args.Uid, ref uid) && ((EntitySystem)this).TryGetEntity(args.Entity, ref entity) && ((EntitySystem)this).TryComp<StorageComponent>(uid, ref storage))
		{
			((HandledEntityEventArgs)args).Handled = true;
			_storage.OpenStorageUI(uid.Value, entity.Value, storage, args.Silent, doAfter: false);
		}
	}

	private void OnStorageSkillOpenAttempt(Entity<StorageSkillRequiredComponent> ent, ref StorageInteractAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && TryCancel(args.User, ent))
		{
			args.Cancelled = true;
		}
	}

	private void OnStorageEquip(Entity<StorageCloseOnMoveComponent> ent, ref GotEquippedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)StorageComponent.StorageUiKey.Key, (EntityUid?)args.Equipee, false);
		StorageOpenComponent comp = default(StorageOpenComponent);
		if (((EntitySystem)this).TryComp<StorageOpenComponent>(Entity<StorageCloseOnMoveComponent>.op_Implicit(ent), ref comp))
		{
			comp.OpenedAt.Remove(args.Equipee);
		}
	}

	private void OnBlockInsertIntoEntityStorageAttempt(Entity<BlockEntityStorageComponent> ent, ref InsertIntoEntityStorageAttemptEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (_entityWhitelist.IsWhitelistPassOrNull(ent.Comp.Whitelist, args.Container))
		{
			args.Cancelled = true;
		}
	}

	private void OnRemovedMarineFromContainer(Entity<MarineComponent> ent, ref EntGotRemovedFromContainerMessage args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.ApplyingState || ((EntitySystem)this).TerminatingOrDeleted(Entity<MarineComponent>.op_Implicit(ent), (MetaDataComponent)null))
		{
			return;
		}
		if (!((EntitySystem)this).HasComp<NoStunOnExitComponent>(((ContainerModifiedMessage)args).Container.Owner))
		{
			_stun.TryStun(Entity<MarineComponent>.op_Implicit(ent), _stunStorage, refresh: true);
		}
		if (((EntitySystem)this).HasComp<SkyFallingComponent>(((ContainerModifiedMessage)args).Container.Owner) || ((EntitySystem)this).HasComp<CrashLandingComponent>(((ContainerModifiedMessage)args).Container.Owner))
		{
			AttemptCrashLandEvent ev = default(AttemptCrashLandEvent);
			((EntitySystem)this).RaiseLocalEvent<AttemptCrashLandEvent>(Entity<MarineComponent>.op_Implicit(ent), ref ev, false);
			if (!ev.Cancelled)
			{
				_crashLand.TryCrashLand(Entity<CrashLandableComponent>.op_Implicit(ent.Owner), doDamage: true);
			}
		}
	}

	private void OnNestedSkillRequiredInteractAttempt(Entity<StorageNestedOpenSkillRequiredComponent> ent, ref StorageInteractAttemptEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		StorageComponent parentStorage = default(StorageComponent);
		if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(Entity<StorageNestedOpenSkillRequiredComponent>.op_Implicit(ent), null)), ref container) && ((EntitySystem)this).TryComp<StorageComponent>(container.Owner, ref parentStorage) && parentStorage.StoredItems.ContainsKey(Entity<StorageNestedOpenSkillRequiredComponent>.op_Implicit(ent)) && !_skills.HasSkills(Entity<SkillsComponent>.op_Implicit(args.User), ent.Comp.Skills))
		{
			args.Cancelled = true;
			if (!args.Silent)
			{
				string msg = base.Loc.GetString("rmc-storage-nested-unable", (ValueTuple<string, object>)("nested", ent), (ValueTuple<string, object>)("parent", container.Owner));
				_popup.PopupClient(msg, Entity<StorageNestedOpenSkillRequiredComponent>.op_Implicit(ent), args.User, PopupType.SmallCaution);
			}
		}
	}

	private void OnEntityStorageWhitelistAttempt(Entity<RMCEntityStorageWhitelistComponent> ent, ref ContainerIsInsertingAttemptEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && !_entityWhitelist.IsWhitelistPass(ent.Comp.Whitelist, ((ContainerAttemptEventBase)args).EntityUid))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnEntityStorageClose(Entity<EntityStorageCloseOnMapInitComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_toClose.Add(Entity<EntityStorageCloseOnMapInitComponent>.op_Implicit(ent));
	}

	private void OnCloseOnMoveUIOpened(Entity<StorageCloseOnMoveComponent> ent, ref BoundUIOpenedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && (!ent.Comp.SkipInHand || !_hands.IsHolding(Entity<HandsComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor), Entity<StorageCloseOnMoveComponent>.op_Implicit(ent))))
		{
			EntityUid user = ((BaseBoundUserInterfaceEvent)args).Actor;
			NetCoordinates coordinates = ((EntitySystem)this).GetNetCoordinates(_transform.GetMoverCoordinates(user), (MetaDataComponent)null);
			((EntitySystem)this).EnsureComp<StorageOpenComponent>(Entity<StorageCloseOnMoveComponent>.op_Implicit(ent)).OpenedAt[user] = coordinates;
		}
	}

	private void OnCloseOnMoveUIClosed(Entity<StorageOpenComponent> ent, ref BoundUIClosedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.OpenedAt.Remove(((BaseBoundUserInterfaceEvent)args).Actor);
	}

	private void OnSkyFalling(Entity<SkyFallingComponent> ent, ref StorageInteractAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnParaDropping(Entity<ParaDroppingComponent> ent, ref StorageInteractAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnCrashLanding(Entity<CrashLandingComponent> ent, ref StorageInteractAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private bool TryCancel(EntityUid user, Entity<StorageSkillRequiredComponent> storage)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!_skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(user), storage.Comp.Skills))
		{
			_popup.PopupClient(base.Loc.GetString("cm-storage-unskilled"), Entity<StorageSkillRequiredComponent>.op_Implicit(storage), user, PopupType.SmallCaution);
			return true;
		}
		return false;
	}

	private bool CanInsertStorageLimit(Entity<StorageComponent?, LimitedStorageComponent?> limited, EntityUid toInsert, out LocId popup)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		popup = default(LocId);
		if (!((EntitySystem)this).Resolve<LimitedStorageComponent>(Entity<StorageComponent, LimitedStorageComponent>.op_Implicit(limited), ref limited.Comp2, false) || !_storageQuery.Resolve(Entity<StorageComponent, LimitedStorageComponent>.op_Implicit(limited), ref limited.Comp1, false))
		{
			return true;
		}
		foreach (LimitedStorageComponent.Limit limit in limited.Comp2.Limits)
		{
			if (!_entityWhitelist.IsWhitelistPassOrNull(limit.Whitelist, toInsert) || _entityWhitelist.IsBlacklistPass(limit.Blacklist, toInsert))
			{
				continue;
			}
			int storedCount = 0;
			foreach (EntityUid stored in limited.Comp1.StoredItems.Keys)
			{
				if (!(stored == toInsert) && _entityWhitelist.IsWhitelistPassOrNull(limit.Whitelist, stored) && !_entityWhitelist.IsBlacklistPass(limit.Blacklist, stored))
				{
					storedCount++;
					if (storedCount >= limit.Count)
					{
						break;
					}
				}
			}
			if (storedCount >= limit.Count)
			{
				popup = ((limit.Popup == default(LocId)) ? LocId.op_Implicit("rmc-storage-limit-cant-fit") : limit.Popup);
				return false;
			}
		}
		return true;
	}

	public bool CanInsertStoreSkill(Entity<StorageComponent?, StorageStoreSkillRequiredComponent?> store, EntityUid toInsert, EntityUid? user, out LocId popup)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		popup = default(LocId);
		if (!user.HasValue)
		{
			return true;
		}
		if (!((EntitySystem)this).Resolve<StorageStoreSkillRequiredComponent>(Entity<StorageComponent, StorageStoreSkillRequiredComponent>.op_Implicit(store), ref store.Comp2, false) || !_storageQuery.Resolve(Entity<StorageComponent, StorageStoreSkillRequiredComponent>.op_Implicit(store), ref store.Comp1, false))
		{
			return true;
		}
		foreach (StorageStoreSkillRequiredComponent.Entry entry in store.Comp2.Entries)
		{
			if (!_entityWhitelist.IsWhitelistFail(entry.Whitelist, toInsert) && !_skills.HasSkills(Entity<SkillsComponent>.op_Implicit(user.Value), entry.Skills))
			{
				popup = LocId.op_Implicit("rmc-storage-store-skill-unable");
				return false;
			}
		}
		return true;
	}

	private bool CanEjectStoreSkill(Entity<StorageComponent?, StorageSkillRequiredComponent?> store, EntityUid? user, out LocId popup)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		popup = default(LocId);
		if (!user.HasValue)
		{
			return true;
		}
		if (!((EntitySystem)this).Resolve<StorageSkillRequiredComponent>(Entity<StorageComponent, StorageSkillRequiredComponent>.op_Implicit(store), ref store.Comp2, false) || !_storageQuery.Resolve(Entity<StorageComponent, StorageSkillRequiredComponent>.op_Implicit(store), ref store.Comp1, false))
		{
			return true;
		}
		if (!_skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(user.Value), store.Comp2.Skills))
		{
			popup = LocId.op_Implicit(base.Loc.GetString("cm-storage-unskilled"));
			return false;
		}
		return true;
	}

	public bool TryGetLastItem(Entity<StorageComponent?> storage, out EntityUid item)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		item = default(EntityUid);
		if (!((EntitySystem)this).Resolve<StorageComponent>(Entity<StorageComponent>.op_Implicit(storage), ref storage.Comp, false))
		{
			return false;
		}
		ItemStorageLocation? lastLocation = null;
		foreach (var (stored, location) in storage.Comp.StoredItems)
		{
			if (lastLocation.HasValue)
			{
				ItemStorageLocation last = lastLocation.GetValueOrDefault();
				if (last.Position.Y >= location.Position.Y)
				{
					if (last.Position.Y == location.Position.Y && last.Position.X > location.Position.X)
					{
						item = stored;
						lastLocation = location;
					}
					continue;
				}
			}
			item = stored;
			lastLocation = location;
		}
		return item != default(EntityUid);
	}

	public bool TryGetFirstItem(Entity<StorageComponent?> storage, out EntityUid item)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		item = default(EntityUid);
		if (!((EntitySystem)this).Resolve<StorageComponent>(Entity<StorageComponent>.op_Implicit(storage), ref storage.Comp, false))
		{
			return false;
		}
		ItemStorageLocation? firstLocation = null;
		foreach (var (stored, location) in storage.Comp.StoredItems)
		{
			if (firstLocation.HasValue)
			{
				ItemStorageLocation first = firstLocation.GetValueOrDefault();
				if (first.Position.Y <= location.Position.Y)
				{
					if (first.Position.Y == location.Position.Y && first.Position.X < location.Position.X)
					{
						item = stored;
						firstLocation = location;
					}
					continue;
				}
			}
			item = stored;
			firstLocation = location;
		}
		return item != default(EntityUid);
	}

	public bool CanInsert(Entity<StorageComponent?> storage, EntityUid toInsert, EntityUid? user, out LocId popup)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!CanInsertStorageLimit(Entity<StorageComponent, LimitedStorageComponent>.op_Implicit((ValueTuple<EntityUid, StorageComponent, LimitedStorageComponent>)(Entity<StorageComponent>.op_Implicit(storage), Entity<StorageComponent>.op_Implicit(storage), null)), toInsert, out popup))
		{
			return false;
		}
		if (!CanInsertStoreSkill(Entity<StorageComponent, StorageStoreSkillRequiredComponent>.op_Implicit((ValueTuple<EntityUid, StorageComponent, StorageStoreSkillRequiredComponent>)(Entity<StorageComponent>.op_Implicit(storage), Entity<StorageComponent>.op_Implicit(storage), null)), toInsert, user, out popup))
		{
			return false;
		}
		return true;
	}

	public bool CanEject(EntityUid storage, EntityUid user, out LocId popup)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (!CanEjectStoreSkill(Entity<StorageComponent, StorageSkillRequiredComponent>.op_Implicit(storage), user, out popup))
		{
			return false;
		}
		return true;
	}

	private void OnContainerEmptyDestroyed(Entity<RMCContainerEmptyOnDestructionComponent> containerEnt, ref DestructionEventArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (containerEnt.Comp.OnDestruction)
		{
			ContainerDestructionEmpty(containerEnt);
		}
	}

	private void OnContainerEmptyDeleted(Entity<RMCContainerEmptyOnDestructionComponent> containerEnt, ref EntityTerminatingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (containerEnt.Comp.OnDelete)
		{
			ContainerDestructionEmpty(containerEnt);
		}
	}

	private unsafe void ContainerDestructionEmpty(Entity<RMCContainerEmptyOnDestructionComponent> containerEnt)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent transform = default(TransformComponent);
		ContainerManagerComponent containerManager = default(ContainerManagerComponent);
		if (!((EntitySystem)this).TryComp(Entity<RMCContainerEmptyOnDestructionComponent>.op_Implicit(containerEnt), ref transform) || ((EntitySystem)this).TerminatingOrDeleted(transform.GridUid, (MetaDataComponent)null) || !((EntitySystem)this).Exists(Entity<RMCContainerEmptyOnDestructionComponent>.op_Implicit(containerEnt)) || !((EntitySystem)this).TryComp<ContainerManagerComponent>(Entity<RMCContainerEmptyOnDestructionComponent>.op_Implicit(containerEnt), ref containerManager))
		{
			return;
		}
		RMCContainerDestructionEmptyEvent ev = default(RMCContainerDestructionEmptyEvent);
		((EntitySystem)this).RaiseLocalEvent<RMCContainerDestructionEmptyEvent>(Entity<RMCContainerEmptyOnDestructionComponent>.op_Implicit(containerEnt), ref ev, false);
		if (ev.Handled)
		{
			return;
		}
		AllContainersEnumerable containers = _container.GetAllContainers(Entity<RMCContainerEmptyOnDestructionComponent>.op_Implicit(containerEnt), containerManager);
		AllContainersEnumerator enumerator = ((AllContainersEnumerable)(ref containers)).GetEnumerator();
		try
		{
			while (((AllContainersEnumerator)(ref enumerator)).MoveNext())
			{
				BaseContainer contain = ((AllContainersEnumerator)(ref enumerator)).Current;
				_container.EmptyContainer(contain, false, (EntityCoordinates?)null, true);
			}
		}
		finally
		{
			((IDisposable)(*(AllContainersEnumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public int EstimateFreeColumns(Entity<StorageComponent?> storage)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StorageComponent>(Entity<StorageComponent>.op_Implicit(storage), ref storage.Comp, false))
		{
			return 0;
		}
		int columns = 0;
		foreach (Box2i item2 in storage.Comp.Grid)
		{
			Box2i grid = item2;
			columns += (((Box2i)(ref grid)).Width + 1) * ((Box2i)(ref grid)).Height;
		}
		ItemComponent itemComp = default(ItemComponent);
		foreach (var (item, _) in storage.Comp.StoredItems)
		{
			if (_itemQuery.TryComp(item, ref itemComp))
			{
				IReadOnlyList<Box2i> shapes = _item.GetItemShape(storage, Entity<ItemComponent>.op_Implicit((item, itemComp)));
				if (shapes.Count != 0)
				{
					Box2i shape = shapes[0];
					columns -= ((Box2i)(ref shape)).Width;
				}
			}
		}
		return columns;
	}

	public override void Update(float frameTime)
	{
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (_net.IsServer)
			{
				foreach (EntityUid toClose in _toClose)
				{
					bool num = _lock.IsLocked(Entity<LockComponent>.op_Implicit(toClose));
					if (num)
					{
						_lock.Unlock(toClose, null);
					}
					_entityStorage.OpenStorage(toClose);
					_entityStorage.CloseStorage(toClose);
					if (num)
					{
						_lock.Lock(toClose, null);
					}
				}
			}
		}
		finally
		{
			_toClose.Clear();
		}
		EntityQueryEnumerator<RemoveOnlyStorageComponent> removeOnlyQuery = ((EntitySystem)this).EntityQueryEnumerator<RemoveOnlyStorageComponent>();
		EntityUid uid = default(EntityUid);
		RemoveOnlyStorageComponent comp = default(RemoveOnlyStorageComponent);
		StorageComponent storage = default(StorageComponent);
		while (removeOnlyQuery.MoveNext(ref uid, ref comp))
		{
			if (((EntitySystem)this).TryComp<StorageComponent>(uid, ref storage))
			{
				if (comp.Blacklist != null)
				{
					storage.Blacklist = comp.Blacklist;
				}
				storage.Whitelist = comp.Whitelist;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)storage, (MetaDataComponent)null);
			}
			((EntitySystem)this).RemCompDeferred<RemoveOnlyStorageComponent>(uid);
		}
		EntityQueryEnumerator<StorageOpenComponent> openQuery = ((EntitySystem)this).EntityQueryEnumerator<StorageOpenComponent>();
		EntityUid uid2 = default(EntityUid);
		StorageOpenComponent open = default(StorageOpenComponent);
		while (openQuery.MoveNext(ref uid2, ref open))
		{
			_toRemove.Clear();
			foreach (var (user, netOrigin) in open.OpenedAt)
			{
				if (((EntitySystem)this).TerminatingOrDeleted(user, (MetaDataComponent)null))
				{
					_toRemove.Add(user);
					continue;
				}
				EntityCoordinates origin = ((EntitySystem)this).GetCoordinates(netOrigin);
				EntityCoordinates current = _transform.GetMoverCoordinates(user);
				if (!_transform.InRange(origin, current, 0.1f))
				{
					_toRemove.Add(user);
				}
			}
			foreach (EntityUid user2 in _toRemove)
			{
				_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(uid2), (Enum)StorageComponent.StorageUiKey.Key, (EntityUid?)user2, false);
				open.OpenedAt.Remove(user2);
			}
			if (open.OpenedAt.Count == 0)
			{
				((EntitySystem)this).RemCompDeferred<StorageOpenComponent>(uid2);
			}
		}
	}
}
