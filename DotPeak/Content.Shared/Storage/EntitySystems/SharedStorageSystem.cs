// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.EntitySystems.SharedStorageSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared.Storage.EntitySystems;

public abstract class SharedStorageSystem : EntitySystem
{
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
  private Robust.Shared.GameObjects.EntityQuery<ItemComponent> _itemQuery;
  private Robust.Shared.GameObjects.EntityQuery<StackComponent> _stackQuery;
  private Robust.Shared.GameObjects.EntityQuery<TransformComponent> _xformQuery;
  private Robust.Shared.GameObjects.EntityQuery<UserInterfaceUserComponent> _userQuery;
  public bool NestedStorage = true;
  public static readonly ProtoId<ItemSizePrototype> DefaultStorageMaxItemSize = (ProtoId<ItemSizePrototype>) "Normal";
  public const float AreaInsertDelayPerItem = 0.075f;
  private static AudioParams _audioParams = AudioParams.Default.WithMaxDistance(7f).WithVolume(-2f);
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
    base.Initialize();
    this._itemQuery = this.GetEntityQuery<ItemComponent>();
    this._stackQuery = this.GetEntityQuery<StackComponent>();
    this._xformQuery = this.GetEntityQuery<TransformComponent>();
    this._userQuery = this.GetEntityQuery<UserInterfaceUserComponent>();
    this._prototype.PrototypesReloaded += new Action<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded);
    this.Subs.CVar<int>(this._cfg, CCVars.StorageLimit, new Action<int>(this.OnStorageLimitChanged), true);
    this.Subs.BuiEvents<StorageComponent>((object) StorageComponent.StorageUiKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<StorageComponent>) (subs => subs.Event<BoundUIClosedEvent>(new ComponentEventHandler<StorageComponent, BoundUIClosedEvent>(this.OnBoundUIClosed))));
    this.SubscribeLocalEvent<StorageComponent, ComponentRemove>(new EntityEventRefHandler<StorageComponent, ComponentRemove>(this.OnRemove));
    this.SubscribeLocalEvent<StorageComponent, MapInitEvent>(new EntityEventRefHandler<StorageComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<StorageComponent, GetVerbsEvent<ActivationVerb>>(new ComponentEventHandler<StorageComponent, GetVerbsEvent<ActivationVerb>>(this.AddUiVerb));
    this.SubscribeLocalEvent<StorageComponent, ComponentGetState>(new ComponentEventRefHandler<StorageComponent, ComponentGetState>(this.OnStorageGetState));
    this.SubscribeLocalEvent<StorageComponent, ComponentInit>(new ComponentEventHandler<StorageComponent, ComponentInit>(this.OnComponentInit), new Type[1]
    {
      typeof (SharedContainerSystem)
    });
    this.SubscribeLocalEvent<StorageComponent, GetVerbsEvent<UtilityVerb>>(new ComponentEventHandler<StorageComponent, GetVerbsEvent<UtilityVerb>>(this.AddTransferVerbs));
    this.SubscribeLocalEvent<StorageComponent, InteractUsingEvent>(new ComponentEventHandler<StorageComponent, InteractUsingEvent>(this.OnInteractUsing), after: new Type[1]
    {
      typeof (ItemSlotsSystem)
    });
    this.SubscribeLocalEvent<StorageComponent, ActivateInWorldEvent>(new ComponentEventHandler<StorageComponent, ActivateInWorldEvent>(this.OnActivate));
    this.SubscribeLocalEvent<StorageComponent, OpenStorageImplantEvent>(new ComponentEventHandler<StorageComponent, OpenStorageImplantEvent>(this.OnImplantActivate));
    this.SubscribeLocalEvent<StorageComponent, AfterInteractEvent>(new ComponentEventHandler<StorageComponent, AfterInteractEvent>(this.AfterInteract));
    this.SubscribeLocalEvent<StorageComponent, DestructionEventArgs>(new ComponentEventHandler<StorageComponent, DestructionEventArgs>(this.OnDestroy));
    this.SubscribeLocalEvent<StorageComponent, BoundUserInterfaceMessageAttempt>(new EntityEventRefHandler<StorageComponent, BoundUserInterfaceMessageAttempt>(this.OnBoundUIAttempt));
    this.SubscribeLocalEvent<StorageComponent, BoundUIOpenedEvent>(new EntityEventRefHandler<StorageComponent, BoundUIOpenedEvent>(this.OnBoundUIOpen));
    this.SubscribeLocalEvent<StorageComponent, LockToggledEvent>(new ComponentEventRefHandler<StorageComponent, LockToggledEvent>(this.OnLockToggled));
    this.SubscribeLocalEvent<StorageComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<StorageComponent, EntInsertedIntoContainerMessage>(this.OnEntInserted));
    this.SubscribeLocalEvent<StorageComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<StorageComponent, EntRemovedFromContainerMessage>(this.OnEntRemoved));
    this.SubscribeLocalEvent<StorageComponent, ContainerIsInsertingAttemptEvent>(new ComponentEventHandler<StorageComponent, ContainerIsInsertingAttemptEvent>(this.OnInsertAttempt));
    this.SubscribeLocalEvent<StorageComponent, AreaPickupDoAfterEvent>(new ComponentEventHandler<StorageComponent, AreaPickupDoAfterEvent>(this.OnDoAfter));
    this.SubscribeLocalEvent<StorageComponent, GotReclaimedEvent>(new ComponentEventHandler<StorageComponent, GotReclaimedEvent>(this.OnReclaimed));
    this.SubscribeLocalEvent<MetaDataComponent, StackCountChangedEvent>(new ComponentEventHandler<MetaDataComponent, StackCountChangedEvent>(this.OnStackCountChanged));
    this.SubscribeAllEvent<OpenNestedStorageEvent>(new EntitySessionEventHandler<OpenNestedStorageEvent>(this.OnStorageNested));
    this.SubscribeAllEvent<StorageTransferItemEvent>(new EntitySessionEventHandler<StorageTransferItemEvent>(this.OnStorageTransfer));
    this.SubscribeAllEvent<StorageInteractWithItemEvent>(new EntitySessionEventHandler<StorageInteractWithItemEvent>(this.OnInteractWithItem));
    this.SubscribeAllEvent<StorageSetItemLocationEvent>(new EntitySessionEventHandler<StorageSetItemLocationEvent>(this.OnSetItemLocation));
    this.SubscribeAllEvent<StorageInsertItemIntoLocationEvent>(new EntitySessionEventHandler<StorageInsertItemIntoLocationEvent>(this.OnInsertItemIntoLocation));
    this.SubscribeAllEvent<StorageSaveItemLocationEvent>(new EntitySessionEventHandler<StorageSaveItemLocationEvent>(this.OnSaveItemLocation));
    this.SubscribeLocalEvent<ItemSizeChangedEvent>(new EntityEventRefHandler<ItemSizeChangedEvent>(this.OnItemSizeChanged));
    CommandBinds.Builder.Bind(ContentKeyFunctions.OpenBackpack, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(this.HandleOpenBackpack), handle: false)).Bind(ContentKeyFunctions.OpenBelt, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(this.HandleOpenBelt), handle: false)).Register<SharedStorageSystem>();
    this.Subs.CVar<bool>(this._cfg, CCVars.NestedStorage, new Action<bool>(this.OnNestedStorageCvar), true);
    this.UpdatePrototypeCache();
  }

  private void OnItemSizeChanged(ref ItemSizeChangedEvent ev)
  {
    Entity<ItemComponent> itemEnt = new Entity<ItemComponent>(ev.Entity, (ItemComponent) null);
    BaseContainer container;
    StorageComponent storage;
    ItemStorageLocation loc;
    if (!this.TryGetStorageLocation(itemEnt, out container, out storage, out loc))
      return;
    this.UpdateOccupied((Entity<StorageComponent>) (container.Owner, storage));
    if (this.ItemFitsInGridLocation((Entity<ItemComponent>) (itemEnt.Owner, itemEnt.Comp), (Entity<StorageComponent>) (container.Owner, storage), loc))
      return;
    this.ContainerSystem.Remove((Entity<TransformComponent, MetaDataComponent>) itemEnt.Owner, container, force: true);
  }

  private void OnNestedStorageCvar(bool obj) => this.NestedStorage = obj;

  private void OnStorageLimitChanged(int obj) => this._openStorageLimit = obj;

  private void OnRemove(Entity<StorageComponent> entity, ref ComponentRemove args)
  {
    this.UI.CloseUi((Entity<UserInterfaceComponent>) entity.Owner, (Enum) StorageComponent.StorageUiKey.Key);
  }

  private void OnMapInit(Entity<StorageComponent> entity, ref MapInitEvent args)
  {
    int num = this.HasComp<UseDelayComponent>((EntityUid) entity) ? 1 : 0;
    this.UseDelay.SetLength((Entity<UseDelayComponent>) entity.Owner, entity.Comp.QuickInsertCooldown, "quickInsert");
    this.UseDelay.SetLength((Entity<UseDelayComponent>) entity.Owner, entity.Comp.OpenUiCooldown, "storage");
    if (num != 0)
      return;
    this.UseDelay.SetLength((Entity<UseDelayComponent>) entity.Owner, TimeSpan.Zero);
  }

  private void OnStorageGetState(
    EntityUid uid,
    StorageComponent component,
    ref ComponentGetState args)
  {
    Dictionary<NetEntity, ItemStorageLocation> dictionary = new Dictionary<NetEntity, ItemStorageLocation>();
    foreach ((EntityUid entityUid, ItemStorageLocation itemStorageLocation) in component.StoredItems)
      dictionary[this.GetNetEntity(entityUid)] = itemStorageLocation;
    args.State = (IComponentState) new SharedStorageSystem.StorageComponentState()
    {
      Grid = new List<Box2i>((IEnumerable<Box2i>) component.Grid),
      MaxItemSize = component.MaxItemSize,
      StoredItems = dictionary,
      SavedLocations = component.SavedLocations,
      Whitelist = component.Whitelist,
      Blacklist = component.Blacklist
    };
  }

  public override void Shutdown()
  {
    this._prototype.PrototypesReloaded -= new Action<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded);
  }

  private void OnPrototypesReloaded(PrototypesReloadedEventArgs args)
  {
    if (!args.ByType.ContainsKey(typeof (ItemSizePrototype)))
    {
      IReadOnlyDictionary<Type, HashSet<string>> removed = args.Removed;
      if ((removed != null ? (removed.ContainsKey(typeof (ItemSizePrototype)) ? 1 : 0) : 0) == 0)
        return;
    }
    this.UpdatePrototypeCache();
  }

  private void UpdatePrototypeCache()
  {
    this._defaultStorageMaxItemSize = this._prototype.Index<ItemSizePrototype>(SharedStorageSystem.DefaultStorageMaxItemSize);
    this._sortedSizes.Clear();
    this._sortedSizes.AddRange(this._prototype.EnumeratePrototypes<ItemSizePrototype>());
    this._sortedSizes.Sort();
    KeyValuePair<string, ItemSizePrototype>[] source = new KeyValuePair<string, ItemSizePrototype>[this._sortedSizes.Count];
    for (int index = 0; index < this._sortedSizes.Count; ++index)
    {
      string id = this._sortedSizes[index].ID;
      ItemSizePrototype sortedSiz = this._sortedSizes[Math.Max(index - 1, 0)];
      source[index] = new KeyValuePair<string, ItemSizePrototype>(id, sortedSiz);
    }
    this._nextSmallest = ((IEnumerable<KeyValuePair<string, ItemSizePrototype>>) source).ToFrozenDictionary<string, ItemSizePrototype>();
  }

  private void OnComponentInit(EntityUid uid, StorageComponent storageComp, ComponentInit args)
  {
    storageComp.Container = this.ContainerSystem.EnsureContainer<Container>(uid, StorageComponent.ContainerId);
    this.UpdateAppearance((Entity<StorageComponent, AppearanceComponent>) (uid, storageComp, (AppearanceComponent) null));
    this.UpdateOccupied((Entity<StorageComponent>) (uid, storageComp));
  }

  private void CloseNestedInterfaces(EntityUid uid, EntityUid actor, StorageComponent? storageComp = null)
  {
    if (!this.Resolve<StorageComponent>(uid, ref storageComp))
      return;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) storageComp.Container.ContainedEntities)
    {
      if (!this.HasComp<RMCItemKeepUIOpenOnStorageClosedComponent>(containedEntity))
        this.UI.CloseUis((Entity<UserInterfaceComponent>) containedEntity, actor);
    }
  }

  private void OnBoundUIClosed(
    EntityUid uid,
    StorageComponent storageComp,
    BoundUIClosedEvent args)
  {
    this.CloseNestedInterfaces(uid, args.Actor, storageComp);
    if (this.UI.IsUiOpen((Entity<UserInterfaceComponent>) uid, args.UiKey))
      return;
    this.UpdateAppearance((Entity<StorageComponent, AppearanceComponent>) (uid, storageComp, (AppearanceComponent) null));
    if (this._tag.HasTag(args.Actor, storageComp.SilentStorageUserTag))
      return;
    this.Audio.PlayPredicted(storageComp.StorageCloseSound, uid, new EntityUid?(args.Actor));
  }

  private void AddUiVerb(
    EntityUid uid,
    StorageComponent component,
    GetVerbsEvent<ActivationVerb> args)
  {
    if (!component.ShowVerb || !this.CanInteract(args.User, (Entity<StorageComponent>) (uid, component), args.CanAccess && args.CanInteract))
      return;
    bool uiOpen = this.UI.IsUiOpen((Entity<UserInterfaceComponent>) uid, (Enum) StorageComponent.StorageUiKey.Key, args.User);
    ActivationVerb activationVerb1 = new ActivationVerb();
    activationVerb1.Act = (Action) (() =>
    {
      if (uiOpen)
        this.UI.CloseUi((Entity<UserInterfaceComponent>) uid, (Enum) StorageComponent.StorageUiKey.Key, new EntityUid?(args.User));
      else
        this.OpenStorageUI(uid, args.User, component, false);
    });
    ActivationVerb activationVerb2 = activationVerb1;
    if (uiOpen)
    {
      activationVerb2.Text = this.Loc.GetString("comp-storage-verb-close-storage");
      activationVerb2.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/close.svg.192dpi.png"));
    }
    else
    {
      activationVerb2.Text = this.Loc.GetString("comp-storage-verb-open-storage");
      activationVerb2.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/open.svg.192dpi.png"));
    }
    args.Verbs.Add(activationVerb2);
  }

  public bool TryGetStorageLocation(
    Entity<ItemComponent?> itemEnt,
    [NotNullWhen(true)] out BaseContainer? container,
    [NotNullWhen(true)] out StorageComponent? storage,
    out ItemStorageLocation loc)
  {
    loc = new ItemStorageLocation();
    storage = (StorageComponent) null;
    if (!this.ContainerSystem.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) itemEnt.Owner, out container) || container.ID != StorageComponent.ContainerId || !this.TryComp<StorageComponent>(container.Owner, out storage) || !this._itemQuery.Resolve((EntityUid) itemEnt, ref itemEnt.Comp, false))
      return false;
    loc = storage.StoredItems[(EntityUid) itemEnt];
    return true;
  }

  public void OpenStorageUI(
    EntityUid uid,
    EntityUid actor,
    StorageComponent? storageComp = null,
    bool silent = true,
    bool doAfter = true)
  {
    BaseContainer container;
    if (this.ContainerSystem.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) uid, out container) && this.UI.IsUiOpen((Entity<UserInterfaceComponent>) container.Owner, (Enum) StorageComponent.StorageUiKey.Key, actor))
    {
      this._nestedCheck = true;
      this.HideStorageWindow(container.Owner, actor);
      this.OpenStorageUIInternal(uid, actor, storageComp, doAfter: doAfter);
      this._nestedCheck = false;
    }
    else
    {
      if (this._openStorageLimit == 1)
        this.UI.CloseUserUis<StorageComponent.StorageUiKey>((Entity<UserInterfaceUserComponent>) actor);
      this.OpenStorageUIInternal(uid, actor, storageComp, silent, doAfter);
    }
  }

  private void OpenStorageUIInternal(
    EntityUid uid,
    EntityUid entity,
    StorageComponent? storageComp = null,
    bool silent = true,
    bool doAfter = true)
  {
    if (doAfter && this.RMCStorage.OpenDoAfter(uid, entity, storageComp, silent) || !this.Resolve<StorageComponent>(uid, ref storageComp, false))
      return;
    UseDelayComponent comp;
    silent = ((silent ? 1 : 0) | (!this.TryComp<UseDelayComponent>(uid, out comp) ? 0 : (this.UseDelay.IsDelayed((Entity<UseDelayComponent>) (uid, comp), "storage") ? 1 : 0))) != 0;
    if (!this.CanInteract(entity, (Entity<StorageComponent>) (uid, storageComp), silent: silent) || !this.UI.TryOpenUi((Entity<UserInterfaceComponent>) uid, (Enum) StorageComponent.StorageUiKey.Key, entity) || silent || this._tag.HasTag(entity, storageComp.SilentStorageUserTag))
      return;
    this.Audio.PlayPredicted(storageComp.StorageOpenSound, uid, new EntityUid?(entity));
    if (comp == null)
      return;
    this.UseDelay.TryResetDelay((Entity<UseDelayComponent>) (uid, comp), id: "storage");
  }

  public virtual void UpdateUI(Entity<StorageComponent?> entity)
  {
  }

  private void AddTransferVerbs(
    EntityUid uid,
    StorageComponent component,
    GetVerbsEvent<UtilityVerb> args)
  {
    StorageComponent targetStorage;
    LockComponent targetLock;
    if (!args.CanAccess || !args.CanInteract || component.Container.ContainedEntities.Count == 0 || !this.CanInteract(args.User, (Entity<StorageComponent>) (uid, component)) || !this.TryComp<StorageComponent>(args.Target, out targetStorage) || this.TryComp<LockComponent>(args.Target, out targetLock) && targetLock.Locked)
      return;
    UtilityVerb utilityVerb1 = new UtilityVerb();
    utilityVerb1.Text = this.Loc.GetString("storage-component-transfer-verb");
    utilityVerb1.IconEntity = this.GetNetEntity(args.Using);
    utilityVerb1.Act = (Action) (() => this.TransferEntities(uid, args.Target, new EntityUid?(args.User), component, targetComp: targetStorage, targetLock: targetLock));
    UtilityVerb utilityVerb2 = utilityVerb1;
    args.Verbs.Add(utilityVerb2);
  }

  private void OnInteractUsing(
    EntityUid uid,
    StorageComponent storageComp,
    InteractUsingEvent args)
  {
    if (args.Handled || !storageComp.ClickInsert || !this.CanInteract(args.User, (Entity<StorageComponent>) (uid, storageComp), silent: false))
      return;
    StorageInteractUsingAttemptEvent args1 = new StorageInteractUsingAttemptEvent();
    this.RaiseLocalEvent<StorageInteractUsingAttemptEvent>(uid, ref args1);
    if (args1.Cancelled)
      return;
    StorageComponent comp;
    if (storageComp.AllowStorageTransfer && this.TryComp<StorageComponent>(args.Used, out comp) && args.Used != uid)
    {
      this.TransferEntities(args.Used, uid, new EntityUid?(args.User), comp, targetComp: storageComp);
      args.Handled = true;
    }
    else
    {
      this.PlayerInsertHeldEntity((Entity<StorageComponent>) (uid, storageComp), (Entity<HandsComponent>) args.User);
      args.Handled = true;
    }
  }

  private void OnActivate(EntityUid uid, StorageComponent storageComp, ActivateInWorldEvent args)
  {
    if (args.Handled || !args.Complex || !storageComp.OpenOnActivate || !this.CanInteract(args.User, (Entity<StorageComponent>) (uid, storageComp)))
      return;
    if (this.UI.IsUiOpen((Entity<UserInterfaceComponent>) uid, (Enum) StorageComponent.StorageUiKey.Key, args.User))
      this.UI.CloseUi((Entity<UserInterfaceComponent>) uid, (Enum) StorageComponent.StorageUiKey.Key, new EntityUid?(args.User));
    else
      this.OpenStorageUI(uid, args.User, storageComp, false);
    args.Handled = true;
  }

  protected virtual void HideStorageWindow(EntityUid uid, EntityUid actor)
  {
  }

  protected virtual void ShowStorageWindow(EntityUid uid, EntityUid actor)
  {
  }

  private void OnImplantActivate(
    EntityUid uid,
    StorageComponent storageComp,
    OpenStorageImplantEvent args)
  {
    if (args.Handled)
      return;
    if (this.UI.IsUiOpen((Entity<UserInterfaceComponent>) uid, (Enum) StorageComponent.StorageUiKey.Key, args.Performer))
      this.UI.CloseUi((Entity<UserInterfaceComponent>) uid, (Enum) StorageComponent.StorageUiKey.Key, new EntityUid?(args.Performer));
    else
      this.OpenStorageUI(uid, args.Performer, storageComp, false);
    args.Handled = true;
  }

  private void AfterInteract(EntityUid uid, StorageComponent storageComp, AfterInteractEvent args)
  {
    if (args.Handled || !args.CanReach || !this.UseDelay.TryResetDelay(uid, true, id: "quickInsert"))
      return;
    if (storageComp.AreaInsert && (!args.Target.HasValue || !this.HasComp<ItemComponent>(args.Target.Value)))
    {
      this._entList.Clear();
      this._entSet.Clear();
      this._entityLookupSystem.GetEntitiesInRange(args.ClickLocation, (float) storageComp.AreaInsertRadius, this._entSet, LookupFlags.Dynamic | LookupFlags.Sundries);
      float seconds = 0.0f;
      foreach (EntityUid ent in this._entSet)
      {
        ItemComponent component;
        ItemSizePrototype prototype;
        if (!(ent == args.User) && this._itemQuery.TryGetComponent(ent, out component) && this._prototype.TryIndex<ItemSizePrototype>(component.Size, out prototype) && this.CanInsert(uid, ent, new EntityUid?(args.User), out string _, storageComp, component) && this._interactionSystem.InRangeUnobstructed((Entity<TransformComponent>) args.User, (Entity<TransformComponent>) ent))
        {
          this._entList.Add(ent);
          seconds += (float) prototype.Weight * 0.075f;
          if (this._entList.Count >= 10)
            break;
        }
      }
      if (this._entList.Count < 1)
        return;
      this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, seconds, (DoAfterEvent) new AreaPickupDoAfterEvent(this.GetNetEntityList(this._entList)), new EntityUid?(uid), new EntityUid?(uid))
      {
        BreakOnDamage = true,
        BreakOnMove = true,
        NeedHand = true
      });
      args.Handled = true;
    }
    else
    {
      if (!storageComp.QuickInsert)
        return;
      EntityUid? target = args.Target;
      if (!target.HasValue)
        return;
      EntityUid valueOrDefault = target.GetValueOrDefault();
      TransformComponent comp1;
      TransformComponent comp2;
      if (!valueOrDefault.Valid || this.ContainerSystem.IsEntityInContainer(valueOrDefault) || valueOrDefault == args.User || !this._itemQuery.HasComponent(valueOrDefault) || !this.TryComp(uid, out comp1) || !this.TryComp(valueOrDefault, out comp2))
        return;
      EntityUid parentUid = comp1.ParentUid;
      EntityCoordinates coordinates = this.TransformSystem.ToCoordinates((Entity<TransformComponent>) (parentUid.IsValid() ? parentUid : uid), this.TransformSystem.GetMapCoordinates(comp2));
      args.Handled = true;
      if (!this.PlayerInsertEntityInWorld((Entity<StorageComponent>) (uid, storageComp), args.User, valueOrDefault))
        return;
      this.EntityManager.RaiseSharedEvent<AnimateInsertingEntitiesEvent>(new AnimateInsertingEntitiesEvent(this.GetNetEntity(uid), new List<NetEntity>()
      {
        this.GetNetEntity(valueOrDefault)
      }, new List<NetCoordinates>()
      {
        this.GetNetCoordinates(coordinates)
      }, new List<Angle>() { comp1.LocalRotation }), new EntityUid?(args.User));
    }
  }

  private void OnDoAfter(EntityUid uid, StorageComponent component, AreaPickupDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    args.Handled = true;
    List<EntityUid> uids = new List<EntityUid>();
    List<EntityCoordinates> entities = new List<EntityCoordinates>();
    List<Angle> entityAngles = new List<Angle>();
    TransformComponent component1;
    if (!this._xformQuery.TryGetComponent(uid, out component1))
      return;
    int num = Math.Min(10, args.Entities.Count);
    for (int index = 0; index < num; ++index)
    {
      EntityUid entity = this.GetEntity(args.Entities[index]);
      TransformComponent component2;
      if (!this.ContainerSystem.IsEntityInContainer(entity) && !(entity == args.Args.User) && this._itemQuery.HasComponent(entity) && this._xformQuery.TryGetComponent(entity, out component2) && !(component2.MapID != component1.MapID))
      {
        EntityCoordinates coordinates = this.TransformSystem.ToCoordinates((Entity<TransformComponent>) (component1.ParentUid.IsValid() ? component1.ParentUid : uid), new MapCoordinates(this.TransformSystem.GetWorldPosition(component2), component2.MapID));
        Angle localRotation = component2.LocalRotation;
        if (this.PlayerInsertEntityInWorld((Entity<StorageComponent>) (uid, component), args.Args.User, entity, false))
        {
          uids.Add(entity);
          entities.Add(coordinates);
          entityAngles.Add(localRotation);
        }
      }
    }
    if (uids.Count > 0)
    {
      if (!this._tag.HasTag(args.User, component.SilentStorageUserTag))
        this.Audio.PlayPredicted(component.StorageInsertSound, uid, new EntityUid?(args.User), new AudioParams?(SharedStorageSystem._audioParams));
      this.EntityManager.RaiseSharedEvent<AnimateInsertingEntitiesEvent>(new AnimateInsertingEntitiesEvent(this.GetNetEntity(uid), this.GetNetEntityList(uids), this.GetNetCoordinatesList(entities), entityAngles), new EntityUid?(args.User));
    }
    args.Handled = true;
  }

  private void OnReclaimed(EntityUid uid, StorageComponent storageComp, GotReclaimedEvent args)
  {
    this.ContainerSystem.EmptyContainer((BaseContainer) storageComp.Container, destination: new EntityCoordinates?(args.ReclaimerCoordinates));
  }

  private void OnDestroy(EntityUid uid, StorageComponent storageComp, DestructionEventArgs args)
  {
    EntityCoordinates moverCoordinates = this.TransformSystem.GetMoverCoordinates(uid);
    this.ContainerSystem.EmptyContainer((BaseContainer) storageComp.Container, destination: new EntityCoordinates?(moverCoordinates));
  }

  private void OnInteractWithItem(StorageInteractWithItemEvent msg, EntitySessionEventArgs args)
  {
    Entity<HandsComponent> player;
    Entity<StorageComponent> storage;
    Entity<ItemComponent> entity;
    if (!this.ValidateInput(args, msg.StorageUid, msg.InteractedItemUid, out player, out storage, out entity))
      return;
    EntityUid? uid;
    if (!this._sharedHandsSystem.TryGetActiveItem(player.AsNullable(), out uid))
    {
      if (this._rmcHands.TryStorageEjectHand((EntityUid) player, (EntityUid) entity))
        return;
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(31 /*0x1F*/, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) player)), "player", "ToPrettyString(player)");
      logStringHandler.AppendLiteral(" is attempting to take ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) entity)), "item", "ToPrettyString(item)");
      logStringHandler.AppendLiteral(" out of ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) storage)), "storage", "ToPrettyString(storage)");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.Storage, LogImpact.Low, ref local);
      if (this._sharedHandsSystem.TryPickupAnyHand((EntityUid) player, (EntityUid) entity, handsComp: player.Comp) && storage.Comp.StorageRemoveSound != null && !this._tag.HasTag((EntityUid) player, storage.Comp.SilentStorageUserTag))
        this.Audio.PlayPredicted(storage.Comp.StorageRemoveSound, (EntityUid) storage, new EntityUid?((EntityUid) player), new AudioParams?(SharedStorageSystem._audioParams));
      this.UpdateUI((Entity<StorageComponent>) ((EntityUid) storage, (StorageComponent) storage));
    }
    else
    {
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(51, 4);
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) player)), "player", "ToPrettyString(player)");
      logStringHandler.AppendLiteral(" is interacting with ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) entity)), "item", "ToPrettyString(item)");
      logStringHandler.AppendLiteral(" while it is stored in ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) storage)), "storage", "ToPrettyString(storage)");
      logStringHandler.AppendLiteral(" using ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(uid), "used", "ToPrettyString(activeItem)");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.Storage, LogImpact.Low, ref local);
      if (this._interactionSystem.InteractUsing((EntityUid) player, uid.Value, (EntityUid) entity, this.Transform((EntityUid) entity).Coordinates, false))
        return;
      StorageInsertFailedEvent args1 = new StorageInsertFailedEvent((Entity<StorageComponent>) ((EntityUid) storage, storage.Comp), (Entity<HandsComponent>) ((EntityUid) player, player.Comp));
      this.RaiseLocalEvent<StorageInsertFailedEvent>((EntityUid) storage, ref args1);
    }
  }

  private void OnSetItemLocation(StorageSetItemLocationEvent msg, EntitySessionEventArgs args)
  {
    Entity<HandsComponent> player;
    Entity<StorageComponent> storage;
    Entity<ItemComponent> itemEnt;
    if (!this.ValidateInput(args, msg.StorageEnt, msg.ItemEnt, out player, out storage, out itemEnt))
      return;
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(37, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) player)), "player", "ToPrettyString(player)");
    logStringHandler.AppendLiteral(" is updating the location of ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) itemEnt)), "item", "ToPrettyString(item)");
    logStringHandler.AppendLiteral(" within ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) storage)), "storage", "ToPrettyString(storage)");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.Storage, LogImpact.Low, ref local);
    this.TrySetItemStorageLocation(itemEnt, storage, msg.Location);
  }

  private void OnStorageNested(OpenNestedStorageEvent msg, EntitySessionEventArgs args)
  {
    if (!this.NestedStorage || !this.TryGetEntity(msg.InteractedItemUid, out EntityUid? _))
      return;
    this._nestedCheck = true;
    Entity<HandsComponent> player;
    Entity<StorageComponent> storage;
    Entity<ItemComponent> entity;
    if (!this.ValidateInput(args, msg.StorageUid, msg.InteractedItemUid, out player, out storage, out entity))
    {
      this._nestedCheck = false;
    }
    else
    {
      this.HideStorageWindow(storage.Owner, player.Owner);
      this.OpenStorageUI(entity.Owner, player.Owner);
      this._nestedCheck = false;
    }
  }

  private void OnStorageTransfer(StorageTransferItemEvent msg, EntitySessionEventArgs args)
  {
    EntityUid? entity1;
    ItemComponent comp1;
    if (!this.TryGetEntity(msg.ItemEnt, out entity1) || !this.TryComp<ItemComponent>(entity1, out comp1))
      return;
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    Entity<ItemComponent> entity2 = new Entity<ItemComponent>(entity1.Value, comp1);
    BaseContainer container;
    HandsComponent comp2;
    Entity<HandsComponent> player;
    Entity<StorageComponent> storage;
    Entity<ItemComponent> insertEnt;
    if (!this.TryGetStorageLocation(entity2, out container, out StorageComponent _, out ItemStorageLocation _) || !this.ValidateInput(args, this.GetNetEntity(container.Owner), out Entity<HandsComponent> _, out Entity<StorageComponent> _) || !this.TryComp<HandsComponent>(attachedEntity, out comp2) || !this._sharedHandsSystem.TryPickup(attachedEntity.Value, (EntityUid) entity2, animate: false, handsComp: comp2) || !this.ValidateInput(args, msg.StorageEnt, msg.ItemEnt, out player, out storage, out insertEnt, true))
      return;
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(20, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) player)), "player", "ToPrettyString(player)");
    logStringHandler.AppendLiteral(" is inserting ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) insertEnt)), "item", "ToPrettyString(item)");
    logStringHandler.AppendLiteral(" into ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) storage)), "storage", "ToPrettyString(storage)");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.Storage, LogImpact.Low, ref local);
    this.InsertAt(storage, insertEnt, msg.Location, out EntityUid? _, new EntityUid?((EntityUid) player), stackAutomatically: false);
  }

  private void OnInsertItemIntoLocation(
    StorageInsertItemIntoLocationEvent msg,
    EntitySessionEventArgs args)
  {
    Entity<HandsComponent> player;
    Entity<StorageComponent> storage;
    Entity<ItemComponent> insertEnt;
    if (!this.ValidateInput(args, msg.StorageEnt, msg.ItemEnt, out player, out storage, out insertEnt, true))
      return;
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(20, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) player)), "player", "ToPrettyString(player)");
    logStringHandler.AppendLiteral(" is inserting ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) insertEnt)), "item", "ToPrettyString(item)");
    logStringHandler.AppendLiteral(" into ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) storage)), "storage", "ToPrettyString(storage)");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.Storage, LogImpact.Low, ref local);
    this.InsertAt(storage, insertEnt, msg.Location, out EntityUid? _, new EntityUid?((EntityUid) player), stackAutomatically: false);
  }

  private void OnSaveItemLocation(StorageSaveItemLocationEvent msg, EntitySessionEventArgs args)
  {
    Entity<StorageComponent> storage;
    Entity<ItemComponent> entity;
    if (!this.ValidateInput(args, msg.Storage, msg.Item, out Entity<HandsComponent> _, out storage, out entity))
      return;
    this.SaveItemLocation(storage, (Entity<MetaDataComponent>) entity.Owner);
  }

  private void OnBoundUIOpen(Entity<StorageComponent> ent, ref BoundUIOpenedEvent args)
  {
    this.UpdateAppearance((Entity<StorageComponent, AppearanceComponent>) (ent.Owner, ent.Comp, (AppearanceComponent) null));
  }

  private void OnBoundUIAttempt(
    Entity<StorageComponent> ent,
    ref BoundUserInterfaceMessageAttempt args)
  {
    if (!(args.UiKey is StorageComponent.StorageUiKey.Key) || this._openStorageLimit == -1 || this._nestedCheck || !(args.Message is OpenBoundInterfaceMessage))
      return;
    EntityUid target = args.Target;
    EntityUid actor = args.Actor;
    int num = 0;
    UserInterfaceUserComponent component;
    if (!this._userQuery.TryComp(actor, out component))
      return;
    foreach ((EntityUid key, List<Enum> enumList) in component.OpenInterfaces)
    {
      EntityUid entityUid = target;
      if (!(key == entityUid))
      {
        foreach (Enum @enum in enumList)
        {
          if (@enum is StorageComponent.StorageUiKey)
          {
            ++num;
            if (num >= this._openStorageLimit)
            {
              args.Cancel();
              break;
            }
            break;
          }
        }
      }
    }
  }

  private void OnEntInserted(
    Entity<StorageComponent> entity,
    ref EntInsertedIntoContainerMessage args)
  {
    if (entity.Comp.Container == null || args.Container.ID != StorageComponent.ContainerId)
      return;
    if (!entity.Comp.StoredItems.ContainsKey(args.Entity))
    {
      ItemStorageLocation location;
      if (!CMInventoryExtensions.TryGetFirst((EntityUid) entity, args.Entity, out location))
      {
        this.ContainerSystem.Remove((Entity<TransformComponent, MetaDataComponent>) args.Entity, args.Container, force: true);
        return;
      }
      entity.Comp.StoredItems[args.Entity] = location;
      this.AddOccupiedEntity(entity, (Entity<ItemComponent>) args.Entity, location);
    }
    this.UpdateAppearance((Entity<StorageComponent, AppearanceComponent>) ((EntityUid) entity, entity.Comp, (AppearanceComponent) null));
    this.UpdateUI((Entity<StorageComponent>) ((EntityUid) entity, entity.Comp));
  }

  private void OnEntRemoved(
    Entity<StorageComponent> entity,
    ref EntRemovedFromContainerMessage args)
  {
    if (entity.Comp.Container == null || args.Container.ID != StorageComponent.ContainerId)
      return;
    ItemStorageLocation location1;
    if (entity.Comp.StoredItems.Remove(args.Entity, out location1))
      this.RemoveOccupiedEntity(entity, (Entity<ItemComponent>) args.Entity, location1);
    this.Dirty((EntityUid) entity, (IComponent) entity.Comp);
    this.UpdateAppearance((Entity<StorageComponent, AppearanceComponent>) ((EntityUid) entity, entity.Comp, (AppearanceComponent) null));
    this.UpdateUI((Entity<StorageComponent>) ((EntityUid) entity, entity.Comp));
    List<(EntityUid, ItemStorageLocation)> valueTupleList = new List<(EntityUid, ItemStorageLocation)>();
    foreach ((EntityUid key, ItemStorageLocation itemStorageLocation) in entity.Comp.StoredItems)
      valueTupleList.Add((key, itemStorageLocation));
    valueTupleList.Sort((Comparison<(EntityUid, ItemStorageLocation)>) ((a, b) =>
    {
      int num = a.Location.Position.Y.CompareTo(b.Location.Position.Y);
      return num != 0 ? num : a.Location.Position.X.CompareTo(b.Location.Position.X);
    }));
    foreach ((EntityUid entityUid, ItemStorageLocation itemStorageLocation) in valueTupleList)
    {
      ItemStorageLocation location2;
      if (CMInventoryExtensions.TryGetFirst((EntityUid) entity, entityUid, out location2) && itemStorageLocation != location2)
        this.TrySetItemStorageLocation((Entity<ItemComponent>) entityUid, (Entity<StorageComponent>) ((EntityUid) entity, (StorageComponent) entity), location2);
    }
  }

  private void OnInsertAttempt(
    EntityUid uid,
    StorageComponent component,
    ContainerIsInsertingAttemptEvent args)
  {
    if (args.Cancelled || args.Container.ID != StorageComponent.ContainerId || this.CheckingCanInsert || this.CanInsert(uid, args.EntityUid, new EntityUid?(), out string _, component, ignoreStacks: true))
      return;
    args.Cancel();
  }

  public void UpdateAppearance(
    Entity<StorageComponent?, AppearanceComponent?> entity)
  {
    (EntityUid entityUid, StorageComponent comp1, AppearanceComponent comp2) = entity;
    if (!this.Resolve<StorageComponent, AppearanceComponent>(entityUid, ref comp1, ref comp2, false) || comp1.Container == null)
      return;
    int area = comp1.Grid.GetArea();
    int cumulativeItemAreas = this.GetCumulativeItemAreas((Entity<StorageComponent>) (entityUid, comp1));
    bool flag = this.UI.IsUiOpen((Entity<UserInterfaceComponent>) entity.Owner, (Enum) StorageComponent.StorageUiKey.Key);
    this._appearance.SetData(entityUid, (Enum) StorageVisuals.StorageUsed, (object) cumulativeItemAreas, comp2);
    this._appearance.SetData(entityUid, (Enum) StorageVisuals.Capacity, (object) area, comp2);
    this._appearance.SetData(entityUid, (Enum) StorageVisuals.Open, (object) flag, comp2);
    this._appearance.SetData(entityUid, (Enum) SharedBagOpenVisuals.BagState, (object) (SharedBagState) (flag ? 0 : 1), comp2);
    StorageFillVisualizerComponent comp;
    if (this.TryComp<StorageFillVisualizerComponent>(entityUid, out comp))
    {
      int levels = ContentHelpers.RoundToLevels((double) cumulativeItemAreas, (double) area, comp.MaxFillLevels);
      this._appearance.SetData(entityUid, (Enum) StorageFillVisuals.FillLevel, (object) levels, comp2);
    }
    if (!comp1.HideStackVisualsWhenClosed)
      return;
    this._appearance.SetData(entityUid, (Enum) StackVisuals.Hide, (object) !flag, comp2);
  }

  public void TransferEntities(
    EntityUid source,
    EntityUid target,
    EntityUid? user = null,
    StorageComponent? sourceComp = null,
    LockComponent? sourceLock = null,
    StorageComponent? targetComp = null,
    LockComponent? targetLock = null)
  {
    if (!this.Resolve<StorageComponent>(source, ref sourceComp) || !this.Resolve<StorageComponent>(target, ref targetComp))
      return;
    IReadOnlyList<EntityUid> containedEntities = sourceComp.Container.ContainedEntities;
    if (containedEntities.Count == 0 || this.Resolve<LockComponent>(source, ref sourceLock, false) && sourceLock.Locked || this.Resolve<LockComponent>(target, ref targetLock, false) && targetLock.Locked)
      return;
    bool flag = false;
    foreach (EntityUid insertEnt in containedEntities.ToArray<EntityUid>())
    {
      string reason;
      if (!this.CanInsert(target, insertEnt, user, out reason, targetComp))
      {
        if (reason == "comp-storage-insufficient-capacity")
        {
          flag = true;
          break;
        }
      }
      else if (!this.Insert(target, insertEnt, out EntityUid? _, user, targetComp, false))
      {
        flag = true;
        break;
      }
    }
    if (user.HasValue && (!this._tag.HasTag(user.Value, sourceComp.SilentStorageUserTag) || !this._tag.HasTag(user.Value, targetComp.SilentStorageUserTag)))
      this.Audio.PlayPredicted(sourceComp.StorageInsertSound, target, user, new AudioParams?(SharedStorageSystem._audioParams));
    if (!flag || !user.HasValue)
      return;
    this._popupSystem.PopupClient(this.Loc.GetString("pubg-storage-transfer-leftover"), target, new EntityUid?(user.Value));
  }

  public bool CanInsert(
    EntityUid uid,
    EntityUid insertEnt,
    EntityUid? user,
    out string? reason,
    StorageComponent? storageComp = null,
    ItemComponent? item = null,
    bool ignoreStacks = false,
    bool ignoreLocation = false)
  {
    if (!this.Resolve<StorageComponent>(uid, ref storageComp) || !this.Resolve<ItemComponent>(insertEnt, ref item, false))
    {
      reason = (string) null;
      return false;
    }
    if (this.Transform(insertEnt).Anchored)
    {
      reason = "comp-storage-anchored-failure";
      return false;
    }
    if (this._whitelistSystem.IsWhitelistFail(storageComp.Whitelist, insertEnt) || this._whitelistSystem.IsBlacklistPass(storageComp.Blacklist, insertEnt))
    {
      reason = "comp-storage-invalid-container";
      return false;
    }
    StackComponent component;
    if (!ignoreStacks && this._stackQuery.TryGetComponent(insertEnt, out component) && this.HasSpaceInStacks((Entity<StorageComponent>) (uid, storageComp), component.StackTypeId))
    {
      reason = (string) null;
      return true;
    }
    ItemSizePrototype maxItemSize = this.GetMaxItemSize((Entity<StorageComponent>) (uid, storageComp));
    if (this.ItemSystem.GetSizePrototype(item.Size) > maxItemSize && !this.RMCStorage.IgnoreItemSize((Entity<StorageComponent>) (uid, storageComp), insertEnt))
    {
      reason = "comp-storage-too-big";
      return false;
    }
    StorageComponent comp;
    if (this.TryComp<StorageComponent>(insertEnt, out comp) && this.GetMaxItemSize((Entity<StorageComponent>) (insertEnt, comp)) >= maxItemSize && !this.RMCStorage.IgnoreItemSize((Entity<StorageComponent>) (uid, storageComp), insertEnt))
    {
      reason = "comp-storage-too-big";
      return false;
    }
    if (!ignoreLocation && !storageComp.StoredItems.ContainsKey(insertEnt) && !this.TryGetAvailableGridSpace((Entity<StorageComponent>) (uid, storageComp), (Entity<ItemComponent>) (insertEnt, item), out ItemStorageLocation? _))
    {
      reason = "comp-storage-insufficient-capacity";
      return false;
    }
    LocId popup;
    if (!this.RMCStorage.CanInsert((Entity<StorageComponent>) (uid, storageComp), insertEnt, user, out popup))
    {
      reason = (string) popup;
      return false;
    }
    this.CheckingCanInsert = true;
    if (!this.ContainerSystem.CanInsert(insertEnt, (BaseContainer) storageComp.Container))
    {
      this.CheckingCanInsert = false;
      reason = (string) null;
      return false;
    }
    this.CheckingCanInsert = false;
    reason = (string) null;
    return true;
  }

  public bool InsertAt(
    Entity<StorageComponent?> uid,
    Entity<ItemComponent?> insertEnt,
    ItemStorageLocation location,
    out EntityUid? stackedEntity,
    EntityUid? user = null,
    bool playSound = true,
    bool stackAutomatically = true)
  {
    stackedEntity = new EntityUid?();
    if (!this.Resolve<StorageComponent>((EntityUid) uid, ref uid.Comp) || !this.ItemFitsInGridLocation(insertEnt, uid, location))
      return false;
    uid.Comp.StoredItems[(EntityUid) insertEnt] = location;
    this.AddOccupiedEntity((Entity<StorageComponent>) (uid.Owner, uid.Comp), insertEnt, location);
    if (this.Insert((EntityUid) uid, (EntityUid) insertEnt, out stackedEntity, out string _, user, uid.Comp, playSound, stackAutomatically))
      return true;
    this.RemoveOccupiedEntity((Entity<StorageComponent>) (uid.Owner, uid.Comp), insertEnt, location);
    uid.Comp.StoredItems.Remove((EntityUid) insertEnt);
    return false;
  }

  public bool Insert(
    EntityUid uid,
    EntityUid insertEnt,
    out EntityUid? stackedEntity,
    EntityUid? user = null,
    StorageComponent? storageComp = null,
    bool playSound = true,
    bool stackAutomatically = true)
  {
    return this.Insert(uid, insertEnt, out stackedEntity, out string _, user, storageComp, playSound, stackAutomatically);
  }

  public bool Insert(
    EntityUid uid,
    EntityUid insertEnt,
    out EntityUid? stackedEntity,
    out string? reason,
    EntityUid? user = null,
    StorageComponent? storageComp = null,
    bool playSound = true,
    bool stackAutomatically = true)
  {
    stackedEntity = new EntityUid?();
    reason = (string) null;
    if (!this.Resolve<StorageComponent>(uid, ref storageComp))
      return false;
    bool flag = playSound && (!user.HasValue || !this._tag.HasTag(user.Value, storageComp.SilentStorageUserTag));
    StackComponent component1;
    if (!stackAutomatically || !this._stackQuery.TryGetComponent(insertEnt, out component1))
    {
      if (!this.ContainerSystem.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) insertEnt, (BaseContainer) storageComp.Container))
        return false;
      if (flag)
        this.Audio.PlayPredicted(storageComp.StorageInsertSound, uid, user, new AudioParams?(SharedStorageSystem._audioParams));
      return true;
    }
    int count = component1.Count;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) storageComp.Container.ContainedEntities)
    {
      StackComponent component2;
      if (this._stackQuery.TryGetComponent(containedEntity, out component2) && this._stack.TryAdd(insertEnt, containedEntity, component1, component2))
      {
        stackedEntity = new EntityUid?(containedEntity);
        if (component1.Count == 0)
          break;
      }
    }
    if (component1.Count > 0 && !this.ContainerSystem.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) insertEnt, (BaseContainer) storageComp.Container) && count == component1.Count)
      return false;
    if (flag)
      this.Audio.PlayPredicted(storageComp.StorageInsertSound, uid, user, new AudioParams?(SharedStorageSystem._audioParams));
    return true;
  }

  public bool PlayerInsertHeldEntity(Entity<StorageComponent?> ent, Entity<HandsComponent?> player)
  {
    EntityUid? nullable1;
    if (!this.Resolve<StorageComponent>(ent.Owner, ref ent.Comp) || !this.Resolve<HandsComponent>(player.Owner, ref player.Comp) || !this._sharedHandsSystem.TryGetActiveItem(player, out nullable1))
      return false;
    EntityUid? nullable2 = nullable1;
    string reason;
    if (!this.CanInsert((EntityUid) ent, nullable2.Value, new EntityUid?(player.Owner), out reason, ent.Comp))
    {
      this._popupSystem.PopupClient(this.Loc.GetString(reason ?? "comp-storage-cant-insert"), (EntityUid) ent, new EntityUid?((EntityUid) player));
      return false;
    }
    if (this._sharedHandsSystem.CanDrop(player, nullable2.Value))
      return this.PlayerInsertEntityInWorld((Entity<StorageComponent>) ((EntityUid) ent, ent.Comp), (EntityUid) player, nullable2.Value);
    this._popupSystem.PopupClient(this.Loc.GetString("comp-storage-cant-drop", ("entity", (object) nullable2.Value)), (EntityUid) ent, new EntityUid?((EntityUid) player));
    return false;
  }

  public bool PlayerInsertEntityInWorld(
    Entity<StorageComponent?> uid,
    EntityUid player,
    EntityUid toInsert,
    bool playSound = true)
  {
    if (this.Resolve<StorageComponent>((EntityUid) uid, ref uid.Comp))
    {
      SharedInteractionSystem interactionSystem = this._interactionSystem;
      Entity<TransformComponent> origin = (Entity<TransformComponent>) player;
      Entity<TransformComponent> owner = (Entity<TransformComponent>) uid.Owner;
      EntityUid? stackedEntity = new EntityUid?();
      EntityUid? user = stackedEntity;
      if (interactionSystem.InRangeUnobstructed(origin, owner, user: user))
      {
        if (this.Insert((EntityUid) uid, toInsert, out stackedEntity, new EntityUid?(player), uid.Comp, playSound))
          return true;
        this._popupSystem.PopupClient(this.Loc.GetString("comp-storage-cant-insert"), (EntityUid) uid, new EntityUid?(player));
        return false;
      }
    }
    return false;
  }

  public bool TrySetItemStorageLocation(
    Entity<ItemComponent?> itemEnt,
    Entity<StorageComponent?> storageEnt,
    ItemStorageLocation location)
  {
    if (!this.Resolve<ItemComponent>((EntityUid) itemEnt, ref itemEnt.Comp) || !this.Resolve<StorageComponent>((EntityUid) storageEnt, ref storageEnt.Comp) || !storageEnt.Comp.Container.ContainedEntities.Contains<EntityUid>((EntityUid) itemEnt) || !this.ItemFitsInGridLocation(itemEnt, storageEnt, location.Position, location.Rotation))
      return false;
    ItemStorageLocation location1;
    if (storageEnt.Comp.StoredItems.Remove((EntityUid) itemEnt, out location1))
      this.RemoveOccupiedEntity((Entity<StorageComponent>) (storageEnt.Owner, storageEnt.Comp), itemEnt, location1);
    storageEnt.Comp.StoredItems.Add((EntityUid) itemEnt, location);
    this.AddOccupiedEntity((Entity<StorageComponent>) (storageEnt.Owner, storageEnt.Comp), itemEnt, location);
    this.UpdateUI(storageEnt);
    return true;
  }

  public bool TryGetAvailableGridSpace(
    Entity<StorageComponent?> storageEnt,
    Entity<ItemComponent?> itemEnt,
    [NotNullWhen(true)] out ItemStorageLocation? storageLocation)
  {
    storageLocation = new ItemStorageLocation?();
    if (!this.Resolve<StorageComponent>((EntityUid) storageEnt, ref storageEnt.Comp) || !this.Resolve<ItemComponent>((EntityUid) itemEnt, ref itemEnt.Comp))
      return false;
    if (this.FindSavedLocation(storageEnt, itemEnt, out storageLocation))
      return true;
    Box2i boundingBox = storageEnt.Comp.Grid.GetBoundingBox();
    if (!storageEnt.Comp.DefaultStorageOrientation.HasValue)
      Angle.FromDegrees(-(double) itemEnt.Comp.StoredRotation);
    else if (((Box2i) ref boundingBox).Width < ((Box2i) ref boundingBox).Height)
    {
      StorageDefaultOrientation? storageOrientation = storageEnt.Comp.DefaultStorageOrientation;
      StorageDefaultOrientation defaultOrientation = StorageDefaultOrientation.Horizontal;
      if (!(storageOrientation.GetValueOrDefault() == defaultOrientation & storageOrientation.HasValue))
      {
        Angle.FromDegrees(90.0);
      }
      else
      {
        Angle zero = Angle.Zero;
      }
    }
    else
    {
      StorageDefaultOrientation? storageOrientation = storageEnt.Comp.DefaultStorageOrientation;
      StorageDefaultOrientation defaultOrientation = StorageDefaultOrientation.Vertical;
      if (!(storageOrientation.GetValueOrDefault() == defaultOrientation & storageOrientation.HasValue))
      {
        Angle.FromDegrees(90.0);
      }
      else
      {
        Angle zero = Angle.Zero;
      }
    }
    for (int bottom = boundingBox.Bottom; bottom <= boundingBox.Top; ++bottom)
    {
      for (int left = boundingBox.Left; left <= boundingBox.Right; ++left)
      {
        ItemStorageLocation location = new ItemStorageLocation(Angle.Zero, Vector2i.op_Implicit((left, bottom)));
        if (this.ItemFitsInGridLocation(itemEnt, storageEnt, location))
        {
          storageLocation = new ItemStorageLocation?(location);
          return true;
        }
      }
    }
    return false;
  }

  public bool FindSavedLocation(
    Entity<StorageComponent?> ent,
    Entity<ItemComponent?> item,
    [NotNullWhen(true)] out ItemStorageLocation? storageLocation)
  {
    storageLocation = new ItemStorageLocation?();
    if (!this.Resolve<StorageComponent>((EntityUid) ent, ref ent.Comp))
      return false;
    string key = this.Name((EntityUid) item);
    List<ItemStorageLocation> itemStorageLocationList;
    if (!ent.Comp.SavedLocations.TryGetValue(key, out itemStorageLocationList))
      return false;
    foreach (ItemStorageLocation location in itemStorageLocationList)
    {
      if (this.ItemFitsInGridLocation(item, ent, location))
      {
        storageLocation = new ItemStorageLocation?(location);
        return true;
      }
    }
    return false;
  }

  public void SaveItemLocation(Entity<StorageComponent?> ent, Entity<MetaDataComponent?> item)
  {
    ItemStorageLocation itemStorageLocation;
    if (!this.Resolve<StorageComponent>((EntityUid) ent, ref ent.Comp) || !ent.Comp.StoredItems.TryGetValue((EntityUid) item, out itemStorageLocation))
      return;
    string key = this.Name((EntityUid) item, item.Comp);
    List<ItemStorageLocation> itemStorageLocationList;
    if (ent.Comp.SavedLocations.TryGetValue(key, out itemStorageLocationList))
    {
      for (int index = 0; index < itemStorageLocationList.Count; ++index)
      {
        if (itemStorageLocationList[index] == itemStorageLocation)
        {
          itemStorageLocationList.Remove(itemStorageLocation);
          return;
        }
      }
      itemStorageLocationList.Add(itemStorageLocation);
    }
    else
    {
      itemStorageLocationList = new List<ItemStorageLocation>()
      {
        itemStorageLocation
      };
      ent.Comp.SavedLocations[key] = itemStorageLocationList;
    }
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
    this.UpdateUI((Entity<StorageComponent>) (ent.Owner, ent.Comp));
  }

  public bool ItemFitsInGridLocation(
    Entity<ItemComponent?> itemEnt,
    Entity<StorageComponent?> storageEnt,
    ItemStorageLocation location)
  {
    if (!this.Resolve<ItemComponent>((EntityUid) itemEnt, ref itemEnt.Comp) || !this.Resolve<StorageComponent>((EntityUid) storageEnt, ref storageEnt.Comp))
      return false;
    Vector2i position = location.Position;
    Angle rotation = location.Rotation;
    Box2i boundingBox = storageEnt.Comp.Grid.GetBoundingBox();
    if (!((Box2i) ref boundingBox).Contains(position, true))
      return false;
    foreach (Box2i box2i in (IEnumerable<Box2i>) this.ItemSystem.GetAdjustedItemShape(storageEnt, itemEnt, rotation, position))
    {
      for (int bottom = box2i.Bottom; bottom <= box2i.Top; ++bottom)
      {
        for (int left = box2i.Left; left <= box2i.Right; ++left)
        {
          (int, int) valueTuple = (left, bottom);
          if (!this.IsGridSpaceEmpty(itemEnt, storageEnt, Vector2i.op_Implicit(valueTuple)))
            return false;
        }
      }
    }
    return true;
  }

  public bool ItemFitsInGridLocation(
    Entity<ItemComponent?> itemEnt,
    Entity<StorageComponent?> storageEnt,
    Vector2i position,
    Angle rotation)
  {
    if (!this.Resolve<ItemComponent>((EntityUid) itemEnt, ref itemEnt.Comp) || !this.Resolve<StorageComponent>((EntityUid) storageEnt, ref storageEnt.Comp))
      return false;
    Box2i boundingBox = storageEnt.Comp.Grid.GetBoundingBox();
    if (!((Box2i) ref boundingBox).Contains(position, true))
      return false;
    foreach (Box2i box2i in (IEnumerable<Box2i>) this.ItemSystem.GetAdjustedItemShape(storageEnt, itemEnt, rotation, position))
    {
      for (int bottom = box2i.Bottom; bottom <= box2i.Top; ++bottom)
      {
        for (int left = box2i.Left; left <= box2i.Right; ++left)
        {
          (int, int) valueTuple = (left, bottom);
          if (!this.IsGridSpaceEmpty(itemEnt, storageEnt, Vector2i.op_Implicit(valueTuple)))
            return false;
        }
      }
    }
    return true;
  }

  public bool IsGridSpaceEmpty(
    Entity<ItemComponent?> itemEnt,
    Entity<StorageComponent?> storageEnt,
    Vector2i location)
  {
    if (!this.Resolve<StorageComponent>((EntityUid) storageEnt, ref storageEnt.Comp))
      return false;
    bool flag = false;
    foreach (Box2i box2i in storageEnt.Comp.Grid)
    {
      if (((Box2i) ref box2i).Contains(location, true))
      {
        flag = true;
        break;
      }
    }
    if (!flag)
      return false;
    foreach ((EntityUid entityUid, ItemStorageLocation location1) in storageEnt.Comp.StoredItems)
    {
      ItemComponent component;
      if (!(entityUid == itemEnt.Owner) && this._itemQuery.TryGetComponent(entityUid, out component))
      {
        foreach (Box2i box2i in (IEnumerable<Box2i>) this.ItemSystem.GetAdjustedItemShape(storageEnt, (Entity<ItemComponent>) (entityUid, component), location1))
        {
          if (((Box2i) ref box2i).Contains(location, true))
            return false;
        }
      }
    }
    return true;
  }

  protected void UpdateOccupied(Entity<StorageComponent> ent)
  {
    ent.Comp.OccupiedGrid.Clear();
    this.RemoveOccupied((IReadOnlyList<Box2i>) ent.Comp.Grid, ent.Comp.OccupiedGrid);
    this.Dirty<StorageComponent>(ent);
    foreach ((EntityUid entityUid, ItemStorageLocation location) in ent.Comp.StoredItems)
    {
      ItemComponent component;
      if (this._itemQuery.TryGetComponent(entityUid, out component))
        this.AddOccupiedEntity(ent, (Entity<ItemComponent>) (entityUid, component), location);
    }
  }

  private void AddOccupiedEntity(
    Entity<StorageComponent> storageEnt,
    Entity<ItemComponent?> itemEnt,
    ItemStorageLocation location)
  {
    this.AddOccupied((Entity<StorageComponent>) ((EntityUid) storageEnt, (StorageComponent) storageEnt), itemEnt, location, storageEnt.Comp.OccupiedGrid);
    this.Dirty<StorageComponent>(storageEnt);
  }

  private void AddOccupied(
    Entity<StorageComponent?> storageEnt,
    Entity<ItemComponent?> itemEnt,
    ItemStorageLocation location,
    Dictionary<Vector2i, ulong> occupied)
  {
    this.AddOccupied(this.ItemSystem.GetAdjustedItemShape(storageEnt, (Entity<ItemComponent>) (itemEnt.Owner, itemEnt.Comp), location), occupied);
  }

  private void RemoveOccupied(
    IReadOnlyList<Box2i> adjustedShape,
    Dictionary<Vector2i, ulong> occupied)
  {
    using (IEnumerator<Box2i> enumerator = adjustedShape.GetEnumerator())
    {
label_11:
      while (enumerator.MoveNext())
      {
        Box2i current = enumerator.Current;
        ChunkIndicesEnumerator indicesEnumerator = new ChunkIndicesEnumerator(Box2i.op_Implicit(current), 8);
        while (true)
        {
          Vector2i? indices;
          if (indicesEnumerator.MoveNext(out indices))
          {
            Vector2i key = Vector2i.op_Multiply(indices.Value, 8);
            int num1 = Math.Max(current.Left, key.X);
            int num2 = Math.Max(current.Bottom, key.Y);
            int num3 = Math.Min(current.Right, key.X + 8 - 1);
            int num4 = Math.Min(current.Top, key.Y + 8 - 1);
            ulong valueOrDefault = occupied.GetValueOrDefault<Vector2i, ulong>(key, ulong.MaxValue);
            for (int index1 = num1; index1 <= num3; ++index1)
            {
              for (int index2 = num2; index2 <= num4; ++index2)
              {
                ulong bitmask = SharedMapSystem.ToBitmask(SharedMapSystem.GetChunkRelative(new Vector2i(index1, index2), (byte) 8));
                valueOrDefault &= ~bitmask;
              }
            }
            occupied[key] = valueOrDefault;
          }
          else
            goto label_11;
        }
      }
    }
  }

  private void AddOccupied(IReadOnlyList<Box2i> adjustedShape, Dictionary<Vector2i, ulong> occupied)
  {
    using (IEnumerator<Box2i> enumerator = adjustedShape.GetEnumerator())
    {
label_11:
      while (enumerator.MoveNext())
      {
        Box2i current = enumerator.Current;
        ChunkIndicesEnumerator indicesEnumerator = new ChunkIndicesEnumerator(Box2i.op_Implicit(current), 8);
        while (true)
        {
          Vector2i? indices;
          if (indicesEnumerator.MoveNext(out indices))
          {
            Vector2i key = Vector2i.op_Multiply(indices.Value, 8);
            ulong orNew = occupied.GetOrNew<Vector2i, ulong>(key);
            int num1 = Math.Max(key.X, current.Left);
            int num2 = Math.Max(key.Y, current.Bottom);
            int num3 = Math.Min(key.X + 8 - 1, current.Right);
            int num4 = Math.Min(key.Y + 8 - 1, current.Top);
            for (int index1 = num1; index1 <= num3; ++index1)
            {
              for (int index2 = num2; index2 <= num4; ++index2)
              {
                ulong bitmask = SharedMapSystem.ToBitmask(SharedMapSystem.GetChunkRelative(new Vector2i(index1, index2), (byte) 8));
                orNew |= bitmask;
              }
            }
            occupied[key] = orNew;
          }
          else
            goto label_11;
        }
      }
    }
  }

  private void RemoveOccupiedEntity(
    Entity<StorageComponent> storageEnt,
    Entity<ItemComponent?> itemEnt,
    ItemStorageLocation location)
  {
    this.RemoveOccupied(this.ItemSystem.GetAdjustedItemShape((Entity<StorageComponent>) ((EntityUid) storageEnt, (StorageComponent) storageEnt), (Entity<ItemComponent>) (itemEnt.Owner, itemEnt.Comp), location), storageEnt.Comp.OccupiedGrid);
    this.Dirty<StorageComponent>(storageEnt);
  }

  public bool HasSpace(Entity<StorageComponent?> uid)
  {
    if (!this.Resolve<StorageComponent>((EntityUid) uid, ref uid.Comp))
      return false;
    return this.GetCumulativeItemAreas(uid) < uid.Comp.Grid.GetArea() || this.HasSpaceInStacks(uid);
  }

  private bool HasSpaceInStacks(Entity<StorageComponent?> uid, string? stackType = null)
  {
    if (!this.Resolve<StorageComponent>((EntityUid) uid, ref uid.Comp))
      return false;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) uid.Comp.Container.ContainedEntities)
    {
      StackComponent component;
      if (this._stackQuery.TryGetComponent(containedEntity, out component) && (stackType == null || component.StackTypeId.Equals(stackType)) && this._stack.GetAvailableSpace(component) != 0)
        return true;
    }
    return false;
  }

  public int GetCumulativeItemAreas(Entity<StorageComponent?> entity)
  {
    if (!this.Resolve<StorageComponent>((EntityUid) entity, ref entity.Comp))
      return 0;
    int cumulativeItemAreas = 0;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) entity.Comp.Container.ContainedEntities)
    {
      ItemComponent component;
      if (this._itemQuery.TryGetComponent(containedEntity, out component))
        cumulativeItemAreas += this.ItemSystem.GetItemShape(entity, (Entity<ItemComponent>) (containedEntity, component)).GetArea();
    }
    return cumulativeItemAreas;
  }

  public ItemSizePrototype GetMaxItemSize(Entity<StorageComponent?> uid)
  {
    if (!this.Resolve<StorageComponent>((EntityUid) uid, ref uid.Comp))
      return this._defaultStorageMaxItemSize;
    if (uid.Comp.MaxItemSize.HasValue)
    {
      ItemSizePrototype prototype;
      if (this._prototype.TryIndex<ItemSizePrototype>(uid.Comp.MaxItemSize.Value, out prototype))
        return prototype;
      this.Log.Error($"{this.ToPrettyString((Entity<MetaDataComponent>) uid.Owner)} tried to get invalid item size prototype: {uid.Comp.MaxItemSize.Value}. Stack trace:\\n{Environment.StackTrace}");
    }
    ItemComponent component;
    return !this._itemQuery.TryGetComponent((EntityUid) uid, out component) ? this._defaultStorageMaxItemSize : this._nextSmallest[(string) component.Size];
  }

  private void OnLockToggled(EntityUid uid, StorageComponent component, ref LockToggledEvent args)
  {
    if (!args.Locked)
      return;
    foreach (EntityUid user in this.UI.GetActors((Entity<UserInterfaceComponent>) uid, (Enum) StorageComponent.StorageUiKey.Key).ToList<EntityUid>())
    {
      if (!this.CanInteract(user, (Entity<StorageComponent>) (uid, component)))
        this.UI.CloseUi((Entity<UserInterfaceComponent>) uid, (Enum) StorageComponent.StorageUiKey.Key, new EntityUid?(user));
    }
  }

  private void OnStackCountChanged(
    EntityUid uid,
    MetaDataComponent component,
    StackCountChangedEvent args)
  {
    BaseContainer container;
    if (!this.ContainerSystem.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (uid, (TransformComponent) null, component), out container) || !(container.ID == StorageComponent.ContainerId))
      return;
    this.UpdateAppearance((Entity<StorageComponent, AppearanceComponent>) container.Owner);
    this.UpdateUI((Entity<StorageComponent>) container.Owner);
  }

  private void HandleOpenBackpack(ICommonSession? session)
  {
    this.HandleToggleSlotUI(session, "back");
  }

  private void HandleOpenBelt(ICommonSession? session) => this.HandleToggleSlotUI(session, "belt");

  private void HandleToggleSlotUI(ICommonSession? session, string slot)
  {
    if (session == null)
      return;
    EntityUid? attachedEntity = session.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    EntityUid? entityUid;
    if (!valueOrDefault.Valid || !this.Exists(valueOrDefault) || !this._inventory.TryGetSlotEntity(valueOrDefault, slot, out entityUid) || !this.ActionBlocker.CanInteract(valueOrDefault, entityUid))
      return;
    if (!this.UI.IsUiOpen((Entity<UserInterfaceComponent>) entityUid.Value, (Enum) StorageComponent.StorageUiKey.Key, valueOrDefault))
      this.OpenStorageUI(entityUid.Value, valueOrDefault, silent: false);
    else
      this.UI.CloseUi((Entity<UserInterfaceComponent>) entityUid.Value, (Enum) StorageComponent.StorageUiKey.Key, new EntityUid?(valueOrDefault));
  }

  protected void ClearCantFillReasons()
  {
  }

  private bool CanInteract(
    EntityUid user,
    Entity<StorageComponent> storage,
    bool canInteract = true,
    bool silent = true)
  {
    if (this.HasComp<BypassInteractionChecksComponent>(user))
      return true;
    if (!canInteract)
      return false;
    StorageInteractAttemptEvent args = new StorageInteractAttemptEvent(user, silent);
    this.RaiseLocalEvent<StorageInteractAttemptEvent>((EntityUid) storage, ref args);
    return !args.Cancelled;
  }

  public abstract void PlayPickupAnimation(
    EntityUid uid,
    EntityCoordinates initialCoordinates,
    EntityCoordinates finalCoordinates,
    Angle initialRotation,
    EntityUid? user = null);

  private bool ValidateInput(
    EntitySessionEventArgs args,
    NetEntity netStorage,
    out Entity<HandsComponent> player,
    out Entity<StorageComponent> storage)
  {
    player = new Entity<HandsComponent>();
    storage = new Entity<StorageComponent>();
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return false;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    HandsComponent comp1;
    EntityUid? entity;
    StorageComponent comp2;
    if (!this.TryComp<HandsComponent>(valueOrDefault, out comp1) || comp1.Count == 0 || !this.TryGetEntity(netStorage, out entity) || !this.TryComp<StorageComponent>(entity, out comp2) || !this.UI.IsUiOpen((Entity<UserInterfaceComponent>) entity.Value, (Enum) StorageComponent.StorageUiKey.Key, valueOrDefault) || !this.ActionBlocker.CanInteract(valueOrDefault, entity))
      return false;
    player = new Entity<HandsComponent>(valueOrDefault, comp1);
    storage = new Entity<StorageComponent>(entity.Value, comp2);
    return true;
  }

  private bool ValidateInput(
    EntitySessionEventArgs args,
    NetEntity netStorage,
    NetEntity netItem,
    out Entity<HandsComponent> player,
    out Entity<StorageComponent> storage,
    out Entity<ItemComponent> item,
    bool held = false)
  {
    item = new Entity<ItemComponent>();
    EntityUid? entity;
    if (!this.ValidateInput(args, netStorage, out player, out storage) || !this.TryGetEntity(netItem, out entity))
      return false;
    if (held)
    {
      if (!this._sharedHandsSystem.IsHolding(player.AsNullable(), entity, out string _))
        return false;
    }
    else if (!storage.Comp.Container.Contains(entity.Value))
      return false;
    ItemComponent comp;
    if (!this.TryComp<ItemComponent>(entity, out comp) || !this.ActionBlocker.CanInteract((EntityUid) player, entity))
      return false;
    item = new Entity<ItemComponent>(entity.Value, comp);
    return true;
  }

  [NetSerializable]
  [Serializable]
  protected sealed class StorageComponentState : ComponentState
  {
    public Dictionary<NetEntity, ItemStorageLocation> StoredItems = new Dictionary<NetEntity, ItemStorageLocation>();
    public Dictionary<string, List<ItemStorageLocation>> SavedLocations = new Dictionary<string, List<ItemStorageLocation>>();
    public List<Box2i> Grid = new List<Box2i>();
    public ProtoId<ItemSizePrototype>? MaxItemSize;
    public EntityWhitelist? Whitelist;
    public EntityWhitelist? Blacklist;
  }
}
