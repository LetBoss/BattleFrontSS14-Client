// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.StationAi.SharedStationAiSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Actions.Events;
using Content.Shared.Administration.Managers;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Electrocution;
using Content.Shared.Holopad;
using Content.Shared.IdentityManagement;
using Content.Shared.Intellicard;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Light.Components;
using Content.Shared.Mind;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.StationAi;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Silicons.StationAi;

public abstract class SharedStationAiSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminManager _admin;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private ItemSlotsSystem _slots;
  [Dependency]
  private ItemToggleSystem _toggles;
  [Dependency]
  private ActionBlockerSystem _blocker;
  [Dependency]
  private MetaDataSystem _metadata;
  [Dependency]
  private SharedAirlockSystem _airlocks;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _containers;
  [Dependency]
  private SharedDoorSystem _doors;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedElectrocutionSystem _electrify;
  [Dependency]
  private SharedEyeSystem _eye;
  [Dependency]
  protected SharedMapSystem Maps;
  [Dependency]
  private SharedMindSystem _mind;
  [Dependency]
  private SharedMoverController _mover;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedPowerReceiverSystem PowerReceiver;
  [Dependency]
  private SharedTransformSystem _xforms;
  [Dependency]
  private SharedUserInterfaceSystem _uiSystem;
  [Dependency]
  private StationAiVisionSystem _vision;
  [Dependency]
  private IPrototypeManager _protoManager;
  private Robust.Shared.GameObjects.EntityQuery<BroadphaseComponent> _broadphaseQuery;
  private Robust.Shared.GameObjects.EntityQuery<MapGridComponent> _gridQuery;
  private static readonly EntProtoId DefaultAi = (EntProtoId) "StationAiBrain";
  private const float MaxVisionMultiplier = 5f;
  private ProtoId<StationAiCustomizationGroupPrototype> _stationAiCoreCustomGroupProtoId = (ProtoId<StationAiCustomizationGroupPrototype>) "StationAiCoreIconography";
  private ProtoId<StationAiCustomizationGroupPrototype> _stationAiHologramCustomGroupProtoId = (ProtoId<StationAiCustomizationGroupPrototype>) "StationAiHolograms";
  private const string JobNameLocId = "job-name-station-ai";

  private void InitializeAirlock()
  {
    this.SubscribeLocalEvent<DoorBoltComponent, StationAiBoltEvent>(new ComponentEventHandler<DoorBoltComponent, StationAiBoltEvent>(this.OnAirlockBolt));
    this.SubscribeLocalEvent<AirlockComponent, StationAiEmergencyAccessEvent>(new ComponentEventHandler<AirlockComponent, StationAiEmergencyAccessEvent>(this.OnAirlockEmergencyAccess));
    this.SubscribeLocalEvent<ElectrifiedComponent, StationAiElectrifiedEvent>(new ComponentEventHandler<ElectrifiedComponent, StationAiElectrifiedEvent>(this.OnElectrified));
  }

  private void OnAirlockBolt(EntityUid ent, DoorBoltComponent component, StationAiBoltEvent args)
  {
    if (component.BoltWireCut)
    {
      this.ShowDeviceNotRespondingPopup(args.User);
    }
    else
    {
      if (this._doors.TrySetBoltDown((Entity<DoorBoltComponent>) (ent, component), args.Bolted, new EntityUid?(args.User), true))
        return;
      this.ShowDeviceNotRespondingPopup(args.User);
    }
  }

  private void OnAirlockEmergencyAccess(
    EntityUid ent,
    AirlockComponent component,
    StationAiEmergencyAccessEvent args)
  {
    if (!this.PowerReceiver.IsPowered((Entity<SharedApcPowerReceiverComponent>) ent))
      this.ShowDeviceNotRespondingPopup(args.User);
    else
      this._airlocks.SetEmergencyAccess((Entity<AirlockComponent>) (ent, component), args.EmergencyAccess, new EntityUid?(args.User), true);
  }

  private void OnElectrified(
    EntityUid ent,
    ElectrifiedComponent component,
    StationAiElectrifiedEvent args)
  {
    if (component.IsWireCut || !this.PowerReceiver.IsPowered((Entity<SharedApcPowerReceiverComponent>) ent))
    {
      this.ShowDeviceNotRespondingPopup(args.User);
    }
    else
    {
      this._electrify.SetElectrified((Entity<ElectrifiedComponent>) (ent, component), args.Electrified);
      this._audio.PlayLocal(component.Enabled ? (SoundSpecifier) component.AirlockElectrifyDisabled : (SoundSpecifier) component.AirlockElectrifyEnabled, ent, new EntityUid?(args.User));
    }
  }

  public override void Initialize()
  {
    base.Initialize();
    this._broadphaseQuery = this.GetEntityQuery<BroadphaseComponent>();
    this._gridQuery = this.GetEntityQuery<MapGridComponent>();
    this.InitializeAirlock();
    this.InitializeHeld();
    this.InitializeLight();
    this.InitializeCustomization();
    this.SubscribeLocalEvent<StationAiWhitelistComponent, BoundUserInterfaceCheckRangeEvent>(new EntityEventRefHandler<StationAiWhitelistComponent, BoundUserInterfaceCheckRangeEvent>(this.OnAiBuiCheck));
    this.SubscribeLocalEvent<StationAiOverlayComponent, AccessibleOverrideEvent>(new EntityEventRefHandler<StationAiOverlayComponent, AccessibleOverrideEvent>(this.OnAiAccessible));
    this.SubscribeLocalEvent<StationAiOverlayComponent, InRangeOverrideEvent>(new EntityEventRefHandler<StationAiOverlayComponent, InRangeOverrideEvent>(this.OnAiInRange));
    this.SubscribeLocalEvent<StationAiOverlayComponent, MenuVisibilityEvent>(new EntityEventRefHandler<StationAiOverlayComponent, MenuVisibilityEvent>(this.OnAiMenu));
    this.SubscribeLocalEvent<StationAiHolderComponent, ComponentInit>(new EntityEventRefHandler<StationAiHolderComponent, ComponentInit>(this.OnHolderInit));
    this.SubscribeLocalEvent<StationAiHolderComponent, ComponentRemove>(new EntityEventRefHandler<StationAiHolderComponent, ComponentRemove>(this.OnHolderRemove));
    this.SubscribeLocalEvent<StationAiHolderComponent, AfterInteractEvent>(new EntityEventRefHandler<StationAiHolderComponent, AfterInteractEvent>(this.OnHolderInteract));
    this.SubscribeLocalEvent<StationAiHolderComponent, MapInitEvent>(new EntityEventRefHandler<StationAiHolderComponent, MapInitEvent>(this.OnHolderMapInit));
    this.SubscribeLocalEvent<StationAiHolderComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<StationAiHolderComponent, EntInsertedIntoContainerMessage>(this.OnHolderConInsert));
    this.SubscribeLocalEvent<StationAiHolderComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<StationAiHolderComponent, EntRemovedFromContainerMessage>(this.OnHolderConRemove));
    this.SubscribeLocalEvent<StationAiHolderComponent, IntellicardDoAfterEvent>(new EntityEventRefHandler<StationAiHolderComponent, IntellicardDoAfterEvent>(this.OnIntellicardDoAfter));
    this.SubscribeLocalEvent<StationAiCoreComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<StationAiCoreComponent, EntInsertedIntoContainerMessage>(this.OnAiInsert));
    this.SubscribeLocalEvent<StationAiCoreComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<StationAiCoreComponent, EntRemovedFromContainerMessage>(this.OnAiRemove));
    this.SubscribeLocalEvent<StationAiCoreComponent, MapInitEvent>(new EntityEventRefHandler<StationAiCoreComponent, MapInitEvent>(this.OnAiMapInit));
    this.SubscribeLocalEvent<StationAiCoreComponent, ComponentShutdown>(new EntityEventRefHandler<StationAiCoreComponent, ComponentShutdown>(this.OnAiShutdown));
    this.SubscribeLocalEvent<StationAiCoreComponent, PowerChangedEvent>(new EntityEventRefHandler<StationAiCoreComponent, PowerChangedEvent>(this.OnCorePower));
    this.SubscribeLocalEvent<StationAiCoreComponent, GetVerbsEvent<Verb>>(new EntityEventRefHandler<StationAiCoreComponent, GetVerbsEvent<Verb>>(this.OnCoreVerbs));
  }

  private void OnCoreVerbs(Entity<StationAiCoreComponent> ent, ref GetVerbsEvent<Verb> args)
  {
    EntityUid user = args.User;
    if (this._admin.IsAdmin(args.User) && !this.TryGetHeld((Entity<StationAiCoreComponent>) (ent.Owner, ent.Comp), out EntityUid _))
      args.Verbs.Add(new Verb()
      {
        Text = this.Loc.GetString("station-ai-takeover"),
        Category = VerbCategory.Debug,
        Act = (Action) (() => this._mind.ControlMob(user, this.SpawnInContainerOrDrop((string) SharedStationAiSystem.DefaultAi, ent.Owner, "station_ai_mind_slot"))),
        Impact = LogImpact.High
      });
    EntityUid insertedAi;
    if (!this.TryGetHeld((Entity<StationAiCoreComponent>) ((EntityUid) ent, ent.Comp), out insertedAi) || !(insertedAi == user))
      return;
    args.Verbs.Add(new Verb()
    {
      Text = this.Loc.GetString("station-ai-customization-menu"),
      Act = (Action) (() => this._uiSystem.TryOpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) StationAiCustomizationUiKey.Key, insertedAi)),
      Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/emotes.svg.192dpi.png"))
    });
  }

  private void OnAiAccessible(
    Entity<StationAiOverlayComponent> ent,
    ref AccessibleOverrideEvent args)
  {
    args.Handled = true;
    BaseContainer container;
    if (this._containers.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) args.Target, out container) || !this._containers.IsInSameOrTransparentContainer((Entity<TransformComponent, MetaDataComponent>) args.User, (Entity<TransformComponent, MetaDataComponent>) args.Target, otherContainer: container))
      return;
    args.Accessible = true;
  }

  private void OnAiMenu(Entity<StationAiOverlayComponent> ent, ref MenuVisibilityEvent args)
  {
    args.Visibility &= ~MenuVisibility.NoFov;
  }

  private void OnAiBuiCheck(
    Entity<StationAiWhitelistComponent> ent,
    ref BoundUserInterfaceCheckRangeEvent args)
  {
    if (!this.HasComp<StationAiHeldComponent>((EntityUid) args.Actor))
      return;
    args.Result = BoundUserInterfaceRangeResult.Fail;
    TransformComponent transformComponent = this.Transform(args.Target);
    EntityUid? gridUid1 = transformComponent.GridUid;
    EntityUid? gridUid2 = args.Actor.Comp.GridUid;
    BroadphaseComponent component1;
    MapGridComponent component2;
    if ((gridUid1.HasValue == gridUid2.HasValue ? (gridUid1.HasValue ? (gridUid1.GetValueOrDefault() != gridUid2.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0 || !this._broadphaseQuery.TryComp(transformComponent.GridUid, out component1) || !this._gridQuery.TryComp(transformComponent.GridUid, out component2))
      return;
    SharedMapSystem maps = this.Maps;
    gridUid2 = transformComponent.GridUid;
    EntityUid uid = gridUid2.Value;
    MapGridComponent grid1 = component2;
    EntityCoordinates coordinates = transformComponent.Coordinates;
    Vector2i tile1 = maps.LocalToTile(uid, grid1, coordinates);
    lock (this._vision)
    {
      StationAiVisionSystem vision = this._vision;
      gridUid2 = transformComponent.GridUid;
      Entity<BroadphaseComponent, MapGridComponent> grid2 = (Entity<BroadphaseComponent, MapGridComponent>) (gridUid2.Value, component1, component2);
      Vector2i tile2 = tile1;
      if (!vision.IsAccessible(grid2, tile2, fastPath: true))
        return;
      args.Result = BoundUserInterfaceRangeResult.Pass;
    }
  }

  private void OnAiInRange(Entity<StationAiOverlayComponent> ent, ref InRangeOverrideEvent args)
  {
    args.Handled = true;
    TransformComponent transformComponent = this.Transform(args.Target);
    EntityUid? gridUid1 = transformComponent.GridUid;
    EntityUid? gridUid2 = this.Transform(args.User).GridUid;
    BroadphaseComponent component1;
    MapGridComponent component2;
    if ((gridUid1.HasValue == gridUid2.HasValue ? (gridUid1.HasValue ? (gridUid1.GetValueOrDefault() != gridUid2.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0 || !this._broadphaseQuery.TryComp(transformComponent.GridUid, out component1) || !this._gridQuery.TryComp(transformComponent.GridUid, out component2))
      return;
    SharedMapSystem maps = this.Maps;
    gridUid2 = transformComponent.GridUid;
    EntityUid uid = gridUid2.Value;
    MapGridComponent grid1 = component2;
    EntityCoordinates coordinates = transformComponent.Coordinates;
    Vector2i tile1 = maps.LocalToTile(uid, grid1, coordinates);
    ref InRangeOverrideEvent local = ref args;
    StationAiVisionSystem vision = this._vision;
    gridUid2 = transformComponent.GridUid;
    Entity<BroadphaseComponent, MapGridComponent> grid2 = (Entity<BroadphaseComponent, MapGridComponent>) (gridUid2.Value, component1, component2);
    Vector2i tile2 = tile1;
    int num = vision.IsAccessible(grid2, tile2) ? 1 : 0;
    local.InRange = num != 0;
  }

  private void OnIntellicardDoAfter(
    Entity<StationAiHolderComponent> ent,
    ref IntellicardDoAfterEvent args)
  {
    StationAiHolderComponent comp;
    if (args.Cancelled || args.Handled || !this.TryComp<StationAiHolderComponent>(args.Args.Target, out comp))
      return;
    if (this._slots.CanEject(ent.Owner, new EntityUid?(args.User), ent.Comp.Slot))
    {
      if (!this._slots.TryInsert(args.Args.Target.Value, comp.Slot, ent.Comp.Slot.Item.Value, new EntityUid?(args.User), true))
        return;
      args.Handled = true;
    }
    else
    {
      if (!this._slots.CanEject(args.Args.Target.Value, new EntityUid?(args.User), comp.Slot) || !this._slots.TryInsert(ent.Owner, ent.Comp.Slot, comp.Slot.Item.Value, new EntityUid?(args.User), true))
        return;
      args.Handled = true;
    }
  }

  private void OnHolderInteract(Entity<StationAiHolderComponent> ent, ref AfterInteractEvent args)
  {
    if (args.Handled || !args.CanReach)
      return;
    EntityUid? nullable = args.Target;
    StationAiHolderComponent comp1;
    IntellicardComponent comp2;
    if (!nullable.HasValue || !this.TryComp<StationAiHolderComponent>(args.Target, out comp1) || this.HasComp<IntellicardComponent>(args.Target) || !this.TryComp<IntellicardComponent>(args.Used, out comp2))
      return;
    ItemSlotsSystem slots1 = this._slots;
    EntityUid owner = ent.Owner;
    EntityUid? user1 = new EntityUid?(args.User);
    ItemSlot slot1 = ent.Comp.Slot;
    nullable = new EntityUid?();
    EntityUid? popup1 = nullable;
    bool flag1 = slots1.CanEject(owner, user1, slot1, popup1);
    ItemSlotsSystem slots2 = this._slots;
    nullable = args.Target;
    EntityUid uid = nullable.Value;
    EntityUid? user2 = new EntityUid?(args.User);
    ItemSlot slot2 = comp1.Slot;
    nullable = new EntityUid?();
    EntityUid? popup2 = nullable;
    bool flag2 = slots2.CanEject(uid, user2, slot2, popup2);
    if (flag1 & flag2)
    {
      this._popup.PopupClient(this.Loc.GetString("intellicard-core-occupied"), args.User, new EntityUid?(args.User), PopupType.Medium);
      args.Handled = true;
    }
    else if (!flag1 && !flag2)
    {
      this._popup.PopupClient(this.Loc.GetString("intellicard-core-empty"), args.User, new EntityUid?(args.User), PopupType.Medium);
      args.Handled = true;
    }
    else
    {
      nullable = args.Target;
      EntityUid held;
      if (this.TryGetHeld((Entity<StationAiHolderComponent>) (nullable.Value, comp1), out held) && this._timing.CurTime > comp2.NextWarningAllowed)
      {
        comp2.NextWarningAllowed = this._timing.CurTime + comp2.WarningDelay;
        this.AnnounceIntellicardUsage(held, comp2.WarningSound);
      }
      EntityManager entityManager = this.EntityManager;
      EntityUid user3 = args.User;
      double seconds = flag1 ? (double) comp2.UploadTime : (double) comp2.DownloadTime;
      IntellicardDoAfterEvent @event = new IntellicardDoAfterEvent();
      EntityUid? target1 = args.Target;
      EntityUid? target2 = new EntityUid?(ent.Owner);
      nullable = new EntityUid?();
      EntityUid? used = nullable;
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, user3, (float) seconds, (DoAfterEvent) @event, target1, target2, used)
      {
        BreakOnDamage = true,
        BreakOnMove = true,
        NeedHand = true,
        BreakOnDropItem = true
      });
      args.Handled = true;
    }
  }

  private void OnHolderInit(Entity<StationAiHolderComponent> ent, ref ComponentInit args)
  {
    this._slots.AddItemSlot(ent.Owner, "station_ai_mind_slot", ent.Comp.Slot);
  }

  private void OnHolderRemove(Entity<StationAiHolderComponent> ent, ref ComponentRemove args)
  {
    this._slots.RemoveItemSlot(ent.Owner, ent.Comp.Slot);
  }

  private void OnHolderConInsert(
    Entity<StationAiHolderComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    this.UpdateAppearance((Entity<StationAiHolderComponent>) (ent.Owner, ent.Comp));
  }

  private void OnHolderConRemove(
    Entity<StationAiHolderComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    this.UpdateAppearance((Entity<StationAiHolderComponent>) (ent.Owner, ent.Comp));
  }

  private void OnHolderMapInit(Entity<StationAiHolderComponent> ent, ref MapInitEvent args)
  {
    this.UpdateAppearance((Entity<StationAiHolderComponent>) ent.Owner);
  }

  private void OnAiShutdown(Entity<StationAiCoreComponent> ent, ref ComponentShutdown args)
  {
    if (this._net.IsClient)
      return;
    this.QueueDel(ent.Comp.RemoteEntity);
    ent.Comp.RemoteEntity = new EntityUid?();
  }

  private void OnCorePower(Entity<StationAiCoreComponent> ent, ref PowerChangedEvent args)
  {
    if (args.Powered)
    {
      if (!this.SetupEye(ent))
        return;
      this.AttachEye(ent);
    }
    else
      this.ClearEye(ent);
  }

  private void OnAiMapInit(Entity<StationAiCoreComponent> ent, ref MapInitEvent args)
  {
    this.SetupEye(ent);
    this.AttachEye(ent);
  }

  public void SwitchRemoteEntityMode(Entity<StationAiCoreComponent?> entity, bool isRemote)
  {
    StationAiCoreComponent comp = entity.Comp;
    int num1;
    if (comp == null)
    {
      num1 = 1;
    }
    else
    {
      int num2 = comp.Remote ? 1 : 0;
      num1 = 0;
    }
    if (num1 != 0 || entity.Comp.Remote == isRemote)
      return;
    Entity<StationAiCoreComponent> ent = new Entity<StationAiCoreComponent>(entity.Owner, entity.Comp);
    ent.Comp.Remote = isRemote;
    EntityCoordinates? coords = ent.Comp.RemoteEntity.HasValue ? new EntityCoordinates?(this.Transform(ent.Comp.RemoteEntity.Value).Coordinates) : new EntityCoordinates?();
    EntityUid? remoteEntity = ent.Comp.RemoteEntity;
    this.ClearEye(ent);
    if (this.SetupEye(ent, coords))
      this.AttachEye(ent);
    if (remoteEntity.HasValue)
    {
      StationAiRemoteEntityReplacementEvent args = new StationAiRemoteEntityReplacementEvent(ent.Comp.RemoteEntity);
      this.RaiseLocalEvent<StationAiRemoteEntityReplacementEvent>(remoteEntity.Value, ref args);
    }
    EntityUid? insertedAi = this.GetInsertedAI(ent);
    if (!this.TryComp<EyeComponent>(insertedAi, out EyeComponent _))
      return;
    this._eye.SetDrawFov(insertedAi.Value, !isRemote);
  }

  private bool SetupEye(Entity<StationAiCoreComponent> ent, EntityCoordinates? coords = null)
  {
    if (this._net.IsClient || ent.Comp.RemoteEntity.HasValue)
      return false;
    EntProtoId? nullable1 = ent.Comp.RemoteEntityProto;
    if (!coords.HasValue)
      coords = new EntityCoordinates?(this.Transform(ent.Owner).Coordinates);
    if (!ent.Comp.Remote)
      nullable1 = ent.Comp.PhysicalEntityProto;
    if (nullable1.HasValue)
    {
      StationAiCoreComponent comp = ent.Comp;
      EntProtoId? nullable2 = nullable1;
      EntityUid? nullable3 = new EntityUid?(this.SpawnAtPosition(nullable2.HasValue ? (string) nullable2.GetValueOrDefault() : (string) null, coords.Value));
      comp.RemoteEntity = nullable3;
      this.Dirty<StationAiCoreComponent>(ent);
    }
    return true;
  }

  private void ClearEye(Entity<StationAiCoreComponent> ent)
  {
    if (this._net.IsClient)
      return;
    this.QueueDel(ent.Comp.RemoteEntity);
    ent.Comp.RemoteEntity = new EntityUid?();
    this.Dirty<StationAiCoreComponent>(ent);
  }

  private void AttachEye(Entity<StationAiCoreComponent> ent)
  {
    BaseContainer container;
    if (!ent.Comp.RemoteEntity.HasValue || !this._containers.TryGetContainer(ent.Owner, "station_ai_mind_slot", out container) || container.ContainedEntities.Count != 1)
      return;
    EntityUid containedEntity = container.ContainedEntities[0];
    EyeComponent comp;
    if (this.TryComp<EyeComponent>(containedEntity, out comp))
    {
      this._eye.SetDrawFov(containedEntity, false, comp);
      this._eye.SetTarget(containedEntity, new EntityUid?(ent.Comp.RemoteEntity.Value), comp);
    }
    this._mover.SetRelay(containedEntity, ent.Comp.RemoteEntity.Value);
  }

  private EntityUid? GetInsertedAI(Entity<StationAiCoreComponent> ent)
  {
    BaseContainer container;
    return !this._containers.TryGetContainer(ent.Owner, "station_ai_mind_slot", out container) || container.ContainedEntities.Count != 1 ? new EntityUid?() : new EntityUid?(container.ContainedEntities[0]);
  }

  private void OnAiInsert(
    Entity<StationAiCoreComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    if (args.Container.ID != "station_ai_mind_slot" || this._timing.ApplyingState)
      return;
    ent.Comp.Remote = true;
    this.SetupEye(ent);
    this._metadata.SetEntityName(ent.Owner, this.MetaData(args.Entity).EntityName);
    this.AttachEye(ent);
  }

  private void OnAiRemove(
    Entity<StationAiCoreComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    if (this._timing.ApplyingState)
      return;
    ent.Comp.Remote = true;
    this._metadata.SetEntityName(ent.Owner, this.Prototype(ent.Owner)?.Name ?? string.Empty);
    this.RemCompDeferred<RelayInputMoverComponent>(args.Entity);
    EyeComponent comp;
    if (this.TryComp<EyeComponent>(args.Entity, out comp))
    {
      this._eye.SetDrawFov(args.Entity, true, comp);
      this._eye.SetTarget(args.Entity, new EntityUid?(), comp);
    }
    this.ClearEye(ent);
  }

  private void UpdateAppearance(Entity<StationAiHolderComponent?> entity)
  {
    if (!this.Resolve<StationAiHolderComponent>(entity.Owner, ref entity.Comp, false))
      return;
    StationAiState state = StationAiState.Empty;
    BaseContainer container;
    if (this._containers.TryGetContainer(entity.Owner, "station_ai_mind_slot", out container) && container.Count > 0)
      state = StationAiState.Occupied;
    StationAiCoreComponent comp;
    if (this.TryComp<StationAiCoreComponent>((EntityUid) entity, out comp))
      this.CustomizeAppearance((Entity<StationAiCoreComponent>) ((EntityUid) entity, comp), state);
    else
      this._appearance.SetData(entity.Owner, (Enum) StationAiVisualState.Key, (object) state);
  }

  public virtual void AnnounceIntellicardUsage(EntityUid uid, SoundSpecifier? cue = null)
  {
  }

  public virtual bool SetVisionEnabled(
    Entity<StationAiVisionComponent> entity,
    bool enabled,
    bool announce = false)
  {
    if (entity.Comp.Enabled == enabled)
      return false;
    entity.Comp.Enabled = enabled;
    this.Dirty<StationAiVisionComponent>(entity);
    return true;
  }

  public virtual bool SetWhitelistEnabled(
    Entity<StationAiWhitelistComponent> entity,
    bool value,
    bool announce = false)
  {
    if (entity.Comp.Enabled == value)
      return false;
    entity.Comp.Enabled = value;
    this.Dirty<StationAiWhitelistComponent>(entity);
    return true;
  }

  private bool ValidateAi(Entity<StationAiHeldComponent?> entity)
  {
    return this.Resolve<StationAiHeldComponent>(entity.Owner, ref entity.Comp, false) && this._blocker.CanComplexInteract(entity.Owner);
  }

  private void InitializeCustomization()
  {
    this.SubscribeLocalEvent<StationAiCoreComponent, StationAiCustomizationMessage>(new EntityEventRefHandler<StationAiCoreComponent, StationAiCustomizationMessage>(this.OnStationAiCustomization));
  }

  private void OnStationAiCustomization(
    Entity<StationAiCoreComponent> entity,
    ref StationAiCustomizationMessage args)
  {
    StationAiCustomizationGroupPrototype prototype;
    EntityUid held;
    StationAiCustomizationComponent comp1;
    ProtoId<StationAiCustomizationPrototype> protoId;
    if (!this._protoManager.TryIndex<StationAiCustomizationGroupPrototype>(args.GroupProtoId, out prototype) || !this._protoManager.TryIndex<StationAiCustomizationPrototype>(args.CustomizationProtoId, out StationAiCustomizationPrototype _) || !this.TryGetHeld((Entity<StationAiCoreComponent>) ((EntityUid) entity, entity.Comp), out held) || !this.TryComp<StationAiCustomizationComponent>(held, out comp1) || comp1.ProtoIds.TryGetValue(args.GroupProtoId, out protoId) && protoId == args.CustomizationProtoId)
      return;
    comp1.ProtoIds[args.GroupProtoId] = args.CustomizationProtoId;
    this.Dirty(held, (IComponent) comp1);
    if (prototype.Category == StationAiCustomizationType.Hologram)
      this.UpdateHolographicAvatar((Entity<StationAiCustomizationComponent>) (held, comp1));
    StationAiHolderComponent comp2;
    if (prototype.Category != StationAiCustomizationType.CoreIconography || !this.TryComp<StationAiHolderComponent>((EntityUid) entity, out comp2))
      return;
    this.UpdateAppearance((Entity<StationAiHolderComponent>) ((EntityUid) entity, comp2));
  }

  private void UpdateHolographicAvatar(Entity<StationAiCustomizationComponent> entity)
  {
    HolographicAvatarComponent comp;
    ProtoId<StationAiCustomizationPrototype> id;
    StationAiCustomizationPrototype prototype;
    PrototypeLayerData prototypeLayerData;
    if (!this.TryComp<HolographicAvatarComponent>((EntityUid) entity, out comp) || !entity.Comp.ProtoIds.TryGetValue(this._stationAiHologramCustomGroupProtoId, out id) || !this._protoManager.TryIndex<StationAiCustomizationPrototype>(id, out prototype) || !prototype.LayerData.TryGetValue(StationAiState.Hologram.ToString(), out prototypeLayerData))
      return;
    comp.LayerData = new PrototypeLayerData[1]
    {
      prototypeLayerData
    };
    this.Dirty((EntityUid) entity, (IComponent) comp);
  }

  private void CustomizeAppearance(Entity<StationAiCoreComponent> entity, StationAiState state)
  {
    EntityUid? insertedAi = this.GetInsertedAI(entity);
    if (!insertedAi.HasValue)
    {
      this._appearance.RemoveData(entity.Owner, (Enum) StationAiVisualState.Key);
    }
    else
    {
      StationAiCustomizationComponent comp;
      ProtoId<StationAiCustomizationPrototype> id;
      StationAiCustomizationPrototype prototype;
      PrototypeLayerData prototypeLayerData;
      if (!this.TryComp<StationAiCustomizationComponent>(insertedAi, out comp) || !comp.ProtoIds.TryGetValue(this._stationAiCoreCustomGroupProtoId, out id) || !this._protoManager.TryIndex<StationAiCustomizationPrototype>(id, out prototype) || !prototype.LayerData.TryGetValue(state.ToString(), out prototypeLayerData))
        return;
      this._appearance.SetData(entity.Owner, (Enum) StationAiVisualState.Key, (object) prototypeLayerData);
    }
  }

  private void InitializeHeld()
  {
    this.SubscribeLocalEvent<StationAiRadialMessage>(new EntityEventHandler<StationAiRadialMessage>(this.OnRadialMessage));
    this.SubscribeLocalEvent<StationAiWhitelistComponent, BoundUserInterfaceMessageAttempt>(new EntityEventRefHandler<StationAiWhitelistComponent, BoundUserInterfaceMessageAttempt>(this.OnMessageAttempt));
    this.SubscribeLocalEvent<StationAiWhitelistComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<StationAiWhitelistComponent, GetVerbsEvent<AlternativeVerb>>(this.OnTargetVerbs));
    this.SubscribeLocalEvent<StationAiHeldComponent, InteractionAttemptEvent>(new EntityEventRefHandler<StationAiHeldComponent, InteractionAttemptEvent>(this.OnHeldInteraction));
    this.SubscribeLocalEvent<StationAiHeldComponent, AttemptRelayActionComponentChangeEvent>(new EntityEventRefHandler<StationAiHeldComponent, AttemptRelayActionComponentChangeEvent>(this.OnHeldRelay));
    this.SubscribeLocalEvent<StationAiHeldComponent, JumpToCoreEvent>(new EntityEventRefHandler<StationAiHeldComponent, JumpToCoreEvent>(this.OnCoreJump));
    this.SubscribeLocalEvent<TryGetIdentityShortInfoEvent>(new EntityEventHandler<TryGetIdentityShortInfoEvent>(this.OnTryGetIdentityShortInfo));
  }

  private void OnTryGetIdentityShortInfo(TryGetIdentityShortInfoEvent args)
  {
    if (args.Handled || !this.HasComp<StationAiHeldComponent>(args.ForActor))
      return;
    args.Title = $"{this.Name(args.ForActor)} ({this.Loc.GetString("job-name-station-ai")})";
    args.Handled = true;
  }

  private void OnCoreJump(Entity<StationAiHeldComponent> ent, ref JumpToCoreEvent args)
  {
    Entity<StationAiCoreComponent> core;
    if (!this.TryGetCore(ent.Owner, out core))
      return;
    StationAiCoreComponent comp = core.Comp;
    if ((comp != null ? (!comp.RemoteEntity.HasValue ? 1 : 0) : 1) != 0)
      return;
    this._xforms.DropNextTo((Entity<TransformComponent>) core.Comp.RemoteEntity.Value, (Entity<TransformComponent>) core.Owner);
  }

  public bool TryGetHeld(Entity<StationAiCoreComponent?> entity, out EntityUid held)
  {
    held = EntityUid.Invalid;
    BaseContainer container;
    if (!this.Resolve<StationAiCoreComponent>(entity.Owner, ref entity.Comp) || !this._containers.TryGetContainer(entity.Owner, "station_ai_mind_slot", out container) || container.ContainedEntities.Count == 0)
      return false;
    held = container.ContainedEntities[0];
    return true;
  }

  public bool TryGetHeld(Entity<StationAiHolderComponent?> entity, out EntityUid held)
  {
    StationAiCoreComponent comp;
    this.TryComp<StationAiCoreComponent>(entity.Owner, out comp);
    return this.TryGetHeld((Entity<StationAiCoreComponent>) (entity.Owner, comp), out held);
  }

  public bool TryGetCore(EntityUid entity, out Entity<StationAiCoreComponent?> core)
  {
    TransformComponent comp1 = this.Transform(entity);
    MetaDataComponent comp2 = this.MetaData(entity);
    BaseContainer container;
    StationAiCoreComponent comp;
    if (!this._containers.TryGetContainingContainer(new Entity<TransformComponent, MetaDataComponent>(entity, comp1, comp2), out container) || container.ID != "station_ai_mind_slot" || !this.TryComp<StationAiCoreComponent>(container.Owner, out comp) || !comp.RemoteEntity.HasValue)
    {
      core = (Entity<StationAiCoreComponent>) (EntityUid.Invalid, (StationAiCoreComponent) null);
      return false;
    }
    core = (Entity<StationAiCoreComponent>) (container.Owner, comp);
    return true;
  }

  private void OnHeldRelay(
    Entity<StationAiHeldComponent> ent,
    ref AttemptRelayActionComponentChangeEvent args)
  {
    Entity<StationAiCoreComponent> core;
    if (!this.TryGetCore(ent.Owner, out core))
      return;
    args.Target = (EntityUid?) core.Comp?.RemoteEntity;
  }

  private void OnRadialMessage(StationAiRadialMessage ev)
  {
    EntityUid? entity;
    if (!this.TryGetEntity(ev.Entity, out entity))
      return;
    ev.Event.User = ev.Actor;
    this.RaiseLocalEvent(entity.Value, (object) ev.Event);
  }

  private void OnMessageAttempt(
    Entity<StationAiWhitelistComponent> ent,
    ref BoundUserInterfaceMessageAttempt ev)
  {
    StationAiHeldComponent comp1;
    StationAiWhitelistComponent comp2;
    if (ev.Actor == ev.Target || !this.TryComp<StationAiHeldComponent>(ev.Actor, out comp1) || this.TryComp<StationAiWhitelistComponent>(ev.Target, out comp2) && this.ValidateAi((Entity<StationAiHeldComponent>) (ev.Actor, comp1)))
      return;
    if (!this.PowerReceiver.IsPowered((Entity<SharedApcPowerReceiverComponent>) ev.Target))
    {
      this.ShowDeviceNotRespondingPopup(ev.Actor);
      ev.Cancel();
    }
    else
    {
      if (comp2 != null && !comp2.Enabled)
        this.ShowDeviceNotRespondingPopup(ev.Actor);
      ev.Cancel();
    }
  }

  private void OnHeldInteraction(
    Entity<StationAiHeldComponent> ent,
    ref InteractionAttemptEvent args)
  {
    ref InteractionAttemptEvent local = ref args;
    StationAiWhitelistComponent comp;
    int num;
    if (!this.TryComp<StationAiWhitelistComponent>(args.Target, out comp) || !comp.Enabled)
    {
      EntityUid owner = ent.Owner;
      EntityUid? target = args.Target;
      if ((target.HasValue ? (owner != target.GetValueOrDefault() ? 1 : 0) : 1) != 0)
      {
        num = args.Target.HasValue ? 1 : 0;
        goto label_4;
      }
    }
    num = 0;
label_4:
    local.Cancelled = num != 0;
    if (comp == null || comp.Enabled)
      return;
    this.ShowDeviceNotRespondingPopup(ent.Owner);
  }

  private void OnTargetVerbs(
    Entity<StationAiWhitelistComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!this._uiSystem.HasUi(args.Target, (Enum) AiUi.Key) || !args.CanComplexInteract || !this.HasComp<StationAiHeldComponent>(args.User) || !args.CanInteract)
      return;
    EntityUid user = args.User;
    bool isOpen = this._uiSystem.IsUiOpen((Entity<UserInterfaceComponent>) args.Target, (Enum) AiUi.Key, user);
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Text = isOpen ? this.Loc.GetString("ai-close") : this.Loc.GetString("ai-open");
    alternativeVerb1.Act = (Action) (() =>
    {
      if (isOpen)
        this._uiSystem.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) AiUi.Key, new EntityUid?(user));
      else
        this._uiSystem.OpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) AiUi.Key, new EntityUid?(user));
    });
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    args.Verbs.Add(alternativeVerb2);
  }

  private void ShowDeviceNotRespondingPopup(EntityUid toEntity)
  {
    this._popup.PopupClient(this.Loc.GetString("ai-device-not-responding"), new EntityUid?(toEntity), PopupType.MediumCaution);
  }

  private void InitializeLight()
  {
    this.SubscribeLocalEvent<ItemTogglePointLightComponent, StationAiLightEvent>(new ComponentEventHandler<ItemTogglePointLightComponent, StationAiLightEvent>(this.OnLight));
  }

  private void OnLight(
    EntityUid ent,
    ItemTogglePointLightComponent component,
    StationAiLightEvent args)
  {
    if (args.Enabled)
      this._toggles.TryActivate((Entity<ItemToggleComponent>) ent, new EntityUid?(args.User));
    else
      this._toggles.TryDeactivate((Entity<ItemToggleComponent>) ent, new EntityUid?(args.User));
  }
}
