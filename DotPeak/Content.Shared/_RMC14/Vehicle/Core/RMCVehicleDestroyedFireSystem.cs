// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.RMCVehicleDestroyedFireSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.OnCollide;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class RMCVehicleDestroyedFireSystem : EntitySystem
{
  [Dependency]
  private SharedRMCFlammableSystem _flammable;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedOnCollideSystem _onCollide;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private VehicleSystem _vehicles;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCVehicleDestroyedFireComponent, RMCVehicleFrameDestroyedEvent>(new EntityEventRefHandler<RMCVehicleDestroyedFireComponent, RMCVehicleFrameDestroyedEvent>(this.OnVehicleFrameDestroyed));
  }

  private void OnVehicleFrameDestroyed(
    Entity<RMCVehicleDestroyedFireComponent> ent,
    ref RMCVehicleFrameDestroyedEvent args)
  {
    if (this._net.IsClient || args.Vehicle != ent.Owner)
      return;
    this.IgniteInterior(ent);
    this.IgniteExterior(ent);
  }

  private void IgniteInterior(Entity<RMCVehicleDestroyedFireComponent> ent)
  {
    EntityUid grid;
    MapGridComponent comp;
    if (!this._vehicles.TryGetInteriorGrid(ent.Owner, out grid) || !this.TryComp<MapGridComponent>(grid, out comp))
      return;
    Entity<CollideChainComponent> chain = this._onCollide.SpawnChain();
    foreach (TileRef allTile in this._map.GetAllTiles(grid, comp))
      this._flammable.SpawnFire(this._map.GridTileToLocal(grid, comp, allTile.GridIndices), ent.Comp.InteriorFire, (EntityUid) chain, 1, new int?(), new int?(), out bool _);
  }

  private void IgniteExterior(Entity<RMCVehicleDestroyedFireComponent> ent)
  {
    EntityUid? gridUid = this.Transform(ent.Owner).GridUid;
    if (!gridUid.HasValue)
      return;
    EntityUid valueOrDefault = gridUid.GetValueOrDefault();
    MapGridComponent comp;
    if (!this.TryComp<MapGridComponent>(valueOrDefault, out comp))
      return;
    Box2 worldAabb = this._lookup.GetWorldAABB(ent.Owner);
    int num = (int) comp.TileSize * Math.Max(ent.Comp.ExteriorPaddingTiles, 0);
    Box2 worldArea;
    // ISSUE: explicit constructor call
    ((Box2) ref worldArea).\u002Ector(worldAabb.Left - (float) num, worldAabb.Bottom - (float) num, worldAabb.Right + (float) num, worldAabb.Top + (float) num);
    HashSet<Vector2i> vector2iSet = new HashSet<Vector2i>();
    foreach (TileRef tileRef in this._map.GetTilesIntersecting(valueOrDefault, comp, worldAabb))
      vector2iSet.Add(tileRef.GridIndices);
    Entity<CollideChainComponent> chain = this._onCollide.SpawnChain();
    foreach (TileRef tileRef in this._map.GetTilesIntersecting(valueOrDefault, comp, worldArea))
    {
      if (!vector2iSet.Contains(tileRef.GridIndices) && this._random.Prob(ent.Comp.ExteriorFireChance))
        this._flammable.SpawnFire(this._map.GridTileToLocal(valueOrDefault, comp, tileRef.GridIndices), ent.Comp.ExteriorFire, (EntityUid) chain, 1, new int?(), new int?(), out bool _);
    }
  }
}
