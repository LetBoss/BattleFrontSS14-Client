// Decompiled with JetBrains decompiler
// Type: Content.Client.SubFloor.TrayScanRevealSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.SubFloor;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using System;
using System.Linq;

#nullable enable
namespace Content.Client.SubFloor;

public sealed class TrayScanRevealSystem : EntitySystem
{
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedMapSystem _map;

  public bool IsUnderRevealingEntity(EntityUid uid)
  {
    EntityUid? grid = this._transform.GetGrid(Entity<TransformComponent>.op_Implicit(uid));
    if (!grid.HasValue)
      return false;
    MapGridComponent mapGridComponent = this.Comp<MapGridComponent>(grid.Value);
    Vector2i orMapTilePosition = this._transform.GetGridOrMapTilePosition(uid, (TransformComponent) null);
    return this.HasTrayScanReveal(Entity<MapGridComponent>.op_Implicit((grid.Value, mapGridComponent)), orMapTilePosition);
  }

  private bool HasTrayScanReveal(Entity<MapGridComponent> ent, Vector2i position)
  {
    return this._map.GetAnchoredEntities(ent, position).Any<EntityUid>(new Func<EntityUid, bool>(((EntitySystem) this).HasComp<TrayScanRevealComponent>));
  }
}
