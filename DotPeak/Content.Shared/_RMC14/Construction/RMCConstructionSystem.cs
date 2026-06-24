// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Construction.RMCConstructionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Teams;
using Content.Shared._PUBG.Construction;
using Content.Shared._RMC14.Construction.Prototypes;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Ladder;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Construction.Components;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Construction;

public sealed class RMCConstructionSystem : EntitySystem
{
  [Dependency]
  private FixtureSystem _fixture;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private TurfSystem _turf;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private SharedStackSystem _stack;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private ExamineSystemShared _examine;
  private static readonly EntProtoId Blocker = (EntProtoId) "RMCDropshipDoorBlocker";
  private readonly List<EntityCoordinates> _toCreate = new List<EntityCoordinates>();
  private Robust.Shared.GameObjects.EntityQuery<DoorComponent> _doorQuery;

  public override void Initialize()
  {
    this._doorQuery = this.GetEntityQuery<DoorComponent>();
    this.SubscribeLocalEvent<DropshipHijackLandedEvent>(new EntityEventRefHandler<DropshipHijackLandedEvent>(this.OnDropshipHijackLanded));
    this.SubscribeLocalEvent<RMCConstructionPreventCollideComponent, PreventCollideEvent>(new EntityEventRefHandler<RMCConstructionPreventCollideComponent, PreventCollideEvent>(this.OnConstructionPreventCollide));
    this.SubscribeLocalEvent<RMCConstructionItemComponent, UseInHandEvent>(new EntityEventRefHandler<RMCConstructionItemComponent, UseInHandEvent>(this.OnUseInHand));
    this.SubscribeLocalEvent<RMCConstructionItemComponent, RMCConstructionBuildDoAfterEvent>(new EntityEventRefHandler<RMCConstructionItemComponent, RMCConstructionBuildDoAfterEvent>(this.OnBuildDoAfter));
    this.SubscribeLocalEvent<RMCConstructionAttemptEvent>(new EntityEventRefHandler<RMCConstructionAttemptEvent>(this.OnConstructionAttempt));
    this.SubscribeLocalEvent<DropshipComponent, DropshipMapInitEvent>(new EntityEventRefHandler<DropshipComponent, DropshipMapInitEvent>(this.OnDropshipMapInit));
    this.SubscribeLocalEvent<RMCDropshipBlockedComponent, MapInitEvent>(new EntityEventRefHandler<RMCDropshipBlockedComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<RMCDropshipBlockedComponent, AnchorAttemptEvent>(new EntityEventRefHandler<RMCDropshipBlockedComponent, AnchorAttemptEvent>(this.OnAnchorAttempt));
    this.SubscribeLocalEvent<RMCDropshipBlockedComponent, UserAnchoredEvent>(new EntityEventRefHandler<RMCDropshipBlockedComponent, UserAnchoredEvent>(this.OnUserAnchored));
    this.Subs.BuiEvents<RMCConstructionItemComponent>((object) RMCConstructionUiKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<RMCConstructionItemComponent>) (subs => subs.Event<RMCConstructionBuiMsg>(new EntityEventRefHandler<RMCConstructionItemComponent, RMCConstructionBuiMsg>(this.OnConstructionBuiMsg))));
  }

  private void OnDropshipHijackLanded(ref DropshipHijackLandedEvent ev)
  {
    if (this._net.IsClient)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCReplaceOnHijackLandComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCReplaceOnHijackLandComponent>();
    EntityUid uid;
    RMCReplaceOnHijackLandComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      EntProtoId? id = comp1.Id;
      if (id.HasValue)
      {
        EntProtoId valueOrDefault = id.GetValueOrDefault();
        EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(uid);
        this.Del(new EntityUid?(uid));
        this.Spawn((string) valueOrDefault, moverCoordinates);
      }
      else
        this.Del(new EntityUid?(uid));
    }
  }

  public void OnUseInHand(Entity<RMCConstructionItemComponent> ent, ref UseInHandEvent args)
  {
    EntityUid user = args.User;
    args.Handled = true;
    this._ui.OpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) RMCConstructionUiKey.Key, new EntityUid?(user));
  }

  private void OnConstructionBuiMsg(
    Entity<RMCConstructionItemComponent> ent,
    ref RMCConstructionBuiMsg args)
  {
    this.Build(ent, args.Actor, args.Build, args.Amount);
  }

  public bool Build(
    Entity<RMCConstructionItemComponent> ent,
    EntityUid user,
    ProtoId<RMCConstructionPrototype> protoID,
    int amount)
  {
    RMCConstructionPrototype prototype;
    TransformComponent comp1;
    if (this._net.IsClient || !this._prototype.TryIndex<RMCConstructionPrototype>(protoID, out prototype) || !this.TryComp(user, out comp1))
      return false;
    EntProtoId<SkillDefinitionComponent>? skill = prototype.Skill;
    if (skill.HasValue)
    {
      EntProtoId<SkillDefinitionComponent> valueOrDefault = skill.GetValueOrDefault();
      if (!this._skills.HasSkill((Entity<SkillsComponent>) user, valueOrDefault, prototype.RequiredSkillLevel))
      {
        this._popup.PopupEntity(this.Loc.GetString("rmc-construction-untrained-build"), (EntityUid) ent, user, PopupType.SmallCaution);
        return false;
      }
    }
    CivTeamMemberComponent comp2;
    if (!string.IsNullOrEmpty(prototype.SideId) && (!this.TryComp<CivTeamMemberComponent>(user, out comp2) || !string.Equals(comp2.SideId, prototype.SideId, StringComparison.OrdinalIgnoreCase)))
    {
      this._popup.PopupEntity(this.Loc.GetString("civ-construction-wrong-side"), (EntityUid) ent, user, PopupType.SmallCaution);
      return false;
    }
    Angle localRotation = comp1.LocalRotation;
    Direction cardinalDir = ((Angle) ref localRotation).GetCardinalDir();
    EntityCoordinates entityCoordinates = comp1.Coordinates;
    if (prototype.PlaceInFront)
      entityCoordinates = entityCoordinates.Offset(DirectionExtensions.ToVec(cardinalDir));
    string popup;
    if (!prototype.IgnoreBuildRestrictions && !this.CanBuildAt(entityCoordinates, prototype.Name, out popup, direction: cardinalDir, collision: prototype.RestrictedCollisionGroup))
    {
      this._popup.PopupEntity(popup, (EntityUid) ent, user, PopupType.SmallCaution);
      return false;
    }
    ProtoId<TagPrototype>[] restrictedTags = prototype.RestrictedTags;
    if (restrictedTags != null && this._rmcMap.TileHasAnyTag(entityCoordinates, restrictedTags))
    {
      this._popup.PopupEntity(this.Loc.GetString("rmc-construction-not-proper-surface", ("construction", (object) prototype.Name)), (EntityUid) ent, user, PopupType.SmallCaution);
      return false;
    }
    if (this.HasComp<PubgBlueprintHammerComponent>((EntityUid) ent))
    {
      PubgPlaceBlueprintEvent message = new PubgPlaceBlueprintEvent(protoID, user, entityCoordinates, cardinalDir);
      this.RaiseLocalEvent<PubgPlaceBlueprintEvent>(ref message);
      return true;
    }
    int? materialCost = prototype.MaterialCost;
    if (materialCost.HasValue)
    {
      int valueOrDefault = materialCost.GetValueOrDefault();
      StackComponent comp3;
      if (this.TryComp<StackComponent>(ent.Owner, out comp3))
      {
        int num1 = amount / prototype.Amount;
        int num2 = amount == prototype.Amount ? valueOrDefault : num1 * valueOrDefault;
        if (comp3.Count < num2)
        {
          this._popup.PopupEntity(this.Loc.GetString("rmc-construction-more-material", ("material", (object) ent), ("object", (object) prototype.Name)), user, user, PopupType.SmallCaution);
          return false;
        }
      }
    }
    RMCConstructionBuildDoAfterEvent @event = new RMCConstructionBuildDoAfterEvent(prototype, amount, this.GetNetCoordinates(entityCoordinates), cardinalDir);
    int num3 = this._skills.HasSkill((Entity<SkillsComponent>) user, prototype.DelaySkill, 2) ? 1 : 2;
    double num4 = Math.Max((prototype.DoAfterTime * (double) num3).TotalSeconds, prototype.DoAfterTimeMin.TotalSeconds);
    if (this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, TimeSpan.FromSeconds(num4), (DoAfterEvent) @event, new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent))
    {
      BreakOnMove = true,
      BreakOnDamage = false,
      MovementThreshold = 0.5f,
      DuplicateCondition = DuplicateConditions.SameEvent,
      CancelDuplicate = true
    }))
      this.SpawnConstructionGhost(user, prototype.Prototype, entityCoordinates, cardinalDir, prototype.NoRotate);
    this.UpdateStackAmountUI(ent);
    return true;
  }

  private void SpawnConstructionGhost(
    EntityUid user,
    EntProtoId targetProto,
    EntityCoordinates coordinates,
    Direction direction,
    bool noRotate)
  {
    if (this._net.IsClient)
      return;
    this.ClearConstructionGhost(user);
    EntityUid uid = this.SpawnAtPosition((string) targetProto, coordinates);
    if (!noRotate)
      this._transform.SetLocalRotation(uid, DirectionExtensions.ToAngle(direction));
    this.EnsureComp<RMCConstructionGhostComponent>(uid);
    this.EnsureComp<RMCActiveConstructionGhostComponent>(user).Ghost = new EntityUid?(uid);
  }

  private void ClearConstructionGhost(EntityUid user)
  {
    RMCActiveConstructionGhostComponent comp;
    if (this._net.IsClient || !this.TryComp<RMCActiveConstructionGhostComponent>(user, out comp))
      return;
    EntityUid? ghost = comp.Ghost;
    if (ghost.HasValue)
    {
      EntityUid valueOrDefault = ghost.GetValueOrDefault();
      if (!this.Deleted(valueOrDefault))
        this.QueueDel(new EntityUid?(valueOrDefault));
    }
    this.RemCompDeferred<RMCActiveConstructionGhostComponent>(user);
  }

  private void OnConstructionPreventCollide(
    Entity<RMCConstructionPreventCollideComponent> ent,
    ref PreventCollideEvent args)
  {
    if (args.Cancelled)
      return;
    EntityUid? target = ent.Comp.Target;
    if (target.HasValue)
    {
      EntityUid valueOrDefault = target.GetValueOrDefault();
      if (!this.Deleted(valueOrDefault))
      {
        if (args.OtherEntity != valueOrDefault)
          return;
        if (!this._examine.InRangeUnOccluded(ent.Owner, valueOrDefault, ent.Comp.Range))
        {
          this.RemCompDeferred<RMCConstructionPreventCollideComponent>(ent.Owner);
          return;
        }
        args.Cancelled = true;
        return;
      }
    }
    this.RemCompDeferred<RMCConstructionPreventCollideComponent>(ent.Owner);
  }

  public void MakeConstructionImmuneToCollision(EntityUid construction, EntityUid user)
  {
    RMCConstructionPreventCollideComponent collideComponent = this.EnsureComp<RMCConstructionPreventCollideComponent>(construction);
    collideComponent.Target = new EntityUid?(user);
    this.Dirty(construction, (IComponent) collideComponent);
  }

  private void OnBuildDoAfter(
    Entity<RMCConstructionItemComponent> ent,
    ref RMCConstructionBuildDoAfterEvent args)
  {
    this.ClearConstructionGhost(args.User);
    if (args.Handled || args.Cancelled || this._net.IsClient)
      return;
    RMCConstructionPrototype prototype = args.Prototype;
    EntityCoordinates coordinates = this.GetCoordinates(args.Coordinates);
    args.Handled = true;
    int num1 = args.Amount / prototype.Amount;
    int? nullable1;
    if (args.Amount != prototype.Amount)
    {
      int num2 = num1;
      int? materialCost = prototype.MaterialCost;
      nullable1 = materialCost.HasValue ? new int?(num2 * materialCost.GetValueOrDefault()) : new int?();
    }
    else
      nullable1 = prototype.MaterialCost;
    int? nullable2 = nullable1;
    StackComponent comp;
    if (this.TryComp<StackComponent>(ent.Owner, out comp))
    {
      if (!this._stack.Use(ent.Owner, nullable2 ?? 1, comp))
      {
        this._popup.PopupEntity(this.Loc.GetString("rmc-construction-more-material", ("material", (object) ent.Owner), ("object", (object) prototype.Name)), args.User, args.User, PopupType.SmallCaution);
        return;
      }
    }
    else if (this._net.IsServer)
      this.QueueDel(new EntityUid?(ent.Owner));
    if (!this.Deleted((EntityUid) ent))
      this.UpdateStackAmountUI(ent);
    if (args.Amount > 1)
    {
      this.SpawnMultiple((string) prototype.Prototype, args.Amount, coordinates);
    }
    else
    {
      EntityUid entityUid = this.SpawnAtPosition((string) prototype.Prototype, coordinates);
      if (!prototype.NoRotate)
        this._transform.SetLocalRotation(entityUid, DirectionExtensions.ToAngle(args.Direction));
      if (!this.HasComp<BarricadeComponent>(entityUid))
        this.MakeConstructionImmuneToCollision(entityUid, args.User);
      RMCConstructionBuiltEvent message = new RMCConstructionBuiltEvent(entityUid, args.User);
      this.RaiseLocalEvent<RMCConstructionBuiltEvent>(ref message);
    }
  }

  public List<EntityUid> SpawnMultiple(
    string entityPrototype,
    int amount,
    EntityCoordinates spawnPosition)
  {
    if (this._net.IsClient)
      return new List<EntityUid>();
    if (amount <= 0)
    {
      this.Log.Error($"Attempted to spawn an invalid stack: {entityPrototype}, {amount}. Trace: {Environment.StackTrace}");
      return new List<EntityUid>();
    }
    List<int> spawns = this.CalculateSpawns(entityPrototype, amount);
    List<EntityUid> entityUidList = new List<EntityUid>();
    foreach (int amount1 in spawns)
    {
      EntityUid uid = this.SpawnAtPosition(entityPrototype, spawnPosition);
      entityUidList.Add(uid);
      this._stack.SetCount(uid, amount1);
    }
    return entityUidList;
  }

  public List<int> CalculateSpawns(string entityPrototype, int amount)
  {
    StackComponent component;
    this._prototype.Index<EntityPrototype>(entityPrototype).TryGetComponent<StackComponent>(out component, this.EntityManager.ComponentFactory);
    int maxCount = this._stack.GetMaxCount(component);
    List<int> spawns = new List<int>();
    while (amount > 0)
    {
      int num = Math.Min(maxCount, amount);
      amount -= num;
      spawns.Add(num);
    }
    return spawns;
  }

  private void UpdateStackAmountUI(Entity<RMCConstructionItemComponent> ent)
  {
    RMCConstructionBuiState state = new RMCConstructionBuiState(string.Empty);
    this._ui.SetUiState((Entity<UserInterfaceComponent>) ent.Owner, (Enum) RMCConstructionUiKey.Key, (BoundUserInterfaceState) state);
  }

  private void OnConstructionAttempt(ref RMCConstructionAttemptEvent ev)
  {
    string popup;
    if (ev.Cancelled || this.CanBuildAt(ev.Location, ev.PrototypeName, out popup, direction: (Direction) -1))
      return;
    ev.Popup = popup;
    ev.Cancelled = true;
  }

  private void OnDropshipMapInit(Entity<DropshipComponent> ent, ref DropshipMapInitEvent args)
  {
    this._toCreate.Clear();
    TransformChildrenEnumerator childEnumerator = this.Transform((EntityUid) ent).ChildEnumerator;
    EntityUid child;
    while (childEnumerator.MoveNext(out child))
    {
      if (this._doorQuery.HasComp(child))
        this._toCreate.Add(child.ToCoordinates());
    }
    foreach (EntityCoordinates coordinates in this._toCreate)
      this.SpawnAtPosition((string) RMCConstructionSystem.Blocker, coordinates);
  }

  private void OnMapInit(Entity<RMCDropshipBlockedComponent> ent, ref MapInitEvent args)
  {
    PhysicsComponent comp;
    if (!this.TryComp<PhysicsComponent>((EntityUid) ent, out comp))
      return;
    PhysShapeCircle shape = new PhysShapeCircle(0.49f);
    this._fixture.TryCreateFixture((EntityUid) ent, (IPhysShape) shape, ent.Comp.FixtureId, collisionMask: 268435456 /*0x10000000*/, body: comp);
  }

  private void OnAnchorAttempt(Entity<RMCDropshipBlockedComponent> ent, ref AnchorAttemptEvent args)
  {
    string popup;
    if (args.Cancelled || this.CanBuildAt(ent.Owner.ToCoordinates(), this.Name((EntityUid) ent), out popup, true, (Direction) -1))
      return;
    this._popup.PopupClient(popup, (EntityUid) ent, new EntityUid?(args.User), PopupType.SmallCaution);
    args.Cancel();
  }

  private void OnUserAnchored(Entity<RMCDropshipBlockedComponent> ent, ref UserAnchoredEvent args)
  {
    if (this.CanBuildAt(ent.Owner.ToCoordinates(), this.Name((EntityUid) ent), out string _, true, (Direction) -1))
      return;
    TransformComponent xform = this.Transform((EntityUid) ent);
    this._transform.Unanchor(ent.Owner, xform);
  }

  public bool CanConstruct(EntityUid? user) => !this.HasComp<DisableConstructionComponent>(user);

  public bool CanBuildAt(
    EntityCoordinates coordinates,
    string? prototypeName,
    out string? popup,
    bool anchoring = false,
    Direction direction = -1,
    CollisionGroup? collision = null)
  {
    popup = (string) null;
    EntityUid? grid = this._transform.GetGrid(coordinates);
    if (!grid.HasValue)
      return true;
    EntityUid valueOrDefault1 = grid.GetValueOrDefault();
    TileRef? tile;
    if (!this._turf.TryGetTileRef(coordinates, out tile))
      return false;
    if (prototypeName == null)
      prototypeName = this.Loc.GetString("rmc-construction-name");
    if (this.HasComp<DropshipComponent>(valueOrDefault1))
    {
      popup = this.Loc.GetString("rmc-construction-not-proper-surface", ("construction", (object) prototypeName));
      return false;
    }
    MapGridComponent comp;
    if (!this.TryComp<MapGridComponent>(valueOrDefault1, out comp))
      return true;
    Vector2i indices = this._map.TileIndicesFor(valueOrDefault1, comp, coordinates);
    ITileDefinition tileDef;
    if (!this._map.TryGetTileDef(comp, indices, out tileDef))
      return true;
    bool flag = tileDef is ContentTileDefinition contentTileDefinition1 && contentTileDefinition1.BlockConstruction;
    if (anchoring)
      flag = tileDef is ContentTileDefinition contentTileDefinition2 && contentTileDefinition2.BlockAnchoring;
    if (flag || this._rmcMap.HasAnchoredEntityEnumerator<LadderComponent>(coordinates, facing: (DirectionFlag) 0))
    {
      popup = this.Loc.GetString("rmc-construction-not-proper-surface", ("construction", (object) prototypeName));
      return false;
    }
    if (direction != -1)
    {
      RMCMapSystem rmcMap = this._rmcMap;
      EntityCoordinates coords = coordinates;
      DirectionFlag directionFlag = DirectionExtensions.AsFlag(direction);
      Direction? offset = new Direction?();
      DirectionFlag facing = directionFlag;
      if (rmcMap.HasAnchoredEntityEnumerator<BarricadeComponent>(coords, offset, facing))
      {
        popup = this.Loc.GetString("rmc-construction-not-barricade-clear");
        return false;
      }
    }
    if (collision.HasValue)
    {
      CollisionGroup valueOrDefault2 = collision.GetValueOrDefault();
      if (this._turf.IsTileBlocked(tile.Value, valueOrDefault2))
      {
        popup = this.Loc.GetString("rmc-construction-not-proper-surface", ("construction", (object) prototypeName));
        return false;
      }
    }
    return true;
  }
}
