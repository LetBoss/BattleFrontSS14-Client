// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.IBroadPhase
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Dynamics;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics;

[NotContentImplementable]
public interface IBroadPhase
{
  int Count { get; }

  B2DynamicTree<FixtureProxy> Tree { get; }

  Box2 GetFatAabb(DynamicTree.Proxy proxy);

  DynamicTree.Proxy AddProxy(ref FixtureProxy proxy);

  void MoveProxy(DynamicTree.Proxy proxyId, in Box2 aabb);

  FixtureProxy? GetProxy(DynamicTree.Proxy proxy);

  void RemoveProxy(DynamicTree.Proxy proxy);

  void QueryAabb<TState>(
    ref TState state,
    DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState> callback,
    Box2 aabb,
    bool approx = false);

  IEnumerable<FixtureProxy> QueryAabb(Box2 aabb, bool approx = false);

  IEnumerable<FixtureProxy> QueryAabb(List<FixtureProxy> proxies, Box2 aabb, bool approx = false);

  void QueryPoint(
    DynamicTree<FixtureProxy>.QueryCallbackDelegate callback,
    Vector2 point,
    bool approx = false);

  void QueryPoint<TState>(
    ref TState state,
    DynamicTree<FixtureProxy>.QueryCallbackDelegate<TState> callback,
    Vector2 point,
    bool approx = false);

  IEnumerable<FixtureProxy> QueryPoint(Vector2 point, bool approx = false);

  void QueryRay(
    DynamicTree<FixtureProxy>.RayQueryCallbackDelegate callback,
    in Ray ray,
    bool approx = false);

  void QueryRay<TState>(
    ref TState state,
    DynamicTree<FixtureProxy>.RayQueryCallbackDelegate<TState> callback,
    in Ray ray,
    bool approx = false);

  void Rebuild(bool fullBuild);

  void RebuildBottomUp();
}
