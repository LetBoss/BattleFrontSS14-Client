using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Robust.Shared.Maths;

namespace Robust.Shared.Physics;

public abstract class DynamicTree
{
	public readonly struct Proxy : IEquatable<Proxy>, IComparable<Proxy>
	{
		private readonly int _value;

		public static Proxy Free
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Proxy(-1);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Proxy(int v)
		{
			_value = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(Proxy other)
		{
			return _value == other._value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int CompareTo(Proxy other)
		{
			return _value.CompareTo(other._value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? obj)
		{
			if (obj is Proxy other)
			{
				return Equals(other);
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return _value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator int(Proxy n)
		{
			return n._value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Proxy(int v)
		{
			return new Proxy(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Proxy a, Proxy b)
		{
			return a._value == b._value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Proxy a, Proxy b)
		{
			return a._value != b._value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >(Proxy a, Proxy b)
		{
			return a._value > b._value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <(Proxy a, Proxy b)
		{
			return a._value < b._value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >=(Proxy a, Proxy b)
		{
			return a._value >= b._value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <=(Proxy a, Proxy b)
		{
			return a._value <= b._value;
		}

		public override string ToString()
		{
			return _value.ToString();
		}
	}

	public const int MinimumCapacity = 16;

	protected const float AabbMultiplier = 2f;

	protected readonly float AabbExtendSize;

	protected readonly Func<int, int> GrowthFunc;

	protected DynamicTree(float aabbExtendSize, Func<int, int>? growthFunc)
	{
		AabbExtendSize = aabbExtendSize;
		GrowthFunc = growthFunc ?? new Func<int, int>(DefaultGrowthFunc);
	}

	private static int DefaultGrowthFunc(int x)
	{
		return x * 2;
	}
}
[DebuggerDisplay("{DebuggerDisplay}")]
public sealed class DynamicTree<T> : IBroadPhase<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : notnull
{
	public delegate Box2 ExtractAabbDelegate(in T value);

	public delegate bool QueryCallbackDelegate(in T value);

	public delegate bool QueryCallbackDelegate<TState>(ref TState state, in T value);

	public delegate bool RayQueryCallbackDelegate(in T value, in Vector2 point, float distFromOrigin);

	public delegate bool RayQueryCallbackDelegate<TState>(ref TState state, in T value, in Vector2 point, float distFromOrigin);

	private static class DelegateCache<TState>
	{
		public static readonly B2DynamicTree<T>.QueryCallback<(TState state, B2DynamicTree<T> tree, QueryCallbackDelegate<TState> callback, Box2 aabb, bool approx, ExtractAabbDelegate extract)> AabbQueryState = DynamicTree<T>.AabbQueryStateCallback<TState>;

		public static readonly B2DynamicTree<T>.RayQueryCallback<(TState state, RayQueryCallbackDelegate<TState> callback, B2DynamicTree<T> tree, ExtractAabbDelegate? extract, Ray srcRay)> RayQueryState = DynamicTree<T>.RayQueryStateCallback<TState>;
	}

	private readonly IEqualityComparer<T> _equalityComparer;

	private readonly ExtractAabbDelegate _extractAabb;

	private Dictionary<T, DynamicTree.Proxy> _nodeLookup;

	public readonly B2DynamicTree<T> _b2Tree;

	private static readonly QueryCallbackDelegate<QueryCallbackDelegate> EasyQueryCallback = delegate(ref QueryCallbackDelegate s, in T v)
	{
		return s(in v);
	};

	private static readonly RayQueryCallbackDelegate<RayQueryCallbackDelegate> RayQueryDelegateCallbackInst = RayQueryDelegateCallback;

	public int Capacity => _b2Tree.Capacity;

	public int Height => _b2Tree.Height;

	public int MaxBalance => _b2Tree.MaxBalance;

	public float AreaRatio => _b2Tree.AreaRatio;

	public string DebuggerDisplay => $"Count = {Count}, Capacity = {Capacity}, Height = {Height}, NodeCount = {NodeCount}";

	public int NodeCount { get; private set; }

	public int Count => _nodeLookup.Count;

	public bool IsReadOnly => false;

	public DynamicTree(ExtractAabbDelegate extractAabbFunc, IEqualityComparer<T>? comparer = null, float aabbExtendSize = 1f / 32f, int capacity = 256, Func<int, int>? growthFunc = null)
	{
		capacity = Math.Max(16, capacity);
		_extractAabb = extractAabbFunc;
		_equalityComparer = comparer ?? EqualityComparer<T>.Default;
		_nodeLookup = new Dictionary<T, DynamicTree.Proxy>(_equalityComparer);
		_b2Tree = new B2DynamicTree<T>(aabbExtendSize, capacity, growthFunc);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return _nodeLookup.Keys.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Clear()
	{
		foreach (DynamicTree.Proxy value in _nodeLookup.Values)
		{
			_b2Tree.DestroyProxy(value);
		}
	}

	public bool Contains(T item)
	{
		return _nodeLookup.ContainsKey(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		_nodeLookup.Keys.CopyTo(array, arrayIndex);
	}

	void ICollection<T>.Add(T item)
	{
		Add(in item);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Add(in T item, Box2? aabb = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetProxy(in item, out var proxy))
		{
			return false;
		}
		Box2 aabb2 = aabb.GetValueOrDefault();
		if (!aabb.HasValue)
		{
			aabb2 = _extractAabb(in item);
			aabb = aabb2;
		}
		aabb2 = aabb.Value;
		if (((Box2)(ref aabb2)).HasNan())
		{
			_nodeLookup[item] = DynamicTree.Proxy.Free;
			return true;
		}
		B2DynamicTree<T> b2Tree = _b2Tree;
		aabb2 = aabb.Value;
		proxy = b2Tree.CreateProxy(in aabb2, uint.MaxValue, item);
		_nodeLookup[item] = proxy;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool TryGetProxy(in T item, out DynamicTree.Proxy proxy)
	{
		return _nodeLookup.TryGetValue(item, out proxy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Box2? GetNodeBounds(T item)
	{
		if (!TryGetProxy(in item, out var proxy))
		{
			return null;
		}
		return _b2Tree.GetFatAabb(proxy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Box2? GetNodeBounds(in T item)
	{
		if (!TryGetProxy(in item, out var proxy))
		{
			return null;
		}
		return _b2Tree.GetFatAabb(proxy);
	}

	public bool Remove(in T item)
	{
		if (!_nodeLookup.Remove(item, out var value))
		{
			return false;
		}
		if (value != DynamicTree.Proxy.Free)
		{
			_b2Tree.DestroyProxy(value);
		}
		return true;
	}

	bool ICollection<T>.Remove(T item)
	{
		return Remove(in item);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool Update(in T item, Box2? newBox = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		ref DynamicTree.Proxy valueRefOrNullRef = ref CollectionsMarshal.GetValueRefOrNullRef(_nodeLookup, item);
		if (Unsafe.IsNullRef(in valueRefOrNullRef))
		{
			return false;
		}
		Box2 aabb = newBox.GetValueOrDefault();
		if (!newBox.HasValue)
		{
			aabb = _extractAabb(in item);
			newBox = aabb;
		}
		aabb = newBox.Value;
		if (((Box2)(ref aabb)).HasNan())
		{
			if (valueRefOrNullRef == DynamicTree.Proxy.Free)
			{
				return false;
			}
			_b2Tree.DestroyProxy(valueRefOrNullRef);
			valueRefOrNullRef = DynamicTree.Proxy.Free;
			return true;
		}
		if (valueRefOrNullRef == DynamicTree.Proxy.Free)
		{
			B2DynamicTree<T> b2Tree = _b2Tree;
			aabb = newBox.Value;
			valueRefOrNullRef = b2Tree.CreateProxy(in aabb, uint.MaxValue, item);
			return true;
		}
		B2DynamicTree<T> b2Tree2 = _b2Tree;
		DynamicTree.Proxy proxy = valueRefOrNullRef;
		aabb = newBox.Value;
		b2Tree2.MoveProxy(proxy, in aabb);
		return true;
	}

	public void QueryAabb(QueryCallbackDelegate callback, Box2 aabb, bool approx = false)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		QueryAabb(ref callback, EasyQueryCallback, aabb, approx);
	}

	public void QueryAabb<TState>(ref TState state, QueryCallbackDelegate<TState> callback, Box2 aabb, bool approx = false)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		(TState, B2DynamicTree<T>, QueryCallbackDelegate<TState>, Box2, bool, ExtractAabbDelegate) state2 = (state, _b2Tree, callback, aabb, approx, _extractAabb);
		_b2Tree.Query<(TState, B2DynamicTree<T>, QueryCallbackDelegate<TState>, Box2, bool, ExtractAabbDelegate)>(ref state2, DelegateCache<TState>.AabbQueryState, in aabb);
		(state, _, _, _, _, _) = state2;
	}

	public IEnumerable<T> QueryAabb(Box2 aabb, bool approx = false)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		List<T> state = new List<T>();
		QueryAabb(ref state, delegate(ref List<T> lst, in T i)
		{
			lst.Add(i);
			return true;
		}, aabb, approx);
		return state;
	}

	public void QueryPoint(QueryCallbackDelegate callback, Vector2 point, bool approx = false)
	{
		QueryPoint(ref callback, EasyQueryCallback, point, approx);
	}

	public void QueryPoint<TState>(ref TState state, QueryCallbackDelegate<TState> callback, Vector2 point, bool approx = false)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		(TState, B2DynamicTree<T>, QueryCallbackDelegate<TState>, Vector2, bool, ExtractAabbDelegate) state2 = (state, _b2Tree, callback, point, approx, _extractAabb);
		_b2Tree.Query<(TState, B2DynamicTree<T>, QueryCallbackDelegate<TState>, Vector2, bool, ExtractAabbDelegate)>(ref state2, delegate(ref (TState state, B2DynamicTree<T> tree, QueryCallbackDelegate<TState> callback, Vector2 point, bool approx, ExtractAabbDelegate extract) reference, DynamicTree.Proxy proxy)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			T value = reference.tree.GetUserData(proxy);
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

	public IEnumerable<T> QueryPoint(Vector2 point, bool approx = false)
	{
		List<T> state = new List<T>();
		QueryPoint(ref state, delegate(ref List<T> list, in T i)
		{
			list.Add(i);
			return true;
		}, point, approx);
		return state;
	}

	public void QueryRay<TState>(ref TState state, RayQueryCallbackDelegate<TState> callback, in Ray ray, bool approx = false)
	{
		(TState, RayQueryCallbackDelegate<TState>, B2DynamicTree<T>, ExtractAabbDelegate, Ray) state2 = (state, callback, _b2Tree, approx ? null : _extractAabb, ray);
		_b2Tree.RayCast<(TState, RayQueryCallbackDelegate<TState>, B2DynamicTree<T>, ExtractAabbDelegate, Ray)>(ref state2, DelegateCache<TState>.RayQueryState, in ray);
		(state, _, _, _, _) = state2;
	}

	private static bool AabbQueryStateCallback<TState>(ref (TState state, B2DynamicTree<T> tree, QueryCallbackDelegate<TState> callback, Box2 aabb, bool approx, ExtractAabbDelegate extract) tuple, DynamicTree.Proxy proxy)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		T value = tuple.tree.GetUserData(proxy);
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

	private static bool RayQueryStateCallback<TState>(ref (TState state, RayQueryCallbackDelegate<TState> callback, B2DynamicTree<T> tree, ExtractAabbDelegate? extract, Ray srcRay) tuple, DynamicTree.Proxy proxy, in Vector2 hitPos, float distance)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		T value = tuple.tree.GetUserData(proxy);
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

	public void QueryRay(RayQueryCallbackDelegate callback, in Ray ray, bool approx = false)
	{
		QueryRay(ref callback, RayQueryDelegateCallbackInst, in ray, approx);
	}

	private static bool RayQueryDelegateCallback(ref RayQueryCallbackDelegate state, in T value, in Vector2 point, float distFromOrigin)
	{
		return state(in value, in point, distFromOrigin);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddOrUpdate(T item, Box2? aabb = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		Box2 aabb2 = aabb.GetValueOrDefault();
		if (!aabb.HasValue)
		{
			aabb2 = _extractAabb(in item);
			aabb = aabb2;
		}
		bool exists;
		ref DynamicTree.Proxy valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(_nodeLookup, item, out exists);
		if (!exists)
		{
			aabb2 = aabb.Value;
			DynamicTree.Proxy proxy;
			if (!((Box2)(ref aabb2)).HasNan())
			{
				B2DynamicTree<T> b2Tree = _b2Tree;
				aabb2 = aabb.Value;
				proxy = b2Tree.CreateProxy(in aabb2, uint.MaxValue, item);
			}
			else
			{
				proxy = DynamicTree.Proxy.Free;
			}
			valueRefOrAddDefault = proxy;
			return;
		}
		aabb2 = aabb.Value;
		if (((Box2)(ref aabb2)).HasNan())
		{
			if (!(valueRefOrAddDefault == DynamicTree.Proxy.Free))
			{
				_b2Tree.DestroyProxy(valueRefOrAddDefault);
				valueRefOrAddDefault = DynamicTree.Proxy.Free;
			}
		}
		else if (valueRefOrAddDefault == DynamicTree.Proxy.Free)
		{
			B2DynamicTree<T> b2Tree2 = _b2Tree;
			aabb2 = aabb.Value;
			valueRefOrAddDefault = b2Tree2.CreateProxy(in aabb2, uint.MaxValue, item);
		}
		else
		{
			B2DynamicTree<T> b2Tree3 = _b2Tree;
			DynamicTree.Proxy proxy2 = valueRefOrAddDefault;
			aabb2 = aabb.Value;
			b2Tree3.MoveProxy(proxy2, in aabb2);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("DEBUG_DYNAMIC_TREE")]
	[Conditional("DEBUG_DYNAMIC_TREE_ASSERTS")]
	[DebuggerNonUserCode]
	[DebuggerHidden]
	[DebuggerStepThrough]
	public static void Assert(bool assertion, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0)
	{
		if (assertion)
		{
			return;
		}
		string message = $"Assertion failure in {member} ({file}:{line})";
		Debugger.Break();
		throw new InvalidOperationException(message);
	}

	bool IBroadPhase<T>.Add(in T item, Box2? newAABB = null)
	{
		return Add(in item, newAABB);
	}

	bool IBroadPhase<T>.Remove(in T item)
	{
		return Remove(in item);
	}

	bool IBroadPhase<T>.Update(in T item, Box2? newAABB = null)
	{
		return Update(in item, newAABB);
	}

	void IBroadPhase<T>.QueryRay(DynamicTree<T>.RayQueryCallbackDelegate callback, in Ray ray, bool approx = false)
	{
		QueryRay(callback, in ray, approx);
	}

	void IBroadPhase<T>.QueryRay<TState>(ref TState state, DynamicTree<T>.RayQueryCallbackDelegate<TState> callback, in Ray ray, bool approx = false)
	{
		QueryRay(ref state, callback, in ray, approx);
	}
}
