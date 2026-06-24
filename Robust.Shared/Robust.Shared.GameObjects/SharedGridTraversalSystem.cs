using System.Numerics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Timing;

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
		SubscribeLocalEvent<TransformStartupEvent>(OnStartup);
	}

	private void OnStartup(ref TransformStartupEvent ev)
	{
		CheckTraverse(ev.Entity);
	}

	internal void CheckTraverse(Entity<TransformComponent> entity)
	{
		if (!Enabled || _timing.ApplyingState)
		{
			return;
		}
		EntityUid owner = entity.Owner;
		TransformComponent comp = entity.Comp;
		if (owner == _recursionGuard || (comp.GridUid != comp.ParentUid && comp.MapUid != comp.ParentUid) || comp.Anchored)
		{
			return;
		}
		EntityUid value = owner;
		EntityUid? gridUid = comp.GridUid;
		if (value == gridUid)
		{
			return;
		}
		value = owner;
		gridUid = comp.MapUid;
		if (value == gridUid)
		{
			return;
		}
		gridUid = comp.MapUid;
		if (!gridUid.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = gridUid.GetValueOrDefault();
		if (!comp.GridTraversal)
		{
			return;
		}
		if (_recursionGuard != EntityUid.Invalid)
		{
			base.Log.Error($"Grid traversal attempted to handle movement of {ToPrettyString(owner)} while moving {ToPrettyString(_recursionGuard)}");
			return;
		}
		_recursionGuard = owner;
		try
		{
			CheckTraversal(owner, comp, valueOrDefault);
		}
		finally
		{
			_recursionGuard = default(EntityUid);
		}
	}

	public void CheckTraversal(EntityUid entity, TransformComponent xform, EntityUid map)
	{
		EntityUid parentUid = xform.ParentUid;
		EntityUid? mapUid = xform.MapUid;
		Vector2 worldPos = ((parentUid == mapUid) ? xform.LocalPosition : Vector2.Transform(xform.LocalPosition, Transform(xform.ParentUid).LocalMatrix));
		if (_mapManager.TryFindGridAt(map, worldPos, out EntityUid uid, out MapGridComponent _))
		{
			parentUid = uid;
			mapUid = xform.GridUid;
			if (parentUid != mapUid && !TerminatingOrDeleted(uid))
			{
				_transform.SetParent(entity, xform, uid);
			}
		}
		else if (xform.GridUid.HasValue)
		{
			_transform.SetParent(entity, xform, map);
		}
	}
}
