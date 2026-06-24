using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Storage;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Implants.Components;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Content.Shared.Lock;
using Content.Shared.Materials;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Rounding;
using Content.Shared.Stacks;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.Events;
using Content.Shared.Tag;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Storage.EntitySystems;

public abstract class SharedStorageSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	protected sealed class StorageComponentState : ComponentState
	{
		public Dictionary<NetEntity, ItemStorageLocation> StoredItems = new Dictionary<NetEntity, ItemStorageLocation>();

		public Dictionary<string, List<ItemStorageLocation>> SavedLocations = new Dictionary<string, List<ItemStorageLocation>>();

		public List<Box2i> Grid = new List<Box2i>();

		public ProtoId<ItemSizePrototype>? MaxItemSize;

		public EntityWhitelist? Whitelist;

		public EntityWhitelist? Blacklist;
	}

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	protected IRobustRandom Random;

	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	protected ActionBlockerSystem ActionBlocker;

	[Dependency]
	private EntityLookupSystem _entityLookupSystem;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	protected SharedAudioSystem Audio;

	[Dependency]
	protected SharedContainerSystem ContainerSystem;

	[Dependency]
	private SharedDoAfterSystem _doAfterSystem;

	[Dependency]
	protected SharedEntityStorageSystem EntityStorage;

	[Dependency]
	private SharedInteractionSystem _interactionSystem;

	[Dependency]
	protected SharedItemSystem ItemSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedHandsSystem _sharedHandsSystem;

	[Dependency]
	private SharedStackSystem _stack;

	[Dependency]
	protected SharedTransformSystem TransformSystem;

	[Dependency]
	protected SharedUserInterfaceSystem UI;

	[Dependency]
	private TagSystem _tag;

	[Dependency]
	protected UseDelaySystem UseDelay;

	[Dependency]
	protected RMCStorageSystem RMCStorage;

	[Dependency]
	private RMCHandsSystem _rmcHands;

	private EntityQuery<ItemComponent> _itemQuery;

	private EntityQuery<StackComponent> _stackQuery;

	private EntityQuery<TransformComponent> _xformQuery;

	private EntityQuery<UserInterfaceUserComponent> _userQuery;

	public bool NestedStorage = true;

	public static readonly ProtoId<ItemSizePrototype> DefaultStorageMaxItemSize = ProtoId<ItemSizePrototype>.op_Implicit("Normal");

	public const float AreaInsertDelayPerItem = 0.075f;

	private static AudioParams _audioParams;

	private ItemSizePrototype _defaultStorageMaxItemSize;

	private bool _nestedCheck;

	public bool CheckingCanInsert;

	private readonly List<EntityUid> _entList = new List<EntityUid>();

	private readonly HashSet<EntityUid> _entSet = new HashSet<EntityUid>();

	private readonly List<ItemSizePrototype> _sortedSizes = new List<ItemSizePrototype>();

	private FrozenDictionary<string, ItemSizePrototype> _nextSmallest = FrozenDictionary<string, ItemSizePrototype>.Empty;

	private const string QuickInsertUseDelayID = "quickInsert";

	private const string OpenUiUseDelayID = "storage";

	private int _openStorageLimit = -1;

	protected readonly List<string> CantFillReasons = new List<string>();

	private readonly Dictionary<Vector2i, ulong> _ignored = new Dictionary<Vector2i, ulong>();

	private List<Box2i> _itemShape = new List<Box2i>();

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Expected O, but got Unknown
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Expected O, but got Unknown
		((EntitySystem)this).Initialize();
		_itemQuery = ((EntitySystem)this).GetEntityQuery<ItemComponent>();
		_stackQuery = ((EntitySystem)this).GetEntityQuery<StackComponent>();
		_xformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		_userQuery = ((EntitySystem)this).GetEntityQuery<UserInterfaceUserComponent>();
		_prototype.PrototypesReloaded += OnPrototypesReloaded;
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _cfg, CCVars.StorageLimit, (Action<int>)OnStorageLimitChanged, true);
		BoundUserInterfaceRegisterExt.BuiEvents<StorageComponent>(((EntitySystem)this).Subs, (object)StorageComponent.StorageUiKey.Key, (BuiEventSubscriber<StorageComponent>)delegate(Subscriber<StorageComponent> subs)
		{
			subs.Event<BoundUIClosedEvent>((ComponentEventHandler<StorageComponent, BoundUIClosedEvent>)OnBoundUIClosed);
		});
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, ComponentRemove>((EntityEventRefHandler<StorageComponent, ComponentRemove>)OnRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, MapInitEvent>((EntityEventRefHandler<StorageComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, GetVerbsEvent<ActivationVerb>>((ComponentEventHandler<StorageComponent, GetVerbsEvent<ActivationVerb>>)AddUiVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, ComponentGetState>((ComponentEventRefHandler<StorageComponent, ComponentGetState>)OnStorageGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, ComponentInit>((ComponentEventHandler<StorageComponent, ComponentInit>)OnComponentInit, new Type[1] { typeof(SharedContainerSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, GetVerbsEvent<UtilityVerb>>((ComponentEventHandler<StorageComponent, GetVerbsEvent<UtilityVerb>>)AddTransferVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, InteractUsingEvent>((ComponentEventHandler<StorageComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, new Type[1] { typeof(ItemSlotsSystem) });
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, ActivateInWorldEvent>((ComponentEventHandler<StorageComponent, ActivateInWorldEvent>)OnActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, OpenStorageImplantEvent>((ComponentEventHandler<StorageComponent, OpenStorageImplantEvent>)OnImplantActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, AfterInteractEvent>((ComponentEventHandler<StorageComponent, AfterInteractEvent>)AfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, DestructionEventArgs>((ComponentEventHandler<StorageComponent, DestructionEventArgs>)OnDestroy, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, BoundUserInterfaceMessageAttempt>((EntityEventRefHandler<StorageComponent, BoundUserInterfaceMessageAttempt>)OnBoundUIAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, BoundUIOpenedEvent>((EntityEventRefHandler<StorageComponent, BoundUIOpenedEvent>)OnBoundUIOpen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, LockToggledEvent>((ComponentEventRefHandler<StorageComponent, LockToggledEvent>)OnLockToggled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<StorageComponent, EntInsertedIntoContainerMessage>)OnEntInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<StorageComponent, EntRemovedFromContainerMessage>)OnEntRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, ContainerIsInsertingAttemptEvent>((ComponentEventHandler<StorageComponent, ContainerIsInsertingAttemptEvent>)OnInsertAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, AreaPickupDoAfterEvent>((ComponentEventHandler<StorageComponent, AreaPickupDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, GotReclaimedEvent>((ComponentEventHandler<StorageComponent, GotReclaimedEvent>)OnReclaimed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MetaDataComponent, StackCountChangedEvent>((ComponentEventHandler<MetaDataComponent, StackCountChangedEvent>)OnStackCountChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<OpenNestedStorageEvent>((EntitySessionEventHandler<OpenNestedStorageEvent>)OnStorageNested, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<StorageTransferItemEvent>((EntitySessionEventHandler<StorageTransferItemEvent>)OnStorageTransfer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<StorageInteractWithItemEvent>((EntitySessionEventHandler<StorageInteractWithItemEvent>)OnInteractWithItem, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<StorageSetItemLocationEvent>((EntitySessionEventHandler<StorageSetItemLocationEvent>)OnSetItemLocation, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<StorageInsertItemIntoLocationEvent>((EntitySessionEventHandler<StorageInsertItemIntoLocationEvent>)OnInsertItemIntoLocation, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<StorageSaveItemLocationEvent>((EntitySessionEventHandler<StorageSaveItemLocationEvent>)OnSaveItemLocation, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemSizeChangedEvent>((EntityEventRefHandler<ItemSizeChangedEvent>)OnItemSizeChanged, (Type[])null, (Type[])null);
		CommandBinds.Builder.Bind(ContentKeyFunctions.OpenBackpack, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(HandleOpenBackpack), (StateInputCmdDelegate)null, false, true)).Bind(ContentKeyFunctions.OpenBelt, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(HandleOpenBelt), (StateInputCmdDelegate)null, false, true)).Register<SharedStorageSystem>();
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _cfg, CCVars.NestedStorage, (Action<bool>)OnNestedStorageCvar, true);
		UpdatePrototypeCache();
	}

	private void OnItemSizeChanged(ref ItemSizeChangedEvent ev)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		Entity<ItemComponent> itemEnt = default(Entity<ItemComponent>);
		itemEnt._002Ector(ev.Entity, (ItemComponent)null);
		if (TryGetStorageLocation(itemEnt, out BaseContainer container, out StorageComponent storage, out ItemStorageLocation loc))
		{
			UpdateOccupied(Entity<StorageComponent>.op_Implicit((container.Owner, storage)));
			if (!ItemFitsInGridLocation(Entity<ItemComponent>.op_Implicit((itemEnt.Owner, itemEnt.Comp)), Entity<StorageComponent>.op_Implicit((container.Owner, storage)), loc))
			{
				ContainerSystem.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(itemEnt.Owner), container, true, true, (EntityCoordinates?)null, (Angle?)null);
			}
		}
	}

	private void OnNestedStorageCvar(bool obj)
	{
		NestedStorage = obj;
	}

	private void OnStorageLimitChanged(int obj)
	{
		_openStorageLimit = obj;
	}

	private void OnRemove(Entity<StorageComponent> entity, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		UI.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(entity.Owner), (Enum)StorageComponent.StorageUiKey.Key);
	}

	private void OnMapInit(Entity<StorageComponent> entity, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		bool num = ((EntitySystem)this).HasComp<UseDelayComponent>(Entity<StorageComponent>.op_Implicit(entity));
		UseDelay.SetLength(Entity<UseDelayComponent>.op_Implicit(entity.Owner), entity.Comp.QuickInsertCooldown, "quickInsert");
		UseDelay.SetLength(Entity<UseDelayComponent>.op_Implicit(entity.Owner), entity.Comp.OpenUiCooldown, "storage");
		if (!num)
		{
			UseDelay.SetLength(Entity<UseDelayComponent>.op_Implicit(entity.Owner), TimeSpan.Zero);
		}
	}

	private void OnStorageGetState(EntityUid uid, StorageComponent component, ref ComponentGetState args)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<NetEntity, ItemStorageLocation> storedItems = new Dictionary<NetEntity, ItemStorageLocation>();
		foreach (var (ent, location) in component.StoredItems)
		{
			storedItems[((EntitySystem)this).GetNetEntity(ent, (MetaDataComponent)null)] = location;
		}
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new StorageComponentState
		{
			Grid = new List<Box2i>(component.Grid),
			MaxItemSize = component.MaxItemSize,
			StoredItems = storedItems,
			SavedLocations = component.SavedLocations,
			Whitelist = component.Whitelist,
			Blacklist = component.Blacklist
		};
	}

	public override void Shutdown()
	{
		_prototype.PrototypesReloaded -= OnPrototypesReloaded;
	}

	private void OnPrototypesReloaded(PrototypesReloadedEventArgs args)
	{
		if (!args.ByType.ContainsKey(typeof(ItemSizePrototype)))
		{
			IReadOnlyDictionary<Type, HashSet<string>> removed = args.Removed;
			if (removed == null || !removed.ContainsKey(typeof(ItemSizePrototype)))
			{
				return;
			}
		}
		UpdatePrototypeCache();
	}

	private void UpdatePrototypeCache()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_defaultStorageMaxItemSize = _prototype.Index<ItemSizePrototype>(DefaultStorageMaxItemSize);
		_sortedSizes.Clear();
		_sortedSizes.AddRange(_prototype.EnumeratePrototypes<ItemSizePrototype>());
		_sortedSizes.Sort();
		KeyValuePair<string, ItemSizePrototype>[] nextSmallest = new KeyValuePair<string, ItemSizePrototype>[_sortedSizes.Count];
		for (int i = 0; i < _sortedSizes.Count; i++)
		{
			string k = _sortedSizes[i].ID;
			ItemSizePrototype v = _sortedSizes[Math.Max(i - 1, 0)];
			nextSmallest[i] = new KeyValuePair<string, ItemSizePrototype>(k, v);
		}
		_nextSmallest = nextSmallest.ToFrozenDictionary();
	}

	private void OnComponentInit(EntityUid uid, StorageComponent storageComp, ComponentInit args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		storageComp.Container = ContainerSystem.EnsureContainer<Container>(uid, StorageComponent.ContainerId, (ContainerManagerComponent)null);
		UpdateAppearance(Entity<StorageComponent, AppearanceComponent>.op_Implicit((ValueTuple<EntityUid, StorageComponent, AppearanceComponent>)(uid, storageComp, null)));
		UpdateOccupied(Entity<StorageComponent>.op_Implicit((uid, storageComp)));
	}

	private void CloseNestedInterfaces(EntityUid uid, EntityUid actor, StorageComponent? storageComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StorageComponent>(uid, ref storageComp, true))
		{
			return;
		}
		foreach (EntityUid entity in ((BaseContainer)storageComp.Container).ContainedEntities)
		{
			if (!((EntitySystem)this).HasComp<RMCItemKeepUIOpenOnStorageClosedComponent>(entity))
			{
				UI.CloseUis(Entity<UserInterfaceComponent>.op_Implicit(entity), actor);
			}
		}
	}

	private void OnBoundUIClosed(EntityUid uid, StorageComponent storageComp, BoundUIClosedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		CloseNestedInterfaces(uid, ((BaseBoundUserInterfaceEvent)args).Actor, storageComp);
		if (!UI.IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(uid), ((BaseBoundUserInterfaceEvent)args).UiKey))
		{
			UpdateAppearance(Entity<StorageComponent, AppearanceComponent>.op_Implicit((ValueTuple<EntityUid, StorageComponent, AppearanceComponent>)(uid, storageComp, null)));
			if (!_tag.HasTag(((BaseBoundUserInterfaceEvent)args).Actor, storageComp.SilentStorageUserTag))
			{
				Audio.PlayPredicted(storageComp.StorageCloseSound, uid, (EntityUid?)((BaseBoundUserInterfaceEvent)args).Actor, (AudioParams?)null);
			}
		}
	}

	private void AddUiVerb(EntityUid uid, StorageComponent component, GetVerbsEvent<ActivationVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		if (!component.ShowVerb || !CanInteract(args.User, Entity<StorageComponent>.op_Implicit((uid, component)), args.CanAccess && args.CanInteract))
		{
			return;
		}
		bool uiOpen = UI.IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)StorageComponent.StorageUiKey.Key, args.User);
		ActivationVerb verb = new ActivationVerb
		{
			Act = delegate
			{
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				if (uiOpen)
				{
					UI.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)StorageComponent.StorageUiKey.Key, (EntityUid?)args.User, false);
				}
				else
				{
					OpenStorageUI(uid, args.User, component, silent: false);
				}
			}
		};
		if (uiOpen)
		{
			verb.Text = base.Loc.GetString("comp-storage-verb-close-storage");
			verb.Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/close.svg.192dpi.png"));
		}
		else
		{
			verb.Text = base.Loc.GetString("comp-storage-verb-open-storage");
			verb.Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/open.svg.192dpi.png"));
		}
		args.Verbs.Add(verb);
	}

	public bool TryGetStorageLocation(Entity<ItemComponent?> itemEnt, [NotNullWhen(true)] out BaseContainer? container, [NotNullWhen(true)] out StorageComponent? storage, out ItemStorageLocation loc)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		loc = default(ItemStorageLocation);
		storage = null;
		if (!ContainerSystem.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(itemEnt.Owner), ref container) || container.ID != StorageComponent.ContainerId || !((EntitySystem)this).TryComp<StorageComponent>(container.Owner, ref storage) || !_itemQuery.Resolve(Entity<ItemComponent>.op_Implicit(itemEnt), ref itemEnt.Comp, false))
		{
			return false;
		}
		loc = storage.StoredItems[Entity<ItemComponent>.op_Implicit(itemEnt)];
		return true;
	}

	public void OpenStorageUI(EntityUid uid, EntityUid actor, StorageComponent? storageComp = null, bool silent = true, bool doAfter = true)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (ContainerSystem.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(uid), ref container) && UI.IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(container.Owner), (Enum)StorageComponent.StorageUiKey.Key, actor))
		{
			_nestedCheck = true;
			HideStorageWindow(container.Owner, actor);
			OpenStorageUIInternal(uid, actor, storageComp, silent: true, doAfter);
			_nestedCheck = false;
		}
		else
		{
			if (_openStorageLimit == 1)
			{
				UI.CloseUserUis<StorageComponent.StorageUiKey>(Entity<UserInterfaceUserComponent>.op_Implicit(actor));
			}
			OpenStorageUIInternal(uid, actor, storageComp, silent, doAfter);
		}
	}

	private void OpenStorageUIInternal(EntityUid uid, EntityUid entity, StorageComponent? storageComp = null, bool silent = true, bool doAfter = true)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if ((doAfter && RMCStorage.OpenDoAfter(uid, entity, storageComp, silent)) || !((EntitySystem)this).Resolve<StorageComponent>(uid, ref storageComp, false))
		{
			return;
		}
		UseDelayComponent useDelay = default(UseDelayComponent);
		silent |= ((EntitySystem)this).TryComp<UseDelayComponent>(uid, ref useDelay) && UseDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((uid, useDelay)), "storage");
		if (CanInteract(entity, Entity<StorageComponent>.op_Implicit((uid, storageComp)), canInteract: true, silent) && UI.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)StorageComponent.StorageUiKey.Key, entity, false) && !silent && !_tag.HasTag(entity, storageComp.SilentStorageUserTag))
		{
			Audio.PlayPredicted(storageComp.StorageOpenSound, uid, (EntityUid?)entity, (AudioParams?)null);
			if (useDelay != null)
			{
				UseDelay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((uid, useDelay)), checkDelayed: false, "storage");
			}
		}
	}

	public virtual void UpdateUI(Entity<StorageComponent?> entity)
	{
	}

	private void AddTransferVerbs(EntityUid uid, StorageComponent component, GetVerbsEvent<UtilityVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		StorageComponent targetStorage = default(StorageComponent);
		LockComponent targetLock = default(LockComponent);
		if (args.CanAccess && args.CanInteract && ((BaseContainer)component.Container).ContainedEntities.Count != 0 && CanInteract(args.User, Entity<StorageComponent>.op_Implicit((uid, component))) && ((EntitySystem)this).TryComp<StorageComponent>(args.Target, ref targetStorage) && (!((EntitySystem)this).TryComp<LockComponent>(args.Target, ref targetLock) || !targetLock.Locked))
		{
			UtilityVerb verb = new UtilityVerb
			{
				Text = base.Loc.GetString("storage-component-transfer-verb"),
				IconEntity = ((EntitySystem)this).GetNetEntity(args.Using, (MetaDataComponent)null),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					TransferEntities(uid, args.Target, args.User, component, null, targetStorage, targetLock);
				}
			};
			args.Verbs.Add(verb);
		}
	}

	private void OnInteractUsing(EntityUid uid, StorageComponent storageComp, InteractUsingEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !storageComp.ClickInsert || !CanInteract(args.User, Entity<StorageComponent>.op_Implicit((uid, storageComp)), canInteract: true, silent: false))
		{
			return;
		}
		StorageInteractUsingAttemptEvent attemptEv = default(StorageInteractUsingAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<StorageInteractUsingAttemptEvent>(uid, ref attemptEv, false);
		if (!attemptEv.Cancelled)
		{
			StorageComponent usingStorage = default(StorageComponent);
			if (storageComp.AllowStorageTransfer && ((EntitySystem)this).TryComp<StorageComponent>(args.Used, ref usingStorage) && args.Used != uid)
			{
				TransferEntities(args.Used, uid, args.User, usingStorage, null, storageComp);
				((HandledEntityEventArgs)args).Handled = true;
			}
			else
			{
				PlayerInsertHeldEntity(Entity<StorageComponent>.op_Implicit((uid, storageComp)), Entity<HandsComponent>.op_Implicit(args.User));
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void OnActivate(EntityUid uid, StorageComponent storageComp, ActivateInWorldEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Complex && storageComp.OpenOnActivate && CanInteract(args.User, Entity<StorageComponent>.op_Implicit((uid, storageComp))))
		{
			if (UI.IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)StorageComponent.StorageUiKey.Key, args.User))
			{
				UI.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)StorageComponent.StorageUiKey.Key, (EntityUid?)args.User, false);
			}
			else
			{
				OpenStorageUI(uid, args.User, storageComp, silent: false);
			}
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	protected virtual void HideStorageWindow(EntityUid uid, EntityUid actor)
	{
	}

	protected virtual void ShowStorageWindow(EntityUid uid, EntityUid actor)
	{
	}

	private void OnImplantActivate(EntityUid uid, StorageComponent storageComp, OpenStorageImplantEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			if (UI.IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)StorageComponent.StorageUiKey.Key, args.Performer))
			{
				UI.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)StorageComponent.StorageUiKey.Key, (EntityUid?)args.Performer, false);
			}
			else
			{
				OpenStorageUI(uid, args.Performer, storageComp, silent: false);
			}
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void AfterInteract(EntityUid uid, StorageComponent storageComp, AfterInteractEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !args.CanReach || !UseDelay.TryResetDelay(uid, checkDelayed: true, null, "quickInsert"))
		{
			return;
		}
		if (storageComp.AreaInsert && (!args.Target.HasValue || !((EntitySystem)this).HasComp<ItemComponent>(args.Target.Value)))
		{
			_entList.Clear();
			_entSet.Clear();
			_entityLookupSystem.GetEntitiesInRange(args.ClickLocation, (float)storageComp.AreaInsertRadius, _entSet, (LookupFlags)10);
			float delay = 0f;
			ItemComponent itemComp = default(ItemComponent);
			ItemSizePrototype itemSize = default(ItemSizePrototype);
			foreach (EntityUid entity in _entSet)
			{
				if (!(entity == args.User) && _itemQuery.TryGetComponent(entity, ref itemComp) && _prototype.TryIndex<ItemSizePrototype>(itemComp.Size, ref itemSize) && CanInsert(uid, entity, args.User, out string _, storageComp, itemComp) && _interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(args.User), Entity<TransformComponent>.op_Implicit(entity)))
				{
					_entList.Add(entity);
					delay += (float)itemSize.Weight * 0.075f;
					if (_entList.Count >= 10)
					{
						break;
					}
				}
			}
			if (_entList.Count >= 1)
			{
				DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, delay, new AreaPickupDoAfterEvent(((EntitySystem)this).GetNetEntityList(_entList)), uid, uid)
				{
					BreakOnDamage = true,
					BreakOnMove = true,
					NeedHand = true
				};
				_doAfterSystem.TryStartDoAfter(doAfterArgs);
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
		else
		{
			if (!storageComp.QuickInsert)
			{
				return;
			}
			EntityUid? target = args.Target;
			if (!target.HasValue)
			{
				return;
			}
			EntityUid target2 = target.GetValueOrDefault();
			TransformComponent transformOwner = default(TransformComponent);
			TransformComponent transformEnt = default(TransformComponent);
			if (((EntityUid)(ref target2)).Valid && !ContainerSystem.IsEntityInContainer(target2, (MetaDataComponent)null) && !(target2 == args.User) && _itemQuery.HasComponent(target2) && ((EntitySystem)this).TryComp(uid, ref transformOwner) && ((EntitySystem)this).TryComp(target2, ref transformEnt))
			{
				EntityUid parent = transformOwner.ParentUid;
				EntityCoordinates position = TransformSystem.ToCoordinates(Entity<TransformComponent>.op_Implicit(((EntityUid)(ref parent)).IsValid() ? parent : uid), TransformSystem.GetMapCoordinates(transformEnt));
				((HandledEntityEventArgs)args).Handled = true;
				if (PlayerInsertEntityInWorld(Entity<StorageComponent>.op_Implicit((uid, storageComp)), args.User, target2))
				{
					base.EntityManager.RaiseSharedEvent<AnimateInsertingEntitiesEvent>(new AnimateInsertingEntitiesEvent(((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null), new List<NetEntity> { ((EntitySystem)this).GetNetEntity(target2, (MetaDataComponent)null) }, new List<NetCoordinates> { ((EntitySystem)this).GetNetCoordinates(position, (MetaDataComponent)null) }, new List<Angle> { transformOwner.LocalRotation }), (EntityUid?)args.User);
				}
			}
		}
	}

	private void OnDoAfter(EntityUid uid, StorageComponent component, AreaPickupDoAfterEvent args)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		List<EntityUid> successfullyInserted = new List<EntityUid>();
		List<EntityCoordinates> successfullyInsertedPositions = new List<EntityCoordinates>();
		List<Angle> successfullyInsertedAngles = new List<Angle>();
		TransformComponent xform = default(TransformComponent);
		if (!_xformQuery.TryGetComponent(uid, ref xform))
		{
			return;
		}
		int entCount = Math.Min(10, args.Entities.Count);
		TransformComponent targetXform = default(TransformComponent);
		for (int i = 0; i < entCount; i++)
		{
			EntityUid entity = ((EntitySystem)this).GetEntity(args.Entities[i]);
			if (!ContainerSystem.IsEntityInContainer(entity, (MetaDataComponent)null) && !(entity == args.Args.User) && _itemQuery.HasComponent(entity) && _xformQuery.TryGetComponent(entity, ref targetXform) && !(targetXform.MapID != xform.MapID))
			{
				SharedTransformSystem transformSystem = TransformSystem;
				EntityUid parentUid = xform.ParentUid;
				EntityCoordinates position = transformSystem.ToCoordinates(Entity<TransformComponent>.op_Implicit(((EntityUid)(ref parentUid)).IsValid() ? xform.ParentUid : uid), new MapCoordinates(TransformSystem.GetWorldPosition(targetXform), targetXform.MapID));
				Angle angle = targetXform.LocalRotation;
				if (PlayerInsertEntityInWorld(Entity<StorageComponent>.op_Implicit((uid, component)), args.Args.User, entity, playSound: false))
				{
					successfullyInserted.Add(entity);
					successfullyInsertedPositions.Add(position);
					successfullyInsertedAngles.Add(angle);
				}
			}
		}
		if (successfullyInserted.Count > 0)
		{
			if (!_tag.HasTag(args.User, component.SilentStorageUserTag))
			{
				Audio.PlayPredicted(component.StorageInsertSound, uid, (EntityUid?)args.User, (AudioParams?)_audioParams);
			}
			base.EntityManager.RaiseSharedEvent<AnimateInsertingEntitiesEvent>(new AnimateInsertingEntitiesEvent(((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null), ((EntitySystem)this).GetNetEntityList(successfullyInserted), ((EntitySystem)this).GetNetCoordinatesList(successfullyInsertedPositions), successfullyInsertedAngles), (EntityUid?)args.User);
		}
		((HandledEntityEventArgs)args).Handled = true;
	}

	private void OnReclaimed(EntityUid uid, StorageComponent storageComp, GotReclaimedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		ContainerSystem.EmptyContainer((BaseContainer)(object)storageComp.Container, false, (EntityCoordinates?)args.ReclaimerCoordinates, true);
	}

	private void OnDestroy(EntityUid uid, StorageComponent storageComp, DestructionEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coordinates = TransformSystem.GetMoverCoordinates(uid);
		ContainerSystem.EmptyContainer((BaseContainer)(object)storageComp.Container, false, (EntityCoordinates?)coordinates, true);
	}

	private void OnInteractWithItem(StorageInteractWithItemEvent msg, EntitySessionEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		if (!ValidateInput(args, msg.StorageUid, msg.InteractedItemUid, out Entity<HandsComponent> player, out Entity<StorageComponent> storage, out Entity<ItemComponent> item))
		{
			return;
		}
		if (!_sharedHandsSystem.TryGetActiveItem(player.AsNullable(), out var activeItem))
		{
			if (!_rmcHands.TryStorageEjectHand(Entity<HandsComponent>.op_Implicit(player), Entity<ItemComponent>.op_Implicit(item)))
			{
				ISharedAdminLogManager adminLog = _adminLog;
				LogStringHandler handler = new LogStringHandler(31, 3);
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HandsComponent>.op_Implicit(player), (MetaDataComponent)null), "player", "ToPrettyString(player)");
				handler.AppendLiteral(" is attempting to take ");
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ItemComponent>.op_Implicit(item), (MetaDataComponent)null), "item", "ToPrettyString(item)");
				handler.AppendLiteral(" out of ");
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<StorageComponent>.op_Implicit(storage), (MetaDataComponent)null), "storage", "ToPrettyString(storage)");
				adminLog.Add(LogType.Storage, LogImpact.Low, ref handler);
				if (_sharedHandsSystem.TryPickupAnyHand(Entity<HandsComponent>.op_Implicit(player), Entity<ItemComponent>.op_Implicit(item), checkActionBlocker: true, animateUser: false, animate: true, player.Comp) && storage.Comp.StorageRemoveSound != null && !_tag.HasTag(Entity<HandsComponent>.op_Implicit(player), storage.Comp.SilentStorageUserTag))
				{
					Audio.PlayPredicted(storage.Comp.StorageRemoveSound, Entity<StorageComponent>.op_Implicit(storage), (EntityUid?)Entity<HandsComponent>.op_Implicit(player), (AudioParams?)_audioParams);
				}
				UpdateUI(Entity<StorageComponent>.op_Implicit((Entity<StorageComponent>.op_Implicit(storage), Entity<StorageComponent>.op_Implicit(storage))));
			}
		}
		else
		{
			ISharedAdminLogManager adminLog2 = _adminLog;
			LogStringHandler handler2 = new LogStringHandler(51, 4);
			handler2.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HandsComponent>.op_Implicit(player), (MetaDataComponent)null), "player", "ToPrettyString(player)");
			handler2.AppendLiteral(" is interacting with ");
			handler2.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ItemComponent>.op_Implicit(item), (MetaDataComponent)null), "item", "ToPrettyString(item)");
			handler2.AppendLiteral(" while it is stored in ");
			handler2.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<StorageComponent>.op_Implicit(storage), (MetaDataComponent)null), "storage", "ToPrettyString(storage)");
			handler2.AppendLiteral(" using ");
			handler2.AppendFormatted(((EntitySystem)this).ToPrettyString(activeItem, (MetaDataComponent)null), "used", "ToPrettyString(activeItem)");
			adminLog2.Add(LogType.Storage, LogImpact.Low, ref handler2);
			if (!_interactionSystem.InteractUsing(Entity<HandsComponent>.op_Implicit(player), activeItem.Value, Entity<ItemComponent>.op_Implicit(item), ((EntitySystem)this).Transform(Entity<ItemComponent>.op_Implicit(item)).Coordinates, checkCanInteract: false))
			{
				StorageInsertFailedEvent failedEv = new StorageInsertFailedEvent(Entity<StorageComponent>.op_Implicit((Entity<StorageComponent>.op_Implicit(storage), storage.Comp)), Entity<HandsComponent>.op_Implicit((Entity<HandsComponent>.op_Implicit(player), player.Comp)));
				((EntitySystem)this).RaiseLocalEvent<StorageInsertFailedEvent>(Entity<StorageComponent>.op_Implicit(storage), ref failedEv, false);
			}
		}
	}

	private void OnSetItemLocation(StorageSetItemLocationEvent msg, EntitySessionEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (ValidateInput(args, msg.StorageEnt, msg.ItemEnt, out Entity<HandsComponent> player, out Entity<StorageComponent> storage, out Entity<ItemComponent> item))
		{
			ISharedAdminLogManager adminLog = _adminLog;
			LogStringHandler handler = new LogStringHandler(37, 3);
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HandsComponent>.op_Implicit(player), (MetaDataComponent)null), "player", "ToPrettyString(player)");
			handler.AppendLiteral(" is updating the location of ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ItemComponent>.op_Implicit(item), (MetaDataComponent)null), "item", "ToPrettyString(item)");
			handler.AppendLiteral(" within ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<StorageComponent>.op_Implicit(storage), (MetaDataComponent)null), "storage", "ToPrettyString(storage)");
			adminLog.Add(LogType.Storage, LogImpact.Low, ref handler);
			TrySetItemStorageLocation(item, storage, msg.Location);
		}
	}

	private void OnStorageNested(OpenNestedStorageEvent msg, EntitySessionEventArgs args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? itemEnt = default(EntityUid?);
		if (NestedStorage && ((EntitySystem)this).TryGetEntity(msg.InteractedItemUid, ref itemEnt))
		{
			_nestedCheck = true;
			if (!ValidateInput(args, msg.StorageUid, msg.InteractedItemUid, out Entity<HandsComponent> player, out Entity<StorageComponent> storage, out Entity<ItemComponent> item))
			{
				_nestedCheck = false;
				return;
			}
			HideStorageWindow(storage.Owner, player.Owner);
			OpenStorageUI(item.Owner, player.Owner);
			_nestedCheck = false;
		}
	}

	private void OnStorageTransfer(StorageTransferItemEvent msg, EntitySessionEventArgs args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? itemUid = default(EntityUid?);
		ItemComponent itemComp = default(ItemComponent);
		if (((EntitySystem)this).TryGetEntity(msg.ItemEnt, ref itemUid) && ((EntitySystem)this).TryComp<ItemComponent>(itemUid, ref itemComp))
		{
			EntityUid? localPlayer = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
			Entity<ItemComponent> itemEnt = default(Entity<ItemComponent>);
			itemEnt._002Ector(itemUid.Value, itemComp);
			HandsComponent handsComp = default(HandsComponent);
			if (TryGetStorageLocation(itemEnt, out BaseContainer container, out StorageComponent _, out ItemStorageLocation _) && ValidateInput(args, ((EntitySystem)this).GetNetEntity(container.Owner, (MetaDataComponent)null), out Entity<HandsComponent> _, out Entity<StorageComponent> _) && ((EntitySystem)this).TryComp<HandsComponent>(localPlayer, ref handsComp) && _sharedHandsSystem.TryPickup(localPlayer.Value, Entity<ItemComponent>.op_Implicit(itemEnt), null, checkActionBlocker: true, animateUser: false, animate: false, handsComp) && ValidateInput(args, msg.StorageEnt, msg.ItemEnt, out Entity<HandsComponent> player2, out Entity<StorageComponent> storage3, out Entity<ItemComponent> item, held: true))
			{
				ISharedAdminLogManager adminLog = _adminLog;
				LogStringHandler handler = new LogStringHandler(20, 3);
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HandsComponent>.op_Implicit(player2), (MetaDataComponent)null), "player", "ToPrettyString(player)");
				handler.AppendLiteral(" is inserting ");
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ItemComponent>.op_Implicit(item), (MetaDataComponent)null), "item", "ToPrettyString(item)");
				handler.AppendLiteral(" into ");
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<StorageComponent>.op_Implicit(storage3), (MetaDataComponent)null), "storage", "ToPrettyString(storage)");
				adminLog.Add(LogType.Storage, LogImpact.Low, ref handler);
				InsertAt(storage3, item, msg.Location, out var _, Entity<HandsComponent>.op_Implicit(player2), playSound: true, stackAutomatically: false);
			}
		}
	}

	private void OnInsertItemIntoLocation(StorageInsertItemIntoLocationEvent msg, EntitySessionEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if (ValidateInput(args, msg.StorageEnt, msg.ItemEnt, out Entity<HandsComponent> player, out Entity<StorageComponent> storage, out Entity<ItemComponent> item, held: true))
		{
			ISharedAdminLogManager adminLog = _adminLog;
			LogStringHandler handler = new LogStringHandler(20, 3);
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HandsComponent>.op_Implicit(player), (MetaDataComponent)null), "player", "ToPrettyString(player)");
			handler.AppendLiteral(" is inserting ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ItemComponent>.op_Implicit(item), (MetaDataComponent)null), "item", "ToPrettyString(item)");
			handler.AppendLiteral(" into ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<StorageComponent>.op_Implicit(storage), (MetaDataComponent)null), "storage", "ToPrettyString(storage)");
			adminLog.Add(LogType.Storage, LogImpact.Low, ref handler);
			InsertAt(storage, item, msg.Location, out var _, Entity<HandsComponent>.op_Implicit(player), playSound: true, stackAutomatically: false);
		}
	}

	private void OnSaveItemLocation(StorageSaveItemLocationEvent msg, EntitySessionEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (ValidateInput(args, msg.Storage, msg.Item, out Entity<HandsComponent> _, out Entity<StorageComponent> storage, out Entity<ItemComponent> item))
		{
			SaveItemLocation(storage, Entity<MetaDataComponent>.op_Implicit(item.Owner));
		}
	}

	private void OnBoundUIOpen(Entity<StorageComponent> ent, ref BoundUIOpenedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(Entity<StorageComponent, AppearanceComponent>.op_Implicit((ValueTuple<EntityUid, StorageComponent, AppearanceComponent>)(ent.Owner, ent.Comp, null)));
	}

	private void OnBoundUIAttempt(Entity<StorageComponent> ent, ref BoundUserInterfaceMessageAttempt args)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		Enum uiKey = args.UiKey;
		if (!(uiKey is StorageComponent.StorageUiKey) || (StorageComponent.StorageUiKey)(object)uiKey != StorageComponent.StorageUiKey.Key || _openStorageLimit == -1 || _nestedCheck || !(args.Message is OpenBoundInterfaceMessage))
		{
			return;
		}
		EntityUid uid = args.Target;
		EntityUid actor = args.Actor;
		int count = 0;
		UserInterfaceUserComponent userComp = default(UserInterfaceUserComponent);
		if (!_userQuery.TryComp(actor, ref userComp))
		{
			return;
		}
		foreach (var (val2, keys) in userComp.OpenInterfaces)
		{
			if (val2 == uid)
			{
				continue;
			}
			foreach (Enum item in keys)
			{
				if (item is StorageComponent.StorageUiKey)
				{
					count++;
					if (count >= _openStorageLimit)
					{
						((CancellableEntityEventArgs)args).Cancel();
					}
					break;
				}
			}
		}
	}

	private void OnEntInserted(Entity<StorageComponent> entity, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.Container == null || ((ContainerModifiedMessage)args).Container.ID != StorageComponent.ContainerId)
		{
			return;
		}
		if (!entity.Comp.StoredItems.ContainsKey(((ContainerModifiedMessage)args).Entity))
		{
			if (!CMInventoryExtensions.TryGetFirst(Entity<StorageComponent>.op_Implicit(entity), ((ContainerModifiedMessage)args).Entity, out var location))
			{
				ContainerSystem.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(((ContainerModifiedMessage)args).Entity), ((ContainerModifiedMessage)args).Container, true, true, (EntityCoordinates?)null, (Angle?)null);
				return;
			}
			entity.Comp.StoredItems[((ContainerModifiedMessage)args).Entity] = location;
			AddOccupiedEntity(entity, Entity<ItemComponent>.op_Implicit(((ContainerModifiedMessage)args).Entity), location);
		}
		UpdateAppearance(Entity<StorageComponent, AppearanceComponent>.op_Implicit((ValueTuple<EntityUid, StorageComponent, AppearanceComponent>)(Entity<StorageComponent>.op_Implicit(entity), entity.Comp, null)));
		UpdateUI(Entity<StorageComponent>.op_Implicit((Entity<StorageComponent>.op_Implicit(entity), entity.Comp)));
	}

	private void OnEntRemoved(Entity<StorageComponent> entity, ref EntRemovedFromContainerMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.Container == null || ((ContainerModifiedMessage)args).Container.ID != StorageComponent.ContainerId)
		{
			return;
		}
		if (entity.Comp.StoredItems.Remove(((ContainerModifiedMessage)args).Entity, out var loc))
		{
			RemoveOccupiedEntity(entity, Entity<ItemComponent>.op_Implicit(((ContainerModifiedMessage)args).Entity), loc);
		}
		((EntitySystem)this).Dirty(Entity<StorageComponent>.op_Implicit(entity), (IComponent)(object)entity.Comp, (MetaDataComponent)null);
		UpdateAppearance(Entity<StorageComponent, AppearanceComponent>.op_Implicit((ValueTuple<EntityUid, StorageComponent, AppearanceComponent>)(Entity<StorageComponent>.op_Implicit(entity), entity.Comp, null)));
		UpdateUI(Entity<StorageComponent>.op_Implicit((Entity<StorageComponent>.op_Implicit(entity), entity.Comp)));
		List<(EntityUid, ItemStorageLocation)> items = new List<(EntityUid, ItemStorageLocation)>();
		foreach (var (item, location) in entity.Comp.StoredItems)
		{
			items.Add((item, location));
		}
		items.Sort(delegate((EntityUid Id, ItemStorageLocation Location) a, (EntityUid Id, ItemStorageLocation Location) b)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			int num = a.Location.Position.Y.CompareTo(b.Location.Position.Y);
			return (num != 0) ? num : a.Location.Position.X.CompareTo(b.Location.Position.X);
		});
		foreach (var (item2, location2) in items)
		{
			if (CMInventoryExtensions.TryGetFirst(Entity<StorageComponent>.op_Implicit(entity), item2, out var newLocation) && location2 != newLocation)
			{
				TrySetItemStorageLocation(Entity<ItemComponent>.op_Implicit(item2), Entity<StorageComponent>.op_Implicit((Entity<StorageComponent>.op_Implicit(entity), Entity<StorageComponent>.op_Implicit(entity))), newLocation);
			}
		}
	}

	private void OnInsertAttempt(EntityUid uid, StorageComponent component, ContainerIsInsertingAttemptEvent args)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && !(((ContainerAttemptEventBase)args).Container.ID != StorageComponent.ContainerId) && !CheckingCanInsert && !CanInsert(uid, ((ContainerAttemptEventBase)args).EntityUid, null, out string _, component, null, ignoreStacks: true))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	public void UpdateAppearance(Entity<StorageComponent?, AppearanceComponent?> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		Entity<StorageComponent, AppearanceComponent> val = entity;
		EntityUid val2 = default(EntityUid);
		StorageComponent storageComponent = default(StorageComponent);
		AppearanceComponent val3 = default(AppearanceComponent);
		val.Deconstruct(ref val2, ref storageComponent, ref val3);
		EntityUid uid = val2;
		StorageComponent storage = storageComponent;
		AppearanceComponent appearance = val3;
		if (((EntitySystem)this).Resolve<StorageComponent, AppearanceComponent>(uid, ref storage, ref appearance, false) && storage.Container != null)
		{
			int capacity = storage.Grid.GetArea();
			int used = GetCumulativeItemAreas(Entity<StorageComponent>.op_Implicit((uid, storage)));
			bool isOpen = UI.IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(entity.Owner), (Enum)StorageComponent.StorageUiKey.Key);
			_appearance.SetData(uid, (Enum)StorageVisuals.StorageUsed, (object)used, appearance);
			_appearance.SetData(uid, (Enum)StorageVisuals.Capacity, (object)capacity, appearance);
			_appearance.SetData(uid, (Enum)StorageVisuals.Open, (object)isOpen, appearance);
			_appearance.SetData(uid, (Enum)SharedBagOpenVisuals.BagState, (object)((!isOpen) ? SharedBagState.Closed : SharedBagState.Open), appearance);
			StorageFillVisualizerComponent storageFillVisualizerComp = default(StorageFillVisualizerComponent);
			if (((EntitySystem)this).TryComp<StorageFillVisualizerComponent>(uid, ref storageFillVisualizerComp))
			{
				int level = ContentHelpers.RoundToLevels(used, capacity, storageFillVisualizerComp.MaxFillLevels);
				_appearance.SetData(uid, (Enum)StorageFillVisuals.FillLevel, (object)level, appearance);
			}
			if (storage.HideStackVisualsWhenClosed)
			{
				_appearance.SetData(uid, (Enum)StackVisuals.Hide, (object)(!isOpen), appearance);
			}
		}
	}

	public void TransferEntities(EntityUid source, EntityUid target, EntityUid? user = null, StorageComponent? sourceComp = null, LockComponent? sourceLock = null, StorageComponent? targetComp = null, LockComponent? targetLock = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StorageComponent>(source, ref sourceComp, true) || !((EntitySystem)this).Resolve<StorageComponent>(target, ref targetComp, true))
		{
			return;
		}
		IReadOnlyList<EntityUid> entities = ((BaseContainer)sourceComp.Container).ContainedEntities;
		if (entities.Count == 0 || (((EntitySystem)this).Resolve<LockComponent>(source, ref sourceLock, false) && sourceLock.Locked) || (((EntitySystem)this).Resolve<LockComponent>(target, ref targetLock, false) && targetLock.Locked))
		{
			return;
		}
		bool noSpace = false;
		EntityUid[] array = entities.ToArray();
		foreach (EntityUid entity in array)
		{
			EntityUid? stackedEntity;
			if (!CanInsert(target, entity, user, out string reason, targetComp))
			{
				if (reason == "comp-storage-insufficient-capacity")
				{
					noSpace = true;
					break;
				}
			}
			else if (!Insert(target, entity, out stackedEntity, user, targetComp, playSound: false))
			{
				noSpace = true;
				break;
			}
		}
		if (user.HasValue && (!_tag.HasTag(user.Value, sourceComp.SilentStorageUserTag) || !_tag.HasTag(user.Value, targetComp.SilentStorageUserTag)))
		{
			Audio.PlayPredicted(sourceComp.StorageInsertSound, target, user, (AudioParams?)_audioParams);
		}
		if (noSpace && user.HasValue)
		{
			_popupSystem.PopupClient(base.Loc.GetString("pubg-storage-transfer-leftover"), target, user.Value);
		}
	}

	public bool CanInsert(EntityUid uid, EntityUid insertEnt, EntityUid? user, out string? reason, StorageComponent? storageComp = null, ItemComponent? item = null, bool ignoreStacks = false, bool ignoreLocation = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StorageComponent>(uid, ref storageComp, true) || !((EntitySystem)this).Resolve<ItemComponent>(insertEnt, ref item, false))
		{
			reason = null;
			return false;
		}
		if (((EntitySystem)this).Transform(insertEnt).Anchored)
		{
			reason = "comp-storage-anchored-failure";
			return false;
		}
		if (_whitelistSystem.IsWhitelistFail(storageComp.Whitelist, insertEnt) || _whitelistSystem.IsBlacklistPass(storageComp.Blacklist, insertEnt))
		{
			reason = "comp-storage-invalid-container";
			return false;
		}
		StackComponent stack = default(StackComponent);
		if (!ignoreStacks && _stackQuery.TryGetComponent(insertEnt, ref stack) && HasSpaceInStacks(Entity<StorageComponent>.op_Implicit((uid, storageComp)), stack.StackTypeId))
		{
			reason = null;
			return true;
		}
		ItemSizePrototype maxSize = GetMaxItemSize(Entity<StorageComponent>.op_Implicit((uid, storageComp)));
		if (ItemSystem.GetSizePrototype(item.Size) > maxSize && !RMCStorage.IgnoreItemSize(Entity<StorageComponent>.op_Implicit((uid, storageComp)), insertEnt))
		{
			reason = "comp-storage-too-big";
			return false;
		}
		StorageComponent insertStorage = default(StorageComponent);
		if (((EntitySystem)this).TryComp<StorageComponent>(insertEnt, ref insertStorage) && GetMaxItemSize(Entity<StorageComponent>.op_Implicit((insertEnt, insertStorage))) >= maxSize && !RMCStorage.IgnoreItemSize(Entity<StorageComponent>.op_Implicit((uid, storageComp)), insertEnt))
		{
			reason = "comp-storage-too-big";
			return false;
		}
		if (!ignoreLocation && !storageComp.StoredItems.ContainsKey(insertEnt) && !TryGetAvailableGridSpace(Entity<StorageComponent>.op_Implicit((uid, storageComp)), Entity<ItemComponent>.op_Implicit((insertEnt, item)), out var _))
		{
			reason = "comp-storage-insufficient-capacity";
			return false;
		}
		if (!RMCStorage.CanInsert(Entity<StorageComponent>.op_Implicit((uid, storageComp)), insertEnt, user, out var popup))
		{
			reason = LocId.op_Implicit(popup);
			return false;
		}
		CheckingCanInsert = true;
		if (!ContainerSystem.CanInsert(insertEnt, (BaseContainer)(object)storageComp.Container, false, (TransformComponent)null))
		{
			CheckingCanInsert = false;
			reason = null;
			return false;
		}
		CheckingCanInsert = false;
		reason = null;
		return true;
	}

	public bool InsertAt(Entity<StorageComponent?> uid, Entity<ItemComponent?> insertEnt, ItemStorageLocation location, out EntityUid? stackedEntity, EntityUid? user = null, bool playSound = true, bool stackAutomatically = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		stackedEntity = null;
		if (!((EntitySystem)this).Resolve<StorageComponent>(Entity<StorageComponent>.op_Implicit(uid), ref uid.Comp, true))
		{
			return false;
		}
		if (!ItemFitsInGridLocation(insertEnt, uid, location))
		{
			return false;
		}
		uid.Comp.StoredItems[Entity<ItemComponent>.op_Implicit(insertEnt)] = location;
		AddOccupiedEntity(Entity<StorageComponent>.op_Implicit((uid.Owner, uid.Comp)), insertEnt, location);
		if (Insert(Entity<StorageComponent>.op_Implicit(uid), Entity<ItemComponent>.op_Implicit(insertEnt), out stackedEntity, out string _, user, uid.Comp, playSound, stackAutomatically))
		{
			return true;
		}
		RemoveOccupiedEntity(Entity<StorageComponent>.op_Implicit((uid.Owner, uid.Comp)), insertEnt, location);
		uid.Comp.StoredItems.Remove(Entity<ItemComponent>.op_Implicit(insertEnt));
		return false;
	}

	public bool Insert(EntityUid uid, EntityUid insertEnt, out EntityUid? stackedEntity, EntityUid? user = null, StorageComponent? storageComp = null, bool playSound = true, bool stackAutomatically = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		string reason;
		return Insert(uid, insertEnt, out stackedEntity, out reason, user, storageComp, playSound, stackAutomatically);
	}

	public bool Insert(EntityUid uid, EntityUid insertEnt, out EntityUid? stackedEntity, out string? reason, EntityUid? user = null, StorageComponent? storageComp = null, bool playSound = true, bool stackAutomatically = true)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		stackedEntity = null;
		reason = null;
		if (!((EntitySystem)this).Resolve<StorageComponent>(uid, ref storageComp, true))
		{
			return false;
		}
		bool canPlaySound = playSound && (!user.HasValue || !_tag.HasTag(user.Value, storageComp.SilentStorageUserTag));
		StackComponent insertStack = default(StackComponent);
		if (!stackAutomatically || !_stackQuery.TryGetComponent(insertEnt, ref insertStack))
		{
			if (!ContainerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(insertEnt), (BaseContainer)(object)storageComp.Container, (TransformComponent)null, false))
			{
				return false;
			}
			if (canPlaySound)
			{
				Audio.PlayPredicted(storageComp.StorageInsertSound, uid, user, (AudioParams?)_audioParams);
			}
			return true;
		}
		int toInsertCount = insertStack.Count;
		StackComponent containedStack = default(StackComponent);
		foreach (EntityUid ent in ((BaseContainer)storageComp.Container).ContainedEntities)
		{
			if (_stackQuery.TryGetComponent(ent, ref containedStack) && _stack.TryAdd(insertEnt, ent, insertStack, containedStack))
			{
				stackedEntity = ent;
				if (insertStack.Count == 0)
				{
					break;
				}
			}
		}
		if (insertStack.Count > 0 && !ContainerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(insertEnt), (BaseContainer)(object)storageComp.Container, (TransformComponent)null, false) && toInsertCount == insertStack.Count)
		{
			return false;
		}
		if (canPlaySound)
		{
			Audio.PlayPredicted(storageComp.StorageInsertSound, uid, user, (AudioParams?)_audioParams);
		}
		return true;
	}

	public bool PlayerInsertHeldEntity(Entity<StorageComponent?> ent, Entity<HandsComponent?> player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StorageComponent>(ent.Owner, ref ent.Comp, true) || !((EntitySystem)this).Resolve<HandsComponent>(player.Owner, ref player.Comp, true) || !_sharedHandsSystem.TryGetActiveItem(player, out var activeItem))
		{
			return false;
		}
		EntityUid? toInsert = activeItem;
		if (!CanInsert(Entity<StorageComponent>.op_Implicit(ent), toInsert.Value, player.Owner, out string reason, ent.Comp))
		{
			_popupSystem.PopupClient(base.Loc.GetString(reason ?? "comp-storage-cant-insert"), Entity<StorageComponent>.op_Implicit(ent), Entity<HandsComponent>.op_Implicit(player));
			return false;
		}
		if (!_sharedHandsSystem.CanDrop(player, toInsert.Value))
		{
			_popupSystem.PopupClient(base.Loc.GetString("comp-storage-cant-drop", (ValueTuple<string, object>)("entity", toInsert.Value)), Entity<StorageComponent>.op_Implicit(ent), Entity<HandsComponent>.op_Implicit(player));
			return false;
		}
		return PlayerInsertEntityInWorld(Entity<StorageComponent>.op_Implicit((Entity<StorageComponent>.op_Implicit(ent), ent.Comp)), Entity<HandsComponent>.op_Implicit(player), toInsert.Value);
	}

	public bool PlayerInsertEntityInWorld(Entity<StorageComponent?> uid, EntityUid player, EntityUid toInsert, bool playSound = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StorageComponent>(Entity<StorageComponent>.op_Implicit(uid), ref uid.Comp, true))
		{
			SharedInteractionSystem interactionSystem = _interactionSystem;
			Entity<TransformComponent> origin = Entity<TransformComponent>.op_Implicit(player);
			Entity<TransformComponent> other = Entity<TransformComponent>.op_Implicit(uid.Owner);
			EntityUid? stackedEntity = null;
			if (interactionSystem.InRangeUnobstructed(origin, other, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, popup: false, overlapCheck: true, stackedEntity))
			{
				if (!Insert(Entity<StorageComponent>.op_Implicit(uid), toInsert, out stackedEntity, player, uid.Comp, playSound))
				{
					_popupSystem.PopupClient(base.Loc.GetString("comp-storage-cant-insert"), Entity<StorageComponent>.op_Implicit(uid), player);
					return false;
				}
				return true;
			}
		}
		return false;
	}

	public bool TrySetItemStorageLocation(Entity<ItemComponent?> itemEnt, Entity<StorageComponent?> storageEnt, ItemStorageLocation location)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ItemComponent>(Entity<ItemComponent>.op_Implicit(itemEnt), ref itemEnt.Comp, true) || !((EntitySystem)this).Resolve<StorageComponent>(Entity<StorageComponent>.op_Implicit(storageEnt), ref storageEnt.Comp, true))
		{
			return false;
		}
		if (!((BaseContainer)storageEnt.Comp.Container).ContainedEntities.Contains(Entity<ItemComponent>.op_Implicit(itemEnt)))
		{
			return false;
		}
		if (!ItemFitsInGridLocation(itemEnt, storageEnt, location.Position, location.Rotation))
		{
			return false;
		}
		if (storageEnt.Comp.StoredItems.Remove(Entity<ItemComponent>.op_Implicit(itemEnt), out var existing))
		{
			RemoveOccupiedEntity(Entity<StorageComponent>.op_Implicit((storageEnt.Owner, storageEnt.Comp)), itemEnt, existing);
		}
		storageEnt.Comp.StoredItems.Add(Entity<ItemComponent>.op_Implicit(itemEnt), location);
		AddOccupiedEntity(Entity<StorageComponent>.op_Implicit((storageEnt.Owner, storageEnt.Comp)), itemEnt, location);
		UpdateUI(storageEnt);
		return true;
	}

	public bool TryGetAvailableGridSpace(Entity<StorageComponent?> storageEnt, Entity<ItemComponent?> itemEnt, [NotNullWhen(true)] out ItemStorageLocation? storageLocation)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		storageLocation = null;
		if (!((EntitySystem)this).Resolve<StorageComponent>(Entity<StorageComponent>.op_Implicit(storageEnt), ref storageEnt.Comp, true) || !((EntitySystem)this).Resolve<ItemComponent>(Entity<ItemComponent>.op_Implicit(itemEnt), ref itemEnt.Comp, true))
		{
			return false;
		}
		if (FindSavedLocation(storageEnt, itemEnt, out storageLocation))
		{
			return true;
		}
		Box2i storageBounding = storageEnt.Comp.Grid.GetBoundingBox();
		if (!storageEnt.Comp.DefaultStorageOrientation.HasValue)
		{
			Angle.FromDegrees((double)(0f - itemEnt.Comp.StoredRotation));
		}
		else if (((Box2i)(ref storageBounding)).Width < ((Box2i)(ref storageBounding)).Height)
		{
			if (storageEnt.Comp.DefaultStorageOrientation != StorageDefaultOrientation.Horizontal)
			{
				Angle.FromDegrees(90.0);
			}
			else
			{
				_ = Angle.Zero;
			}
		}
		else if (storageEnt.Comp.DefaultStorageOrientation != StorageDefaultOrientation.Vertical)
		{
			Angle.FromDegrees(90.0);
		}
		else
		{
			_ = Angle.Zero;
		}
		for (int y = storageBounding.Bottom; y <= storageBounding.Top; y++)
		{
			for (int x = storageBounding.Left; x <= storageBounding.Right; x++)
			{
				ItemStorageLocation location = new ItemStorageLocation(Angle.Zero, Vector2i.op_Implicit((x, y)));
				if (ItemFitsInGridLocation(itemEnt, storageEnt, location))
				{
					storageLocation = location;
					return true;
				}
			}
		}
		return false;
	}

	public bool FindSavedLocation(Entity<StorageComponent?> ent, Entity<ItemComponent?> item, [NotNullWhen(true)] out ItemStorageLocation? storageLocation)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		storageLocation = null;
		if (!((EntitySystem)this).Resolve<StorageComponent>(Entity<StorageComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return false;
		}
		string name = ((EntitySystem)this).Name(Entity<ItemComponent>.op_Implicit(item), (MetaDataComponent)null);
		if (!ent.Comp.SavedLocations.TryGetValue(name, out List<ItemStorageLocation> list))
		{
			return false;
		}
		foreach (ItemStorageLocation location in list)
		{
			if (ItemFitsInGridLocation(item, ent, location))
			{
				storageLocation = location;
				return true;
			}
		}
		return false;
	}

	public void SaveItemLocation(Entity<StorageComponent?> ent, Entity<MetaDataComponent?> item)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StorageComponent>(Entity<StorageComponent>.op_Implicit(ent), ref ent.Comp, true) || !ent.Comp.StoredItems.TryGetValue(Entity<MetaDataComponent>.op_Implicit(item), out var location))
		{
			return;
		}
		string name = ((EntitySystem)this).Name(Entity<MetaDataComponent>.op_Implicit(item), item.Comp);
		if (ent.Comp.SavedLocations.TryGetValue(name, out List<ItemStorageLocation> list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] == location)
				{
					list.Remove(location);
					return;
				}
			}
			list.Add(location);
		}
		else
		{
			list = new List<ItemStorageLocation> { location };
			ent.Comp.SavedLocations[name] = list;
		}
		((EntitySystem)this).Dirty(Entity<StorageComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		UpdateUI(Entity<StorageComponent>.op_Implicit((ent.Owner, ent.Comp)));
	}

	public bool ItemFitsInGridLocation(Entity<ItemComponent?> itemEnt, Entity<StorageComponent?> storageEnt, ItemStorageLocation location)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ItemComponent>(Entity<ItemComponent>.op_Implicit(itemEnt), ref itemEnt.Comp, true) || !((EntitySystem)this).Resolve<StorageComponent>(Entity<StorageComponent>.op_Implicit(storageEnt), ref storageEnt.Comp, true))
		{
			return false;
		}
		Vector2i position = location.Position;
		Angle rotation = location.Rotation;
		Box2i gridBounds = storageEnt.Comp.Grid.GetBoundingBox();
		if (!((Box2i)(ref gridBounds)).Contains(position, true))
		{
			return false;
		}
		foreach (Box2i box in ItemSystem.GetAdjustedItemShape(storageEnt, itemEnt, rotation, position))
		{
			for (int offsetY = box.Bottom; offsetY <= box.Top; offsetY++)
			{
				for (int offsetX = box.Left; offsetX <= box.Right; offsetX++)
				{
					if (!IsGridSpaceEmpty(location: Vector2i.op_Implicit((offsetX, offsetY)), itemEnt: itemEnt, storageEnt: storageEnt))
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public bool ItemFitsInGridLocation(Entity<ItemComponent?> itemEnt, Entity<StorageComponent?> storageEnt, Vector2i position, Angle rotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ItemComponent>(Entity<ItemComponent>.op_Implicit(itemEnt), ref itemEnt.Comp, true) || !((EntitySystem)this).Resolve<StorageComponent>(Entity<StorageComponent>.op_Implicit(storageEnt), ref storageEnt.Comp, true))
		{
			return false;
		}
		Box2i gridBounds = storageEnt.Comp.Grid.GetBoundingBox();
		if (!((Box2i)(ref gridBounds)).Contains(position, true))
		{
			return false;
		}
		foreach (Box2i box in ItemSystem.GetAdjustedItemShape(storageEnt, itemEnt, rotation, position))
		{
			for (int offsetY = box.Bottom; offsetY <= box.Top; offsetY++)
			{
				for (int offsetX = box.Left; offsetX <= box.Right; offsetX++)
				{
					if (!IsGridSpaceEmpty(location: Vector2i.op_Implicit((offsetX, offsetY)), itemEnt: itemEnt, storageEnt: storageEnt))
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public bool IsGridSpaceEmpty(Entity<ItemComponent?> itemEnt, Entity<StorageComponent?> storageEnt, Vector2i location)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StorageComponent>(Entity<StorageComponent>.op_Implicit(storageEnt), ref storageEnt.Comp, true))
		{
			return false;
		}
		bool validGrid = false;
		foreach (Box2i item in storageEnt.Comp.Grid)
		{
			Box2i grid = item;
			if (((Box2i)(ref grid)).Contains(location, true))
			{
				validGrid = true;
				break;
			}
		}
		if (!validGrid)
		{
			return false;
		}
		ItemComponent itemComp = default(ItemComponent);
		foreach (var (ent, storedItem) in storageEnt.Comp.StoredItems)
		{
			if (ent == itemEnt.Owner || !_itemQuery.TryGetComponent(ent, ref itemComp))
			{
				continue;
			}
			foreach (Box2i item2 in ItemSystem.GetAdjustedItemShape(storageEnt, Entity<ItemComponent>.op_Implicit((ent, itemComp)), storedItem))
			{
				Box2i box = item2;
				if (((Box2i)(ref box)).Contains(location, true))
				{
					return false;
				}
			}
		}
		return true;
	}

	protected void UpdateOccupied(Entity<StorageComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.OccupiedGrid.Clear();
		RemoveOccupied(ent.Comp.Grid, ent.Comp.OccupiedGrid);
		((EntitySystem)this).Dirty<StorageComponent>(ent, (MetaDataComponent)null);
		ItemComponent itemComp = default(ItemComponent);
		foreach (var (stent, storedItem) in ent.Comp.StoredItems)
		{
			if (_itemQuery.TryGetComponent(stent, ref itemComp))
			{
				AddOccupiedEntity(ent, Entity<ItemComponent>.op_Implicit((stent, itemComp)), storedItem);
			}
		}
	}

	private void AddOccupiedEntity(Entity<StorageComponent> storageEnt, Entity<ItemComponent?> itemEnt, ItemStorageLocation location)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		AddOccupied(Entity<StorageComponent>.op_Implicit((Entity<StorageComponent>.op_Implicit(storageEnt), Entity<StorageComponent>.op_Implicit(storageEnt))), itemEnt, location, storageEnt.Comp.OccupiedGrid);
		((EntitySystem)this).Dirty<StorageComponent>(storageEnt, (MetaDataComponent)null);
	}

	private void AddOccupied(Entity<StorageComponent?> storageEnt, Entity<ItemComponent?> itemEnt, ItemStorageLocation location, Dictionary<Vector2i, ulong> occupied)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		IReadOnlyList<Box2i> adjustedShape = ItemSystem.GetAdjustedItemShape(storageEnt, Entity<ItemComponent>.op_Implicit((itemEnt.Owner, itemEnt.Comp)), location);
		AddOccupied(adjustedShape, occupied);
	}

	private void RemoveOccupied(IReadOnlyList<Box2i> adjustedShape, Dictionary<Vector2i, ulong> occupied)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		ChunkIndicesEnumerator chunks = default(ChunkIndicesEnumerator);
		Vector2i? chunk = default(Vector2i?);
		foreach (Box2i box in adjustedShape)
		{
			((ChunkIndicesEnumerator)(ref chunks))._002Ector(Box2i.op_Implicit(box), 8);
			while (((ChunkIndicesEnumerator)(ref chunks)).MoveNext(ref chunk))
			{
				Vector2i chunkOrigin = chunk.Value * 8;
				int num = Math.Max(box.Left, chunkOrigin.X);
				int bottom = Math.Max(box.Bottom, chunkOrigin.Y);
				int right = Math.Min(box.Right, chunkOrigin.X + 8 - 1);
				int top = Math.Min(box.Top, chunkOrigin.Y + 8 - 1);
				ulong existing = occupied.GetValueOrDefault(chunkOrigin, ulong.MaxValue);
				for (int x = num; x <= right; x++)
				{
					for (int y = bottom; y <= top; y++)
					{
						ulong flag = SharedMapSystem.ToBitmask(SharedMapSystem.GetChunkRelative(new Vector2i(x, y), (byte)8), (byte)8);
						existing &= ~flag;
					}
				}
				occupied[chunkOrigin] = existing;
			}
		}
	}

	private void AddOccupied(IReadOnlyList<Box2i> adjustedShape, Dictionary<Vector2i, ulong> occupied)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		ChunkIndicesEnumerator chunkEnumerator = default(ChunkIndicesEnumerator);
		Vector2i? chunk = default(Vector2i?);
		foreach (Box2i box in adjustedShape)
		{
			((ChunkIndicesEnumerator)(ref chunkEnumerator))._002Ector(Box2i.op_Implicit(box), 8);
			while (((ChunkIndicesEnumerator)(ref chunkEnumerator)).MoveNext(ref chunk))
			{
				Vector2i chunkOrigin = chunk.Value * 8;
				ulong existing = Extensions.GetOrNew<Vector2i, ulong>(occupied, chunkOrigin);
				int num = Math.Max(chunkOrigin.X, box.Left);
				int bottom = Math.Max(chunkOrigin.Y, box.Bottom);
				int right = Math.Min(chunkOrigin.X + 8 - 1, box.Right);
				int top = Math.Min(chunkOrigin.Y + 8 - 1, box.Top);
				for (int x = num; x <= right; x++)
				{
					for (int y = bottom; y <= top; y++)
					{
						ulong flag = SharedMapSystem.ToBitmask(SharedMapSystem.GetChunkRelative(new Vector2i(x, y), (byte)8), (byte)8);
						existing |= flag;
					}
				}
				occupied[chunkOrigin] = existing;
			}
		}
	}

	private void RemoveOccupiedEntity(Entity<StorageComponent> storageEnt, Entity<ItemComponent?> itemEnt, ItemStorageLocation location)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		IReadOnlyList<Box2i> adjustedShape = ItemSystem.GetAdjustedItemShape(Entity<StorageComponent>.op_Implicit((Entity<StorageComponent>.op_Implicit(storageEnt), Entity<StorageComponent>.op_Implicit(storageEnt))), Entity<ItemComponent>.op_Implicit((itemEnt.Owner, itemEnt.Comp)), location);
		RemoveOccupied(adjustedShape, storageEnt.Comp.OccupiedGrid);
		((EntitySystem)this).Dirty<StorageComponent>(storageEnt, (MetaDataComponent)null);
	}

	public bool HasSpace(Entity<StorageComponent?> uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StorageComponent>(Entity<StorageComponent>.op_Implicit(uid), ref uid.Comp, true))
		{
			return false;
		}
		if (GetCumulativeItemAreas(uid) >= uid.Comp.Grid.GetArea())
		{
			return HasSpaceInStacks(uid);
		}
		return true;
	}

	private bool HasSpaceInStacks(Entity<StorageComponent?> uid, string? stackType = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StorageComponent>(Entity<StorageComponent>.op_Implicit(uid), ref uid.Comp, true))
		{
			return false;
		}
		StackComponent stack = default(StackComponent);
		foreach (EntityUid contained in ((BaseContainer)uid.Comp.Container).ContainedEntities)
		{
			if (_stackQuery.TryGetComponent(contained, ref stack) && (stackType == null || stack.StackTypeId.Equals(stackType)) && _stack.GetAvailableSpace(stack) != 0)
			{
				return true;
			}
		}
		return false;
	}

	public int GetCumulativeItemAreas(Entity<StorageComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StorageComponent>(Entity<StorageComponent>.op_Implicit(entity), ref entity.Comp, true))
		{
			return 0;
		}
		int sum = 0;
		ItemComponent itemComp = default(ItemComponent);
		foreach (EntityUid item in ((BaseContainer)entity.Comp.Container).ContainedEntities)
		{
			if (_itemQuery.TryGetComponent(item, ref itemComp))
			{
				sum += ItemSystem.GetItemShape(entity, Entity<ItemComponent>.op_Implicit((item, itemComp))).GetArea();
			}
		}
		return sum;
	}

	public ItemSizePrototype GetMaxItemSize(Entity<StorageComponent?> uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StorageComponent>(Entity<StorageComponent>.op_Implicit(uid), ref uid.Comp, true))
		{
			return _defaultStorageMaxItemSize;
		}
		if (uid.Comp.MaxItemSize.HasValue)
		{
			ItemSizePrototype proto = default(ItemSizePrototype);
			if (_prototype.TryIndex<ItemSizePrototype>(uid.Comp.MaxItemSize.Value, ref proto))
			{
				return proto;
			}
			((EntitySystem)this).Log.Error($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid.Owner))} tried to get invalid item size prototype: {uid.Comp.MaxItemSize.Value}. Stack trace:\\n{Environment.StackTrace}");
		}
		ItemComponent item = default(ItemComponent);
		if (!_itemQuery.TryGetComponent(Entity<StorageComponent>.op_Implicit(uid), ref item))
		{
			return _defaultStorageMaxItemSize;
		}
		return _nextSmallest[ProtoId<ItemSizePrototype>.op_Implicit(item.Size)];
	}

	private void OnLockToggled(EntityUid uid, StorageComponent component, ref LockToggledEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Locked)
		{
			return;
		}
		foreach (EntityUid actor in UI.GetActors(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)StorageComponent.StorageUiKey.Key).ToList())
		{
			if (!CanInteract(actor, Entity<StorageComponent>.op_Implicit((uid, component))))
			{
				UI.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)StorageComponent.StorageUiKey.Key, (EntityUid?)actor, false);
			}
		}
	}

	private void OnStackCountChanged(EntityUid uid, MetaDataComponent component, StackCountChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (ContainerSystem.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(uid, null, component)), ref container) && container.ID == StorageComponent.ContainerId)
		{
			UpdateAppearance(Entity<StorageComponent, AppearanceComponent>.op_Implicit(container.Owner));
			UpdateUI(Entity<StorageComponent>.op_Implicit(container.Owner));
		}
	}

	private void HandleOpenBackpack(ICommonSession? session)
	{
		HandleToggleSlotUI(session, "back");
	}

	private void HandleOpenBelt(ICommonSession? session)
	{
		HandleToggleSlotUI(session, "belt");
	}

	private void HandleToggleSlotUI(ICommonSession? session, string slot)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (session == null)
		{
			return;
		}
		EntityUid? attachedEntity = session.AttachedEntity;
		if (!attachedEntity.HasValue)
		{
			return;
		}
		EntityUid playerEnt = attachedEntity.GetValueOrDefault();
		if (((EntityUid)(ref playerEnt)).Valid && ((EntitySystem)this).Exists(playerEnt) && _inventory.TryGetSlotEntity(playerEnt, slot, out var storageEnt) && ActionBlocker.CanInteract(playerEnt, storageEnt))
		{
			if (!UI.IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(storageEnt.Value), (Enum)StorageComponent.StorageUiKey.Key, playerEnt))
			{
				OpenStorageUI(storageEnt.Value, playerEnt, null, silent: false);
			}
			else
			{
				UI.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(storageEnt.Value), (Enum)StorageComponent.StorageUiKey.Key, (EntityUid?)playerEnt, false);
			}
		}
	}

	protected void ClearCantFillReasons()
	{
	}

	private bool CanInteract(EntityUid user, Entity<StorageComponent> storage, bool canInteract = true, bool silent = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<BypassInteractionChecksComponent>(user))
		{
			return true;
		}
		if (!canInteract)
		{
			return false;
		}
		StorageInteractAttemptEvent ev = new StorageInteractAttemptEvent(user, silent);
		((EntitySystem)this).RaiseLocalEvent<StorageInteractAttemptEvent>(Entity<StorageComponent>.op_Implicit(storage), ref ev, false);
		return !ev.Cancelled;
	}

	public abstract void PlayPickupAnimation(EntityUid uid, EntityCoordinates initialCoordinates, EntityCoordinates finalCoordinates, Angle initialRotation, EntityUid? user = null);

	private bool ValidateInput(EntitySessionEventArgs args, NetEntity netStorage, out Entity<HandsComponent> player, out Entity<StorageComponent> storage)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		player = default(Entity<HandsComponent>);
		storage = default(Entity<StorageComponent>);
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid playerUid = attachedEntity.GetValueOrDefault();
			HandsComponent hands = default(HandsComponent);
			if (!((EntitySystem)this).TryComp<HandsComponent>(playerUid, ref hands) || hands.Count == 0)
			{
				return false;
			}
			EntityUid? storageUid = default(EntityUid?);
			if (!((EntitySystem)this).TryGetEntity(netStorage, ref storageUid))
			{
				return false;
			}
			StorageComponent storageComp = default(StorageComponent);
			if (!((EntitySystem)this).TryComp<StorageComponent>(storageUid, ref storageComp))
			{
				return false;
			}
			if (!UI.IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(storageUid.Value), (Enum)StorageComponent.StorageUiKey.Key, playerUid))
			{
				return false;
			}
			if (!ActionBlocker.CanInteract(playerUid, storageUid))
			{
				return false;
			}
			player = new Entity<HandsComponent>(playerUid, hands);
			storage = new Entity<StorageComponent>(storageUid.Value, storageComp);
			return true;
		}
		return false;
	}

	private bool ValidateInput(EntitySessionEventArgs args, NetEntity netStorage, NetEntity netItem, out Entity<HandsComponent> player, out Entity<StorageComponent> storage, out Entity<ItemComponent> item, bool held = false)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		item = default(Entity<ItemComponent>);
		if (!ValidateInput(args, netStorage, out player, out storage))
		{
			return false;
		}
		EntityUid? itemUid = default(EntityUid?);
		if (!((EntitySystem)this).TryGetEntity(netItem, ref itemUid))
		{
			return false;
		}
		if (held)
		{
			if (!_sharedHandsSystem.IsHolding(player.AsNullable(), itemUid, out string _))
			{
				return false;
			}
		}
		else if (!((BaseContainer)storage.Comp.Container).Contains(itemUid.Value))
		{
			return false;
		}
		ItemComponent itemComp = default(ItemComponent);
		if (!((EntitySystem)this).TryComp<ItemComponent>(itemUid, ref itemComp))
		{
			return false;
		}
		if (!ActionBlocker.CanInteract(Entity<HandsComponent>.op_Implicit(player), itemUid))
		{
			return false;
		}
		item = new Entity<ItemComponent>(itemUid.Value, itemComp);
		return true;
	}

	static SharedStorageSystem()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		AudioParams val = ((AudioParams)(ref AudioParams.Default)).WithMaxDistance(7f);
		_audioParams = ((AudioParams)(ref val)).WithVolume(-2f);
	}
}
