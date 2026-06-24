// Decompiled with JetBrains decompiler
// Type: Content.Client.NPC.PathfindingSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.NPC;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.NPC;

public sealed class PathfindingSystem : SharedPathfindingSystem
{
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private IResourceCache _cache;
  [Dependency]
  private NPCSteeringSystem _steering;
  [Dependency]
  private MapSystem _mapSystem;
  [Dependency]
  private SharedTransformSystem _transformSystem;
  private PathfindingDebugMode _modes;
  public Dictionary<NetEntity, Dictionary<Vector2i, List<PathfindingBreadcrumb>>> Breadcrumbs = new Dictionary<NetEntity, Dictionary<Vector2i, List<PathfindingBreadcrumb>>>();
  public Dictionary<NetEntity, Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>>> Polys = new Dictionary<NetEntity, Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>>>();
  public readonly List<(TimeSpan Time, PathRouteMessage Message)> Routes = new List<(TimeSpan, PathRouteMessage)>();

  public PathfindingDebugMode Modes
  {
    get => this._modes;
    set
    {
      IOverlayManager ioverlayManager = IoCManager.Resolve<IOverlayManager>();
      if (value == PathfindingDebugMode.None)
      {
        this.Breadcrumbs.Clear();
        this.Polys.Clear();
        ioverlayManager.RemoveOverlay<PathfindingOverlay>();
      }
      else if (!ioverlayManager.HasOverlay<PathfindingOverlay>())
        ioverlayManager.AddOverlay((Overlay) new PathfindingOverlay((IEntityManager) this.EntityManager, this._eyeManager, this._inputManager, this._mapManager, this._cache, this, this._mapSystem, this._transformSystem));
      this._steering.DebugEnabled = (value & PathfindingDebugMode.Steering) != PathfindingDebugMode.None;
      this._modes = value;
      this.RaiseNetworkEvent((EntityEventArgs) new RequestPathfindingDebugMessage()
      {
        Mode = this._modes
      });
    }
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PathBreadcrumbsMessage>(new EntityEventHandler<PathBreadcrumbsMessage>(this.OnBreadcrumbs), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PathBreadcrumbsRefreshMessage>(new EntityEventHandler<PathBreadcrumbsRefreshMessage>(this.OnBreadcrumbsRefresh), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PathPolysMessage>(new EntityEventHandler<PathPolysMessage>(this.OnPolys), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PathPolysRefreshMessage>(new EntityEventHandler<PathPolysRefreshMessage>(this.OnPolysRefresh), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PathRouteMessage>(new EntityEventHandler<PathRouteMessage>(this.OnRoute), (Type[]) null, (Type[]) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._timing.IsFirstTimePredicted)
      return;
    for (int index = 0; index < this.Routes.Count && !(this._timing.RealTime < this.Routes[index].Time); ++index)
      this.Routes.RemoveAt(index);
  }

  private void OnRoute(PathRouteMessage ev)
  {
    this.Routes.Add((this._timing.RealTime + TimeSpan.FromSeconds(0.5), ev));
  }

  private void OnPolys(PathPolysMessage ev) => this.Polys = ev.Polys;

  private void OnPolysRefresh(PathPolysRefreshMessage ev)
  {
    Extensions.GetOrNew<NetEntity, Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>>>(this.Polys, ev.GridUid)[ev.Origin] = ev.Polys;
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._modes = PathfindingDebugMode.None;
  }

  private void OnBreadcrumbs(PathBreadcrumbsMessage ev) => this.Breadcrumbs = ev.Breadcrumbs;

  private void OnBreadcrumbsRefresh(PathBreadcrumbsRefreshMessage ev)
  {
    Dictionary<Vector2i, List<PathfindingBreadcrumb>> dictionary;
    if (!this.Breadcrumbs.TryGetValue(ev.GridUid, out dictionary))
      return;
    dictionary[ev.Origin] = ev.Data;
  }
}
