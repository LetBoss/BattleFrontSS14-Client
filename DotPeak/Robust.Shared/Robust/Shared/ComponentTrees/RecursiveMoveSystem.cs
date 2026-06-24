// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ComponentTrees.RecursiveMoveSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;

#nullable enable
namespace Robust.Shared.ComponentTrees;

internal sealed class RecursiveMoveSystem : EntitySystem
{
  [Dependency]
  private readonly SharedTransformSystem _transform;
  private Robust.Shared.GameObjects.EntityQuery<MapComponent> _mapQuery;
  private Robust.Shared.GameObjects.EntityQuery<MapGridComponent> _gridQuery;
  private Robust.Shared.GameObjects.EntityQuery<TransformComponent> _xformQuery;
  private bool _subscribed;

  public event RecursiveMoveSystem.TreeRecursiveMoveEventHandler? OnTreeRecursiveMove;

  public override void Initialize()
  {
    base.Initialize();
    this._gridQuery = this.GetEntityQuery<MapGridComponent>();
    this._mapQuery = this.GetEntityQuery<MapComponent>();
    this._xformQuery = this.GetEntityQuery<TransformComponent>();
  }

  public override void Shutdown()
  {
    if (this._subscribed)
      this._transform.OnBeforeMoveEvent -= new SharedTransformSystem.MoveEventHandler(this.AnythingMoved);
    this._subscribed = false;
  }

  internal void AddSubscription()
  {
    if (this._subscribed)
      return;
    this._subscribed = true;
    this._transform.OnBeforeMoveEvent += new SharedTransformSystem.MoveEventHandler(this.AnythingMoved);
  }

  private void AnythingMoved(ref MoveEvent args)
  {
    EntityUid? mapUid = args.Component.MapUid;
    EntityUid sender1 = args.Sender;
    if ((mapUid.HasValue ? (mapUid.GetValueOrDefault() == sender1 ? 1 : 0) : 0) != 0)
      return;
    EntityUid? gridUid = args.Component.GridUid;
    EntityUid sender2 = args.Sender;
    if ((gridUid.HasValue ? (gridUid.GetValueOrDefault() == sender2 ? 1 : 0) : 0) != 0)
      return;
    this.AnythingMovedSubHandler(args.Sender, args.Component);
  }

  private void AnythingMovedSubHandler(EntityUid uid, TransformComponent xform)
  {
    RecursiveMoveSystem.TreeRecursiveMoveEventHandler treeRecursiveMove = this.OnTreeRecursiveMove;
    if (treeRecursiveMove != null)
      treeRecursiveMove(uid, xform);
    foreach (EntityUid child in xform._children)
    {
      TransformComponent component;
      if (this._xformQuery.TryGetComponent(child, out component))
        this.AnythingMovedSubHandler(child, component);
    }
  }

  public delegate void TreeRecursiveMoveEventHandler(EntityUid uid, TransformComponent xform);
}
