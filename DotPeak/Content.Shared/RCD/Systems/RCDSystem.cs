// Decompiled with JetBrains decompiler
// Type: Content.Shared.RCD.Systems.RCDSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Construction;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.RCD.Components;
using Content.Shared.Tag;
using Content.Shared.Tiles;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Prototypes;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.RCD.Systems;

public sealed class RCDSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private ITileDefinitionManager _tileDefMan;
  [Dependency]
  private FloorTileSystem _floors;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedChargesSystem _sharedCharges;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private TurfSystem _turf;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private IPrototypeManager _protoManager;
  [Dependency]
  private SharedMapSystem _mapSystem;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private TagSystem _tags;
  private readonly int _instantConstructionDelay;
  private readonly EntProtoId _instantConstructionFx = (EntProtoId) "EffectRCDConstruct0";
  private readonly ProtoId<RCDPrototype> _deconstructTileProto = (ProtoId<RCDPrototype>) "DeconstructTile";
  private readonly ProtoId<RCDPrototype> _deconstructLatticeProto = (ProtoId<RCDPrototype>) "DeconstructLattice";
  private static readonly ProtoId<TagPrototype> CatwalkTag = (ProtoId<TagPrototype>) "Catwalk";
  private HashSet<EntityUid> _intersectingEntities = new HashSet<EntityUid>();

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RCDComponent, MapInitEvent>(new ComponentEventHandler<RCDComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<RCDComponent, ExaminedEvent>(new ComponentEventHandler<RCDComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<RCDComponent, AfterInteractEvent>(new ComponentEventHandler<RCDComponent, AfterInteractEvent>(this.OnAfterInteract));
    this.SubscribeLocalEvent<RCDComponent, RCDDoAfterEvent>(new ComponentEventHandler<RCDComponent, RCDDoAfterEvent>(this.OnDoAfter));
    this.SubscribeLocalEvent<RCDComponent, DoAfterAttemptEvent<RCDDoAfterEvent>>(new ComponentEventHandler<RCDComponent, DoAfterAttemptEvent<RCDDoAfterEvent>>(this.OnDoAfterAttempt));
    this.SubscribeLocalEvent<RCDComponent, RCDSystemMessage>(new ComponentEventHandler<RCDComponent, RCDSystemMessage>(this.OnRCDSystemMessage));
    this.SubscribeNetworkEvent<RCDConstructionGhostRotationEvent>(new EntitySessionEventHandler<RCDConstructionGhostRotationEvent>(this.OnRCDconstructionGhostRotationEvent));
  }

  private void OnMapInit(EntityUid uid, RCDComponent component, MapInitEvent args)
  {
    if (component.AvailablePrototypes.Count > 0)
    {
      component.ProtoId = component.AvailablePrototypes.ElementAt<ProtoId<RCDPrototype>>(0);
      this.Dirty(uid, (IComponent) component);
    }
    else
      this.QueueDel(new EntityUid?(uid));
  }

  private void OnRCDSystemMessage(EntityUid uid, RCDComponent component, RCDSystemMessage args)
  {
    if (!component.AvailablePrototypes.Contains(args.ProtoId) || !this._protoManager.HasIndex<RCDPrototype>(args.ProtoId))
      return;
    component.ProtoId = args.ProtoId;
    this.Dirty(uid, (IComponent) component);
  }

  private void OnExamine(EntityUid uid, RCDComponent component, ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    RCDPrototype rcdPrototype = this._protoManager.Index<RCDPrototype>(component.ProtoId);
    string markup = this.Loc.GetString("rcd-component-examine-mode-details", ("mode", (object) this.Loc.GetString(rcdPrototype.SetName)));
    if (rcdPrototype.Mode == RcdMode.ConstructTile || rcdPrototype.Mode == RcdMode.ConstructObject)
    {
      string name = this.Loc.GetString(rcdPrototype.SetName);
      EntityPrototype prototype;
      if (rcdPrototype.Prototype != null && this._protoManager.TryIndex((EntProtoId) rcdPrototype.Prototype, out prototype))
        name = prototype.Name;
      markup = this.Loc.GetString("rcd-component-examine-build-details", ("name", (object) name));
    }
    args.PushMarkup(markup);
  }

  private void OnAfterInteract(EntityUid uid, RCDComponent component, AfterInteractEvent args)
  {
    if (args.Handled || !args.CanReach)
      return;
    EntityUid user = args.User;
    EntityCoordinates clickLocation = args.ClickLocation;
    RCDPrototype rcdPrototype = this._protoManager.Index<RCDPrototype>(component.ProtoId);
    if (!clickLocation.IsValid((IEntityManager) this.EntityManager))
      return;
    EntityUid? grid = this._transform.GetGrid(clickLocation);
    MapGridComponent comp1;
    if (!this.TryComp<MapGridComponent>(grid, out comp1))
    {
      this._popup.PopupClient(this.Loc.GetString("rcd-component-no-valid-grid"), uid, new EntityUid?(user));
    }
    else
    {
      TileRef tileRef = this._mapSystem.GetTileRef(grid.Value, comp1, clickLocation);
      Vector2i position = this._mapSystem.TileIndicesFor(grid.Value, comp1, clickLocation);
      if (!this.IsRCDOperationStillValid(uid, component, grid.Value, comp1, tileRef, position, args.Target, args.User) || !this._net.IsServer)
        return;
      int cost = rcdPrototype.Cost;
      float seconds = rcdPrototype.Delay;
      EntProtoId? nullable1 = rcdPrototype.Effect;
      switch (rcdPrototype.Mode)
      {
        case RcdMode.Deconstruct:
          if (args.Target.HasValue)
          {
            RCDDeconstructableComponent comp2;
            if (this.TryComp<RCDDeconstructableComponent>(args.Target, out comp2))
            {
              cost = comp2.Cost;
              seconds = comp2.Delay;
              nullable1 = comp2.Effect;
              break;
            }
            break;
          }
          RCDPrototype prototype;
          if (this._protoManager.TryIndex<RCDPrototype>(!this._turf.IsSpace(this._mapSystem.GetTileRef(grid.Value, comp1, clickLocation)) ? this._deconstructTileProto : this._deconstructLatticeProto, out prototype))
          {
            cost = prototype.Cost;
            seconds = prototype.Delay;
            nullable1 = prototype.Effect;
            break;
          }
          break;
        case RcdMode.ConstructTile:
          if (!this._mapSystem.GetTileRef(grid.Value, comp1, clickLocation).Tile.IsEmpty)
          {
            seconds = (float) this._instantConstructionDelay;
            nullable1 = new EntProtoId?(this._instantConstructionFx);
            break;
          }
          break;
      }
      EntProtoId? nullable2 = nullable1;
      EntityUid uid1 = this.Spawn(nullable2.HasValue ? (string) nullable2.GetValueOrDefault() : (string) null, clickLocation);
      RCDDoAfterEvent @event = new RCDDoAfterEvent(this.GetNetCoordinates(clickLocation), component.ConstructionDirection, component.ProtoId, cost, new NetEntity?(this.GetNetEntity(uid1)));
      DoAfterArgs args1 = new DoAfterArgs((IEntityManager) this.EntityManager, user, seconds, (DoAfterEvent) @event, new EntityUid?(uid), args.Target, new EntityUid?(uid))
      {
        BreakOnDamage = true,
        BreakOnHandChange = true,
        BreakOnMove = true,
        AttemptFrequency = AttemptFrequency.EveryTick,
        CancelDuplicate = false,
        BlockDuplicate = false
      };
      args.Handled = true;
      if (this._doAfter.TryStartDoAfter(args1))
        return;
      this.QueueDel(new EntityUid?(uid1));
    }
  }

  private void OnDoAfterAttempt(
    EntityUid uid,
    RCDComponent component,
    DoAfterAttemptEvent<RCDDoAfterEvent> args)
  {
    if (args.Event?.DoAfter?.Args == null)
      return;
    if (component.ProtoId != args.Event.StartingProtoId)
    {
      args.Cancel();
    }
    else
    {
      EntityCoordinates coordinates = this.GetCoordinates(args.Event.Location);
      EntityUid? grid = this._transform.GetGrid(coordinates);
      MapGridComponent comp;
      if (!this.TryComp<MapGridComponent>(grid, out comp))
      {
        args.Cancel();
      }
      else
      {
        TileRef tileRef = this._mapSystem.GetTileRef(grid.Value, comp, coordinates);
        Vector2i position = this._mapSystem.TileIndicesFor(grid.Value, comp, coordinates);
        if (this.IsRCDOperationStillValid(uid, component, grid.Value, comp, tileRef, position, args.Event.Target, args.Event.User))
          return;
        args.Cancel();
      }
    }
  }

  private void OnDoAfter(EntityUid uid, RCDComponent component, RCDDoAfterEvent args)
  {
    if (args.Cancelled)
    {
      if (!this._net.IsServer)
        return;
      this.QueueDel(this.GetEntity(args.Effect));
    }
    else
    {
      if (args.Handled)
        return;
      args.Handled = true;
      EntityCoordinates coordinates = this.GetCoordinates(args.Location);
      EntityUid? grid = this._transform.GetGrid(coordinates);
      MapGridComponent comp;
      if (!this.TryComp<MapGridComponent>(grid, out comp))
        return;
      TileRef tileRef = this._mapSystem.GetTileRef(grid.Value, comp, coordinates);
      Vector2i position = this._mapSystem.TileIndicesFor(grid.Value, comp, coordinates);
      if (!this.IsRCDOperationStillValid(uid, component, grid.Value, comp, tileRef, position, args.Target, args.User))
        return;
      this.FinalizeRCDOperation(uid, component, grid.Value, comp, tileRef, position, args.Direction, args.Target, args.User);
      this._audio.PlayPredicted(component.SuccessSound, uid, new EntityUid?(args.User));
      this._sharedCharges.AddCharges((Entity<LimitedChargesComponent, AutoRechargeComponent>) uid, -args.Cost);
    }
  }

  private void OnRCDconstructionGhostRotationEvent(
    RCDConstructionGhostRotationEvent ev,
    EntitySessionEventArgs session)
  {
    EntityUid entity = this.GetEntity(ev.NetEntity);
    EntityUid? nullable = session.SenderSession.AttachedEntity;
    if (!nullable.HasValue)
      return;
    nullable = this._hands.GetActiveItem((Entity<HandsComponent>) nullable.GetValueOrDefault());
    EntityUid entityUid = entity;
    RCDComponent comp;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0 || !this.TryComp<RCDComponent>(entity, out comp))
      return;
    comp.ConstructionDirection = ev.Direction;
    this.Dirty(entity, (IComponent) comp);
  }

  public bool IsRCDOperationStillValid(
    EntityUid uid,
    RCDComponent component,
    EntityUid gridUid,
    MapGridComponent mapGrid,
    TileRef tile,
    Vector2i position,
    EntityUid? target,
    EntityUid user,
    bool popMsgs = true)
  {
    RCDPrototype rcdPrototype = this._protoManager.Index<RCDPrototype>(component.ProtoId);
    int currentCharges = this._sharedCharges.GetCurrentCharges((Entity<LimitedChargesComponent, AutoRechargeComponent>) uid);
    if (currentCharges == 0)
    {
      if (popMsgs)
        this._popup.PopupClient(this.Loc.GetString("rcd-component-no-ammo-message"), uid, new EntityUid?(user));
      return false;
    }
    if (rcdPrototype.Cost > currentCharges)
    {
      if (popMsgs)
        this._popup.PopupClient(this.Loc.GetString("rcd-component-insufficient-ammo-message"), uid, new EntityUid?(user));
      return false;
    }
    if ((!target.HasValue ? (this._interaction.InRangeUnobstructed(user, this._mapSystem.GridTileToWorld(gridUid, mapGrid, position), popup: popMsgs) ? 1 : 0) : (this._interaction.InRangeUnobstructed((Entity<TransformComponent>) user, (Entity<TransformComponent>) target.Value, popup: popMsgs) ? 1 : 0)) == 0)
      return false;
    switch (rcdPrototype.Mode)
    {
      case RcdMode.Deconstruct:
        return this.IsDeconstructionStillValid(uid, tile, target, user, popMsgs);
      case RcdMode.ConstructTile:
      case RcdMode.ConstructObject:
        return this.IsConstructionLocationValid(uid, component, gridUid, mapGrid, tile, position, user, popMsgs);
      default:
        return false;
    }
  }

  private bool IsConstructionLocationValid(
    EntityUid uid,
    RCDComponent component,
    EntityUid gridUid,
    MapGridComponent mapGrid,
    TileRef tile,
    Vector2i position,
    EntityUid user,
    bool popMsgs = true)
  {
    RCDPrototype rcdPrototype = this._protoManager.Index<RCDPrototype>(component.ProtoId);
    if (rcdPrototype.ConstructionRules.Contains(RcdConstructionRule.MustBuildOnEmptyTile) && !tile.Tile.IsEmpty)
    {
      if (popMsgs)
        this._popup.PopupClient(this.Loc.GetString("rcd-component-must-build-on-empty-tile-message"), uid, new EntityUid?(user));
      return false;
    }
    if (!rcdPrototype.ConstructionRules.Contains(RcdConstructionRule.CanBuildOnEmptyTile) && tile.Tile.IsEmpty)
    {
      if (popMsgs)
        this._popup.PopupClient(this.Loc.GetString("rcd-component-cannot-build-on-empty-tile-message"), uid, new EntityUid?(user));
      return false;
    }
    if (rcdPrototype.ConstructionRules.Contains(RcdConstructionRule.MustBuildOnSubfloor) && !this._turf.GetContentTileDefinition(tile).IsSubFloor)
    {
      if (popMsgs)
        this._popup.PopupClient(this.Loc.GetString("rcd-component-must-build-on-subfloor-message"), uid, new EntityUid?(user));
      return false;
    }
    if (rcdPrototype.Mode == RcdMode.ConstructTile)
    {
      string reason;
      if (!this._floors.CanPlaceTile(gridUid, mapGrid, tile.GridIndices, out reason))
      {
        if (popMsgs)
          this._popup.PopupClient(reason, uid, new EntityUid?(user));
        return false;
      }
      if (!(this._turf.GetContentTileDefinition(tile).ID == rcdPrototype.Prototype))
        return true;
      if (popMsgs)
        this._popup.PopupClient(this.Loc.GetString("rcd-component-cannot-build-identical-tile"), uid, new EntityUid?(user));
      return false;
    }
    bool flag1 = rcdPrototype.ConstructionRules.Contains(RcdConstructionRule.IsWindow);
    bool flag2 = rcdPrototype.ConstructionRules.Contains(RcdConstructionRule.IsCatwalk);
    this._intersectingEntities.Clear();
    this._lookup.GetLocalEntitiesIntersecting(gridUid, position, this._intersectingEntities, -0.05f, LookupFlags.Uncontained);
    foreach (EntityUid intersectingEntity in this._intersectingEntities)
    {
      if (!flag1 || !this.HasComp<SharedCanBuildWindowOnTopComponent>(intersectingEntity))
      {
        if (flag2 && this._tags.HasTag(intersectingEntity, RCDSystem.CatwalkTag))
        {
          if (popMsgs)
            this._popup.PopupClient(this.Loc.GetString("rcd-component-cannot-build-on-occupied-tile-message"), uid, new EntityUid?(user));
          return false;
        }
        FixturesComponent comp;
        if (rcdPrototype.CollisionMask != CollisionGroup.None && this.TryComp<FixturesComponent>(intersectingEntity, out comp))
        {
          foreach (Fixture fixture in comp.Fixtures.Values)
          {
            if (fixture.Hard && fixture.CollisionLayer > 0 && ((CollisionGroup) fixture.CollisionLayer & rcdPrototype.CollisionMask) != CollisionGroup.None && (rcdPrototype.CollisionPolygon == null || this.DoesCustomBoundsIntersectWithFixture(rcdPrototype.CollisionPolygon, component.ConstructionTransform, intersectingEntity, fixture)))
            {
              if (popMsgs)
                this._popup.PopupClient(this.Loc.GetString("rcd-component-cannot-build-on-occupied-tile-message"), uid, new EntityUid?(user));
              return false;
            }
          }
        }
      }
    }
    return true;
  }

  private bool IsDeconstructionStillValid(
    EntityUid uid,
    TileRef tile,
    EntityUid? target,
    EntityUid user,
    bool popMsgs = true)
  {
    if (!target.HasValue)
    {
      if (tile.Tile.IsEmpty)
      {
        if (popMsgs)
          this._popup.PopupClient(this.Loc.GetString("rcd-component-nothing-to-deconstruct-message"), uid, new EntityUid?(user));
        return false;
      }
      if (this._turf.IsTileBlocked(tile, CollisionGroup.MobMask))
      {
        if (popMsgs)
          this._popup.PopupClient(this.Loc.GetString("rcd-component-tile-obstructed-message"), uid, new EntityUid?(user));
        return false;
      }
      if (this._turf.GetContentTileDefinition(tile).Indestructible)
      {
        if (popMsgs)
          this._popup.PopupClient(this.Loc.GetString("rcd-component-tile-indestructible-message"), uid, new EntityUid?(user));
        return false;
      }
    }
    else
    {
      RCDDeconstructableComponent comp;
      if (!this.TryComp<RCDDeconstructableComponent>(target, out comp) || !comp.Deconstructable)
      {
        if (popMsgs)
          this._popup.PopupClient(this.Loc.GetString("rcd-component-deconstruct-target-not-on-whitelist-message"), uid, new EntityUid?(user));
        return false;
      }
    }
    return true;
  }

  private void FinalizeRCDOperation(
    EntityUid uid,
    RCDComponent component,
    EntityUid gridUid,
    MapGridComponent mapGrid,
    TileRef tile,
    Vector2i position,
    Direction direction,
    EntityUid? target,
    EntityUid user)
  {
    if (!this._net.IsServer)
      return;
    RCDPrototype rcdPrototype = this._protoManager.Index<RCDPrototype>(component.ProtoId);
    if (rcdPrototype.Prototype == null)
      return;
    switch (rcdPrototype.Mode)
    {
      case RcdMode.Deconstruct:
        if (!target.HasValue)
        {
          Tile tile1 = this._turf.GetContentTileDefinition(tile).ID != "Lattice" ? new Tile((int) this._tileDefMan["Lattice"].TileId) : Tile.Empty;
          this._mapSystem.SetTile(gridUid, mapGrid, position, tile1);
          ISharedAdminLogManager adminLogger = this._adminLogger;
          LogStringHandler logStringHandler = new LogStringHandler(44, 3);
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), nameof (user), "ToPrettyString(user)");
          logStringHandler.AppendLiteral(" used RCD to set grid: ");
          logStringHandler.AppendFormatted<EntityUid>(gridUid, nameof (gridUid));
          logStringHandler.AppendLiteral(" tile: ");
          logStringHandler.AppendFormatted<Vector2i>(position, nameof (position));
          logStringHandler.AppendLiteral(" open to space");
          ref LogStringHandler local = ref logStringHandler;
          adminLogger.Add(LogType.RCD, LogImpact.High, ref local);
          break;
        }
        ISharedAdminLogManager adminLogger1 = this._adminLogger;
        LogStringHandler logStringHandler1 = new LogStringHandler(20, 2);
        logStringHandler1.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), nameof (user), "ToPrettyString(user)");
        logStringHandler1.AppendLiteral(" used RCD to delete ");
        logStringHandler1.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(target), nameof (target), "ToPrettyString(target)");
        ref LogStringHandler local1 = ref logStringHandler1;
        adminLogger1.Add(LogType.RCD, LogImpact.High, ref local1);
        this.QueueDel(target);
        break;
      case RcdMode.ConstructTile:
        this._mapSystem.SetTile(gridUid, mapGrid, position, new Tile((int) this._tileDefMan[rcdPrototype.Prototype].TileId));
        ISharedAdminLogManager adminLogger2 = this._adminLogger;
        LogStringHandler logStringHandler2 = new LogStringHandler(28, 4);
        logStringHandler2.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), nameof (user), "ToPrettyString(user)");
        logStringHandler2.AppendLiteral(" used RCD to set grid: ");
        logStringHandler2.AppendFormatted<EntityUid>(gridUid, nameof (gridUid));
        logStringHandler2.AppendLiteral(" ");
        logStringHandler2.AppendFormatted<Vector2i>(position, nameof (position));
        logStringHandler2.AppendLiteral(" to ");
        logStringHandler2.AppendFormatted(rcdPrototype.Prototype);
        ref LogStringHandler local2 = ref logStringHandler2;
        adminLogger2.Add(LogType.RCD, LogImpact.High, ref local2);
        break;
      case RcdMode.ConstructObject:
        EntityUid uid1 = this.Spawn(rcdPrototype.Prototype, this._mapSystem.GridTileToLocal(gridUid, mapGrid, position));
        switch (rcdPrototype.Rotation)
        {
          case RcdRotation.Fixed:
            this.Transform(uid1).LocalRotation = Angle.Zero;
            break;
          case RcdRotation.Camera:
            this.Transform(uid1).LocalRotation = this.Transform(uid).LocalRotation;
            break;
          case RcdRotation.User:
            this.Transform(uid1).LocalRotation = DirectionExtensions.ToAngle(direction);
            break;
        }
        ISharedAdminLogManager adminLogger3 = this._adminLogger;
        LogStringHandler logStringHandler3 = new LogStringHandler(32 /*0x20*/, 4);
        logStringHandler3.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), nameof (user), "ToPrettyString(user)");
        logStringHandler3.AppendLiteral(" used RCD to spawn ");
        logStringHandler3.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid1), "ToPrettyString(ent)");
        logStringHandler3.AppendLiteral(" at ");
        logStringHandler3.AppendFormatted<Vector2i>(position, nameof (position));
        logStringHandler3.AppendLiteral(" on grid ");
        logStringHandler3.AppendFormatted<EntityUid>(gridUid, nameof (gridUid));
        ref LogStringHandler local3 = ref logStringHandler3;
        adminLogger3.Add(LogType.RCD, LogImpact.High, ref local3);
        break;
    }
  }

  private bool DoesCustomBoundsIntersectWithFixture(
    PolygonShape boundingPolygon,
    Robust.Shared.Physics.Transform boundingTransform,
    EntityUid fixtureOwner,
    Fixture fixture)
  {
    Robust.Shared.Physics.Transform transform = new Robust.Shared.Physics.Transform(new Vector2(), this.Transform(fixtureOwner).LocalRotation);
    Box2 aabb1 = boundingPolygon.ComputeAABB(boundingTransform, 0);
    ref Box2 local1 = ref aabb1;
    Box2 aabb2 = fixture.Shape.ComputeAABB(transform, 0);
    ref Box2 local2 = ref aabb2;
    return ((Box2) ref local1).Intersects(ref local2);
  }
}
