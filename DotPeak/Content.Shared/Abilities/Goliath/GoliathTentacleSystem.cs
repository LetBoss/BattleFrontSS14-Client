// Decompiled with JetBrains decompiler
// Type: Content.Shared.Abilities.Goliath.GoliathTentacleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Directions;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Abilities.Goliath;

public sealed class GoliathTentacleSystem : EntitySystem
{
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private TurfSystem _turf;
  [Dependency]
  private SharedPopupSystem _popup;

  public virtual void Initialize()
  {
    this.SubscribeLocalEvent<GoliathSummonTentacleAction>(new EntityEventHandler<GoliathSummonTentacleAction>(this.OnSummonAction), (Type[]) null, (Type[]) null);
  }

  private void OnSummonAction(GoliathSummonTentacleAction args)
  {
    if (args.Handled)
      return;
    this._popup.PopupPredicted(this.Loc.GetString("tentacle-ability-use-popup", ("entity", (object) args.Performer)), args.Performer, new EntityUid?(args.Performer), PopupType.SmallCaution);
    this._stun.TryStun(args.Performer, TimeSpan.FromSeconds(0.800000011920929), false);
    EntityCoordinates target = args.Target;
    List<EntityCoordinates> entityCoordinatesList = new List<EntityCoordinates>();
    entityCoordinatesList.Add(target);
    List<Direction> directionList = new List<Direction>();
    directionList.AddRange((IEnumerable<Direction>) args.OffsetDirections);
    for (int index = 0; index < 3; ++index)
    {
      Direction direction = RandomExtensions.PickAndTake<Direction>(this._random, (IList<Direction>) directionList);
      entityCoordinatesList.Add(target.Offset(direction));
    }
    EntityUid? grid = this._transform.GetGrid(target);
    if (!grid.HasValue)
      return;
    EntityUid valueOrDefault = grid.GetValueOrDefault();
    MapGridComponent mapGridComponent;
    if (!this.TryComp<MapGridComponent>(valueOrDefault, ref mapGridComponent))
      return;
    foreach (EntityCoordinates entityCoordinates in entityCoordinatesList)
    {
      TileRef tileRef;
      if (this._map.TryGetTileRef(valueOrDefault, mapGridComponent, entityCoordinates, ref tileRef) && !this._turf.IsSpace(tileRef) && !this._turf.IsTileBlocked(tileRef, CollisionGroup.Impassable) && this._net.IsServer)
        this.Spawn(EntProtoId.op_Implicit(args.EntityId), entityCoordinates);
    }
    args.Handled = true;
  }
}
