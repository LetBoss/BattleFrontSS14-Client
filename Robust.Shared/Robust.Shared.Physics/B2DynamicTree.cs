using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Utility;

namespace Robust.Shared.Physics;

public sealed class B2DynamicTree<T> : DynamicTree
{
	public delegate bool RayQueryCallback<TState>(ref TState state, Proxy proxy, in Vector2 hitPos, float distance);

	public delegate bool RayQueryCallback(Proxy proxy, in Vector2 hitPos, float distance);

	public delegate bool QueryCallback(Proxy proxy);

	public delegate bool QueryCallback<TState>(ref TState state, Proxy proxy);

	private enum RotateType : byte
	{
		None,
		BF,
		BG,
		CD,
		CE
	}

	private struct Node
	{
		public Box2 Aabb;

		public uint CategoryBits;

		public Proxy Parent;

		public Proxy Next;

		public Proxy Child1;

		public Proxy Child2;

		public T UserData;

		public short Height;

		public bool Enlarged;

		public bool IsLeaf
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Height == 0;
			}
		}

		public bool IsFree
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Height == -1;
			}
		}

		public override string ToString()
		{
			return "Parent: " + ((Parent == Proxy.Free) ? "None" : Parent.ToString()) + ", " + ((!IsLeaf) ? (IsFree ? "Free" : $"Branch at height {Height}, children: {Child1} and {Child2}") : ((Height == 0) ? $"Leaf: {UserData}" : $"Leaf (invalid height of {Height}): {UserData}"));
		}
	}

	public delegate bool TreeQueryCallback(Proxy proxyId, T userData);

	private struct RebuildItem
	{
		public Proxy NodeIndex;

		public int ChildCount;

		public int StartIndex;

		public int SplitIndex;

		public int EndIndex;
	}

	public delegate void FastQueryCallback(ref T userData);

	internal delegate float RayCallback(RayCastInput input, T context, ref WorldRayCastContext state);

	internal delegate float TreeShapeCastCallback(ShapeCastInput input, T userData, ref WorldRayCastContext state);

	public const int B2_Bin_Count = 8;

	private const int TreeStackSize = 1024;

	private Node[] _nodes;

	private Proxy _root;

	private Proxy _freeList;

	public int ProxyCount;

	public Proxy[] LeafIndices = Array.Empty<Proxy>();

	public Box2[] LeafBoxes = Array.Empty<Box2>();

	public Vector2[] LeafCenters = Array.Empty<Vector2>();

	public int[] BinIndices = Array.Empty<int>();

	public int RebuildCapacity;

	private static readonly QueryCallback<QueryCallback> EasyQueryCallback = delegate(ref QueryCallback callback, Proxy proxy)
	{
		return callback(proxy);
	};

	private static readonly RayQueryCallback<RayQueryCallback> EasyRayQueryCallback = delegate(ref RayQueryCallback callback, Proxy proxy, in Vector2 hitPos, float distance)
	{
		return callback(proxy, in hitPos, distance);
	};

	public int NodeCount { get; private set; }

	public int Capacity => _nodes.Length;

	public int Height
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (!(_root == Proxy.Free))
			{
				return _nodes[(int)_root].Height;
			}
			return 0;
		}
	}

	public int MaxBalance
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			int num = 0;
			for (int i = 0; i < Capacity; i++)
			{
				ref Node reference = ref _nodes[i];
				if (reference.Height > 1)
				{
					ref Node reference2 = ref _nodes[(int)reference.Child1];
					int val = Math.Abs(_nodes[(int)reference.Child2].Height - reference2.Height);
					num = Math.Max(num, val);
				}
			}
			return num;
		}
	}

	public float AreaRatio
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (_root == Proxy.Free)
			{
				return 0f;
			}
			float num = Box2.Perimeter(ref _nodes[(int)_root].Aabb);
			float num2 = 0f;
			for (int i = 0; i < Capacity; i++)
			{
				ref Node reference = ref _nodes[i];
				if (reference.Height >= 0 && !reference.IsLeaf && i != (int)_root)
				{
					num2 += Box2.Perimeter(ref reference.Aabb);
				}
			}
			return num2 / num;
		}
	}

	private IEnumerable<(Proxy, Node)> DebugAllocatedNodesEnumerable
	{
		get
		{
			for (int i = 0; i < _nodes.Length; i++)
			{
				Node item = _nodes[i];
				if (!item.IsFree)
				{
					yield return ((Proxy)i, item);
				}
			}
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	private (Proxy, Node)[] DebugAllocatedNodes
	{
		get
		{
			(Proxy, Node)[] array = new(Proxy, Node)[NodeCount];
			int num = 0;
			foreach (var item in DebugAllocatedNodesEnumerable)
			{
				array[num++] = item;
			}
			return array;
		}
	}

	public B2DynamicTree(float aabbExtendSize = 1f / 32f, int capacity = 256, Func<int, int>? growthFunc = null)
		: base(aabbExtendSize, growthFunc)
	{
		capacity = Math.Max(16, capacity);
		_root = Proxy.Free;
		_nodes = new Node[capacity];
		ref Node reference = ref _nodes[0];
		int num = Capacity - 1;
		int num2 = 0;
		while (num2 < num)
		{
			reference.Next = (Proxy)(num2 + 1);
			reference.Height = -1;
			num2++;
			reference = ref Unsafe.Add(ref reference, 1);
		}
		ref Node reference2 = ref _nodes[^1];
		reference2.Next = Proxy.Free;
		reference2.Height = -1;
	}

	public Box2? GetFatAabb(Proxy proxy)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return _nodes[(int)proxy].Aabb;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ref Node AllocateNode(out Proxy proxy)
	{
		if (_freeList == Proxy.Free)
		{
			Expand();
		}
		Proxy freeList = _freeList;
		ref Node reference = ref _nodes[(int)freeList];
		_freeList = reference.Next;
		reference = default(Node);
		int nodeCount = NodeCount + 1;
		NodeCount = nodeCount;
		proxy = freeList;
		return ref reference;
		void Expand()
		{
			int num = GrowthFunc(Capacity);
			if (num <= Capacity)
			{
				throw new InvalidOperationException("Growth function returned invalid new capacity, must be greater than current capacity.");
			}
			Array.Resize(ref _nodes, num);
			int num2 = _nodes.Length - 1;
			ref Node reference2 = ref _nodes[NodeCount];
			int num3 = NodeCount;
			while (num3 < num2)
			{
				reference2.Next = (Proxy)(num3 + 1);
				reference2.Height = -1;
				num3++;
				reference2 = ref Unsafe.Add(ref reference2, 1);
			}
			ref Node reference3 = ref _nodes[num2];
			reference3.Next = Proxy.Free;
			reference3.Height = -1;
			_freeList = (Proxy)NodeCount;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void FreeNode(Proxy proxy)
	{
		ref Node reference = ref _nodes[(int)proxy];
		reference.Next = _freeList;
		reference.Height = -1;
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			reference.UserData = default(T);
		}
		_freeList = proxy;
		int nodeCount = NodeCount - 1;
		NodeCount = nodeCount;
	}

	public Proxy FindBestSibling(Box2 boxD)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		Vector2 center = ((Box2)(ref boxD)).Center;
		float num = Box2.Perimeter(ref boxD);
		Node[] nodes = _nodes;
		Proxy root = _root;
		ref Box2 aabb = ref nodes[(int)root].Aabb;
		float num2 = Box2.Perimeter(ref aabb);
		Box2 val = ((Box2)(ref aabb)).Union(ref boxD);
		float num3 = Box2.Perimeter(ref val);
		float num4 = 0f;
		Proxy result = root;
		float num5 = num3;
		Proxy proxy = root;
		while (nodes[(int)proxy].Height > 0)
		{
			ref Proxy child = ref nodes[(int)proxy].Child1;
			ref Proxy child2 = ref nodes[(int)proxy].Child2;
			float num6 = num3 + num4;
			if (num6 < num5)
			{
				result = proxy;
				num5 = num6;
			}
			num4 += num3 - num2;
			bool flag = nodes[(int)child].Height == 0;
			bool flag2 = nodes[(int)child2].Height == 0;
			float num7 = float.MaxValue;
			Box2 aabb2 = nodes[(int)child].Aabb;
			val = ((Box2)(ref aabb2)).Union(ref boxD);
			float num8 = Box2.Perimeter(ref val);
			float num9 = 0f;
			if (flag)
			{
				float num10 = num8 + num4;
				if (num10 < num5)
				{
					result = child;
					num5 = num10;
				}
			}
			else
			{
				num9 = Box2.Perimeter(ref aabb2);
				num7 = num4 + num8 + MathF.Min(num - num9, 0f);
			}
			float num11 = float.MaxValue;
			Box2 aabb3 = nodes[(int)child2].Aabb;
			val = ((Box2)(ref aabb3)).Union(ref boxD);
			float num12 = Box2.Perimeter(ref val);
			float num13 = 0f;
			if (flag2)
			{
				float num14 = num12 + num4;
				if (num14 < num5)
				{
					result = child2;
					num5 = num14;
				}
			}
			else
			{
				num13 = Box2.Perimeter(ref aabb3);
				num11 = num4 + num12 + MathF.Min(num - num13, 0f);
			}
			if ((flag && flag2) || (num5 <= num7 && num5 <= num11))
			{
				break;
			}
			if (num7 == num11 && !flag)
			{
				Vector2 vector = Vector2.Subtract(((Box2)(ref aabb2)).Center, center);
				Vector2 vector2 = Vector2.Subtract(((Box2)(ref aabb3)).Center, center);
				num7 = vector.LengthSquared();
				num11 = vector2.LengthSquared();
			}
			if (num7 < num11 && !flag)
			{
				proxy = child;
				num2 = num9;
				num3 = num8;
			}
			else
			{
				proxy = child2;
				num2 = num13;
				num3 = num12;
			}
		}
		return result;
	}

	public void RotateNodes(Proxy iA)
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0776: Unknown result type (might be due to invalid IL or missing references)
		//IL_0778: Unknown result type (might be due to invalid IL or missing references)
		//IL_082f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0831: Unknown result type (might be due to invalid IL or missing references)
		ref Node reference = ref _nodes[(int)iA];
		if (reference.Height < 2)
		{
			return;
		}
		Proxy child = reference.Child1;
		Proxy child2 = reference.Child2;
		ref Node reference2 = ref _nodes[(int)child];
		ref Node reference3 = ref _nodes[(int)child2];
		if (reference2.Height == 0)
		{
			Proxy child3 = reference3.Child1;
			Proxy child4 = reference3.Child2;
			ref Node reference4 = ref _nodes[(int)child3];
			ref Node reference5 = ref _nodes[(int)child4];
			float num = Box2.Perimeter(ref reference3.Aabb);
			Box2 aabb = ((Box2)(ref reference2.Aabb)).Union(ref reference5.Aabb);
			float num2 = Box2.Perimeter(ref aabb);
			Box2 aabb2 = ((Box2)(ref reference2.Aabb)).Union(ref reference4.Aabb);
			float num3 = Box2.Perimeter(ref aabb2);
			if (!(num < num2) || !(num < num3))
			{
				if (num2 < num3)
				{
					reference.Child1 = child3;
					reference3.Child1 = child;
					reference2.Parent = child2;
					reference4.Parent = iA;
					reference3.Aabb = aabb;
					reference3.Height = (short)(1 + Math.Max(reference2.Height, reference5.Height));
					reference.Height = (short)(1 + Math.Max(reference3.Height, reference4.Height));
					reference3.CategoryBits = reference2.CategoryBits | reference5.CategoryBits;
					reference.CategoryBits = reference3.CategoryBits | reference4.CategoryBits;
					reference3.Enlarged = reference2.Enlarged || reference5.Enlarged;
					reference.Enlarged = reference3.Enlarged || reference4.Enlarged;
				}
				else
				{
					reference.Child1 = child4;
					reference3.Child2 = child;
					reference2.Parent = child2;
					reference5.Parent = iA;
					reference3.Aabb = aabb2;
					reference3.Height = (short)(1 + Math.Max(reference2.Height, reference4.Height));
					reference.Height = (short)(1 + Math.Max(reference3.Height, reference5.Height));
					reference3.CategoryBits = reference2.CategoryBits | reference4.CategoryBits;
					reference.CategoryBits = reference3.CategoryBits | reference5.CategoryBits;
					reference3.Enlarged = reference2.Enlarged || reference4.Enlarged;
					reference.Enlarged = reference3.Enlarged || reference5.Enlarged;
				}
			}
			return;
		}
		if (reference3.Height == 0)
		{
			Proxy child5 = reference2.Child1;
			Proxy child6 = reference2.Child2;
			ref Node reference6 = ref _nodes[(int)child5];
			ref Node reference7 = ref _nodes[(int)child6];
			float num4 = Box2.Perimeter(ref reference2.Aabb);
			Box2 aabb3 = ((Box2)(ref reference3.Aabb)).Union(ref reference7.Aabb);
			float num5 = Box2.Perimeter(ref aabb3);
			Box2 aabb4 = ((Box2)(ref reference3.Aabb)).Union(ref reference6.Aabb);
			float num6 = Box2.Perimeter(ref aabb4);
			if (!(num4 < num5) || !(num4 < num6))
			{
				if (num5 < num6)
				{
					reference.Child2 = child5;
					reference2.Child1 = child2;
					reference3.Parent = child;
					reference6.Parent = iA;
					reference2.Aabb = aabb3;
					reference2.Height = (short)(1 + Math.Max(reference3.Height, reference7.Height));
					reference.Height = (short)(1 + Math.Max(reference2.Height, reference6.Height));
					reference2.CategoryBits = reference3.CategoryBits | reference7.CategoryBits;
					reference.CategoryBits = reference2.CategoryBits | reference6.CategoryBits;
					reference2.Enlarged = reference3.Enlarged || reference7.Enlarged;
					reference.Enlarged = reference2.Enlarged || reference6.Enlarged;
				}
				else
				{
					reference.Child2 = child6;
					reference2.Child2 = child2;
					reference3.Parent = child;
					reference7.Parent = iA;
					reference2.Aabb = aabb4;
					reference2.Height = (short)(1 + Math.Max(reference3.Height, reference6.Height));
					reference.Height = (short)(1 + Math.Max(reference2.Height, reference7.Height));
					reference2.CategoryBits = reference3.CategoryBits | reference6.CategoryBits;
					reference.CategoryBits = reference2.CategoryBits | reference7.CategoryBits;
					reference2.Enlarged = reference3.Enlarged || reference6.Enlarged;
					reference.Enlarged = reference2.Enlarged || reference7.Enlarged;
				}
			}
			return;
		}
		Proxy child7 = reference2.Child1;
		Proxy child8 = reference2.Child2;
		Proxy child9 = reference3.Child1;
		Proxy child10 = reference3.Child2;
		ref Node reference8 = ref _nodes[(int)child7];
		ref Node reference9 = ref _nodes[(int)child8];
		ref Node reference10 = ref _nodes[(int)child9];
		ref Node reference11 = ref _nodes[(int)child10];
		float num7 = Box2.Perimeter(ref reference2.Aabb);
		float num8 = Box2.Perimeter(ref reference3.Aabb);
		float num9 = num7 + num8;
		RotateType rotateType = RotateType.None;
		float num10 = num9;
		Box2 aabb5 = ((Box2)(ref reference2.Aabb)).Union(ref reference11.Aabb);
		float num11 = num7 + Box2.Perimeter(ref aabb5);
		if (num11 < num10)
		{
			rotateType = RotateType.BF;
			num10 = num11;
		}
		Box2 aabb6 = ((Box2)(ref reference2.Aabb)).Union(ref reference10.Aabb);
		float num12 = num7 + Box2.Perimeter(ref aabb6);
		if (num12 < num10)
		{
			rotateType = RotateType.BG;
			num10 = num12;
		}
		Box2 aabb7 = ((Box2)(ref reference3.Aabb)).Union(ref reference9.Aabb);
		float num13 = num8 + Box2.Perimeter(ref aabb7);
		if (num13 < num10)
		{
			rotateType = RotateType.CD;
			num10 = num13;
		}
		Box2 aabb8 = ((Box2)(ref reference3.Aabb)).Union(ref reference8.Aabb);
		if (num8 + Box2.Perimeter(ref aabb8) < num10)
		{
			rotateType = RotateType.CE;
		}
		switch (rotateType)
		{
		case RotateType.BF:
			reference.Child1 = child9;
			reference3.Child1 = child;
			reference2.Parent = child2;
			reference10.Parent = iA;
			reference3.Aabb = aabb5;
			reference3.Height = (short)(1 + Math.Max(reference2.Height, reference11.Height));
			reference.Height = (short)(1 + Math.Max(reference3.Height, reference10.Height));
			reference3.CategoryBits = reference2.CategoryBits | reference11.CategoryBits;
			reference.CategoryBits = reference3.CategoryBits | reference10.CategoryBits;
			reference3.Enlarged = reference2.Enlarged || reference11.Enlarged;
			reference.Enlarged = reference3.Enlarged || reference10.Enlarged;
			break;
		case RotateType.BG:
			reference.Child1 = child10;
			reference3.Child2 = child;
			reference2.Parent = child2;
			reference11.Parent = iA;
			reference3.Aabb = aabb6;
			reference3.Height = (short)(1 + Math.Max(reference2.Height, reference10.Height));
			reference.Height = (short)(1 + Math.Max(reference3.Height, reference11.Height));
			reference3.CategoryBits = reference2.CategoryBits | reference10.CategoryBits;
			reference.CategoryBits = reference3.CategoryBits | reference11.CategoryBits;
			reference3.Enlarged = reference2.Enlarged || reference10.Enlarged;
			reference.Enlarged = reference3.Enlarged || reference11.Enlarged;
			break;
		case RotateType.CD:
			reference.Child2 = child7;
			reference2.Child1 = child2;
			reference3.Parent = child;
			reference8.Parent = iA;
			reference2.Aabb = aabb7;
			reference2.Height = (short)(1 + Math.Max(reference3.Height, reference9.Height));
			reference.Height = (short)(1 + Math.Max(reference2.Height, reference8.Height));
			reference2.CategoryBits = reference3.CategoryBits | reference9.CategoryBits;
			reference.CategoryBits = reference2.CategoryBits | reference8.CategoryBits;
			reference2.Enlarged = reference3.Enlarged || reference9.Enlarged;
			reference.Enlarged = reference2.Enlarged || reference8.Enlarged;
			break;
		case RotateType.CE:
			reference.Child2 = child8;
			reference2.Child2 = child2;
			reference3.Parent = child;
			reference9.Parent = iA;
			reference2.Aabb = aabb8;
			reference2.Height = (short)(1 + Math.Max(reference3.Height, reference8.Height));
			reference.Height = (short)(1 + Math.Max(reference2.Height, reference9.Height));
			reference2.CategoryBits = reference3.CategoryBits | reference8.CategoryBits;
			reference.CategoryBits = reference2.CategoryBits | reference9.CategoryBits;
			reference2.Enlarged = reference3.Enlarged || reference8.Enlarged;
			reference.Enlarged = reference2.Enlarged || reference9.Enlarged;
			break;
		case RotateType.None:
			break;
		}
	}

	public Proxy CreateProxy(in Box2 aabb, uint categoryBits, T userData)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Proxy proxy;
		ref Node reference = ref AllocateNode(out proxy);
		reference.Aabb = aabb;
		reference.UserData = userData;
		reference.CategoryBits = categoryBits;
		reference.Height = 0;
		bool shouldRotate = true;
		InsertLeaf(proxy, shouldRotate);
		ProxyCount++;
		return proxy;
	}

	public void DestroyProxy(Proxy proxy)
	{
		RemoveLeaf(proxy);
		FreeNode(proxy);
		ProxyCount--;
	}

	public void MoveProxy(Proxy proxy, in Box2 aabb)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		RemoveLeaf(proxy);
		_nodes[(int)proxy].Aabb = aabb;
		bool shouldRotate = false;
		InsertLeaf(proxy, shouldRotate);
	}

	public void EnlargeProxy(Proxy proxy, Box2 aabb)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Node[] nodes = _nodes;
		ref Node reference = ref _nodes[(int)proxy];
		reference.Aabb = aabb;
		Proxy parent = reference.Parent;
		while (parent != Proxy.Free)
		{
			ref Node reference2 = ref nodes[(int)parent];
			bool flag = ((Box2)(ref reference2.Aabb)).EnlargeAabb(aabb);
			reference2.Enlarged = true;
			parent = reference2.Parent;
			if (!flag)
			{
				break;
			}
		}
		while (parent != Proxy.Free)
		{
			ref Node reference3 = ref nodes[(int)parent];
			if (!reference3.Enlarged)
			{
				reference3.Enlarged = true;
				parent = reference3.Parent;
				continue;
			}
			break;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: MaybeNull]
	public T GetUserData(Proxy proxy)
	{
		return _nodes[(int)proxy].UserData;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void RemoveLeaf(Proxy leaf)
	{
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		if (leaf == _root)
		{
			_root = Proxy.Free;
			return;
		}
		Node[] nodes = _nodes;
		Proxy parent = nodes[(int)leaf].Parent;
		Proxy parent2 = nodes[(int)parent].Parent;
		Proxy proxy = ((!(nodes[(int)parent].Child1 == leaf)) ? nodes[(int)parent].Child1 : nodes[(int)parent].Child2);
		if (parent2 != Proxy.Free)
		{
			if (nodes[(int)parent2].Child1 == parent)
			{
				nodes[(int)parent2].Child1 = proxy;
			}
			else
			{
				nodes[(int)parent2].Child2 = proxy;
			}
			nodes[(int)proxy].Parent = parent2;
			FreeNode(parent);
			Proxy proxy2 = parent2;
			while (proxy2 != Proxy.Free)
			{
				ref Node reference = ref nodes[(int)proxy2];
				ref Node reference2 = ref nodes[(int)reference.Child1];
				ref Node reference3 = ref nodes[(int)reference.Child2];
				reference.Aabb = ((Box2)(ref reference2.Aabb)).Union(ref reference3.Aabb);
				reference.CategoryBits = reference2.CategoryBits | reference3.CategoryBits;
				reference.Height = (short)(1 + Math.Max(reference2.Height, reference3.Height));
				proxy2 = reference.Parent;
			}
		}
		else
		{
			_root = proxy;
			_nodes[(int)proxy].Parent = Proxy.Free;
			FreeNode(parent);
		}
	}

	private void InsertLeaf(Proxy leaf, bool shouldRotate)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		if (_root == Proxy.Free)
		{
			_root = leaf;
			_nodes[(int)_root].Parent = Proxy.Free;
			return;
		}
		ref Box2 aabb = ref _nodes[(int)leaf].Aabb;
		Proxy proxy = FindBestSibling(aabb);
		ref Proxy parent = ref _nodes[(int)proxy].Parent;
		Proxy proxy2;
		ref Node reference = ref AllocateNode(out proxy2);
		Node[] nodes = _nodes;
		reference.Parent = parent;
		reference.UserData = default(T);
		reference.Aabb = ((Box2)(ref aabb)).Union(ref nodes[(int)proxy].Aabb);
		reference.CategoryBits = nodes[(int)leaf].CategoryBits | nodes[(int)proxy].CategoryBits;
		reference.Height = (short)(nodes[(int)proxy].Height + 1);
		if (parent != Proxy.Free)
		{
			if (nodes[(int)parent].Child1 == proxy)
			{
				nodes[(int)parent].Child1 = proxy2;
			}
			else
			{
				nodes[(int)parent].Child2 = proxy2;
			}
			reference.Child1 = proxy;
			reference.Child2 = leaf;
			nodes[(int)proxy].Parent = proxy2;
			nodes[(int)leaf].Parent = proxy2;
		}
		else
		{
			reference.Child1 = proxy;
			reference.Child2 = leaf;
			nodes[(int)proxy].Parent = proxy2;
			nodes[(int)leaf].Parent = proxy2;
			_root = proxy2;
		}
		Proxy parent2 = nodes[(int)leaf].Parent;
		while (parent2 != Proxy.Free)
		{
			ref Node reference2 = ref nodes[(int)parent2];
			Proxy child = reference2.Child1;
			Proxy child2 = reference2.Child2;
			ref Node reference3 = ref nodes[(int)child];
			ref Node reference4 = ref nodes[(int)child2];
			reference2.Aabb = ((Box2)(ref reference3.Aabb)).Union(ref reference4.Aabb);
			reference2.CategoryBits = reference3.CategoryBits | reference4.CategoryBits;
			reference2.Height = (short)(1 + Math.Max(reference3.Height, reference4.Height));
			reference2.Enlarged = reference3.Enlarged || reference4.Enlarged;
			if (shouldRotate)
			{
				RotateNodes(parent2);
			}
			parent2 = reference2.Parent;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float EstimateCost(in Box2 baseAabb, in Node node)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Box2 val = ((Box2)(ref baseAabb)).Union(ref node.Aabb);
		float num = Box2.Perimeter(ref val);
		if (!node.IsLeaf)
		{
			num -= Box2.Perimeter(ref node.Aabb);
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void Balance(Proxy index)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		while (index != Proxy.Free)
		{
			index = BalanceStep(index);
			ref Node reference = ref _nodes[(int)index];
			Proxy child = reference.Child1;
			Proxy child2 = reference.Child2;
			ref Node reference2 = ref _nodes[(int)child];
			ref Node reference3 = ref _nodes[(int)child2];
			reference.Height = (short)(Math.Max(reference2.Height, reference3.Height) + 1);
			reference.Aabb = ((Box2)(ref reference2.Aabb)).Union(ref reference3.Aabb);
			if (index == reference.Parent)
			{
				throw new Exception("Infinite loop in B2DynamicTree.Balance(). Trace: " + Environment.StackTrace);
			}
			index = reference.Parent;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Proxy BalanceStep(Proxy iA)
	{
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		ref Node source = ref _nodes[0];
		ref Node reference = ref Unsafe.Add(ref source, iA);
		if (reference.IsLeaf || reference.Height < 2)
		{
			return iA;
		}
		Proxy child = reference.Child1;
		Proxy child2 = reference.Child2;
		ref Node reference2 = ref Unsafe.Add(ref source, child);
		ref Node reference3 = ref Unsafe.Add(ref source, child2);
		int num = reference3.Height - reference2.Height;
		if (num > 1)
		{
			Proxy child3 = reference3.Child1;
			Proxy child4 = reference3.Child2;
			ref Node reference4 = ref Unsafe.Add(ref source, child3);
			ref Node reference5 = ref Unsafe.Add(ref source, child4);
			reference3.Child1 = iA;
			reference3.Parent = reference.Parent;
			reference.Parent = child2;
			if (reference3.Parent == Proxy.Free)
			{
				_root = child2;
			}
			else
			{
				ref Node reference6 = ref Unsafe.Add(ref source, reference3.Parent);
				if (reference6.Child1 == iA)
				{
					reference6.Child1 = child2;
				}
				else
				{
					reference6.Child2 = child2;
				}
			}
			if (reference4.Height > reference5.Height)
			{
				reference3.Child2 = child3;
				reference.Child2 = child4;
				reference5.Parent = iA;
				reference.Aabb = ((Box2)(ref reference2.Aabb)).Union(ref reference5.Aabb);
				reference3.Aabb = ((Box2)(ref reference.Aabb)).Union(ref reference4.Aabb);
				reference.Height = (short)(Math.Max(reference2.Height, reference5.Height) + 1);
				reference3.Height = (short)(Math.Max(reference.Height, reference4.Height) + 1);
			}
			else
			{
				reference3.Child2 = child4;
				reference.Child2 = child3;
				reference4.Parent = iA;
				reference.Aabb = ((Box2)(ref reference2.Aabb)).Union(ref reference4.Aabb);
				reference3.Aabb = ((Box2)(ref reference.Aabb)).Union(ref reference5.Aabb);
				reference.Height = (short)(Math.Max(reference2.Height, reference4.Height) + 1);
				reference3.Height = (short)(Math.Max(reference.Height, reference5.Height) + 1);
			}
			return child2;
		}
		if (num < -1)
		{
			Proxy child5 = reference2.Child1;
			Proxy child6 = reference2.Child2;
			ref Node reference7 = ref Unsafe.Add(ref source, child5);
			ref Node reference8 = ref Unsafe.Add(ref source, child6);
			reference2.Child1 = iA;
			reference2.Parent = reference.Parent;
			reference.Parent = child;
			if (reference2.Parent == Proxy.Free)
			{
				_root = child;
			}
			else
			{
				ref Node reference9 = ref Unsafe.Add(ref source, reference2.Parent);
				if (reference9.Child1 == iA)
				{
					reference9.Child1 = child;
				}
				else
				{
					reference9.Child2 = child;
				}
			}
			if (reference7.Height > reference8.Height)
			{
				reference2.Child2 = child5;
				reference.Child1 = child6;
				reference8.Parent = iA;
				reference.Aabb = ((Box2)(ref reference3.Aabb)).Union(ref reference8.Aabb);
				reference2.Aabb = ((Box2)(ref reference.Aabb)).Union(ref reference7.Aabb);
				reference.Height = (short)(Math.Max(reference3.Height, reference8.Height) + 1);
				reference2.Height = (short)(Math.Max(reference.Height, reference7.Height) + 1);
			}
			else
			{
				reference2.Child2 = child6;
				reference.Child1 = child5;
				reference7.Parent = iA;
				reference.Aabb = ((Box2)(ref reference3.Aabb)).Union(ref reference7.Aabb);
				reference2.Aabb = ((Box2)(ref reference.Aabb)).Union(ref reference8.Aabb);
				reference.Height = (short)(Math.Max(reference3.Height, reference7.Height) + 1);
				reference2.Height = (short)(Math.Max(reference.Height, reference8.Height) + 1);
			}
			return child;
		}
		return iA;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int ComputeHeight()
	{
		return ComputeHeight(_root);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private int ComputeHeight(Proxy proxy)
	{
		ref Node reference = ref _nodes[(int)proxy];
		if (reference.IsLeaf)
		{
			return 0;
		}
		return Math.Max(ComputeHeight(reference.Child1), ComputeHeight(reference.Child2)) + 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void RebuildBottomUp()
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		Span<Proxy> span = stackalloc Proxy[Capacity];
		int num = 0;
		for (int i = 0; i < Capacity; i++)
		{
			ref Node reference = ref _nodes[i];
			if (reference.Height >= 0)
			{
				if (reference.IsLeaf)
				{
					reference.Parent = Proxy.Free;
					span[num] = new Proxy(i);
					num++;
				}
				else
				{
					FreeNode(new Proxy(i));
				}
			}
		}
		while (num > 1)
		{
			float num2 = float.MaxValue;
			int index = -1;
			int index2 = -1;
			for (int j = 0; j < num; j++)
			{
				Box2 aabb = _nodes[(int)span[j]].Aabb;
				for (int k = j + 1; k < num; k++)
				{
					Box2 aabb2 = _nodes[(int)span[k]].Aabb;
					Box2 val = ((Box2)(ref aabb)).Union(ref aabb2);
					float num3 = Box2.Perimeter(ref val);
					if (num3 < num2)
					{
						index = j;
						index2 = k;
						num2 = num3;
					}
				}
			}
			Proxy proxy = span[index];
			Proxy proxy2 = span[index2];
			ref Node reference2 = ref _nodes[(int)proxy];
			ref Node reference3 = ref _nodes[(int)proxy2];
			Proxy proxy3;
			ref Node reference4 = ref AllocateNode(out proxy3);
			reference4.Child1 = proxy;
			reference4.Child2 = proxy2;
			reference4.Aabb = ((Box2)(ref reference2.Aabb)).Union(ref reference3.Aabb);
			reference4.CategoryBits = reference2.CategoryBits | reference3.CategoryBits;
			reference4.Height = (short)(1 + Math.Max(reference2.Height, reference3.Height));
			reference4.Parent = Proxy.Free;
			reference2.Parent = proxy3;
			reference3.Parent = proxy3;
			span[index2] = span[num - 1];
			span[index] = proxy3;
			num--;
		}
		_root = span[0];
	}

	public void ShiftOrigin(Vector2 newOrigin)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < _nodes.Length; i++)
		{
			ref Node reference = ref _nodes[i];
			Vector2 bottomLeft = reference.Aabb.BottomLeft;
			Vector2 topRight = reference.Aabb.TopRight;
			reference.Aabb = new Box2(bottomLeft - newOrigin, topRight - newOrigin);
		}
	}

	public void Query(Box2 aabb, uint maskBits, TreeQueryCallback callback)
	{
		Span<Proxy> stackSpace = stackalloc Proxy[1024];
		GrowableStack<Proxy> growableStack = new GrowableStack<Proxy>(stackSpace);
		growableStack.Push(in _root);
		ref Node source = ref _nodes[0];
		while (growableStack.GetCount() > 0)
		{
			Proxy proxy = growableStack.Pop();
			if (proxy == Proxy.Free)
			{
				continue;
			}
			ref Node reference = ref Unsafe.Add(ref source, proxy);
			if (!((Box2)(ref reference.Aabb)).Intersects(ref aabb) || (reference.CategoryBits & maskBits) == 0)
			{
				continue;
			}
			if (reference.IsLeaf)
			{
				if (!callback(proxy, reference.UserData))
				{
					break;
				}
			}
			else if (growableStack.GetCount() < 1023)
			{
				growableStack.Push(in reference.Child1);
				growableStack.Push(in reference.Child2);
			}
		}
	}

	public int PartitionMid(Proxy[] indices, Vector2[] centers, int count)
	{
		if (count <= 2)
		{
			return count / 2;
		}
		ref Vector2 reference = ref centers[0];
		Vector2 vector = reference;
		Vector2 vector2 = reference;
		for (int i = 1; i < count; i++)
		{
			Vector2 value = Unsafe.Add(ref reference, i);
			vector = Vector2.Min(vector, value);
			vector2 = Vector2.Max(vector2, value);
		}
		Vector2 vector3 = Vector2.Subtract(vector2, vector);
		Vector2 vector4 = new Vector2(0.5f * (vector.X + vector2.X), 0.5f * (vector.Y + vector2.Y));
		int j = 0;
		int num = count;
		if (vector3.X > vector3.Y)
		{
			float x = vector4.X;
			while (j < num)
			{
				for (; j < num && centers[j].X < x; j++)
				{
				}
				while (j < num && centers[num - 1].X >= x)
				{
					num--;
				}
				if (j < num)
				{
					ref Proxy reference2 = ref indices[j];
					ref Proxy reference3 = ref indices[num - 1];
					Proxy proxy = indices[num - 1];
					Proxy proxy2 = indices[j];
					reference2 = proxy;
					reference3 = proxy2;
					ref Vector2 reference4 = ref centers[j];
					ref Vector2 reference5 = ref centers[num - 1];
					Vector2 vector5 = centers[num - 1];
					Vector2 vector6 = centers[j];
					reference4 = vector5;
					reference5 = vector6;
					j++;
					num--;
				}
			}
		}
		else
		{
			float y = vector4.Y;
			while (j < num)
			{
				for (; j < num && centers[j].Y < y; j++)
				{
				}
				while (j < num && centers[num - 1].Y >= y)
				{
					num--;
				}
				if (j < num)
				{
					ref Proxy reference2 = ref indices[j];
					ref Proxy reference6 = ref indices[num - 1];
					Proxy proxy2 = indices[num - 1];
					Proxy proxy = indices[j];
					reference2 = proxy2;
					reference6 = proxy;
					ref Vector2 reference4 = ref centers[j];
					ref Vector2 reference7 = ref centers[num - 1];
					Vector2 vector6 = centers[num - 1];
					Vector2 vector5 = centers[j];
					reference4 = vector6;
					reference7 = vector5;
					j++;
					num--;
				}
			}
		}
		if (j > 0 && j < count)
		{
			return j;
		}
		return count / 2;
	}

	public Proxy BuildTree(int leafCount)
	{
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		Node[] nodes = _nodes;
		Proxy[] leafIndices = LeafIndices;
		if (leafCount == 1)
		{
			nodes[(int)leafIndices[0]].Parent = Proxy.Free;
			return leafIndices[0];
		}
		Vector2[] leafCenters = LeafCenters;
		Span<RebuildItem> stackSpace = stackalloc RebuildItem[1024];
		GrowableStack<RebuildItem> growableStack = new GrowableStack<RebuildItem>(stackSpace);
		int num = 0;
		AllocateNode(out var proxy);
		growableStack.Push(new RebuildItem
		{
			NodeIndex = proxy,
			ChildCount = -1,
			StartIndex = 0,
			EndIndex = leafCount,
			SplitIndex = PartitionMid(leafIndices, leafCenters, leafCount)
		});
		while (true)
		{
			ref RebuildItem reference = ref growableStack[num];
			reference.ChildCount++;
			if (reference.ChildCount == 2)
			{
				if (num == 0)
				{
					break;
				}
				ref RebuildItem reference2 = ref growableStack[num - 1];
				ref Node reference3 = ref nodes[(int)reference2.NodeIndex];
				if (reference2.ChildCount == 0)
				{
					reference3.Child1 = reference.NodeIndex;
				}
				else
				{
					reference3.Child2 = reference.NodeIndex;
				}
				ref Node reference4 = ref nodes[(int)reference.NodeIndex];
				reference4.Parent = reference2.NodeIndex;
				ref Node reference5 = ref nodes[(int)reference4.Child1];
				ref Node reference6 = ref nodes[(int)reference4.Child2];
				reference4.Aabb = Box2.Union(reference5.Aabb, reference6.Aabb);
				reference4.Height = (short)(1 + Math.Max(reference5.Height, reference6.Height));
				reference4.CategoryBits = reference5.CategoryBits | reference6.CategoryBits;
				num--;
				continue;
			}
			int num2;
			int num3;
			if (reference.ChildCount == 0)
			{
				num2 = reference.StartIndex;
				num3 = reference.SplitIndex;
			}
			else
			{
				num2 = reference.SplitIndex;
				num3 = reference.EndIndex;
			}
			int num4 = num3 - num2;
			if (num4 == 1)
			{
				Proxy proxy2 = leafIndices[num2];
				ref Node reference7 = ref nodes[(int)reference.NodeIndex];
				if (reference.ChildCount == 0)
				{
					reference7.Child1 = proxy2;
				}
				else
				{
					reference7.Child2 = proxy2;
				}
				nodes[(int)proxy2].Parent = reference.NodeIndex;
			}
			else
			{
				num++;
				ref RebuildItem reference8 = ref growableStack[num];
				AllocateNode(out var proxy3);
				reference8.NodeIndex = proxy3;
				reference8.ChildCount = -1;
				reference8.StartIndex = num2;
				reference8.EndIndex = num3;
				reference8.SplitIndex = PartitionMid(leafIndices[num2..], leafCenters[num2..], num4);
				reference8.SplitIndex += num2;
			}
		}
		ref Node reference9 = ref nodes[(int)growableStack[0].NodeIndex];
		ref Node reference10 = ref nodes[(int)reference9.Child1];
		ref Node reference11 = ref nodes[(int)reference9.Child2];
		reference9.Aabb = Box2.Union(reference10.Aabb, reference11.Aabb);
		reference9.Height = (short)(1 + Math.Max(reference10.Height, reference11.Height));
		reference9.CategoryBits = reference10.CategoryBits | reference11.CategoryBits;
		return growableStack[0].NodeIndex;
	}

	public int Rebuild(bool fullBuild)
	{
		int proxyCount = ProxyCount;
		if (proxyCount == 0)
		{
			return 0;
		}
		if (proxyCount > RebuildCapacity)
		{
			int num = proxyCount + proxyCount / 2;
			Array.Resize(ref LeafIndices, num);
			Array.Resize(ref LeafCenters, num);
			RebuildCapacity = num;
		}
		int num2 = 0;
		Span<Proxy> stackSpace = stackalloc Proxy[1024];
		GrowableStack<Proxy> growableStack = new GrowableStack<Proxy>(stackSpace);
		Proxy proxy = _root;
		ref Node reference = ref _nodes[0];
		Node node = reference;
		Proxy[] leafIndices = LeafIndices;
		Vector2[] leafCenters = LeafCenters;
		while (true)
		{
			if (node.Height == 0 || (!node.Enlarged && !fullBuild))
			{
				leafIndices[num2] = proxy;
				leafCenters[num2] = ((Box2)(ref node.Aabb)).Center;
				num2++;
				node.Parent = Proxy.Free;
				if (growableStack.GetCount() == 0)
				{
					break;
				}
				proxy = growableStack.Pop();
				node = Unsafe.Add(ref reference, proxy);
			}
			else
			{
				Proxy proxy2 = proxy;
				proxy = node.Child1;
				if (growableStack.GetCount() < 1024)
				{
					growableStack.Push(in node.Child2);
				}
				node = Unsafe.Add(ref reference, proxy);
				FreeNode(proxy2);
			}
		}
		_root = BuildTree(num2);
		return num2;
	}

	public void Query(QueryCallback callback, in Box2 aabb)
	{
		Query(ref callback, EasyQueryCallback, in aabb);
	}

	public void Query<TState>(ref TState state, QueryCallback<TState> callback, in Box2 aabb)
	{
		Span<Proxy> stackSpace = stackalloc Proxy[256];
		GrowableStack<Proxy> growableStack = new GrowableStack<Proxy>(stackSpace);
		growableStack.Push(in _root);
		ref Node source = ref _nodes[0];
		while (growableStack.GetCount() != 0)
		{
			Proxy proxy = growableStack.Pop();
			if (proxy == Proxy.Free)
			{
				continue;
			}
			ref Node reference = ref Unsafe.Add(ref source, proxy);
			if (!((Box2)(ref reference.Aabb)).Intersects(ref aabb))
			{
				continue;
			}
			if (reference.IsLeaf)
			{
				if (!callback(ref state, proxy))
				{
					break;
				}
			}
			else
			{
				growableStack.Push(in reference.Child1);
				growableStack.Push(in reference.Child2);
			}
		}
	}

	public void FastQuery(ref Box2 aabb, FastQueryCallback callback)
	{
		Span<Proxy> stackSpace = stackalloc Proxy[256];
		GrowableStack<Proxy> growableStack = new GrowableStack<Proxy>(stackSpace);
		growableStack.Push(in _root);
		ref Node source = ref _nodes[0];
		while (growableStack.GetCount() != 0)
		{
			Proxy proxy = growableStack.Pop();
			if (proxy == Proxy.Free)
			{
				continue;
			}
			ref Node reference = ref Unsafe.Add(ref source, proxy);
			if (((Box2)(ref reference.Aabb)).Intersects(ref aabb))
			{
				if (reference.IsLeaf)
				{
					callback(ref reference.UserData);
					continue;
				}
				growableStack.Push(in reference.Child1);
				growableStack.Push(in reference.Child2);
			}
		}
	}

	internal void RayCastNew(RayCastInput input, long mask, ref WorldRayCastContext state, RayCallback callback)
	{
		Vector2 origin = input.Origin;
		Vector2 translation = input.Translation;
		Vector2 vector = Vector2Helpers.Normalized(translation);
		Vector2 vector2 = Vector2Helpers.Cross(1f, ref vector);
		Vector2 value = Vector2.Abs(vector2);
		float num = input.MaxFraction;
		Vector2 value2 = Vector2.Add(origin, num * translation);
		Unsafe.SkipInit(out Box2 val);
		((Box2)(ref val))._002Ector(Vector2.Min(origin, value2), Vector2.Max(origin, value2));
		Span<Proxy> stackSpace = stackalloc Proxy[256];
		GrowableStack<Proxy> growableStack = new GrowableStack<Proxy>(stackSpace);
		ref Node source = ref _nodes[0];
		growableStack.Push(in _root);
		RayCastInput input2 = input;
		while (growableStack.GetCount() > 0)
		{
			Proxy proxy = growableStack.Pop();
			if (proxy == Proxy.Free)
			{
				continue;
			}
			Node node = Unsafe.Add(ref source, proxy);
			if (!((Box2)(ref node.Aabb)).Intersects(ref val))
			{
				continue;
			}
			Vector2 center = ((Box2)(ref node.Aabb)).Center;
			Vector2 extents = ((Box2)(ref node.Aabb)).Extents;
			float num2 = MathF.Abs(Vector2.Dot(vector2, Vector2.Subtract(origin, center)));
			if (Vector2.Dot(value, extents) < num2)
			{
				continue;
			}
			if (node.IsLeaf)
			{
				input2.MaxFraction = num;
				float num3 = callback(input2, node.UserData, ref state);
				if (num3 == 0f)
				{
					break;
				}
				if (0f < num3 && num3 < num)
				{
					num = num3;
					value2 = Vector2.Add(origin, num * translation);
					val.BottomLeft = Vector2.Min(origin, value2);
					val.TopRight = Vector2.Max(origin, value2);
				}
			}
			else if (growableStack.GetCount() < 255)
			{
				growableStack.Push(in node.Child1);
				growableStack.Push(in node.Child2);
			}
		}
	}

	internal void ShapeCast(ShapeCastInput input, long maskBits, TreeShapeCastCallback callback, ref WorldRayCastContext state)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		if (input.Count == 0)
		{
			return;
		}
		Unsafe.SkipInit(out Box2 val);
		((Box2)(ref val))._002Ector(input.Points[0], input.Points[0]);
		for (int i = 1; i < input.Count; i++)
		{
			val.BottomLeft = Vector2.Min(val.BottomLeft, input.Points[i]);
			val.TopRight = Vector2.Max(val.TopRight, input.Points[i]);
		}
		Vector2 right = new Vector2(input.Radius, input.Radius);
		val.BottomLeft = Vector2.Subtract(val.BottomLeft, right);
		val.TopRight = Vector2.Add(val.TopRight, right);
		Vector2 center = ((Box2)(ref val)).Center;
		Vector2 extents = ((Box2)(ref val)).Extents;
		Vector2 translation = input.Translation;
		Vector2 vector = Vector2Helpers.Cross(1f, ref translation);
		Vector2 value = Vector2.Abs(vector);
		float num = input.MaxFraction;
		Vector2 right2 = Vector2.Multiply(num, input.Translation);
		Unsafe.SkipInit(out Box2 val2);
		((Box2)(ref val2))._002Ector(Vector2.Min(val.BottomLeft, Vector2.Add(val.BottomLeft, right2)), Vector2.Max(val.TopRight, Vector2.Add(val.TopRight, right2)));
		ShapeCastInput input2 = input;
		ref Node source = ref _nodes[0];
		Span<Proxy> stackSpace = stackalloc Proxy[256];
		GrowableStack<Proxy> growableStack = new GrowableStack<Proxy>(stackSpace);
		growableStack.Push(in _root);
		while (growableStack.GetCount() > 0)
		{
			Proxy proxy = growableStack.Pop();
			if (proxy == Proxy.Free)
			{
				continue;
			}
			Node node = Unsafe.Add(ref source, proxy);
			if (!((Box2)(ref node.Aabb)).Intersects(ref val2))
			{
				continue;
			}
			Vector2 center2 = ((Box2)(ref node.Aabb)).Center;
			Vector2 value2 = Vector2.Add(((Box2)(ref node.Aabb)).Extents, extents);
			float num2 = MathF.Abs(Vector2.Dot(vector, Vector2.Subtract(center, center2)));
			if (Vector2.Dot(value, value2) < num2)
			{
				continue;
			}
			if (node.IsLeaf)
			{
				input2.MaxFraction = num;
				float num3 = callback(input2, node.UserData, ref state);
				if (num3 == 0f)
				{
					break;
				}
				if (0f < num3 && num3 < num)
				{
					num = num3;
					right2 = Vector2.Multiply(num, input.Translation);
					val2.BottomLeft = Vector2.Min(val.BottomLeft, Vector2.Add(val.BottomLeft, right2));
					val2.TopRight = Vector2.Max(val.TopRight, Vector2.Add(val.TopRight, right2));
				}
			}
			else if (growableStack.GetCount() < 255)
			{
				growableStack.Push(in node.Child1);
				growableStack.Push(in node.Child2);
			}
		}
	}

	public void RayCast(RayQueryCallback callback, in Ray input)
	{
		RayCast(ref callback, EasyRayQueryCallback, in input);
	}

	public void RayCast<TState>(ref TState state, RayQueryCallback<TState> callback, in Ray input)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		Span<Proxy> stackSpace = stackalloc Proxy[256];
		GrowableStack<Proxy> growableStack = new GrowableStack<Proxy>(stackSpace);
		growableStack.Push(in _root);
		ref Node source = ref _nodes[0];
		while (growableStack.GetCount() > 0)
		{
			Proxy proxy = growableStack.Pop();
			if (proxy == Proxy.Free)
			{
				continue;
			}
			ref Node reference = ref Unsafe.Add(ref source, proxy);
			if (!input.Intersects(reference.Aabb, out var distance, out var hitPos))
			{
				continue;
			}
			if (reference.IsLeaf)
			{
				if (!callback(ref state, proxy, in hitPos, distance))
				{
					break;
				}
				continue;
			}
			if (reference.Child1 != Proxy.Free)
			{
				growableStack.Push(in reference.Child1);
			}
			if (reference.Child2 != Proxy.Free)
			{
				growableStack.Push(in reference.Child2);
			}
		}
	}

	[Conditional("DEBUG_DYNAMIC_TREE")]
	public void ValidateStructure(Proxy proxy)
	{
		if (!(proxy == Proxy.Free))
		{
			_ = proxy == _root;
			ref Node reference = ref _nodes[(int)proxy];
			Proxy child = reference.Child1;
			Proxy child2 = reference.Child2;
			if (!reference.IsLeaf && !_nodes[(int)child].Enlarged)
			{
				_ = _nodes[(int)child2].Enlarged;
			}
		}
	}

	[Conditional("DEBUG_DYNAMIC_TREE")]
	public void ValidateMetrics(Proxy proxy)
	{
		if (!(proxy == Proxy.Free))
		{
			ref Node reference = ref _nodes[(int)proxy];
			Proxy child = reference.Child1;
			Proxy child2 = reference.Child2;
			if (!reference.IsLeaf)
			{
				short height = _nodes[(int)child].Height;
				short height2 = _nodes[(int)child2].Height;
				Math.Max(height, height2);
				_ = ref _nodes[(int)child];
				_ = ref _nodes[(int)child2];
			}
		}
	}

	[Conditional("DEBUG_DYNAMIC_TREE")]
	private void Validate()
	{
		if (!(_root == Proxy.Free))
		{
			int num = 0;
			Proxy proxy = _freeList;
			while (proxy != Proxy.Free)
			{
				proxy = _nodes[(int)proxy].Next;
				num++;
			}
			_ = Height;
			ComputeHeight();
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
}
