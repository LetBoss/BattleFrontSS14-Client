// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivCommanderVisionHideSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderVisionHideSystem : EntitySystem
{
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private SharedMapSystem _maps;
  [Dependency]
  private SpriteSystem _sprites;
  [Dependency]
  private SharedTransformSystem _xform;
  [Dependency]
  private CivCommanderVisionSystem _vision;
  public readonly List<(Entity<SpriteComponent?> Ent, float BaseAlpha)> CachedBaseAlphas = new List<(Entity<SpriteComponent>, float)>(64 /*0x40*/);
  private readonly HashSet<EntityUid> _seen = new HashSet<EntityUid>();
  private EntityUid _localEntity = EntityUid.Invalid;
  private MapId _mapId = MapId.Nullspace;

  public bool Prepare(MapId mapId)
  {
    this.RestoreCachedAlphas();
    this._seen.Clear();
    this._localEntity = EntityUid.Invalid;
    this._mapId = MapId.Nullspace;
    if (!this._vision.Active || (double) this._vision.VisionRange <= 0.0 || MapId.op_Equality(mapId, MapId.Nullspace))
      return false;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    CivTeamMemberComponent teamMemberComponent;
    if (!localEntity.HasValue || !this.TryComp<CivTeamMemberComponent>(localEntity.Value, ref teamMemberComponent) || !teamMemberComponent.IsCommander || teamMemberComponent.TeamId <= 0)
      return false;
    this._localEntity = localEntity.Value;
    this._mapId = mapId;
    return true;
  }

  public void RestoreCachedAlphas()
  {
    if (this.CachedBaseAlphas.Count == 0)
      return;
    foreach ((Entity<SpriteComponent> Ent, float BaseAlpha) in this.CachedBaseAlphas)
    {
      if (Ent.Comp != null)
      {
        SpriteSystem sprites = this._sprites;
        Entity<SpriteComponent> entity = Ent;
        Color color1 = Ent.Comp.Color;
        Color color2 = ((Color) ref color1).WithAlpha(BaseAlpha);
        sprites.SetColor(entity, color2);
      }
    }
    this.CachedBaseAlphas.Clear();
  }

  public void Apply()
  {
    if (MapId.op_Equality(this._mapId, MapId.Nullspace))
      return;
    EntityQueryEnumerator<TransformComponent, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<TransformComponent, SpriteComponent>();
    EntityUid uid;
    TransformComponent xform;
    SpriteComponent sprite;
    while (entityQueryEnumerator.MoveNext(ref uid, ref xform, ref sprite))
      this.ApplyEntity(uid, xform, sprite);
  }

  private void ApplyEntity(EntityUid uid, TransformComponent xform, SpriteComponent sprite)
  {
    CivTeamMemberComponent teamMemberComponent;
    if (EntityUid.op_Equality(uid, this._localEntity) || !this._seen.Add(uid) || MapId.op_Inequality(xform.MapID, this._mapId) || !sprite.Visible || this.TryComp<CivTeamMemberComponent>(uid, ref teamMemberComponent) && teamMemberComponent.IsCommander)
      return;
    float num = this.IsVisible(xform) ? sprite.Color.A : 0.0f;
    if ((double) MathF.Abs(sprite.Color.A - num) <= 1.0 / 1000.0)
      return;
    (EntityUid, SpriteComponent) valueTuple = (uid, sprite);
    this.CachedBaseAlphas.Add((Entity<SpriteComponent>.op_Implicit(valueTuple), sprite.Color.A));
    SpriteSystem sprites = this._sprites;
    Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit(valueTuple);
    Color color1 = sprite.Color;
    Color color2 = ((Color) ref color1).WithAlpha(num);
    sprites.SetColor(entity, color2);
  }

  private bool IsVisible(TransformComponent xform)
  {
    EntityUid gridUid;
    Vector2i tile;
    CivCommanderVisionTileState state;
    return this.TryGetTile(xform, out gridUid, out tile) && this._vision.TryGetTileState(gridUid, tile, out state) && state == CivCommanderVisionTileState.Visible;
  }

  private bool TryGetTile(TransformComponent xform, out EntityUid gridUid, out Vector2i tile)
  {
    gridUid = EntityUid.Invalid;
    tile = new Vector2i();
    if (MapId.op_Inequality(xform.MapID, this._mapId) || MapId.op_Equality(xform.MapID, MapId.Nullspace))
      return false;
    EntityUid? gridUid1 = xform.GridUid;
    if (gridUid1.HasValue)
    {
      EntityUid valueOrDefault = gridUid1.GetValueOrDefault();
      MapGridComponent mapGridComponent;
      if (this.TryComp<MapGridComponent>(valueOrDefault, ref mapGridComponent))
      {
        gridUid = valueOrDefault;
        tile = this._maps.GetTileRef(valueOrDefault, mapGridComponent, xform.Coordinates).GridIndices;
        return true;
      }
    }
    EntityUid entityUid;
    MapGridComponent mapGridComponent1;
    if (!this._mapManager.TryFindGridAt(this._xform.ToMapCoordinates(xform.Coordinates, true), ref entityUid, ref mapGridComponent1) || mapGridComponent1 == null)
      return false;
    gridUid = entityUid;
    tile = this._maps.GetTileRef(entityUid, mapGridComponent1, xform.Coordinates).GridIndices;
    return true;
  }
}
