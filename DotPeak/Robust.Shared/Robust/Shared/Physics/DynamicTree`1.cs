// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.DynamicTree`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.Physics;

[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay}")]
public sealed class DynamicTree<T> : IBroadPhase<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : notnull
{
  private readonly IEqualityComparer<T> _equalityComparer;
  private readonly DynamicTree<
  #nullable disable
  T>.ExtractAabbDelegate _extractAabb;
  private 
  #nullable enable
  Dictionary<T, DynamicTree.Proxy> _nodeLookup;
  public readonly B2DynamicTree<T> _b2Tree;
  private static readonly DynamicTree<
  #nullable disable
  T>.QueryCallbackDelegate<
  #nullable enable
  DynamicTree<
  #nullable disable
  T>.QueryCallbackDelegate> EasyQueryCallback = (DynamicTree<T>.QueryCallbackDelegate<DynamicTree<T>.QueryCallbackDelegate>) ((ref 
  #nullable enable
  DynamicTree<
  #nullable disable
  T>.QueryCallbackDelegate s, in 
  #nullable enable
  T v) => s(in v));
  private static readonly DynamicTree<
  #nullable disable
  T>.RayQueryCallbackDelegate<
  #nullable enable
  DynamicTree<
  #nullable disable
  T>.RayQueryCallbackDelegate> RayQueryDelegateCallbackInst = new DynamicTree<T>.RayQueryCallbackDelegate<DynamicTree<T>.RayQueryCallbackDelegate>(DynamicTree<T>.RayQueryDelegateCallback);

  public DynamicTree(
    #nullable enable
    DynamicTree<
    #nullable disable
    T>.ExtractAabbDelegate extractAabbFunc,
    #nullable enable
    IEqualityComparer<T>? comparer = null,
    float aabbExtendSize = 0.03125f,
    int capacity = 256 /*0x0100*/,
    Func<int, int>? growthFunc = null)
  {
    capacity = Math.Max(16 /*0x10*/, capacity);
    this._extractAabb = extractAabbFunc;
    this._equalityComparer = comparer ?? (IEqualityComparer<T>) EqualityComparer<T>.Default;
    this._nodeLookup = new Dictionary<T, DynamicTree.Proxy>(this._equalityComparer);
    this._b2Tree = new B2DynamicTree<T>(aabbExtendSize, capacity, growthFunc);
  }

  public int Capacity => this._b2Tree.Capacity;

  public int Height => this._b2Tree.Height;

  public int MaxBalance => this._b2Tree.MaxBalance;

  public float AreaRatio => this._b2Tree.AreaRatio;

  public string DebuggerDisplay
  {
    get
    {
      return $"Count = {this.Count}, Capacity = {this.Capacity}, Height = {this.Height}, NodeCount = {this.NodeCount}";
    }
  }

  public IEnumerator<T> GetEnumerator() => (IEnumerator<T>) this._nodeLookup.Keys.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public void Clear()
  {
    foreach (DynamicTree.Proxy proxy in this._nodeLookup.Values)
      this._b2Tree.DestroyProxy(proxy);
  }

  public bool Contains(T item) => this._nodeLookup.ContainsKey(item);

  public void CopyTo(T[] array, int arrayIndex) => this._nodeLookup.Keys.CopyTo(array, arrayIndex);

  public int NodeCount { get; private set; }

  public int Count => this._nodeLookup.Count;

  public bool IsReadOnly => false;

  void ICollection<T>.Add(T item) => this.Add(in item, new Box2?());

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Add(in T item, Box2? aabb = null)
  {
    if (this.TryGetProxy(in item, out DynamicTree.Proxy _))
      return false;
    aabb.GetValueOrDefault();
    if (!aabb.HasValue)
      aabb = new Box2?(this._extractAabb(in item));
    Box2 box2 = aabb.Value;
    if (((Box2) ref box2).HasNan())
    {
      this._nodeLookup[item] = DynamicTree.Proxy.Free;
      return true;
    }
    DynamicTree.Proxy proxy = this._b2Tree.CreateProxy(aabb.Value, uint.MaxValue, item);
    this._nodeLookup[item] = proxy;
    return true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private bool TryGetProxy(in T item, out DynamicTree.Proxy proxy)
  {
    return this._nodeLookup.TryGetValue(item, out proxy);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Box2? GetNodeBounds(T item)
  {
    DynamicTree.Proxy proxy;
    return !this.TryGetProxy(in item, out proxy) ? new Box2?() : this._b2Tree.GetFatAabb(proxy);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Box2? GetNodeBounds(in T item)
  {
    DynamicTree.Proxy proxy;
    return !this.TryGetProxy(in item, out proxy) ? new Box2?() : this._b2Tree.GetFatAabb(proxy);
  }

  public bool Remove(in T item)
  {
    DynamicTree.Proxy proxy;
    if (!this._nodeLookup.Remove(item, out proxy))
      return false;
    if (proxy != DynamicTree.Proxy.Free)
      this._b2Tree.DestroyProxy(proxy);
    return true;
  }

  bool ICollection<T>.Remove(T item) => this.Remove(in item);

  [MethodImpl(MethodImplOptions.NoInlining)]
  public bool Update(in T item, Box2? newBox = null)
  {
    ref DynamicTree.Proxy local = ref CollectionsMarshal.GetValueRefOrNullRef<T, DynamicTree.Proxy>(this._nodeLookup, item);
    if (Unsafe.IsNullRef<DynamicTree.Proxy>(ref local))
      return false;
    newBox.GetValueOrDefault();
    if (!newBox.HasValue)
      newBox = new Box2?(this._extractAabb(in item));
    Box2 box2 = newBox.Value;
    if (((Box2) ref box2).HasNan())
    {
      if (local == DynamicTree.Proxy.Free)
        return false;
      this._b2Tree.DestroyProxy(local);
      local = DynamicTree.Proxy.Free;
      return true;
    }
    if (local == DynamicTree.Proxy.Free)
    {
      local = this._b2Tree.CreateProxy(newBox.Value, uint.MaxValue, item);
      return true;
    }
    this._b2Tree.MoveProxy(local, newBox.Value);
    return true;
  }

  public void QueryAabb(DynamicTree<
  #nullable disable
  T>.QueryCallbackDelegate callback, Box2 aabb, bool approx = false)
  {
    this.QueryAabb<DynamicTree<T>.QueryCallbackDelegate>(ref callback, DynamicTree<T>.EasyQueryCallback, aabb, approx);
  }

  public void QueryAabb<TState>(
    ref 
    #nullable enable
    TState state,
    DynamicTree<
    #nullable disable
    T>.QueryCallbackDelegate<
    #nullable enable
    TState> callback,
    Box2 aabb,
    bool approx = false)
  {
    (TState, B2DynamicTree<T>, DynamicTree<T>.QueryCallbackDelegate<TState>, Box2, bool, DynamicTree<T>.ExtractAabbDelegate) state1 = (state, this._b2Tree, callback, aabb, approx, this._extractAabb);
    this._b2Tree.Query<(TState, B2DynamicTree<T>, DynamicTree<T>.QueryCallbackDelegate<TState>, Box2, bool, DynamicTree<T>.ExtractAabbDelegate)>(ref state1, DynamicTree<T>.DelegateCache<TState>.AabbQueryState, in aabb);
    state = state1.Item1;
  }

  public IEnumerable<T> QueryAabb(Box2 aabb, bool approx = false)
  {
    List<T> state = new List<T>();
    this.QueryAabb<List<T>>(ref state, (DynamicTree<T>.QueryCallbackDelegate<List<T>>) ((ref List<T> lst, in T i) =>
    {
      lst.Add(i);
      return true;
    }), aabb, approx);
    return (IEnumerable<T>) state;
  }

  public void QueryPoint(DynamicTree<
  #nullable disable
  T>.QueryCallbackDelegate callback, Vector2 point, bool approx = false)
  {
    this.QueryPoint<DynamicTree<T>.QueryCallbackDelegate>(ref callback, DynamicTree<T>.EasyQueryCallback, point, approx);
  }

  public void QueryPoint<TState>(
    ref 
    #nullable enable
    TState state,
    DynamicTree<
    #nullable disable
    T>.QueryCallbackDelegate<
    #nullable enable
    TState> callback,
    Vector2 point,
    bool approx = false)
  {
    (TState, B2DynamicTree<T>, DynamicTree<T>.QueryCallbackDelegate<TState>, Vector2, bool, DynamicTree<T>.ExtractAabbDelegate) state1 = (state, this._b2Tree, callback, point, approx, this._extractAabb);
    this._b2Tree.Query<(TState, B2DynamicTree<T>, DynamicTree<T>.QueryCallbackDelegate<TState>, Vector2, bool, DynamicTree<T>.ExtractAabbDelegate)>(ref state1, (B2DynamicTree<T>.QueryCallback<(TState, B2DynamicTree<T>, DynamicTree<T>.QueryCallbackDelegate<TState>, Vector2, bool, DynamicTree<T>.ExtractAabbDelegate)>) ((ref (
    #nullable disable
    TState state, 
    #nullable enable
    B2DynamicTree<T> tree, DynamicTree<
    #nullable disable
    T>.QueryCallbackDelegate<TState> callback, Vector2 point, bool approx, 
    #nullable enable
    DynamicTree<
    #nullable disable
    T>.ExtractAabbDelegate extract) tuple, DynamicTree.Proxy proxy) =>
    {
      T userData = tuple.Item2.GetUserData(proxy);
      if (!tuple.Item5)
      {
        Box2 box2 = tuple.Item6(in userData);
        if (!((Box2) ref box2).Contains(tuple.Item4, true))
          return true;
      }
      return tuple.Item3(ref tuple.Item1, in userData);
    }), Box2.CenteredAround(point, new Vector2(0.1f, 0.1f)));
    state = state1.Item1;
  }

  public 
  #nullable enable
  IEnumerable<T> QueryPoint(Vector2 point, bool approx = false)
  {
    List<T> state = new List<T>();
    this.QueryPoint<List<T>>(ref state, (DynamicTree<T>.QueryCallbackDelegate<List<T>>) ((ref List<T> list, in T i) =>
    {
      list.Add(i);
      return true;
    }), point, approx);
    return (IEnumerable<T>) state;
  }

  public void QueryRay<TState>(
    ref TState state,
    DynamicTree<
    #nullable disable
    T>.RayQueryCallbackDelegate<
    #nullable enable
    TState> callback,
    in Ray ray,
    bool approx = false)
  {
    (TState, DynamicTree<T>.RayQueryCallbackDelegate<TState>, B2DynamicTree<T>, DynamicTree<T>.ExtractAabbDelegate, Ray) state1 = (state, callback, this._b2Tree, approx ? (DynamicTree<T>.ExtractAabbDelegate) null : this._extractAabb, ray);
    this._b2Tree.RayCast<(TState, DynamicTree<T>.RayQueryCallbackDelegate<TState>, B2DynamicTree<T>, DynamicTree<T>.ExtractAabbDelegate, Ray)>(ref state1, DynamicTree<T>.DelegateCache<TState>.RayQueryState, in ray);
    state = state1.Item1;
  }

  private static bool AabbQueryStateCallback<TState>(
    ref (TState state, B2DynamicTree<T> tree, DynamicTree<
    #nullable disable
    T>.QueryCallbackDelegate<
    #nullable enable
    TState> callback, Box2 aabb, bool approx, DynamicTree<
    #nullable disable
    T>.ExtractAabbDelegate extract) tuple,
    DynamicTree.Proxy proxy)
  {
    T userData = tuple.Item2.GetUserData(proxy);
    if (!tuple.Item5)
    {
      Box2 box2 = tuple.Item6(in userData);
      if (!((Box2) ref box2).Intersects(ref tuple.Item4))
        return true;
    }
    return tuple.Item3(ref tuple.Item1, in userData);
  }

  private static bool RayQueryStateCallback<TState>(
    ref (
    #nullable enable
    TState state, DynamicTree<
    #nullable disable
    T>.RayQueryCallbackDelegate<
    #nullable enable
    TState> callback, B2DynamicTree<T> tree, DynamicTree<
    #nullable disable
    T>.ExtractAabbDelegate
    #nullable enable
    ? extract, Ray srcRay) tuple,
    DynamicTree.Proxy proxy,
    in Vector2 hitPos,
    float distance)
  {
    T userData = tuple.Item3.GetUserData(proxy);
    Vector2 point = hitPos;
    if (tuple.Item4 != null)
    {
      Box2 box = tuple.Item4(in userData);
      if (!tuple.Item5.Intersects(box, out distance, out point))
        return true;
    }
    return tuple.Item2(ref tuple.Item1, in userData, in point, distance);
  }

  public void QueryRay(DynamicTree<
  #nullable disable
  T>.RayQueryCallbackDelegate callback, in Ray ray, bool approx = false)
  {
    this.QueryRay<DynamicTree<T>.RayQueryCallbackDelegate>(ref callback, DynamicTree<T>.RayQueryDelegateCallbackInst, in ray, approx);
  }

  private static bool RayQueryDelegateCallback(
    ref 
    #nullable enable
    DynamicTree<
    #nullable disable
    T>.RayQueryCallbackDelegate state,
    in 
    #nullable enable
    T value,
    in Vector2 point,
    float distFromOrigin)
  {
    return state(in value, in point, distFromOrigin);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void AddOrUpdate(T item, Box2? aabb = null)
  {
    aabb.GetValueOrDefault();
    if (!aabb.HasValue)
      aabb = new Box2?(this._extractAabb(in item));
    bool exists;
    ref DynamicTree.Proxy local1 = ref CollectionsMarshal.GetValueRefOrAddDefault<T, DynamicTree.Proxy>(this._nodeLookup, item, out exists);
    if (!exists)
    {
      ref DynamicTree.Proxy local2 = ref local1;
      Box2 box2 = aabb.Value;
      DynamicTree.Proxy proxy = ((Box2) ref box2).HasNan() ? DynamicTree.Proxy.Free : this._b2Tree.CreateProxy(aabb.Value, uint.MaxValue, item);
      local2 = proxy;
    }
    else
    {
      Box2 box2 = aabb.Value;
      if (((Box2) ref box2).HasNan())
      {
        if (local1 == DynamicTree.Proxy.Free)
          return;
        this._b2Tree.DestroyProxy(local1);
        local1 = DynamicTree.Proxy.Free;
      }
      else if (local1 == DynamicTree.Proxy.Free)
        local1 = this._b2Tree.CreateProxy(aabb.Value, uint.MaxValue, item);
      else
        this._b2Tree.MoveProxy(local1, aabb.Value);
    }
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

  bool IBroadPhase<T>.Add(in T item, Box2? newAABB = null) => this.Add(in item, newAABB);

  bool IBroadPhase<T>.Remove(in T item) => this.Remove(in item);

  bool IBroadPhase<T>.Update(in T item, Box2? newAABB = null) => this.Update(in item, newAABB);

  void IBroadPhase<T>.QueryRay(
    DynamicTree<T>.RayQueryCallbackDelegate callback,
    in Ray ray,
    bool approx = false)
  {
    this.QueryRay(callback, in ray, approx);
  }

  void IBroadPhase<T>.QueryRay<TState>(
    ref 
    #nullable disable
    TState state,
    #nullable enable
    DynamicTree<T>.RayQueryCallbackDelegate<
    #nullable disable
    TState> callback,
    in Ray ray,
    bool approx = false)
  {
    this.QueryRay<TState>(ref state, callback, in ray, approx);
  }

  public delegate Box2 ExtractAabbDelegate(in 
  #nullable enable
  T value);

  public delegate bool QueryCallbackDelegate(in T value);

  public delegate bool QueryCallbackDelegate<TState>(ref TState state, in T value);

  public delegate bool RayQueryCallbackDelegate(in T value, in Vector2 point, float distFromOrigin);

  public delegate bool RayQueryCallbackDelegate<TState>(
    ref TState state,
    in T value,
    in Vector2 point,
    float distFromOrigin);

  private static class DelegateCache<TState>
  {
    public static readonly B2DynamicTree<T>.QueryCallback<(TState state, B2DynamicTree<T> tree, DynamicTree<
    #nullable disable
    T>.QueryCallbackDelegate<
    #nullable enable
    TState> callback, Box2 aabb, bool approx, DynamicTree<
    #nullable disable
    T>.ExtractAabbDelegate extract)> AabbQueryState = new B2DynamicTree<T>.QueryCallback<(TState, B2DynamicTree<T>, DynamicTree<T>.QueryCallbackDelegate<TState>, Box2, bool, DynamicTree<T>.ExtractAabbDelegate)>(DynamicTree<T>.AabbQueryStateCallback<TState>);
    public static readonly 
    #nullable enable
    B2DynamicTree<T>.RayQueryCallback<(TState state, DynamicTree<
    #nullable disable
    T>.RayQueryCallbackDelegate<
    #nullable enable
    TState> callback, B2DynamicTree<T> tree, DynamicTree<
    #nullable disable
    T>.ExtractAabbDelegate
    #nullable enable
    ? extract, Ray srcRay)> RayQueryState = new B2DynamicTree<T>.RayQueryCallback<(TState, DynamicTree<T>.RayQueryCallbackDelegate<TState>, B2DynamicTree<T>, DynamicTree<T>.ExtractAabbDelegate, Ray)>(DynamicTree<T>.RayQueryStateCallback<TState>);
  }
}
