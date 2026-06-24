using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;

namespace Robust.Shared.ComponentTrees;

internal sealed class RecursiveMoveSystem : EntitySystem
{
	public delegate void TreeRecursiveMoveEventHandler(EntityUid uid, TransformComponent xform);

	[Dependency]
	private readonly SharedTransformSystem _transform;

	private EntityQuery<MapComponent> _mapQuery;

	private EntityQuery<MapGridComponent> _gridQuery;

	private EntityQuery<TransformComponent> _xformQuery;

	private bool _subscribed;

	public event TreeRecursiveMoveEventHandler? OnTreeRecursiveMove;

	public override void Initialize()
	{
		base.Initialize();
		_gridQuery = GetEntityQuery<MapGridComponent>();
		_mapQuery = GetEntityQuery<MapComponent>();
		_xformQuery = GetEntityQuery<TransformComponent>();
	}

	public override void Shutdown()
	{
		if (_subscribed)
		{
			_transform.OnBeforeMoveEvent -= AnythingMoved;
		}
		_subscribed = false;
	}

	internal void AddSubscription()
	{
		if (!_subscribed)
		{
			_subscribed = true;
			_transform.OnBeforeMoveEvent += AnythingMoved;
		}
	}

	private void AnythingMoved(ref MoveEvent args)
	{
		if (!(args.Component.MapUid == args.Sender) && !(args.Component.GridUid == args.Sender))
		{
			AnythingMovedSubHandler(args.Sender, args.Component);
		}
	}

	private void AnythingMovedSubHandler(EntityUid uid, TransformComponent xform)
	{
		this.OnTreeRecursiveMove?.Invoke(uid, xform);
		foreach (EntityUid child in xform._children)
		{
			if (_xformQuery.TryGetComponent(child, out TransformComponent component))
			{
				AnythingMovedSubHandler(child, component);
			}
		}
	}
}
