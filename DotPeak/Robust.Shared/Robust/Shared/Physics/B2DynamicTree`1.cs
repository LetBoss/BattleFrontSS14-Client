// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.B2DynamicTree`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Physics;

public sealed class B2DynamicTree<T> : DynamicTree
{
  public const int B2_Bin_Count = 8;
  private const int TreeStackSize = 1024 /*0x0400*/;
  private B2DynamicTree<
  #nullable disable
  T>.Node[] _nodes;
  private DynamicTree.Proxy _root;
  private DynamicTree.Proxy _freeList;
  public int ProxyCount;
  public 
  #nullable enable
  DynamicTree.Proxy[] LeafIndices = Array.Empty<DynamicTree.Proxy>();
  public Box2[] LeafBoxes = Array.Empty<Box2>();
  public Vector2[] LeafCenters = Array.Empty<Vector2>();
  public int[] BinIndices = Array.Empty<int>();
  public int RebuildCapacity;
  private static readonly B2DynamicTree<
  #nullable disable
  T>.QueryCallback<
  #nullable enable
  B2DynamicTree<
  #nullable disable
  T>.QueryCallback> EasyQueryCallback = (B2DynamicTree<T>.QueryCallback<B2DynamicTree<T>.QueryCallback>) ((ref 
  #nullable enable
  B2DynamicTree<
  #nullable disable
  T>.QueryCallback callback, DynamicTree.Proxy proxy) => callback(proxy));
  private static readonly 
  #nullable enable
  B2DynamicTree<
  #nullable disable
  T>.RayQueryCallback<
  #nullable enable
  B2DynamicTree<
  #nullable disable
  T>.RayQueryCallback> EasyRayQueryCallback = (B2DynamicTree<T>.RayQueryCallback<B2DynamicTree<T>.RayQueryCallback>) ((ref 
  #nullable enable
  B2DynamicTree<
  #nullable disable
  T>.RayQueryCallback callback, DynamicTree.Proxy proxy, in Vector2 hitPos, float distance) => callback(proxy, in hitPos, distance));

  public int NodeCount { get; private set; }

  public int Capacity => this._nodes.Length;

  public int Height
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)] get
    {
      return !(this._root == DynamicTree.Proxy.Free) ? (int) this._nodes[(int) this._root].Height : 0;
    }
  }

  public int MaxBalance
  {
    [MethodImpl(MethodImplOptions.NoInlining)] get
    {
      int val1 = 0;
      for (int index = 0; index < this.Capacity; ++index)
      {
        ref B2DynamicTree<T>.Node local1 = ref this._nodes[index];
        if (local1.Height > (short) 1)
        {
          ref B2DynamicTree<T>.Node local2 = ref this._nodes[(int) local1.Child1];
          int val2 = Math.Abs((int) this._nodes[(int) local1.Child2].Height - (int) local2.Height);
          val1 = Math.Max(val1, val2);
        }
      }
      return val1;
    }
  }

  public float AreaRatio
  {
    [MethodImpl(MethodImplOptions.NoInlining)] get
    {
      if (this._root == DynamicTree.Proxy.Free)
        return 0.0f;
      float num1 = Box2.Perimeter(ref this._nodes[(int) this._root].Aabb);
      float num2 = 0.0f;
      for (int index = 0; index < this.Capacity; ++index)
      {
        ref B2DynamicTree<T>.Node local = ref this._nodes[index];
        if (local.Height >= (short) 0 && !local.IsLeaf && index != (int) this._root)
          num2 += Box2.Perimeter(ref local.Aabb);
      }
      return num2 / num1;
    }
  }

  public B2DynamicTree(float aabbExtendSize = 0.03125f, int capacity = 256 /*0x0100*/, 
  #nullable enable
  Func<int, int>? growthFunc = null)
    : base(aabbExtendSize, growthFunc)
  {
    capacity = Math.Max(16 /*0x10*/, capacity);
    this._root = DynamicTree.Proxy.Free;
    this._nodes = new B2DynamicTree<T>.Node[capacity];
    ref B2DynamicTree<T>.Node local1 = ref this._nodes[0];
    int num1 = this.Capacity - 1;
    int num2 = 0;
    while (num2 < num1)
    {
      local1.Next = (DynamicTree.Proxy) (num2 + 1);
      local1.Height = (short) -1;
      ++num2;
      local1 = ref Unsafe.Add<B2DynamicTree<T>.Node>(ref local1, 1);
    }
    B2DynamicTree<T>.Node[] nodes = this._nodes;
    ref B2DynamicTree<T>.Node local2 = ref nodes[nodes.Length - 1];
    local2.Next = DynamicTree.Proxy.Free;
    local2.Height = (short) -1;
  }

  public Box2? GetFatAabb(DynamicTree.Proxy proxy) => new Box2?(this._nodes[(int) proxy].Aabb);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private unsafe ref B2DynamicTree<
  #nullable disable
  T>.Node AllocateNode(out DynamicTree.Proxy proxy)
  {
    if (this._freeList == DynamicTree.Proxy.Free)
      Expand();
    DynamicTree.Proxy freeList = this._freeList;
    ref B2DynamicTree<T>.Node local = ref this._nodes[(int) freeList];
    this._freeList = local.Next;
    *(B2DynamicTree<T>.Node*) ref local = new B2DynamicTree<T>.Node();
    ++this.NodeCount;
    proxy = freeList;
    return ref local;

    void Expand()
    {
      int newSize = this.GrowthFunc(this.Capacity);
      if (newSize <= this.Capacity)
        throw new InvalidOperationException("Growth function returned invalid new capacity, must be greater than current capacity.");
      Array.Resize<B2DynamicTree<T>.Node>(ref this._nodes, newSize);
      int index = this._nodes.Length - 1;
      ref B2DynamicTree<T>.Node local1 = ref this._nodes[this.NodeCount];
      int nodeCount = this.NodeCount;
      while (nodeCount < index)
      {
        local1.Next = (DynamicTree.Proxy) (nodeCount + 1);
        local1.Height = (short) -1;
        ++nodeCount;
        local1 = ref Unsafe.Add<B2DynamicTree<T>.Node>(ref local1, 1);
      }
      ref B2DynamicTree<T>.Node local2 = ref this._nodes[index];
      local2.Next = DynamicTree.Proxy.Free;
      local2.Height = (short) -1;
      this._freeList = (DynamicTree.Proxy) this.NodeCount;
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private void FreeNode(DynamicTree.Proxy proxy)
  {
    ref B2DynamicTree<T>.Node local = ref this._nodes[(int) proxy];
    local.Next = this._freeList;
    local.Height = (short) -1;
    if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
      local.UserData = default (T);
    this._freeList = proxy;
    --this.NodeCount;
  }

  public DynamicTree.Proxy FindBestSibling(Box2 boxD)
  {
    Vector2 center = ((Box2) ref boxD).Center;
    float num1 = Box2.Perimeter(ref boxD);
    B2DynamicTree<T>.Node[] nodes = this._nodes;
    DynamicTree.Proxy root = this._root;
    ref Box2 local1 = ref nodes[(int) root].Aabb;
    float num2 = Box2.Perimeter(ref local1);
    Box2 box2_1 = ((Box2) ref local1).Union(ref boxD);
    float num3 = Box2.Perimeter(ref box2_1);
    float num4 = 0.0f;
    DynamicTree.Proxy bestSibling = root;
    float num5 = num3;
    DynamicTree.Proxy index = root;
    while (nodes[(int) index].Height > (short) 0)
    {
      ref DynamicTree.Proxy local2 = ref nodes[(int) index].Child1;
      ref DynamicTree.Proxy local3 = ref nodes[(int) index].Child2;
      float num6 = num3 + num4;
      if ((double) num6 < (double) num5)
      {
        bestSibling = index;
        num5 = num6;
      }
      num4 += num3 - num2;
      bool flag1 = nodes[(int) local2].Height == (short) 0;
      bool flag2 = nodes[(int) local3].Height == (short) 0;
      float num7 = float.MaxValue;
      Box2 aabb1 = nodes[(int) local2].Aabb;
      Box2 box2_2 = ((Box2) ref aabb1).Union(ref boxD);
      float num8 = Box2.Perimeter(ref box2_2);
      float num9 = 0.0f;
      if (flag1)
      {
        float num10 = num8 + num4;
        if ((double) num10 < (double) num5)
        {
          bestSibling = local2;
          num5 = num10;
        }
      }
      else
      {
        num9 = Box2.Perimeter(ref aabb1);
        num7 = num4 + num8 + MathF.Min(num1 - num9, 0.0f);
      }
      float num11 = float.MaxValue;
      Box2 aabb2 = nodes[(int) local3].Aabb;
      Box2 box2_3 = ((Box2) ref aabb2).Union(ref boxD);
      float num12 = Box2.Perimeter(ref box2_3);
      float num13 = 0.0f;
      if (flag2)
      {
        float num14 = num12 + num4;
        if ((double) num14 < (double) num5)
        {
          bestSibling = local3;
          num5 = num14;
        }
      }
      else
      {
        num13 = Box2.Perimeter(ref aabb2);
        num11 = num4 + num12 + MathF.Min(num1 - num13, 0.0f);
      }
      if (!(flag1 & flag2) && ((double) num5 > (double) num7 || (double) num5 > (double) num11))
      {
        if ((double) num7 == (double) num11 && !flag1)
        {
          Vector2 vector2_1 = Vector2.Subtract(((Box2) ref aabb1).Center, center);
          Vector2 vector2_2 = Vector2.Subtract(((Box2) ref aabb2).Center, center);
          num7 = vector2_1.LengthSquared();
          num11 = vector2_2.LengthSquared();
        }
        if ((double) num7 < (double) num11 && !flag1)
        {
          index = local2;
          num2 = num9;
          num3 = num8;
        }
        else
        {
          index = local3;
          num2 = num13;
          num3 = num12;
        }
      }
      else
        break;
    }
    return bestSibling;
  }

  public void RotateNodes(DynamicTree.Proxy iA)
  {
    ref B2DynamicTree<T>.Node local1 = ref this._nodes[(int) iA];
    if (local1.Height < (short) 2)
      return;
    DynamicTree.Proxy child1_1 = local1.Child1;
    DynamicTree.Proxy child2_1 = local1.Child2;
    ref B2DynamicTree<T>.Node local2 = ref this._nodes[(int) child1_1];
    ref B2DynamicTree<T>.Node local3 = ref this._nodes[(int) child2_1];
    if (local2.Height == (short) 0)
    {
      DynamicTree.Proxy child1_2 = local3.Child1;
      DynamicTree.Proxy child2_2 = local3.Child2;
      ref B2DynamicTree<T>.Node local4 = ref this._nodes[(int) child1_2];
      ref B2DynamicTree<T>.Node local5 = ref this._nodes[(int) child2_2];
      float num1 = Box2.Perimeter(ref local3.Aabb);
      Box2 box2_1 = ((Box2) ref local2.Aabb).Union(ref local5.Aabb);
      float num2 = Box2.Perimeter(ref box2_1);
      Box2 box2_2 = ((Box2) ref local2.Aabb).Union(ref local4.Aabb);
      float num3 = Box2.Perimeter(ref box2_2);
      if ((double) num1 < (double) num2 && (double) num1 < (double) num3)
        return;
      if ((double) num2 < (double) num3)
      {
        local1.Child1 = child1_2;
        local3.Child1 = child1_1;
        local2.Parent = child2_1;
        local4.Parent = iA;
        local3.Aabb = box2_1;
        local3.Height = (short) (1 + (int) Math.Max(local2.Height, local5.Height));
        local1.Height = (short) (1 + (int) Math.Max(local3.Height, local4.Height));
        local3.CategoryBits = local2.CategoryBits | local5.CategoryBits;
        local1.CategoryBits = local3.CategoryBits | local4.CategoryBits;
        local3.Enlarged = local2.Enlarged || local5.Enlarged;
        local1.Enlarged = local3.Enlarged || local4.Enlarged;
      }
      else
      {
        local1.Child1 = child2_2;
        local3.Child2 = child1_1;
        local2.Parent = child2_1;
        local5.Parent = iA;
        local3.Aabb = box2_2;
        local3.Height = (short) (1 + (int) Math.Max(local2.Height, local4.Height));
        local1.Height = (short) (1 + (int) Math.Max(local3.Height, local5.Height));
        local3.CategoryBits = local2.CategoryBits | local4.CategoryBits;
        local1.CategoryBits = local3.CategoryBits | local5.CategoryBits;
        local3.Enlarged = local2.Enlarged || local4.Enlarged;
        local1.Enlarged = local3.Enlarged || local5.Enlarged;
      }
    }
    else if (local3.Height == (short) 0)
    {
      DynamicTree.Proxy child1_3 = local2.Child1;
      DynamicTree.Proxy child2_3 = local2.Child2;
      ref B2DynamicTree<T>.Node local6 = ref this._nodes[(int) child1_3];
      ref B2DynamicTree<T>.Node local7 = ref this._nodes[(int) child2_3];
      float num4 = Box2.Perimeter(ref local2.Aabb);
      Box2 box2_3 = ((Box2) ref local3.Aabb).Union(ref local7.Aabb);
      float num5 = Box2.Perimeter(ref box2_3);
      Box2 box2_4 = ((Box2) ref local3.Aabb).Union(ref local6.Aabb);
      float num6 = Box2.Perimeter(ref box2_4);
      if ((double) num4 < (double) num5 && (double) num4 < (double) num6)
        return;
      if ((double) num5 < (double) num6)
      {
        local1.Child2 = child1_3;
        local2.Child1 = child2_1;
        local3.Parent = child1_1;
        local6.Parent = iA;
        local2.Aabb = box2_3;
        local2.Height = (short) (1 + (int) Math.Max(local3.Height, local7.Height));
        local1.Height = (short) (1 + (int) Math.Max(local2.Height, local6.Height));
        local2.CategoryBits = local3.CategoryBits | local7.CategoryBits;
        local1.CategoryBits = local2.CategoryBits | local6.CategoryBits;
        local2.Enlarged = local3.Enlarged || local7.Enlarged;
        local1.Enlarged = local2.Enlarged || local6.Enlarged;
      }
      else
      {
        local1.Child2 = child2_3;
        local2.Child2 = child2_1;
        local3.Parent = child1_1;
        local7.Parent = iA;
        local2.Aabb = box2_4;
        local2.Height = (short) (1 + (int) Math.Max(local3.Height, local6.Height));
        local1.Height = (short) (1 + (int) Math.Max(local2.Height, local7.Height));
        local2.CategoryBits = local3.CategoryBits | local6.CategoryBits;
        local1.CategoryBits = local2.CategoryBits | local7.CategoryBits;
        local2.Enlarged = local3.Enlarged || local6.Enlarged;
        local1.Enlarged = local2.Enlarged || local7.Enlarged;
      }
    }
    else
    {
      DynamicTree.Proxy child1_4 = local2.Child1;
      DynamicTree.Proxy child2_4 = local2.Child2;
      DynamicTree.Proxy child1_5 = local3.Child1;
      DynamicTree.Proxy child2_5 = local3.Child2;
      ref B2DynamicTree<T>.Node local8 = ref this._nodes[(int) child1_4];
      ref B2DynamicTree<T>.Node local9 = ref this._nodes[(int) child2_4];
      ref B2DynamicTree<T>.Node local10 = ref this._nodes[(int) child1_5];
      ref B2DynamicTree<T>.Node local11 = ref this._nodes[(int) child2_5];
      double num7 = (double) Box2.Perimeter(ref local2.Aabb);
      float num8 = Box2.Perimeter(ref local3.Aabb);
      double num9 = num7 + (double) num8;
      B2DynamicTree<T>.RotateType rotateType = B2DynamicTree<T>.RotateType.None;
      float num10 = (float) num9;
      Box2 box2_5 = ((Box2) ref local2.Aabb).Union(ref local11.Aabb);
      float num11 = (float) num7 + Box2.Perimeter(ref box2_5);
      if ((double) num11 < (double) num10)
      {
        rotateType = B2DynamicTree<T>.RotateType.BF;
        num10 = num11;
      }
      Box2 box2_6 = ((Box2) ref local2.Aabb).Union(ref local10.Aabb);
      float num12 = (float) num7 + Box2.Perimeter(ref box2_6);
      if ((double) num12 < (double) num10)
      {
        rotateType = B2DynamicTree<T>.RotateType.BG;
        num10 = num12;
      }
      Box2 box2_7 = ((Box2) ref local3.Aabb).Union(ref local9.Aabb);
      float num13 = num8 + Box2.Perimeter(ref box2_7);
      if ((double) num13 < (double) num10)
      {
        rotateType = B2DynamicTree<T>.RotateType.CD;
        num10 = num13;
      }
      Box2 box2_8 = ((Box2) ref local3.Aabb).Union(ref local8.Aabb);
      if ((double) num8 + (double) Box2.Perimeter(ref box2_8) < (double) num10)
        rotateType = B2DynamicTree<T>.RotateType.CE;
      switch (rotateType)
      {
        case B2DynamicTree<T>.RotateType.BF:
          local1.Child1 = child1_5;
          local3.Child1 = child1_1;
          local2.Parent = child2_1;
          local10.Parent = iA;
          local3.Aabb = box2_5;
          local3.Height = (short) (1 + (int) Math.Max(local2.Height, local11.Height));
          local1.Height = (short) (1 + (int) Math.Max(local3.Height, local10.Height));
          local3.CategoryBits = local2.CategoryBits | local11.CategoryBits;
          local1.CategoryBits = local3.CategoryBits | local10.CategoryBits;
          local3.Enlarged = local2.Enlarged || local11.Enlarged;
          local1.Enlarged = local3.Enlarged || local10.Enlarged;
          break;
        case B2DynamicTree<T>.RotateType.BG:
          local1.Child1 = child2_5;
          local3.Child2 = child1_1;
          local2.Parent = child2_1;
          local11.Parent = iA;
          local3.Aabb = box2_6;
          local3.Height = (short) (1 + (int) Math.Max(local2.Height, local10.Height));
          local1.Height = (short) (1 + (int) Math.Max(local3.Height, local11.Height));
          local3.CategoryBits = local2.CategoryBits | local10.CategoryBits;
          local1.CategoryBits = local3.CategoryBits | local11.CategoryBits;
          local3.Enlarged = local2.Enlarged || local10.Enlarged;
          local1.Enlarged = local3.Enlarged || local11.Enlarged;
          break;
        case B2DynamicTree<T>.RotateType.CD:
          local1.Child2 = child1_4;
          local2.Child1 = child2_1;
          local3.Parent = child1_1;
          local8.Parent = iA;
          local2.Aabb = box2_7;
          local2.Height = (short) (1 + (int) Math.Max(local3.Height, local9.Height));
          local1.Height = (short) (1 + (int) Math.Max(local2.Height, local8.Height));
          local2.CategoryBits = local3.CategoryBits | local9.CategoryBits;
          local1.CategoryBits = local2.CategoryBits | local8.CategoryBits;
          local2.Enlarged = local3.Enlarged || local9.Enlarged;
          local1.Enlarged = local2.Enlarged || local8.Enlarged;
          break;
        case B2DynamicTree<T>.RotateType.CE:
          local1.Child2 = child2_4;
          local2.Child2 = child2_1;
          local3.Parent = child1_1;
          local9.Parent = iA;
          local2.Aabb = box2_8;
          local2.Height = (short) (1 + (int) Math.Max(local3.Height, local8.Height));
          local1.Height = (short) (1 + (int) Math.Max(local2.Height, local9.Height));
          local2.CategoryBits = local3.CategoryBits | local8.CategoryBits;
          local1.CategoryBits = local2.CategoryBits | local9.CategoryBits;
          local2.Enlarged = local3.Enlarged || local8.Enlarged;
          local1.Enlarged = local2.Enlarged || local9.Enlarged;
          break;
      }
    }
  }

  public DynamicTree.Proxy CreateProxy(in Box2 aabb, uint categoryBits, 
  #nullable enable
  T userData)
  {
    DynamicTree.Proxy proxy;
    ref B2DynamicTree<T>.Node local = ref this.AllocateNode(out proxy);
    local.Aabb = aabb;
    local.UserData = userData;
    local.CategoryBits = categoryBits;
    local.Height = (short) 0;
    bool shouldRotate = true;
    this.InsertLeaf(proxy, shouldRotate);
    ++this.ProxyCount;
    return proxy;
  }

  public void DestroyProxy(DynamicTree.Proxy proxy)
  {
    this.RemoveLeaf(proxy);
    this.FreeNode(proxy);
    --this.ProxyCount;
  }

  public void MoveProxy(DynamicTree.Proxy proxy, in Box2 aabb)
  {
    this.RemoveLeaf(proxy);
    this._nodes[(int) proxy].Aabb = aabb;
    bool shouldRotate = false;
    this.InsertLeaf(proxy, shouldRotate);
  }

  public void EnlargeProxy(DynamicTree.Proxy proxy, Box2 aabb)
  {
    B2DynamicTree<T>.Node[] nodes = this._nodes;
    ref B2DynamicTree<T>.Node local1 = ref this._nodes[(int) proxy];
    local1.Aabb = aabb;
    DynamicTree.Proxy parent = local1.Parent;
    while (parent != DynamicTree.Proxy.Free)
    {
      ref B2DynamicTree<T>.Node local2 = ref nodes[(int) parent];
      bool flag = ((Box2) ref local2.Aabb).EnlargeAabb(aabb);
      local2.Enlarged = true;
      parent = local2.Parent;
      if (!flag)
        break;
    }
    // ISSUE: variable of a reference type
    B2DynamicTree<T>.Node& local3;
    for (; parent != DynamicTree.Proxy.Free; parent = local3.Parent)
    {
      local3 = ref nodes[(int) parent];
      if (local3.Enlarged)
        break;
      local3.Enlarged = true;
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  [return: MaybeNull]
  public T GetUserData(DynamicTree.Proxy proxy) => this._nodes[(int) proxy].UserData;

  [MethodImpl(MethodImplOptions.NoInlining)]
  private void RemoveLeaf(DynamicTree.Proxy leaf)
  {
    if (leaf == this._root)
    {
      this._root = DynamicTree.Proxy.Free;
    }
    else
    {
      B2DynamicTree<T>.Node[] nodes = this._nodes;
      DynamicTree.Proxy parent1 = nodes[(int) leaf].Parent;
      DynamicTree.Proxy parent2 = nodes[(int) parent1].Parent;
      DynamicTree.Proxy index1 = !(nodes[(int) parent1].Child1 == leaf) ? nodes[(int) parent1].Child1 : nodes[(int) parent1].Child2;
      if (parent2 != DynamicTree.Proxy.Free)
      {
        if (nodes[(int) parent2].Child1 == parent1)
          nodes[(int) parent2].Child1 = index1;
        else
          nodes[(int) parent2].Child2 = index1;
        nodes[(int) index1].Parent = parent2;
        this.FreeNode(parent1);
        // ISSUE: variable of a reference type
        B2DynamicTree<T>.Node& local1;
        for (DynamicTree.Proxy index2 = parent2; index2 != DynamicTree.Proxy.Free; index2 = local1.Parent)
        {
          local1 = ref nodes[(int) index2];
          ref B2DynamicTree<T>.Node local2 = ref nodes[(int) local1.Child1];
          ref B2DynamicTree<T>.Node local3 = ref nodes[(int) local1.Child2];
          local1.Aabb = ((Box2) ref local2.Aabb).Union(ref local3.Aabb);
          local1.CategoryBits = local2.CategoryBits | local3.CategoryBits;
          local1.Height = (short) (1 + (int) Math.Max(local2.Height, local3.Height));
        }
      }
      else
      {
        this._root = index1;
        this._nodes[(int) index1].Parent = DynamicTree.Proxy.Free;
        this.FreeNode(parent1);
      }
    }
  }

  private void InsertLeaf(DynamicTree.Proxy leaf, bool shouldRotate)
  {
    if (this._root == DynamicTree.Proxy.Free)
    {
      this._root = leaf;
      this._nodes[(int) this._root].Parent = DynamicTree.Proxy.Free;
    }
    else
    {
      ref Box2 local1 = ref this._nodes[(int) leaf].Aabb;
      DynamicTree.Proxy bestSibling = this.FindBestSibling(local1);
      ref DynamicTree.Proxy local2 = ref this._nodes[(int) bestSibling].Parent;
      DynamicTree.Proxy proxy;
      ref B2DynamicTree<T>.Node local3 = ref this.AllocateNode(out proxy);
      B2DynamicTree<T>.Node[] nodes = this._nodes;
      local3.Parent = local2;
      local3.UserData = default (T);
      local3.Aabb = ((Box2) ref local1).Union(ref nodes[(int) bestSibling].Aabb);
      local3.CategoryBits = nodes[(int) leaf].CategoryBits | nodes[(int) bestSibling].CategoryBits;
      local3.Height = (short) ((int) nodes[(int) bestSibling].Height + 1);
      if (local2 != DynamicTree.Proxy.Free)
      {
        if (nodes[(int) local2].Child1 == bestSibling)
          nodes[(int) local2].Child1 = proxy;
        else
          nodes[(int) local2].Child2 = proxy;
        local3.Child1 = bestSibling;
        local3.Child2 = leaf;
        nodes[(int) bestSibling].Parent = proxy;
        nodes[(int) leaf].Parent = proxy;
      }
      else
      {
        local3.Child1 = bestSibling;
        local3.Child2 = leaf;
        nodes[(int) bestSibling].Parent = proxy;
        nodes[(int) leaf].Parent = proxy;
        this._root = proxy;
      }
      // ISSUE: variable of a reference type
      B2DynamicTree<T>.Node& local4;
      for (DynamicTree.Proxy parent = nodes[(int) leaf].Parent; parent != DynamicTree.Proxy.Free; parent = local4.Parent)
      {
        local4 = ref nodes[(int) parent];
        DynamicTree.Proxy child1 = local4.Child1;
        DynamicTree.Proxy child2 = local4.Child2;
        ref B2DynamicTree<T>.Node local5 = ref nodes[(int) child1];
        ref B2DynamicTree<T>.Node local6 = ref nodes[(int) child2];
        local4.Aabb = ((Box2) ref local5.Aabb).Union(ref local6.Aabb);
        local4.CategoryBits = local5.CategoryBits | local6.CategoryBits;
        local4.Height = (short) (1 + (int) Math.Max(local5.Height, local6.Height));
        local4.Enlarged = local5.Enlarged || local6.Enlarged;
        if (shouldRotate)
          this.RotateNodes(parent);
      }
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static float EstimateCost(in Box2 baseAabb, in B2DynamicTree<
  #nullable disable
  T>.Node node)
  {
    Box2 box2 = ((Box2) ref baseAabb).Union(ref node.Aabb);
    float num = Box2.Perimeter(ref box2);
    if (!node.IsLeaf)
      num -= Box2.Perimeter(ref node.Aabb);
    return num;
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private void Balance(DynamicTree.Proxy index)
  {
    // ISSUE: variable of a reference type
    B2DynamicTree<T>.Node& local1;
    for (; index != DynamicTree.Proxy.Free; index = local1.Parent)
    {
      index = this.BalanceStep(index);
      local1 = ref this._nodes[(int) index];
      DynamicTree.Proxy child1 = local1.Child1;
      DynamicTree.Proxy child2 = local1.Child2;
      ref B2DynamicTree<T>.Node local2 = ref this._nodes[(int) child1];
      ref B2DynamicTree<T>.Node local3 = ref this._nodes[(int) child2];
      local1.Height = (short) ((int) Math.Max(local2.Height, local3.Height) + 1);
      local1.Aabb = ((Box2) ref local2.Aabb).Union(ref local3.Aabb);
      if (index == local1.Parent)
        throw new Exception("Infinite loop in B2DynamicTree.Balance(). Trace: " + Environment.StackTrace);
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private DynamicTree.Proxy BalanceStep(DynamicTree.Proxy iA)
  {
    ref B2DynamicTree<T>.Node local1 = ref this._nodes[0];
    ref B2DynamicTree<T>.Node local2 = ref Unsafe.Add<B2DynamicTree<T>.Node>(ref local1, (int) iA);
    if (local2.IsLeaf || local2.Height < (short) 2)
      return iA;
    DynamicTree.Proxy child1_1 = local2.Child1;
    DynamicTree.Proxy child2_1 = local2.Child2;
    ref B2DynamicTree<T>.Node local3 = ref Unsafe.Add<B2DynamicTree<T>.Node>(ref local1, (int) child1_1);
    ref B2DynamicTree<T>.Node local4 = ref Unsafe.Add<B2DynamicTree<T>.Node>(ref local1, (int) child2_1);
    int num = (int) local4.Height - (int) local3.Height;
    if (num > 1)
    {
      DynamicTree.Proxy child1_2 = local4.Child1;
      DynamicTree.Proxy child2_2 = local4.Child2;
      ref B2DynamicTree<T>.Node local5 = ref Unsafe.Add<B2DynamicTree<T>.Node>(ref local1, (int) child1_2);
      ref B2DynamicTree<T>.Node local6 = ref Unsafe.Add<B2DynamicTree<T>.Node>(ref local1, (int) child2_2);
      local4.Child1 = iA;
      local4.Parent = local2.Parent;
      local2.Parent = child2_1;
      if (local4.Parent == DynamicTree.Proxy.Free)
      {
        this._root = child2_1;
      }
      else
      {
        ref B2DynamicTree<T>.Node local7 = ref Unsafe.Add<B2DynamicTree<T>.Node>(ref local1, (int) local4.Parent);
        if (local7.Child1 == iA)
          local7.Child1 = child2_1;
        else
          local7.Child2 = child2_1;
      }
      if ((int) local5.Height > (int) local6.Height)
      {
        local4.Child2 = child1_2;
        local2.Child2 = child2_2;
        local6.Parent = iA;
        local2.Aabb = ((Box2) ref local3.Aabb).Union(ref local6.Aabb);
        local4.Aabb = ((Box2) ref local2.Aabb).Union(ref local5.Aabb);
        local2.Height = (short) ((int) Math.Max(local3.Height, local6.Height) + 1);
        local4.Height = (short) ((int) Math.Max(local2.Height, local5.Height) + 1);
      }
      else
      {
        local4.Child2 = child2_2;
        local2.Child2 = child1_2;
        local5.Parent = iA;
        local2.Aabb = ((Box2) ref local3.Aabb).Union(ref local5.Aabb);
        local4.Aabb = ((Box2) ref local2.Aabb).Union(ref local6.Aabb);
        local2.Height = (short) ((int) Math.Max(local3.Height, local5.Height) + 1);
        local4.Height = (short) ((int) Math.Max(local2.Height, local6.Height) + 1);
      }
      return child2_1;
    }
    if (num >= -1)
      return iA;
    DynamicTree.Proxy child1_3 = local3.Child1;
    DynamicTree.Proxy child2_3 = local3.Child2;
    ref B2DynamicTree<T>.Node local8 = ref Unsafe.Add<B2DynamicTree<T>.Node>(ref local1, (int) child1_3);
    ref B2DynamicTree<T>.Node local9 = ref Unsafe.Add<B2DynamicTree<T>.Node>(ref local1, (int) child2_3);
    local3.Child1 = iA;
    local3.Parent = local2.Parent;
    local2.Parent = child1_1;
    if (local3.Parent == DynamicTree.Proxy.Free)
    {
      this._root = child1_1;
    }
    else
    {
      ref B2DynamicTree<T>.Node local10 = ref Unsafe.Add<B2DynamicTree<T>.Node>(ref local1, (int) local3.Parent);
      if (local10.Child1 == iA)
        local10.Child1 = child1_1;
      else
        local10.Child2 = child1_1;
    }
    if ((int) local8.Height > (int) local9.Height)
    {
      local3.Child2 = child1_3;
      local2.Child1 = child2_3;
      local9.Parent = iA;
      local2.Aabb = ((Box2) ref local4.Aabb).Union(ref local9.Aabb);
      local3.Aabb = ((Box2) ref local2.Aabb).Union(ref local8.Aabb);
      local2.Height = (short) ((int) Math.Max(local4.Height, local9.Height) + 1);
      local3.Height = (short) ((int) Math.Max(local2.Height, local8.Height) + 1);
    }
    else
    {
      local3.Child2 = child2_3;
      local2.Child1 = child1_3;
      local8.Parent = iA;
      local2.Aabb = ((Box2) ref local4.Aabb).Union(ref local8.Aabb);
      local3.Aabb = ((Box2) ref local2.Aabb).Union(ref local9.Aabb);
      local2.Height = (short) ((int) Math.Max(local4.Height, local8.Height) + 1);
      local3.Height = (short) ((int) Math.Max(local2.Height, local9.Height) + 1);
    }
    return child1_1;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private int ComputeHeight() => this.ComputeHeight(this._root);

  [MethodImpl(MethodImplOptions.NoInlining)]
  private int ComputeHeight(DynamicTree.Proxy proxy)
  {
    ref B2DynamicTree<T>.Node local = ref this._nodes[(int) proxy];
    return local.IsLeaf ? 0 : Math.Max(this.ComputeHeight(local.Child1), this.ComputeHeight(local.Child2)) + 1;
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  public void RebuildBottomUp()
  {
    Span<DynamicTree.Proxy> span = stackalloc DynamicTree.Proxy[this.Capacity];
    int index1 = 0;
    for (int v = 0; v < this.Capacity; ++v)
    {
      ref B2DynamicTree<T>.Node local = ref this._nodes[v];
      if (local.Height >= (short) 0)
      {
        if (local.IsLeaf)
        {
          local.Parent = DynamicTree.Proxy.Free;
          span[index1] = new DynamicTree.Proxy(v);
          ++index1;
        }
        else
          this.FreeNode(new DynamicTree.Proxy(v));
      }
    }
    for (; index1 > 1; --index1)
    {
      float num1 = float.MaxValue;
      int index2 = -1;
      int index3 = -1;
      for (int index4 = 0; index4 < index1; ++index4)
      {
        Box2 aabb1 = this._nodes[(int) span[index4]].Aabb;
        for (int index5 = index4 + 1; index5 < index1; ++index5)
        {
          Box2 aabb2 = this._nodes[(int) span[index5]].Aabb;
          Box2 box2 = ((Box2) ref aabb1).Union(ref aabb2);
          float num2 = Box2.Perimeter(ref box2);
          if ((double) num2 < (double) num1)
          {
            index2 = index4;
            index3 = index5;
            num1 = num2;
          }
        }
      }
      DynamicTree.Proxy index6 = span[index2];
      DynamicTree.Proxy index7 = span[index3];
      ref B2DynamicTree<T>.Node local1 = ref this._nodes[(int) index6];
      ref B2DynamicTree<T>.Node local2 = ref this._nodes[(int) index7];
      DynamicTree.Proxy proxy;
      ref B2DynamicTree<T>.Node local3 = ref this.AllocateNode(out proxy);
      local3.Child1 = index6;
      local3.Child2 = index7;
      local3.Aabb = ((Box2) ref local1.Aabb).Union(ref local2.Aabb);
      local3.CategoryBits = local1.CategoryBits | local2.CategoryBits;
      local3.Height = (short) (1 + (int) Math.Max(local1.Height, local2.Height));
      local3.Parent = DynamicTree.Proxy.Free;
      local1.Parent = proxy;
      local2.Parent = proxy;
      span[index3] = span[index1 - 1];
      span[index2] = proxy;
    }
    this._root = span[0];
  }

  public void ShiftOrigin(Vector2 newOrigin)
  {
    for (int index = 0; index < this._nodes.Length; ++index)
    {
      ref B2DynamicTree<T>.Node local = ref this._nodes[index];
      Vector2 bottomLeft = local.Aabb.BottomLeft;
      Vector2 topRight = local.Aabb.TopRight;
      local.Aabb = new Box2(bottomLeft - newOrigin, topRight - newOrigin);
    }
  }

  public unsafe void Query(Box2 aabb, uint maskBits, 
  #nullable enable
  B2DynamicTree<
  #nullable disable
  T>.TreeQueryCallback callback)
  {
    ; // Unable to render the statement
    GrowableStack<DynamicTree.Proxy> growableStack = new GrowableStack<DynamicTree.Proxy>(new Span<DynamicTree.Proxy>((void*) pointer, 1024 /*0x0400*/));
    growableStack.Push(in this._root);
    ref B2DynamicTree<T>.Node local1 = ref this._nodes[0];
    while (growableStack.GetCount() > 0)
    {
      DynamicTree.Proxy proxy = growableStack.Pop();
      if (!(proxy == DynamicTree.Proxy.Free))
      {
        ref B2DynamicTree<T>.Node local2 = ref Unsafe.Add<B2DynamicTree<T>.Node>(ref local1, (int) proxy);
        if (((Box2) ref local2.Aabb).Intersects(ref aabb) && ((int) local2.CategoryBits & (int) maskBits) != 0)
        {
          if (local2.IsLeaf)
          {
            if (!callback(proxy, local2.UserData))
              break;
          }
          else if (growableStack.GetCount() < 1023 /*0x03FF*/)
          {
            growableStack.Push(in local2.Child1);
            growableStack.Push(in local2.Child2);
          }
        }
      }
    }
  }

  public int PartitionMid(
  #nullable enable
  DynamicTree.Proxy[] indices, Vector2[] centers, int count)
  {
    if (count <= 2)
      return count / 2;
    ref Vector2 local1 = ref centers[0];
    Vector2 right = local1;
    Vector2 left = local1;
    for (int elementOffset = 1; elementOffset < count; ++elementOffset)
    {
      Vector2 vector2 = Unsafe.Add<Vector2>(ref local1, elementOffset);
      right = Vector2.Min(right, vector2);
      left = Vector2.Max(left, vector2);
    }
    Vector2 vector2_1 = Vector2.Subtract(left, right);
    Vector2 vector2_2 = new Vector2((float) (0.5 * ((double) right.X + (double) left.X)), (float) (0.5 * ((double) right.Y + (double) left.Y)));
    int index1 = 0;
    int num = count;
    if ((double) vector2_1.X > (double) vector2_1.Y)
    {
      float x = vector2_2.X;
      while (index1 < num)
      {
        while (index1 < num && (double) centers[index1].X < (double) x)
          ++index1;
        while (index1 < num && (double) centers[num - 1].X >= (double) x)
          --num;
        if (index1 < num)
        {
          ref DynamicTree.Proxy local2 = ref indices[index1];
          ref DynamicTree.Proxy local3 = ref indices[num - 1];
          DynamicTree.Proxy index2 = indices[num - 1];
          DynamicTree.Proxy index3 = indices[index1];
          local2 = index2;
          DynamicTree.Proxy proxy = index3;
          local3 = proxy;
          ref Vector2 local4 = ref centers[index1];
          ref Vector2 local5 = ref centers[num - 1];
          Vector2 center1 = centers[num - 1];
          Vector2 center2 = centers[index1];
          local4 = center1;
          Vector2 vector2_3 = center2;
          local5 = vector2_3;
          ++index1;
          --num;
        }
      }
    }
    else
    {
      float y = vector2_2.Y;
      while (index1 < num)
      {
        while (index1 < num && (double) centers[index1].Y < (double) y)
          ++index1;
        while (index1 < num && (double) centers[num - 1].Y >= (double) y)
          --num;
        if (index1 < num)
        {
          ref DynamicTree.Proxy local6 = ref indices[index1];
          ref DynamicTree.Proxy local7 = ref indices[num - 1];
          DynamicTree.Proxy index4 = indices[num - 1];
          DynamicTree.Proxy index5 = indices[index1];
          local6 = index4;
          DynamicTree.Proxy proxy = index5;
          local7 = proxy;
          ref Vector2 local8 = ref centers[index1];
          ref Vector2 local9 = ref centers[num - 1];
          Vector2 center3 = centers[num - 1];
          Vector2 center4 = centers[index1];
          local8 = center3;
          Vector2 vector2_4 = center4;
          local9 = vector2_4;
          ++index1;
          --num;
        }
      }
    }
    return index1 > 0 && index1 < count ? index1 : count / 2;
  }

  public unsafe DynamicTree.Proxy BuildTree(int leafCount)
  {
    B2DynamicTree<T>.Node[] nodes = this._nodes;
    DynamicTree.Proxy[] leafIndices = this.LeafIndices;
    if (leafCount == 1)
    {
      nodes[(int) leafIndices[0]].Parent = DynamicTree.Proxy.Free;
      return leafIndices[0];
    }
    Vector2[] leafCenters = this.LeafCenters;
    ; // Unable to render the statement
    GrowableStack<B2DynamicTree<T>.RebuildItem> growableStack = new GrowableStack<B2DynamicTree<T>.RebuildItem>(new Span<B2DynamicTree<T>.RebuildItem>((void*) pointer, 1024 /*0x0400*/));
    int index1 = 0;
    DynamicTree.Proxy proxy1;
    this.AllocateNode(out proxy1);
    growableStack.Push(new B2DynamicTree<T>.RebuildItem()
    {
      NodeIndex = proxy1,
      ChildCount = -1,
      StartIndex = 0,
      EndIndex = leafCount,
      SplitIndex = this.PartitionMid(leafIndices, leafCenters, leafCount)
    });
    while (true)
    {
      ref B2DynamicTree<T>.RebuildItem local1 = ref growableStack[index1];
      ++local1.ChildCount;
      if (local1.ChildCount == 2)
      {
        if (index1 != 0)
        {
          ref B2DynamicTree<T>.RebuildItem local2 = ref growableStack[index1 - 1];
          ref B2DynamicTree<T>.Node local3 = ref nodes[(int) local2.NodeIndex];
          if (local2.ChildCount == 0)
            local3.Child1 = local1.NodeIndex;
          else
            local3.Child2 = local1.NodeIndex;
          ref B2DynamicTree<T>.Node local4 = ref nodes[(int) local1.NodeIndex];
          local4.Parent = local2.NodeIndex;
          ref B2DynamicTree<T>.Node local5 = ref nodes[(int) local4.Child1];
          ref B2DynamicTree<T>.Node local6 = ref nodes[(int) local4.Child2];
          local4.Aabb = Box2.Union(local5.Aabb, local6.Aabb);
          local4.Height = (short) (1 + (int) Math.Max(local5.Height, local6.Height));
          local4.CategoryBits = local5.CategoryBits | local6.CategoryBits;
          --index1;
        }
        else
          break;
      }
      else
      {
        int start;
        int num;
        if (local1.ChildCount == 0)
        {
          start = local1.StartIndex;
          num = local1.SplitIndex;
        }
        else
        {
          start = local1.SplitIndex;
          num = local1.EndIndex;
        }
        int count = num - start;
        if (count == 1)
        {
          DynamicTree.Proxy index2 = leafIndices[start];
          ref B2DynamicTree<T>.Node local7 = ref nodes[(int) local1.NodeIndex];
          if (local1.ChildCount == 0)
            local7.Child1 = index2;
          else
            local7.Child2 = index2;
          nodes[(int) index2].Parent = local1.NodeIndex;
        }
        else
        {
          ++index1;
          ref B2DynamicTree<T>.RebuildItem local8 = ref growableStack[index1];
          DynamicTree.Proxy proxy2;
          this.AllocateNode(out proxy2);
          local8.NodeIndex = proxy2;
          local8.ChildCount = -1;
          local8.StartIndex = start;
          local8.EndIndex = num;
          local8.SplitIndex = this.PartitionMid(RuntimeHelpers.GetSubArray<DynamicTree.Proxy>(leafIndices, Range.StartAt((Index) start)), RuntimeHelpers.GetSubArray<Vector2>(leafCenters, Range.StartAt((Index) start)), count);
          local8.SplitIndex += start;
        }
      }
    }
    ref B2DynamicTree<T>.Node local9 = ref nodes[(int) growableStack[0].NodeIndex];
    ref B2DynamicTree<T>.Node local10 = ref nodes[(int) local9.Child1];
    ref B2DynamicTree<T>.Node local11 = ref nodes[(int) local9.Child2];
    local9.Aabb = Box2.Union(local10.Aabb, local11.Aabb);
    local9.Height = (short) (1 + (int) Math.Max(local10.Height, local11.Height));
    local9.CategoryBits = local10.CategoryBits | local11.CategoryBits;
    return growableStack[0].NodeIndex;
  }

  public unsafe int Rebuild(bool fullBuild)
  {
    int proxyCount = this.ProxyCount;
    if (proxyCount == 0)
      return 0;
    if (proxyCount > this.RebuildCapacity)
    {
      int newSize = proxyCount + proxyCount / 2;
      Array.Resize<DynamicTree.Proxy>(ref this.LeafIndices, newSize);
      Array.Resize<Vector2>(ref this.LeafCenters, newSize);
      this.RebuildCapacity = newSize;
    }
    int leafCount = 0;
    ; // Unable to render the statement
    GrowableStack<DynamicTree.Proxy> growableStack = new GrowableStack<DynamicTree.Proxy>(new Span<DynamicTree.Proxy>((void*) pointer, 1024 /*0x0400*/));
    DynamicTree.Proxy elementOffset = this._root;
    ref B2DynamicTree<T>.Node local = ref this._nodes[0];
    B2DynamicTree<T>.Node node = local;
    DynamicTree.Proxy[] leafIndices = this.LeafIndices;
    Vector2[] leafCenters = this.LeafCenters;
    while (true)
    {
      for (; node.Height == (short) 0 || !node.Enlarged && !fullBuild; node = Unsafe.Add<B2DynamicTree<T>.Node>(ref local, (int) elementOffset))
      {
        leafIndices[leafCount] = elementOffset;
        leafCenters[leafCount] = ((Box2) ref node.Aabb).Center;
        ++leafCount;
        node.Parent = DynamicTree.Proxy.Free;
        if (growableStack.GetCount() != 0)
        {
          elementOffset = growableStack.Pop();
        }
        else
        {
          this._root = this.BuildTree(leafCount);
          return leafCount;
        }
      }
      DynamicTree.Proxy proxy = elementOffset;
      elementOffset = node.Child1;
      if (growableStack.GetCount() < 1024 /*0x0400*/)
        growableStack.Push(in node.Child2);
      node = Unsafe.Add<B2DynamicTree<T>.Node>(ref local, (int) elementOffset);
      this.FreeNode(proxy);
    }
  }

  public void Query(B2DynamicTree<
  #nullable disable
  T>.QueryCallback callback, in Box2 aabb)
  {
    this.Query<B2DynamicTree<T>.QueryCallback>(ref callback, B2DynamicTree<T>.EasyQueryCallback, in aabb);
  }

  public unsafe void Query<TState>(
    ref 
    #nullable enable
    TState state,
    B2DynamicTree<
    #nullable disable
    T>.QueryCallback<
    #nullable enable
    TState> callback,
    in Box2 aabb)
  {
    ; // Unable to render the statement
    GrowableStack<DynamicTree.Proxy> growableStack = new GrowableStack<DynamicTree.Proxy>(new Span<DynamicTree.Proxy>((void*) pointer, 256 /*0x0100*/));
    growableStack.Push(in this._root);
    ref B2DynamicTree<T>.Node local1 = ref this._nodes[0];
    while (growableStack.GetCount() != 0)
    {
      DynamicTree.Proxy proxy = growableStack.Pop();
      if (!(proxy == DynamicTree.Proxy.Free))
      {
        ref B2DynamicTree<T>.Node local2 = ref Unsafe.Add<B2DynamicTree<T>.Node>(ref local1, (int) proxy);
        if (((Box2) ref local2.Aabb).Intersects(ref aabb))
        {
          if (local2.IsLeaf)
          {
            if (!callback(ref state, proxy))
              break;
          }
          else
          {
            growableStack.Push(in local2.Child1);
            growableStack.Push(in local2.Child2);
          }
        }
      }
    }
  }

  public unsafe void FastQuery(ref Box2 aabb, B2DynamicTree<
  #nullable disable
  T>.FastQueryCallback callback)
  {
    ; // Unable to render the statement
    GrowableStack<DynamicTree.Proxy> growableStack = new GrowableStack<DynamicTree.Proxy>(new Span<DynamicTree.Proxy>((void*) pointer, 256 /*0x0100*/));
    growableStack.Push(in this._root);
    ref B2DynamicTree<T>.Node local1 = ref this._nodes[0];
    while (growableStack.GetCount() != 0)
    {
      DynamicTree.Proxy elementOffset = growableStack.Pop();
      if (!(elementOffset == DynamicTree.Proxy.Free))
      {
        ref B2DynamicTree<T>.Node local2 = ref Unsafe.Add<B2DynamicTree<T>.Node>(ref local1, (int) elementOffset);
        if (((Box2) ref local2.Aabb).Intersects(ref aabb))
        {
          if (local2.IsLeaf)
          {
            callback(ref local2.UserData);
          }
          else
          {
            growableStack.Push(in local2.Child1);
            growableStack.Push(in local2.Child2);
          }
        }
      }
    }
  }

  internal unsafe void RayCastNew(
    RayCastInput input,
    long mask,
    ref WorldRayCastContext state,
    #nullable enable
    B2DynamicTree<
    #nullable disable
    T>.RayCallback callback)
  {
    Vector2 origin = input.Origin;
    Vector2 translation = input.Translation;
    Vector2 vector2_1 = Vector2Helpers.Normalized(translation);
    Vector2 vector2_2 = Vector2Helpers.Cross(1f, ref vector2_1);
    Vector2 vector2_3 = Vector2.Abs(vector2_2);
    float num1 = input.MaxFraction;
    Vector2 vector2_4 = Vector2.Add(origin, num1 * translation);
    Box2 box2;
    // ISSUE: explicit constructor call
    ((Box2) ref box2).\u002Ector(Vector2.Min(origin, vector2_4), Vector2.Max(origin, vector2_4));
    ; // Unable to render the statement
    GrowableStack<DynamicTree.Proxy> growableStack = new GrowableStack<DynamicTree.Proxy>(new Span<DynamicTree.Proxy>((void*) pointer, 256 /*0x0100*/));
    ref B2DynamicTree<T>.Node local = ref this._nodes[0];
    growableStack.Push(in this._root);
    RayCastInput input1 = input;
    while (growableStack.GetCount() > 0)
    {
      DynamicTree.Proxy elementOffset = growableStack.Pop();
      if (!(elementOffset == DynamicTree.Proxy.Free))
      {
        B2DynamicTree<T>.Node node = Unsafe.Add<B2DynamicTree<T>.Node>(ref local, (int) elementOffset);
        if (((Box2) ref node.Aabb).Intersects(ref box2))
        {
          Vector2 center = ((Box2) ref node.Aabb).Center;
          Vector2 extents = ((Box2) ref node.Aabb).Extents;
          float num2 = MathF.Abs(Vector2.Dot(vector2_2, Vector2.Subtract(origin, center)));
          if ((double) Vector2.Dot(vector2_3, extents) >= (double) num2)
          {
            if (node.IsLeaf)
            {
              input1.MaxFraction = num1;
              float num3 = callback(input1, node.UserData, ref state);
              if ((double) num3 == 0.0)
                break;
              if (0.0 < (double) num3 && (double) num3 < (double) num1)
              {
                num1 = num3;
                Vector2 vector2_5 = Vector2.Add(origin, num1 * translation);
                box2.BottomLeft = Vector2.Min(origin, vector2_5);
                box2.TopRight = Vector2.Max(origin, vector2_5);
              }
            }
            else if (growableStack.GetCount() < (int) byte.MaxValue)
            {
              growableStack.Push(in node.Child1);
              growableStack.Push(in node.Child2);
            }
          }
        }
      }
    }
  }

  internal unsafe void ShapeCast(
    ShapeCastInput input,
    long maskBits,
    #nullable enable
    B2DynamicTree<
    #nullable disable
    T>.TreeShapeCastCallback callback,
    ref WorldRayCastContext state)
  {
    if (input.Count == 0)
      return;
    Box2 box2_1;
    // ISSUE: explicit constructor call
    ((Box2) ref box2_1).\u002Ector(input.Points[0], input.Points[0]);
    for (int index = 1; index < input.Count; ++index)
    {
      box2_1.BottomLeft = Vector2.Min(box2_1.BottomLeft, input.Points[index]);
      box2_1.TopRight = Vector2.Max(box2_1.TopRight, input.Points[index]);
    }
    Vector2 right1 = new Vector2(input.Radius, input.Radius);
    box2_1.BottomLeft = Vector2.Subtract(box2_1.BottomLeft, right1);
    box2_1.TopRight = Vector2.Add(box2_1.TopRight, right1);
    Vector2 center1 = ((Box2) ref box2_1).Center;
    Vector2 extents = ((Box2) ref box2_1).Extents;
    Vector2 translation = input.Translation;
    Vector2 vector2_1 = Vector2Helpers.Cross(1f, ref translation);
    Vector2 vector2_2 = Vector2.Abs(vector2_1);
    float left = input.MaxFraction;
    Vector2 right2 = Vector2.Multiply(left, input.Translation);
    Box2 box2_2;
    // ISSUE: explicit constructor call
    ((Box2) ref box2_2).\u002Ector(Vector2.Min(box2_1.BottomLeft, Vector2.Add(box2_1.BottomLeft, right2)), Vector2.Max(box2_1.TopRight, Vector2.Add(box2_1.TopRight, right2)));
    ShapeCastInput input1 = input;
    ref B2DynamicTree<T>.Node local = ref this._nodes[0];
    ; // Unable to render the statement
    GrowableStack<DynamicTree.Proxy> growableStack = new GrowableStack<DynamicTree.Proxy>(new Span<DynamicTree.Proxy>((void*) pointer, 256 /*0x0100*/));
    growableStack.Push(in this._root);
    while (growableStack.GetCount() > 0)
    {
      DynamicTree.Proxy elementOffset = growableStack.Pop();
      if (!(elementOffset == DynamicTree.Proxy.Free))
      {
        B2DynamicTree<T>.Node node = Unsafe.Add<B2DynamicTree<T>.Node>(ref local, (int) elementOffset);
        if (((Box2) ref node.Aabb).Intersects(ref box2_2))
        {
          Vector2 center2 = ((Box2) ref node.Aabb).Center;
          Vector2 vector2_3 = Vector2.Add(((Box2) ref node.Aabb).Extents, extents);
          float num1 = MathF.Abs(Vector2.Dot(vector2_1, Vector2.Subtract(center1, center2)));
          if ((double) Vector2.Dot(vector2_2, vector2_3) >= (double) num1)
          {
            if (node.IsLeaf)
            {
              input1.MaxFraction = left;
              float num2 = callback(input1, node.UserData, ref state);
              if ((double) num2 == 0.0)
                break;
              if (0.0 < (double) num2 && (double) num2 < (double) left)
              {
                left = num2;
                Vector2 right3 = Vector2.Multiply(left, input.Translation);
                box2_2.BottomLeft = Vector2.Min(box2_1.BottomLeft, Vector2.Add(box2_1.BottomLeft, right3));
                box2_2.TopRight = Vector2.Max(box2_1.TopRight, Vector2.Add(box2_1.TopRight, right3));
              }
            }
            else if (growableStack.GetCount() < (int) byte.MaxValue)
            {
              growableStack.Push(in node.Child1);
              growableStack.Push(in node.Child2);
            }
          }
        }
      }
    }
  }

  public void RayCast(
  #nullable enable
  B2DynamicTree<
  #nullable disable
  T>.RayQueryCallback callback, in Ray input)
  {
    this.RayCast<B2DynamicTree<T>.RayQueryCallback>(ref callback, B2DynamicTree<T>.EasyRayQueryCallback, in input);
  }

  public unsafe void RayCast<TState>(
    ref 
    #nullable enable
    TState state,
    B2DynamicTree<
    #nullable disable
    T>.RayQueryCallback<
    #nullable enable
    TState> callback,
    in Ray input)
  {
    ; // Unable to render the statement
    GrowableStack<DynamicTree.Proxy> growableStack = new GrowableStack<DynamicTree.Proxy>(new Span<DynamicTree.Proxy>((void*) pointer, 256 /*0x0100*/));
    growableStack.Push(in this._root);
    ref B2DynamicTree<T>.Node local1 = ref this._nodes[0];
    while (growableStack.GetCount() > 0)
    {
      DynamicTree.Proxy proxy = growableStack.Pop();
      if (!(proxy == DynamicTree.Proxy.Free))
      {
        ref B2DynamicTree<T>.Node local2 = ref Unsafe.Add<B2DynamicTree<T>.Node>(ref local1, (int) proxy);
        float distance;
        Vector2 hitPos;
        if (input.Intersects(local2.Aabb, out distance, out hitPos))
        {
          if (local2.IsLeaf)
          {
            if (!callback(ref state, proxy, in hitPos, distance))
              break;
          }
          else
          {
            if (local2.Child1 != DynamicTree.Proxy.Free)
              growableStack.Push(in local2.Child1);
            if (local2.Child2 != DynamicTree.Proxy.Free)
              growableStack.Push(in local2.Child2);
          }
        }
      }
    }
  }

  [Conditional("DEBUG_DYNAMIC_TREE")]
  public void ValidateStructure(DynamicTree.Proxy proxy)
  {
    if (proxy == DynamicTree.Proxy.Free)
      return;
    int num1 = proxy == this._root ? 1 : 0;
    ref B2DynamicTree<T>.Node local = ref this._nodes[(int) proxy];
    DynamicTree.Proxy child1 = local.Child1;
    DynamicTree.Proxy child2 = local.Child2;
    if (local.IsLeaf || this._nodes[(int) child1].Enlarged)
      return;
    int num2 = this._nodes[(int) child2].Enlarged ? 1 : 0;
  }

  [Conditional("DEBUG_DYNAMIC_TREE")]
  public void ValidateMetrics(DynamicTree.Proxy proxy)
  {
    if (proxy == DynamicTree.Proxy.Free)
      return;
    ref B2DynamicTree<T>.Node local1 = ref this._nodes[(int) proxy];
    DynamicTree.Proxy child1 = local1.Child1;
    DynamicTree.Proxy child2 = local1.Child2;
    if (local1.IsLeaf)
      return;
    int num = (int) Math.Max(this._nodes[(int) child1].Height, this._nodes[(int) child2].Height);
    ref B2DynamicTree<T>.Node local2 = ref this._nodes[(int) child1];
    ref B2DynamicTree<T>.Node local3 = ref this._nodes[(int) child2];
  }

  [Conditional("DEBUG_DYNAMIC_TREE")]
  private void Validate()
  {
    if (this._root == DynamicTree.Proxy.Free)
      return;
    int num = 0;
    DynamicTree.Proxy index = this._freeList;
    while (index != DynamicTree.Proxy.Free)
    {
      index = this._nodes[(int) index].Next;
      ++num;
    }
    int height = this.Height;
    this.ComputeHeight();
  }

  [Conditional("DEBUG_DYNAMIC_TREE")]
  [Conditional("DEBUG_DYNAMIC_TREE_ASSERTS")]
  [DebuggerNonUserCode]
  [DebuggerHidden]
  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Assert(bool assertion, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0)
  {
    if (!assertion)
    {
      string message = $"Assertion failure in {member} ({file}:{line})";
      Debugger.Break();
      throw new InvalidOperationException(message);
    }
  }

  private IEnumerable<(DynamicTree.Proxy, B2DynamicTree<
  #nullable disable
  T>.Node)> DebugAllocatedNodesEnumerable
  {
    get
    {
      for (int i = 0; i < this._nodes.Length; ++i)
      {
        B2DynamicTree<T>.Node node = this._nodes[i];
        if (!node.IsFree)
          yield return ((DynamicTree.Proxy) i, node);
      }
    }
  }

  [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
  private 
  #nullable enable
  (DynamicTree.Proxy, B2DynamicTree<
  #nullable disable
  T>.Node)[] DebugAllocatedNodes
  {
    get
    {
      (DynamicTree.Proxy, B2DynamicTree<T>.Node)[] debugAllocatedNodes1 = new (DynamicTree.Proxy, B2DynamicTree<T>.Node)[this.NodeCount];
      int num = 0;
      foreach ((DynamicTree.Proxy, B2DynamicTree<T>.Node) debugAllocatedNodes2 in this.DebugAllocatedNodesEnumerable)
        debugAllocatedNodes1[num++] = debugAllocatedNodes2;
      return debugAllocatedNodes1;
    }
  }

  public delegate bool RayQueryCallback<TState>(
    ref 
    #nullable enable
    TState state,
    DynamicTree.Proxy proxy,
    in Vector2 hitPos,
    float distance);

  public delegate bool RayQueryCallback(DynamicTree.Proxy proxy, in Vector2 hitPos, float distance);

  public delegate bool QueryCallback(DynamicTree.Proxy proxy);

  public delegate bool QueryCallback<TState>(ref TState state, DynamicTree.Proxy proxy);

  private enum RotateType : byte
  {
    None,
    BF,
    BG,
    CD,
    CE,
  }

  private struct Node
  {
    public Box2 Aabb;
    public uint CategoryBits;
    public DynamicTree.Proxy Parent;
    public DynamicTree.Proxy Next;
    public DynamicTree.Proxy Child1;
    public DynamicTree.Proxy Child2;
    public T UserData;
    public short Height;
    public bool Enlarged;

    public bool IsLeaf
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.Height == (short) 0;
    }

    public bool IsFree
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.Height == (short) -1;
    }

    public override string ToString()
    {
      string str1 = this.Parent == DynamicTree.Proxy.Free ? "None" : this.Parent.ToString();
      string str2;
      if (!this.IsLeaf)
      {
        if (!this.IsFree)
          str2 = $"Branch at height {this.Height}, children: {this.Child1} and {this.Child2}";
        else
          str2 = "Free";
      }
      else if (this.Height != (short) 0)
        str2 = $"Leaf (invalid height of {this.Height}): {this.UserData}";
      else
        str2 = $"Leaf: {this.UserData}";
      return $"Parent: {str1}, {str2}";
    }
  }

  public delegate bool TreeQueryCallback(DynamicTree.Proxy proxyId, T userData);

  private struct RebuildItem
  {
    public DynamicTree.Proxy NodeIndex;
    public int ChildCount;
    public int StartIndex;
    public int SplitIndex;
    public int EndIndex;
  }

  public delegate void FastQueryCallback(ref T userData);

  internal delegate float RayCallback(RayCastInput input, T context, ref WorldRayCastContext state);

  internal delegate float TreeShapeCastCallback(
    ShapeCastInput input,
    T userData,
    ref WorldRayCastContext state);
}
