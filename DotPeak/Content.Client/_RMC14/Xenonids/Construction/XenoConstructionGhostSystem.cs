// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Construction.XenoConstructionGhostSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.IconSmoothing;
using Content.Client.UserInterface.Systems.Actions;
using Content.Shared._RMC14.Sentry;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Construction.Events;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Construction.ResinWhisper;
using Content.Shared._RMC14.Xenonids.Construction.Tunnel;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Eye;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Atmos;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Tag;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Linq;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Construction;

public sealed class XenoConstructionGhostSystem : EntitySystem
{
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IUserInterfaceManager _uiManager;
  [Dependency]
  private ITileDefinitionManager _tile;
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private SpriteSystem _sprite;
  private SharedTransformSystem _transform;
  private SharedMapSystem _mapSystem;
  private SharedXenoConstructionSystem _xenoConstruction;
  private SharedXenoWeedsSystem _xenoWeeds;
  private SharedXenoHiveSystem _hive;
  private TurfSystem _turf;
  private TagSystem _tags;
  private XenoNestSystem _xenoNest;
  private QueenEyeSystem _queenEye;
  private ExamineSystemShared _examineSystem;
  private SharedInteractionSystem _interaction;
  private EntityQuery<BlockXenoConstructionComponent> _blockXenoConstructionQuery;
  private EntityQuery<XenoConstructionSupportComponent> _constructionSupportQuery;
  private EntityQuery<HiveConstructionNodeComponent> _hiveConstructionNodeQuery;
  private EntityQuery<SentryComponent> _sentryQuery;
  private EntityQuery<XenoConstructComponent> _xenoConstructQuery;
  private EntityQuery<XenoEggComponent> _xenoEggQuery;
  private EntityQuery<XenoTunnelComponent> _xenoTunnelQuery;
  private EntityUid? _currentGhost;
  private string? _currentGhostStructure;
  private EntityCoordinates _lastPosition = EntityCoordinates.Invalid;
  private static readonly ProtoId<TagPrototype> AirlockTag = ProtoId<TagPrototype>.op_Implicit("Airlock");
  private static readonly ProtoId<TagPrototype> StructureTag = ProtoId<TagPrototype>.op_Implicit("Structure");

  public virtual void Initialize()
  {
    base.Initialize();
    this.UpdatesOutsidePrediction = true;
    this._transform = this.EntityManager.System<SharedTransformSystem>();
    this._mapSystem = this.EntityManager.System<SharedMapSystem>();
    this._xenoConstruction = this.EntityManager.System<SharedXenoConstructionSystem>();
    this._xenoWeeds = this.EntityManager.System<SharedXenoWeedsSystem>();
    this._hive = this.EntityManager.System<SharedXenoHiveSystem>();
    this._turf = this.EntityManager.System<TurfSystem>();
    this._tags = this.EntityManager.System<TagSystem>();
    this._xenoNest = this.EntityManager.System<XenoNestSystem>();
    this._queenEye = this.EntityManager.System<QueenEyeSystem>();
    this._examineSystem = this.EntityManager.System<ExamineSystemShared>();
    this._interaction = this.EntityManager.System<SharedInteractionSystem>();
    this._blockXenoConstructionQuery = this.GetEntityQuery<BlockXenoConstructionComponent>();
    this._constructionSupportQuery = this.GetEntityQuery<XenoConstructionSupportComponent>();
    this._hiveConstructionNodeQuery = this.GetEntityQuery<HiveConstructionNodeComponent>();
    this._sentryQuery = this.GetEntityQuery<SentryComponent>();
    this._xenoConstructQuery = this.GetEntityQuery<XenoConstructComponent>();
    this._xenoEggQuery = this.GetEntityQuery<XenoEggComponent>();
    this._xenoTunnelQuery = this.GetEntityQuery<XenoTunnelComponent>();
    // ISSUE: method pointer
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(EngineKeyFunctions.Use, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleUse)), true, true)).Bind(EngineKeyFunctions.UseSecondary, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleRightClick)), true, true)).Register<XenoConstructionGhostSystem>();
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<XenoConstructionGhostSystem>();
  }

  private bool HandleUse(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (args.State != 1)
      return false;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    XenoConstructionComponent constructionComponent;
    if (!localEntity.HasValue || !this.TryComp<XenoConstructionComponent>(localEntity.Value, ref constructionComponent) || !constructionComponent.OrderConstructionTargeting || !constructionComponent.OrderConstructionChoice.HasValue)
      return false;
    EntityCoordinates grid = this.SnapToGrid(this._inputManager.MouseScreenPosition);
    if (!((EntityCoordinates) ref grid).IsValid((IEntityManager) this.EntityManager) || !this.IsValidConstructionLocation(localEntity.Value, grid))
      return false;
    this.RaiseNetworkEvent((EntityEventArgs) new XenoOrderConstructionClickEvent(this.GetNetCoordinates(grid, (MetaDataComponent) null), constructionComponent.OrderConstructionChoice.Value));
    return true;
  }

  private bool HandleRightClick(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (args.State != 1)
      return false;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    XenoConstructionComponent constructionComponent;
    if (!localEntity.HasValue || !this.TryComp<XenoConstructionComponent>(localEntity.Value, ref constructionComponent) || !constructionComponent.OrderConstructionTargeting)
      return false;
    this.ClearCurrentGhost();
    this.RaiseNetworkEvent((EntityEventArgs) new XenoOrderConstructionCancelEvent());
    return true;
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue)
    {
      this.ClearCurrentGhost();
    }
    else
    {
      (string buildChoice, bool isActive) constructionState = this.GetConstructionState(localEntity.Value);
      string buildChoice = constructionState.buildChoice;
      int num = constructionState.isActive ? 1 : 0;
      bool flag = this.IsBuilding(localEntity.Value);
      if ((num == 0 || string.IsNullOrEmpty(buildChoice) ? 0 : (!flag ? 1 : 0)) != 0)
      {
        string actualBuildPrototype = this.GetActualBuildPrototype(localEntity.Value, buildChoice);
        if (!this._currentGhost.HasValue || this._currentGhostStructure != buildChoice || this.GetActualBuildPrototype(localEntity.Value, this._currentGhostStructure ?? "") != actualBuildPrototype)
        {
          this.ClearCurrentGhost();
          this.CreateGhost(localEntity.Value, buildChoice);
        }
        this.UpdateGhostPosition();
      }
      else
        this.ClearCurrentGhost();
    }
  }

  private (string? buildChoice, bool isActive) GetConstructionState(EntityUid player)
  {
    XenoConstructionComponent constructionComponent;
    if (this.TryComp<XenoConstructionComponent>(player, ref constructionComponent) && constructionComponent.OrderConstructionTargeting && constructionComponent.OrderConstructionChoice.HasValue)
    {
      EntProtoId entProtoId = constructionComponent.OrderConstructionChoice.Value;
      return (((EntProtoId) ref entProtoId).Id, true);
    }
    EntityUid? selectingTargetFor = this._uiManager.GetUIController<ActionUIController>().SelectingTargetFor;
    if (!selectingTargetFor.HasValue)
      return ((string) null, false);
    XenoConstructionActionComponent constructionActionComponent;
    if (!this.TryComp<XenoConstructionActionComponent>(selectingTargetFor.GetValueOrDefault(), ref constructionActionComponent) || constructionComponent == null)
      return ((string) null, false);
    ref EntProtoId? local = ref constructionComponent.BuildChoice;
    string str;
    if (!local.HasValue)
    {
      str = (string) null;
    }
    else
    {
      EntProtoId valueOrDefault = local.GetValueOrDefault();
      str = ((EntProtoId) ref valueOrDefault).Id;
    }
    return (str, true);
  }

  private bool IsBuilding(EntityUid player)
  {
    DoAfterComponent doAfterComponent;
    return this.TryComp<DoAfterComponent>(player, ref doAfterComponent) && doAfterComponent.DoAfters.Values.Any<Content.Shared.DoAfter.DoAfter>((Func<Content.Shared.DoAfter.DoAfter, bool>) (activeDoAfter =>
    {
      bool flag;
      switch (activeDoAfter.Args.Event)
      {
        case XenoSecreteStructureDoAfterEvent _:
        case XenoOrderConstructionDoAfterEvent _:
          flag = true;
          break;
        default:
          flag = false;
          break;
      }
      return flag;
    }));
  }

  private void CreateGhost(EntityUid player, string structurePrototype)
  {
    EntityUid ghost = this.Spawn("XenoConstructionGhost", this.Comp<TransformComponent>(player).Coordinates);
    string actualBuildPrototype = this.GetActualBuildPrototype(player, structurePrototype);
    this.ConfigureGhostSprite(ghost, actualBuildPrototype);
    this._currentGhost = new EntityUid?(ghost);
    this._currentGhostStructure = structurePrototype;
    this._lastPosition = EntityCoordinates.Invalid;
  }

  private string GetActualBuildPrototype(EntityUid player, string originalPrototype)
  {
    if (this.HasComp<QueenBuildingBoostComponent>(player))
    {
      string queenVariant = this.GetQueenVariant(originalPrototype);
      if (this._prototypeManager.HasIndex(EntProtoId.op_Implicit(queenVariant)))
        return queenVariant;
    }
    return originalPrototype;
  }

  private string GetQueenVariant(string originalId)
  {
    string queenVariant;
    switch (originalId)
    {
      case "WallXenoResin":
        queenVariant = "WallXenoResinQueen";
        break;
      case "WallXenoMembrane":
        queenVariant = "WallXenoMembraneQueen";
        break;
      case "DoorXenoResin":
        queenVariant = "DoorXenoResinQueen";
        break;
      default:
        queenVariant = originalId;
        break;
    }
    return queenVariant;
  }

  private void ConfigureGhostSprite(EntityUid ghost, string structurePrototype)
  {
    SpriteComponent sprite;
    if (!this.TryComp<SpriteComponent>(ghost, ref sprite))
      return;
    this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((ghost, sprite)), new Color((byte) 48 /*0x30*/, byte.MaxValue, (byte) 48 /*0x30*/, (byte) 128 /*0x80*/));
    this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((ghost, sprite)), 9);
    this._sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((ghost, sprite)), true);
    EntityPrototype prototype;
    SpriteComponent spriteComponent;
    if (!this._prototypeManager.TryIndex<EntityPrototype>(structurePrototype, ref prototype) || this.TryConfigureIconSmoothSprite(ghost, sprite, prototype) || !prototype.TryGetComponent<SpriteComponent>(ref spriteComponent, this.EntityManager.ComponentFactory))
      return;
    this._sprite.CopySprite(Entity<SpriteComponent>.op_Implicit((EntityUid.Invalid, spriteComponent)), Entity<SpriteComponent>.op_Implicit((ghost, sprite)));
    this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((ghost, sprite)), new Color((byte) 48 /*0x30*/, byte.MaxValue, (byte) 48 /*0x30*/, (byte) 128 /*0x80*/));
    this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((ghost, sprite)), 9);
    for (int index = 0; index < sprite.AllLayers.Count<ISpriteLayer>(); ++index)
    {
      sprite.LayerSetShader(index, "unshaded");
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ghost, sprite)), index, true);
    }
  }

  private bool TryConfigureIconSmoothSprite(
    EntityUid ghost,
    SpriteComponent sprite,
    EntityPrototype prototype)
  {
    IconSmoothComponent iconSmoothComponent;
    SpriteComponent spriteComponent;
    if (prototype.TryGetComponent<IconSmoothComponent>(ref iconSmoothComponent, this.EntityManager.ComponentFactory) && prototype.TryGetComponent<SpriteComponent>(ref spriteComponent, this.EntityManager.ComponentFactory))
    {
      if (!string.IsNullOrEmpty(iconSmoothComponent.StateBase))
      {
        try
        {
          if (!this._sprite.LayerExists(Entity<SpriteComponent>.op_Implicit((ghost, sprite)), 0))
            this._sprite.AddBlankLayer(Entity<SpriteComponent>.op_Implicit((ghost, sprite)), new int?(0));
          this._sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((ghost, sprite)), 0, spriteComponent.BaseRSI, new RSI.StateId?());
          RSI baseRsi = spriteComponent.BaseRSI;
          RSI.State state;
          if ((baseRsi != null ? (baseRsi.TryGetState(RSI.StateId.op_Implicit(iconSmoothComponent.StateBase), ref state) ? 1 : 0) : 0) == 0)
            return false;
          this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ghost, sprite)), 0, RSI.StateId.op_Implicit(iconSmoothComponent.StateBase));
          sprite.LayerSetShader(0, "unshaded");
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ghost, sprite)), 0, true);
          this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((ghost, sprite)), new Color((byte) 48 /*0x30*/, byte.MaxValue, (byte) 48 /*0x30*/, (byte) 128 /*0x80*/));
          return true;
        }
        catch
        {
          return false;
        }
      }
    }
    return false;
  }

  private void UpdateGhostPosition()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue || !this._currentGhost.HasValue || !this.Exists(this._currentGhost.Value))
      return;
    EntityCoordinates grid = this.SnapToGrid(this._inputManager.MouseScreenPosition);
    if (!((EntityCoordinates) ref grid).IsValid((IEntityManager) this.EntityManager))
      return;
    if (!((EntityCoordinates) ref grid).Equals(this._lastPosition))
    {
      this._transform.SetCoordinates(this._currentGhost.Value, this.Comp<TransformComponent>(this._currentGhost.Value), grid, new Angle?(), true, (TransformComponent) null, (TransformComponent) null);
      this._lastPosition = grid;
    }
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(this._currentGhost.Value, ref spriteComponent))
      return;
    this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((this._currentGhost.Value, spriteComponent)), this.IsValidConstructionLocation(localEntity.Value, grid) ? new Color((byte) 48 /*0x30*/, byte.MaxValue, (byte) 48 /*0x30*/, (byte) 128 /*0x80*/) : new Color(byte.MaxValue, (byte) 48 /*0x30*/, (byte) 48 /*0x30*/, (byte) 128 /*0x80*/));
  }

  private EntityCoordinates SnapToGrid(ScreenCoordinates screenCoords)
  {
    MapCoordinates map = this._eyeManager.PixelToMap(screenCoords.Position);
    if (MapId.op_Equality(map.MapId, MapId.Nullspace))
    {
      EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
      return !localEntity.HasValue ? EntityCoordinates.Invalid : this._transform.GetMoverCoordinates(localEntity.Value);
    }
    EntityUid entityUid;
    MapGridComponent mapGridComponent;
    if (!this._mapManager.TryFindGridAt(map, ref entityUid, ref mapGridComponent))
      return this._transform.ToCoordinates(map);
    EntityCoordinates coordinates = this._transform.ToCoordinates(Entity<TransformComponent>.op_Implicit(entityUid), map);
    Vector2i tile = this._mapSystem.CoordinatesToTile(entityUid, mapGridComponent, coordinates);
    return this._mapSystem.GridTileToLocal(entityUid, mapGridComponent, tile);
  }

  private bool IsValidConstructionLocation(EntityUid player, EntityCoordinates coords)
  {
    XenoConstructionComponent constructionComponent;
    if (!this.TryComp<XenoConstructionComponent>(player, ref constructionComponent))
      return false;
    try
    {
      if (constructionComponent.OrderConstructionTargeting && constructionComponent.OrderConstructionChoice.HasValue)
        return this.CanOrderConstruction(Entity<XenoConstructionComponent>.op_Implicit((player, constructionComponent)), coords, constructionComponent.OrderConstructionChoice);
      return constructionComponent.BuildChoice.HasValue && this.CanSecreteOnTile(Entity<XenoConstructionComponent>.op_Implicit((player, constructionComponent)), constructionComponent.BuildChoice, coords, true, true);
    }
    catch
    {
      return false;
    }
  }

  private bool CanSecreteOnTile(
    Entity<XenoConstructionComponent> xeno,
    EntProtoId? buildChoice,
    EntityCoordinates target,
    bool checkStructureSelected,
    bool checkWeeds)
  {
    if (checkStructureSelected && !buildChoice.HasValue)
      return false;
    EntityUid? grid = this._transform.GetGrid(target);
    if (grid.HasValue)
    {
      EntityUid valueOrDefault1 = grid.GetValueOrDefault();
      MapGridComponent mapGridComponent;
      if (this.TryComp<MapGridComponent>(valueOrDefault1, ref mapGridComponent))
      {
        target = target.SnapToGrid((IEntityManager) this.EntityManager, this._mapManager);
        if (checkWeeds && !this._queenEye.IsInQueenEye(Entity<QueenEyeActionComponent>.op_Implicit(xeno.Owner)) && !this._xenoWeeds.IsOnWeeds(Entity<MapGridComponent>.op_Implicit((valueOrDefault1, mapGridComponent)), target))
          return false;
        if (!this._queenEye.IsInQueenEye(Entity<QueenEyeActionComponent>.op_Implicit(xeno.Owner)))
        {
          EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(xeno.Owner);
          (float range, bool isRemote) = this.GetEffectiveBuildRange(xeno, target);
          if ((double) range > 0.0 && !this._transform.InRange(moverCoordinates, target, range) || this._transform.InRange(moverCoordinates, target, this._xenoConstruction.GetStructureMinRange(buildChoice).Float()) || isRemote && !this.CanDoRemoteConstruction(xeno, target))
            return false;
        }
        if (!this._queenEye.IsInQueenEye(Entity<QueenEyeActionComponent>.op_Implicit(xeno.Owner)) && !this.TileSolidAndNotBlocked(target))
          return false;
        Vector2i tile = this._mapSystem.CoordinatesToTile(valueOrDefault1, mapGridComponent, target);
        AnchoredEntitiesEnumerator entitiesEnumerator = this._mapSystem.GetAnchoredEntitiesEnumerator(valueOrDefault1, mapGridComponent, tile);
        EntityUid? nullable;
        while (((AnchoredEntitiesEnumerator) ref entitiesEnumerator).MoveNext(ref nullable))
        {
          if (this._xenoConstructQuery.HasComp(nullable) || this._xenoEggQuery.HasComp(nullable) || this._xenoTunnelQuery.HasComp(nullable) || this._sentryQuery.HasComp(nullable) || this._blockXenoConstructionQuery.HasComp(nullable))
            return false;
        }
        if (checkStructureSelected && buildChoice.HasValue && !this.HasComp<QueenBuildingBoostComponent>(xeno.Owner))
        {
          FixedPoint2? structurePlasmaCost = this._xenoConstruction.GetStructurePlasmaCost(buildChoice.Value);
          if (structurePlasmaCost.HasValue)
          {
            FixedPoint2 valueOrDefault2 = structurePlasmaCost.GetValueOrDefault();
            XenoPlasmaComponent xenoPlasmaComponent;
            if (!this.TryComp<XenoPlasmaComponent>(xeno.Owner, ref xenoPlasmaComponent) || xenoPlasmaComponent.Plasma < valueOrDefault2)
              return false;
          }
        }
        EntityPrototype entityPrototype;
        XenoConstructionRequiresSupportComponent supportComponent;
        return !checkStructureSelected || !buildChoice.HasValue || !this._prototypeManager.TryIndex(buildChoice.GetValueOrDefault(), ref entityPrototype) || !entityPrototype.TryGetComponent<XenoConstructionRequiresSupportComponent>(ref supportComponent, this._compFactory) || this.IsSupported(Entity<MapGridComponent>.op_Implicit((valueOrDefault1, mapGridComponent)), target);
      }
    }
    return false;
  }

  private bool CanOrderConstruction(
    Entity<XenoConstructionComponent> xeno,
    EntityCoordinates target,
    EntProtoId? choice)
  {
    // ISSUE: unable to decompile the method.
  }

  private bool TileSolidAndNotBlocked(EntityCoordinates target)
  {
    TileRef? tileRef = this._turf.GetTileRef(target);
    if (tileRef.HasValue)
    {
      TileRef valueOrDefault = tileRef.GetValueOrDefault();
      if (!this._turf.IsSpace(valueOrDefault) && this._turf.GetContentTileDefinition(valueOrDefault).Sturdy && !this._turf.IsTileBlocked(valueOrDefault, CollisionGroup.Impassable))
        return !this._xenoNest.HasAdjacentNestFacing(target);
    }
    return false;
  }

  private bool IsSupported(Entity<MapGridComponent> grid, EntityCoordinates coordinates)
  {
    Vector2i tile = this._mapSystem.TileIndicesFor(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), coordinates);
    return this.IsSupported(grid, tile);
  }

  private bool IsSupported(Entity<MapGridComponent> grid, Vector2i tile)
  {
    for (int index = 0; index < 4; ++index)
    {
      AtmosDirection dir = (AtmosDirection) (1 << index);
      Vector2i vector2i = tile.Offset(dir);
      AnchoredEntitiesEnumerator entitiesEnumerator = this._mapSystem.GetAnchoredEntitiesEnumerator(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), vector2i);
      EntityUid? nullable;
      while (((AnchoredEntitiesEnumerator) ref entitiesEnumerator).MoveNext(ref nullable))
      {
        if (!this.TerminatingOrDeleted(nullable.Value, (MetaDataComponent) null) && !this.EntityManager.IsQueuedForDeletion(nullable.Value) && this._constructionSupportQuery.HasComp(nullable))
          return true;
      }
    }
    return false;
  }

  private bool CanPlaceSpaceRequiringStructure(
    MapCoordinates mapCoords,
    Entity<MapGridComponent> map)
  {
    TileRef tileRef = this._mapSystem.GetTileRef(map, mapCoords);
    for (int index1 = ((TileRef) ref tileRef).X - 1; index1 <= ((TileRef) ref tileRef).X + 1; ++index1)
    {
      for (int index2 = ((TileRef) ref tileRef).Y - 1; index2 <= ((TileRef) ref tileRef).Y + 1; ++index2)
      {
        if (index1 != 0 || index2 != 0)
        {
          Vector2i indices;
          // ISSUE: explicit constructor call
          ((Vector2i) ref indices).\u002Ector(index1, index2);
          if (this._turf.IsTileBlocked(Entity<MapGridComponent>.op_Implicit(map), indices, CollisionGroup.MobMask, map.Comp))
            return false;
        }
      }
    }
    return true;
  }

  private bool CanPlaceLimitedHiveStructure(
    EntityUid hiveMember,
    HiveConstructionLimitedComponent comp)
  {
    EntProtoId id = comp.Id;
    Entity<HiveComponent>? hive = this._hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(hiveMember));
    int limit;
    if (!hive.HasValue || !this._hive.TryGetStructureLimit(hive.GetValueOrDefault(), id, out limit))
      return false;
    int num = 0;
    EntityQueryEnumerator<HiveConstructionLimitedComponent, HiveMemberComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HiveConstructionLimitedComponent, HiveMemberComponent>();
    HiveConstructionLimitedComponent limitedComponent;
    HiveMemberComponent hiveMemberComponent;
    while (entityQueryEnumerator.MoveNext(ref limitedComponent, ref hiveMemberComponent))
    {
      if (EntProtoId.op_Equality(limitedComponent.Id, id))
        ++num;
    }
    return limit > num;
  }

  private (float range, bool isRemote) GetEffectiveBuildRange(
    Entity<XenoConstructionComponent> xeno,
    EntityCoordinates target)
  {
    FixedPoint2 buildRange = xeno.Comp.BuildRange;
    if (this._queenEye.IsInQueenEye(Entity<QueenEyeActionComponent>.op_Implicit(xeno.Owner)))
      return (float.MaxValue, false);
    ResinWhispererComponent whispererComponent;
    if (!this.TryComp<ResinWhispererComponent>(xeno.Owner, ref whispererComponent))
      return (buildRange.Float(), false);
    ref FixedPoint2? local = ref whispererComponent.MaxConstructDistance;
    float range = local.HasValue ? local.GetValueOrDefault().Float() : buildRange.Float();
    return this._interaction.InRangeUnobstructed(xeno.Owner, target, range) ? (range, false) : (whispererComponent.MaxRemoteConstructDistance, true);
  }

  private bool CanDoRemoteConstruction(
    Entity<XenoConstructionComponent> xeno,
    EntityCoordinates target)
  {
    ResinWhispererComponent whispererComponent;
    return this.TryComp<ResinWhispererComponent>(xeno.Owner, ref whispererComponent) && (this._queenEye.IsInQueenEye(Entity<QueenEyeActionComponent>.op_Implicit(xeno.Owner)) || this._xenoWeeds.IsOnFriendlyWeeds(Entity<TransformComponent>.op_Implicit(xeno.Owner)) && this.TileIsVisible(xeno.Owner, target, whispererComponent.MaxRemoteConstructDistance));
  }

  private bool TileIsVisible(
    EntityUid user,
    EntityCoordinates targetCoordinates,
    float maxDistance)
  {
    MapCoordinates other = this._transform.ToMapCoordinates(targetCoordinates, true);
    for (int index = 0; index < 9; ++index)
    {
      switch (index - 1)
      {
        case 0:
        case 6:
        case 7:
          other = ((MapCoordinates) ref other).Offset(0.499f, 0.0f);
          break;
        case 1:
          other = ((MapCoordinates) ref other).Offset(0.0f, -0.499f);
          break;
        case 2:
        case 3:
          other = ((MapCoordinates) ref other).Offset(-0.499f, 0.0f);
          break;
        case 4:
        case 5:
          other = ((MapCoordinates) ref other).Offset(0.0f, 0.499f);
          break;
      }
      if (this._examineSystem.InRangeUnOccluded(user, other, maxDistance))
        return true;
    }
    return false;
  }

  private void ClearCurrentGhost()
  {
    if (this._currentGhost.HasValue && this.Exists(this._currentGhost.Value))
      this.QueueDel(new EntityUid?(this._currentGhost.Value));
    this._currentGhost = new EntityUid?();
    this._currentGhostStructure = (string) null;
    this._lastPosition = EntityCoordinates.Invalid;
  }
}
