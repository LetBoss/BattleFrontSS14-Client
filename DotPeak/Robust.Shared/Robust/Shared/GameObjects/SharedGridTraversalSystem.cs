// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.SharedGridTraversalSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Timing;
using System.Numerics;

#nullable enable
namespace Robust.Shared.GameObjects;

public sealed class SharedGridTraversalSystem : EntitySystem
{
  [Dependency]
  private readonly IMapManagerInternal _mapManager;
  [Dependency]
  private readonly SharedTransformSystem _transform;
  [Dependency]
  private readonly IGameTiming _timing;
  private EntityUid _recursionGuard;
  public bool Enabled = true;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<TransformStartupEvent>(new EntityEventRefHandler<TransformStartupEvent>(this.OnStartup));
  }

  private void OnStartup(ref TransformStartupEvent ev) => this.CheckTraverse(ev.Entity);

  internal void CheckTraverse(Entity<TransformComponent> entity)
  {
    if (!this.Enabled || this._timing.ApplyingState)
      return;
    EntityUid owner = entity.Owner;
    TransformComponent comp = entity.Comp;
    if (owner == this._recursionGuard)
      return;
    EntityUid? nullable = comp.GridUid;
    EntityUid parentUid1 = comp.ParentUid;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() != parentUid1 ? 1 : 0) : 1) != 0)
    {
      nullable = comp.MapUid;
      EntityUid parentUid2 = comp.ParentUid;
      if ((nullable.HasValue ? (nullable.GetValueOrDefault() != parentUid2 ? 1 : 0) : 1) != 0)
        return;
    }
    if (comp.Anchored)
      return;
    EntityUid entityUid1 = owner;
    nullable = comp.GridUid;
    if ((nullable.HasValue ? (entityUid1 == nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
      return;
    EntityUid entityUid2 = owner;
    nullable = comp.MapUid;
    if ((nullable.HasValue ? (entityUid2 == nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
      return;
    nullable = comp.MapUid;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    if (!comp.GridTraversal)
      return;
    if (this._recursionGuard != EntityUid.Invalid)
    {
      this.Log.Error($"Grid traversal attempted to handle movement of {this.ToPrettyString((Entity<MetaDataComponent>) owner)} while moving {this.ToPrettyString((Entity<MetaDataComponent>) this._recursionGuard)}");
    }
    else
    {
      this._recursionGuard = owner;
      try
      {
        this.CheckTraversal(owner, comp, valueOrDefault);
      }
      finally
      {
        this._recursionGuard = new EntityUid();
      }
    }
  }

  public void CheckTraversal(EntityUid entity, TransformComponent xform, EntityUid map)
  {
    EntityUid parentUid = xform.ParentUid;
    EntityUid? mapUid = xform.MapUid;
    Vector2 worldPos = (mapUid.HasValue ? (parentUid == mapUid.GetValueOrDefault() ? 1 : 0) : 0) != 0 ? xform.LocalPosition : Vector2.Transform(xform.LocalPosition, this.Transform(xform.ParentUid).LocalMatrix);
    EntityUid uid;
    if (this._mapManager.TryFindGridAt(map, worldPos, out uid, out MapGridComponent _))
    {
      EntityUid entityUid = uid;
      EntityUid? gridUid = xform.GridUid;
      if ((gridUid.HasValue ? (entityUid != gridUid.GetValueOrDefault() ? 1 : 0) : 1) == 0 || this.TerminatingOrDeleted(uid))
        return;
      this._transform.SetParent(entity, xform, uid);
    }
    else
    {
      if (!xform.GridUid.HasValue)
        return;
      this._transform.SetParent(entity, xform, map);
    }
  }
}
