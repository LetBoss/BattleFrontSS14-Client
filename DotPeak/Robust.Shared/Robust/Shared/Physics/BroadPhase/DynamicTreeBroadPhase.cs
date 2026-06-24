// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.BroadPhase.DynamicTreeBroadPhase
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Physics.Dynamics;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics.BroadPhase;

public sealed class DynamicTreeBroadPhase : IBroadPhase
{
  private readonly B2DynamicTree<FixtureProxy> _tree;
  private readonly DynamicTree<FixtureProxy>.ExtractAabbDelegate _extractAabb = DynamicTreeBroadPhase.\u003C\u003EO.\u003C0\u003E__ExtractAabbFunc ?? (DynamicTreeBroadPhase.\u003C\u003EO.\u003C0\u003E__ExtractAabbFunc = new DynamicTree<FixtureProxy>.ExtractAabbDelegate(DynamicTreeBroadPhase.ExtractAabbFunc));
  private static readonly DynamicTree<FixtureProxy>.QueryCallbackDelegate<DynamicTree<FixtureProxy>.QueryCallbackDelegate> EasyQueryCallback = (DynamicTree<FixtureProxy>.QueryCallbackDelegate<DynamicTree<FixtureProxy>.QueryCallbackDelegate>) ((ref DynamicTree<FixtureProxy>.QueryCallbackDelegate s, in FixtureProxy v) => s(in v));
  private static readonly DynamicTree<FixtureProxy>.RayQueryCallbackDelegate<DynamicTree<FixtureProxy>.RayQueryCallbackDelegate> RayQueryDelegateCallbackInst = new DynamicTree<FixtureProxy>.RayQueryCallbackDelegate<DynamicTree<FixtureProxy>.RayQueryCallbackDelegate>(DynamicTreeBroadPhase.RayQueryDelegateCallback);

  public DynamicTreeBroadPhase(int capacity)
  {
    this._tree = new B2DynamicTree<FixtureProxy>(capacity: capacity);
  }

  public DynamicTreeBroadPhase()
    : this(256 /*0x0100*/)
  {
  }

  private static Box2 ExtractAabbFunc(in FixtureProxy proxy) => proxy.AABB;

  public int Count => this._tree.NodeCount;

  public B2DynamicTree<FixtureProxy> Tree => this._tree;

  public Box2 GetFatAabb(DynamicTree.Proxy proxy) => this._tree.GetUserData(proxy).AABB;

  public DynamicTree.Proxy AddProxy(ref FixtureProxy proxy)
  {
    return this._tree.CreateProxy(in proxy.AABB, uint.MaxValue, proxy);
  }

  public void MoveProxy(DynamicTree.Proxy proxy, in Box2 aabb)
  {
    this._tree.MoveProxy(proxy, in aabb);
  }

  public void RemoveProxy(DynamicTree.Proxy proxy) => this._tree.DestroyProxy(proxy);

  public FixtureProxy? GetProxy(DynamicTree.Proxy proxy) => this._tree.GetUserData(proxy);

  public void QueryAabb(
    DynamicTree<FixtureProxy>.QueryCallbackDelegate callback,
    Box2 aabb,
    bool approx = false)
  {
    this.QueryAabb<DynamicTree<FixtureProxy>.QueryCallbackDelegate>(ref callback, DynamicTreeBroadPhase.EasyQueryCallback, aabb, approx);
  }

  public void QueryAabb<TState>(
    ref TState state,
    DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState> callback,
    Box2 aabb,
    bool approx = false)
  {
    (TState, B2DynamicTree<FixtureProxy>, DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState>, Box2, bool, DynamicTree<FixtureProxy>.ExtractAabbDelegate) state1 = (state, this._tree, callback, aabb, approx, this._extractAabb);
    this._tree.Query<(TState, B2DynamicTree<FixtureProxy>, DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState>, Box2, bool, DynamicTree<FixtureProxy>.ExtractAabbDelegate)>(ref state1, DynamicTreeBroadPhase.DelegateCache<TState>.AabbQueryState, in aabb);
    state = state1.Item1;
  }

  public IEnumerable<FixtureProxy> QueryAabb(Box2 aabb, bool approx = false)
  {
    return this.QueryAabb(new List<FixtureProxy>(), aabb, approx);
  }

  public IEnumerable<FixtureProxy> QueryAabb(List<FixtureProxy> proxies, Box2 aabb, bool approx = false)
  {
    this.QueryAabb<List<FixtureProxy>>(ref proxies, (DynamicTree<FixtureProxy>.QueryCallbackDelegate<List<FixtureProxy>>) ((ref List<FixtureProxy> lst, in FixtureProxy i) =>
    {
      lst.Add(i);
      return true;
    }), aabb, approx);
    return (IEnumerable<FixtureProxy>) proxies;
  }

  public void QueryPoint(
    DynamicTree<FixtureProxy>.QueryCallbackDelegate callback,
    Vector2 point,
    bool approx = false)
  {
    this.QueryPoint<DynamicTree<FixtureProxy>.QueryCallbackDelegate>(ref callback, DynamicTreeBroadPhase.EasyQueryCallback, point, approx);
  }

  public void QueryPoint<TState>(
    ref TState state,
    DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState> callback,
    Vector2 point,
    bool approx = false)
  {
    (TState, B2DynamicTree<FixtureProxy>, DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState>, Vector2, bool, DynamicTree<FixtureProxy>.ExtractAabbDelegate) state1 = (state, this._tree, callback, point, approx, this._extractAabb);
    this._tree.Query<(TState, B2DynamicTree<FixtureProxy>, DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState>, Vector2, bool, DynamicTree<FixtureProxy>.ExtractAabbDelegate)>(ref state1, (B2DynamicTree<FixtureProxy>.QueryCallback<(TState, B2DynamicTree<FixtureProxy>, DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState>, Vector2, bool, DynamicTree<FixtureProxy>.ExtractAabbDelegate)>) ((ref (
    #nullable disable
    TState state, 
    #nullable enable
    B2DynamicTree<FixtureProxy> tree, DynamicTree<FixtureProxy>.QueryCallbackDelegate<
    #nullable disable
    TState> callback, Vector2 point, bool approx, 
    #nullable enable
    DynamicTree<FixtureProxy>.ExtractAabbDelegate extract) tuple, DynamicTree.Proxy proxy) =>
    {
      FixtureProxy userData = tuple.Item2.GetUserData(proxy);
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

  public IEnumerable<FixtureProxy> QueryPoint(Vector2 point, bool approx = false)
  {
    List<FixtureProxy> state = new List<FixtureProxy>();
    this.QueryPoint<List<FixtureProxy>>(ref state, (DynamicTree<FixtureProxy>.QueryCallbackDelegate<List<FixtureProxy>>) ((ref List<FixtureProxy> list, in FixtureProxy i) =>
    {
      list.Add(i);
      return true;
    }), point, approx);
    return (IEnumerable<FixtureProxy>) state;
  }

  public void QueryRay<TState>(
    ref TState state,
    DynamicTree<FixtureProxy>.RayQueryCallbackDelegate<TState> callback,
    in Ray ray,
    bool approx = false)
  {
    (TState, DynamicTree<FixtureProxy>.RayQueryCallbackDelegate<TState>, B2DynamicTree<FixtureProxy>, DynamicTree<FixtureProxy>.ExtractAabbDelegate, Ray) state1 = (state, callback, this._tree, approx ? (DynamicTree<FixtureProxy>.ExtractAabbDelegate) null : this._extractAabb, ray);
    this._tree.RayCast<(TState, DynamicTree<FixtureProxy>.RayQueryCallbackDelegate<TState>, B2DynamicTree<FixtureProxy>, DynamicTree<FixtureProxy>.ExtractAabbDelegate, Ray)>(ref state1, DynamicTreeBroadPhase.DelegateCache<TState>.RayQueryState, in ray);
    state = state1.Item1;
  }

  public void Rebuild(bool fullBuild) => this._tree.Rebuild(fullBuild);

  public void RebuildBottomUp() => this._tree.RebuildBottomUp();

  private static bool AabbQueryStateCallback<TState>(
    ref (TState state, B2DynamicTree<FixtureProxy> tree, DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState> callback, Box2 aabb, bool approx, DynamicTree<FixtureProxy>.ExtractAabbDelegate extract) tuple,
    DynamicTree.Proxy proxy)
  {
    FixtureProxy userData = tuple.Item2.GetUserData(proxy);
    if (!tuple.Item5)
    {
      Box2 box2 = tuple.Item6(in userData);
      if (!((Box2) ref box2).Intersects(ref tuple.Item4))
        return true;
    }
    return tuple.Item3(ref tuple.Item1, in userData);
  }

  private static bool RayQueryStateCallback<TState>(
    ref (TState state, DynamicTree<FixtureProxy>.RayQueryCallbackDelegate<TState> callback, B2DynamicTree<FixtureProxy> tree, DynamicTree<FixtureProxy>.ExtractAabbDelegate? extract, Ray srcRay) tuple,
    DynamicTree.Proxy proxy,
    in Vector2 hitPos,
    float distance)
  {
    FixtureProxy userData = tuple.Item3.GetUserData(proxy);
    Vector2 point = hitPos;
    if (tuple.Item4 != null)
    {
      Box2 box = tuple.Item4(in userData);
      if (!tuple.Item5.Intersects(box, out distance, out point))
        return true;
    }
    return tuple.Item2(ref tuple.Item1, in userData, in point, distance);
  }

  public void QueryRay(
    DynamicTree<FixtureProxy>.RayQueryCallbackDelegate callback,
    in Ray ray,
    bool approx = false)
  {
    this.QueryRay<DynamicTree<FixtureProxy>.RayQueryCallbackDelegate>(ref callback, DynamicTreeBroadPhase.RayQueryDelegateCallbackInst, in ray, approx);
  }

  private static bool RayQueryDelegateCallback(
    ref DynamicTree<FixtureProxy>.RayQueryCallbackDelegate state,
    in FixtureProxy value,
    in Vector2 point,
    float distFromOrigin)
  {
    return state(in value, in point, distFromOrigin);
  }

  void IBroadPhase.MoveProxy(DynamicTree.Proxy proxyId, in Box2 aabb)
  {
    this.MoveProxy(proxyId, in aabb);
  }

  void IBroadPhase.QueryRay(
    DynamicTree<FixtureProxy>.RayQueryCallbackDelegate callback,
    in Ray ray,
    bool approx = false)
  {
    this.QueryRay(callback, in ray, approx);
  }

  void IBroadPhase.QueryRay<TState>(
    ref 
    #nullable disable
    TState state,
    #nullable enable
    DynamicTree<FixtureProxy>.RayQueryCallbackDelegate<
    #nullable disable
    TState> callback,
    in Ray ray,
    bool approx = false)
  {
    this.QueryRay<TState>(ref state, callback, in ray, approx);
  }

  private static class DelegateCache<TState>
  {
    public static readonly 
    #nullable enable
    B2DynamicTree<FixtureProxy>.QueryCallback<(TState state, B2DynamicTree<FixtureProxy> tree, DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState> callback, Box2 aabb, bool approx, DynamicTree<FixtureProxy>.ExtractAabbDelegate extract)> AabbQueryState = new B2DynamicTree<FixtureProxy>.QueryCallback<(TState, B2DynamicTree<FixtureProxy>, DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState>, Box2, bool, DynamicTree<FixtureProxy>.ExtractAabbDelegate)>(DynamicTreeBroadPhase.AabbQueryStateCallback<TState>);
    public static readonly B2DynamicTree<FixtureProxy>.RayQueryCallback<(TState state, DynamicTree<FixtureProxy>.RayQueryCallbackDelegate<TState> callback, B2DynamicTree<FixtureProxy> tree, DynamicTree<FixtureProxy>.ExtractAabbDelegate? extract, Ray srcRay)> RayQueryState = new B2DynamicTree<FixtureProxy>.RayQueryCallback<(TState, DynamicTree<FixtureProxy>.RayQueryCallbackDelegate<TState>, B2DynamicTree<FixtureProxy>, DynamicTree<FixtureProxy>.ExtractAabbDelegate, Ray)>(DynamicTreeBroadPhase.RayQueryStateCallback<TState>);
  }
}
