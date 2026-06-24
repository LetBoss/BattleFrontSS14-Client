using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Dynamics;

namespace Robust.Shared.Physics.BroadPhase;

public sealed class DynamicTreeBroadPhase : IBroadPhase
{
	private static class DelegateCache<TState>
	{
		public static readonly B2DynamicTree<FixtureProxy>.QueryCallback<(TState state, B2DynamicTree<FixtureProxy> tree, DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState> callback, Box2 aabb, bool approx, DynamicTree<FixtureProxy>.ExtractAabbDelegate extract)> AabbQueryState = AabbQueryStateCallback;

		public static readonly B2DynamicTree<FixtureProxy>.RayQueryCallback<(TState state, DynamicTree<FixtureProxy>.RayQueryCallbackDelegate<TState> callback, B2DynamicTree<FixtureProxy> tree, DynamicTree<FixtureProxy>.ExtractAabbDelegate? extract, Ray srcRay)> RayQueryState = RayQueryStateCallback;
	}

	private readonly B2DynamicTree<FixtureProxy> _tree;

	private readonly DynamicTree<FixtureProxy>.ExtractAabbDelegate _extractAabb = ExtractAabbFunc;

	private static readonly DynamicTree<FixtureProxy>.QueryCallbackDelegate<DynamicTree<FixtureProxy>.QueryCallbackDelegate> EasyQueryCallback = delegate(ref DynamicTree<FixtureProxy>.QueryCallbackDelegate s, in FixtureProxy v)
	{
		return s(in v);
	};

	private static readonly DynamicTree<FixtureProxy>.RayQueryCallbackDelegate<DynamicTree<FixtureProxy>.RayQueryCallbackDelegate> RayQueryDelegateCallbackInst = RayQueryDelegateCallback;

	public int Count => _tree.NodeCount;

	public B2DynamicTree<FixtureProxy> Tree => _tree;

	public DynamicTreeBroadPhase(int capacity)
	{
		_tree = new B2DynamicTree<FixtureProxy>(1f / 32f, capacity);
	}

	public DynamicTreeBroadPhase()
		: this(256)
	{
	}

	private static Box2 ExtractAabbFunc(in FixtureProxy proxy)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return proxy.AABB;
	}

	public Box2 GetFatAabb(DynamicTree.Proxy proxy)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return _tree.GetUserData(proxy).AABB;
	}

	public DynamicTree.Proxy AddProxy(ref FixtureProxy proxy)
	{
		return _tree.CreateProxy(in proxy.AABB, uint.MaxValue, proxy);
	}

	public void MoveProxy(DynamicTree.Proxy proxy, in Box2 aabb)
	{
		_tree.MoveProxy(proxy, in aabb);
	}

	public void RemoveProxy(DynamicTree.Proxy proxy)
	{
		_tree.DestroyProxy(proxy);
	}

	public FixtureProxy? GetProxy(DynamicTree.Proxy proxy)
	{
		return _tree.GetUserData(proxy);
	}

	public void QueryAabb(DynamicTree<FixtureProxy>.QueryCallbackDelegate callback, Box2 aabb, bool approx = false)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		QueryAabb(ref callback, EasyQueryCallback, aabb, approx);
	}

	public void QueryAabb<TState>(ref TState state, DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState> callback, Box2 aabb, bool approx = false)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		(TState, B2DynamicTree<FixtureProxy>, DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState>, Box2, bool, DynamicTree<FixtureProxy>.ExtractAabbDelegate) state2 = (state, _tree, callback, aabb, approx, _extractAabb);
		_tree.Query<(TState, B2DynamicTree<FixtureProxy>, DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState>, Box2, bool, DynamicTree<FixtureProxy>.ExtractAabbDelegate)>(ref state2, DelegateCache<TState>.AabbQueryState, in aabb);
		(state, _, _, _, _, _) = state2;
	}

	public IEnumerable<FixtureProxy> QueryAabb(Box2 aabb, bool approx = false)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		List<FixtureProxy> proxies = new List<FixtureProxy>();
		return QueryAabb(proxies, aabb, approx);
	}

	public IEnumerable<FixtureProxy> QueryAabb(List<FixtureProxy> proxies, Box2 aabb, bool approx = false)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		QueryAabb(ref proxies, delegate(ref List<FixtureProxy> lst, in FixtureProxy i)
		{
			lst.Add(i);
			return true;
		}, aabb, approx);
		return proxies;
	}

	public void QueryPoint(DynamicTree<FixtureProxy>.QueryCallbackDelegate callback, Vector2 point, bool approx = false)
	{
		QueryPoint(ref callback, EasyQueryCallback, point, approx);
	}

	public void QueryPoint<TState>(ref TState state, DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState> callback, Vector2 point, bool approx = false)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		(TState, B2DynamicTree<FixtureProxy>, DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState>, Vector2, bool, DynamicTree<FixtureProxy>.ExtractAabbDelegate) state2 = (state, _tree, callback, point, approx, _extractAabb);
		_tree.Query<(TState, B2DynamicTree<FixtureProxy>, DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState>, Vector2, bool, DynamicTree<FixtureProxy>.ExtractAabbDelegate)>(ref state2, delegate(ref (TState state, B2DynamicTree<FixtureProxy> tree, DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState> callback, Vector2 point, bool approx, DynamicTree<FixtureProxy>.ExtractAabbDelegate extract) reference, DynamicTree.Proxy proxy)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			FixtureProxy value = reference.tree.GetUserData(proxy);
			if (!reference.approx)
			{
				Box2 val = reference.extract(in value);
				if (!((Box2)(ref val)).Contains(reference.point, true))
				{
					return true;
				}
			}
			return reference.callback(ref reference.state, in value);
		}, Box2.CenteredAround(point, new Vector2(0.1f, 0.1f)));
		(state, _, _, _, _, _) = state2;
	}

	public IEnumerable<FixtureProxy> QueryPoint(Vector2 point, bool approx = false)
	{
		List<FixtureProxy> state = new List<FixtureProxy>();
		QueryPoint<List<FixtureProxy>>(ref state, delegate(ref List<FixtureProxy> list, in FixtureProxy i)
		{
			list.Add(i);
			return true;
		}, point, approx);
		return state;
	}

	public void QueryRay<TState>(ref TState state, DynamicTree<FixtureProxy>.RayQueryCallbackDelegate<TState> callback, in Ray ray, bool approx = false)
	{
		(TState, DynamicTree<FixtureProxy>.RayQueryCallbackDelegate<TState>, B2DynamicTree<FixtureProxy>, DynamicTree<FixtureProxy>.ExtractAabbDelegate, Ray) state2 = (state, callback, _tree, approx ? null : _extractAabb, ray);
		_tree.RayCast<(TState, DynamicTree<FixtureProxy>.RayQueryCallbackDelegate<TState>, B2DynamicTree<FixtureProxy>, DynamicTree<FixtureProxy>.ExtractAabbDelegate, Ray)>(ref state2, DelegateCache<TState>.RayQueryState, in ray);
		(state, _, _, _, _) = state2;
	}

	public void Rebuild(bool fullBuild)
	{
		_tree.Rebuild(fullBuild);
	}

	public void RebuildBottomUp()
	{
		_tree.RebuildBottomUp();
	}

	private static bool AabbQueryStateCallback<TState>(ref (TState state, B2DynamicTree<FixtureProxy> tree, DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState> callback, Box2 aabb, bool approx, DynamicTree<FixtureProxy>.ExtractAabbDelegate extract) tuple, DynamicTree.Proxy proxy)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		FixtureProxy value = tuple.tree.GetUserData(proxy);
		if (!tuple.approx)
		{
			Box2 val = tuple.extract(in value);
			if (!((Box2)(ref val)).Intersects(ref tuple.aabb))
			{
				return true;
			}
		}
		return tuple.callback(ref tuple.state, in value);
	}

	private static bool RayQueryStateCallback<TState>(ref (TState state, DynamicTree<FixtureProxy>.RayQueryCallbackDelegate<TState> callback, B2DynamicTree<FixtureProxy> tree, DynamicTree<FixtureProxy>.ExtractAabbDelegate? extract, Ray srcRay) tuple, DynamicTree.Proxy proxy, in Vector2 hitPos, float distance)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		FixtureProxy value = tuple.tree.GetUserData(proxy);
		Vector2 hitPos2 = hitPos;
		if (tuple.extract != null)
		{
			Box2 box = tuple.extract(in value);
			if (!tuple.srcRay.Intersects(box, out distance, out hitPos2))
			{
				return true;
			}
		}
		return tuple.callback(ref tuple.state, in value, in hitPos2, distance);
	}

	public void QueryRay(DynamicTree<FixtureProxy>.RayQueryCallbackDelegate callback, in Ray ray, bool approx = false)
	{
		QueryRay(ref callback, RayQueryDelegateCallbackInst, in ray, approx);
	}

	private static bool RayQueryDelegateCallback(ref DynamicTree<FixtureProxy>.RayQueryCallbackDelegate state, in FixtureProxy value, in Vector2 point, float distFromOrigin)
	{
		return state(in value, in point, distFromOrigin);
	}

	void IBroadPhase.MoveProxy(DynamicTree.Proxy proxyId, in Box2 aabb)
	{
		MoveProxy(proxyId, in aabb);
	}

	void IBroadPhase.QueryRay(DynamicTree<FixtureProxy>.RayQueryCallbackDelegate callback, in Ray ray, bool approx = false)
	{
		QueryRay(callback, in ray, approx);
	}

	void IBroadPhase.QueryRay<TState>(ref TState state, DynamicTree<FixtureProxy>.RayQueryCallbackDelegate<TState> callback, in Ray ray, bool approx = false)
	{
		QueryRay(ref state, callback, in ray, approx);
	}
}
