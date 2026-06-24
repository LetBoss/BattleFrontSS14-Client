using System;
using System.Collections.Generic;
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
		get
		{
			return _modes;
		}
		set
		{
			IOverlayManager val = IoCManager.Resolve<IOverlayManager>();
			if (value == PathfindingDebugMode.None)
			{
				Breadcrumbs.Clear();
				Polys.Clear();
				val.RemoveOverlay<PathfindingOverlay>();
			}
			else if (!val.HasOverlay<PathfindingOverlay>())
			{
				val.AddOverlay((Overlay)(object)new PathfindingOverlay((IEntityManager)(object)((EntitySystem)this).EntityManager, _eyeManager, _inputManager, _mapManager, _cache, this, _mapSystem, _transformSystem));
			}
			if ((value & PathfindingDebugMode.Steering) != PathfindingDebugMode.None)
			{
				_steering.DebugEnabled = true;
			}
			else
			{
				_steering.DebugEnabled = false;
			}
			_modes = value;
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RequestPathfindingDebugMessage
			{
				Mode = _modes
			});
		}
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PathBreadcrumbsMessage>((EntityEventHandler<PathBreadcrumbsMessage>)OnBreadcrumbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PathBreadcrumbsRefreshMessage>((EntityEventHandler<PathBreadcrumbsRefreshMessage>)OnBreadcrumbsRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PathPolysMessage>((EntityEventHandler<PathPolysMessage>)OnPolys, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PathPolysRefreshMessage>((EntityEventHandler<PathPolysRefreshMessage>)OnPolysRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PathRouteMessage>((EntityEventHandler<PathRouteMessage>)OnRoute, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		for (int i = 0; i < Routes.Count; i++)
		{
			(TimeSpan, PathRouteMessage) tuple = Routes[i];
			if (!(_timing.RealTime < tuple.Item1))
			{
				Routes.RemoveAt(i);
				continue;
			}
			break;
		}
	}

	private void OnRoute(PathRouteMessage ev)
	{
		Routes.Add((_timing.RealTime + TimeSpan.FromSeconds(0.5), ev));
	}

	private void OnPolys(PathPolysMessage ev)
	{
		Polys = ev.Polys;
	}

	private void OnPolysRefresh(PathPolysRefreshMessage ev)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Extensions.GetOrNew<NetEntity, Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>>>(Polys, ev.GridUid)[ev.Origin] = ev.Polys;
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_modes = PathfindingDebugMode.None;
	}

	private void OnBreadcrumbs(PathBreadcrumbsMessage ev)
	{
		Breadcrumbs = ev.Breadcrumbs;
	}

	private void OnBreadcrumbsRefresh(PathBreadcrumbsRefreshMessage ev)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (Breadcrumbs.TryGetValue(ev.GridUid, out Dictionary<Vector2i, List<PathfindingBreadcrumb>> value))
		{
			value[ev.Origin] = ev.Data;
		}
	}
}
