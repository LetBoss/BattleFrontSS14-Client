// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.IBroadPhase`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics;

[NotContentImplementable]
public interface IBroadPhase<T> : ICollection<T>, IEnumerable<T>, IEnumerable where T : notnull
{
  int Capacity { get; }

  int Height { get; }

  int MaxBalance { get; }

  float AreaRatio { get; }

  bool Add(in T item, Box2? newAABB = null);

  bool Remove(in T item);

  bool Update(in T item, Box2? newAABB = null);

  void QueryAabb(DynamicTree<T>.QueryCallbackDelegate callback, Box2 aabb, bool approx = false);

  void QueryAabb<TState>(
    ref TState state,
    DynamicTree<T>.QueryCallbackDelegate<TState> callback,
    Box2 aabb,
    bool approx = false);

  IEnumerable<T> QueryAabb(Box2 aabb, bool approx = false);

  void QueryPoint(DynamicTree<T>.QueryCallbackDelegate callback, Vector2 point, bool approx = false);

  void QueryPoint<TState>(
    ref TState state,
    DynamicTree<T>.QueryCallbackDelegate<TState> callback,
    Vector2 point,
    bool approx = false);

  IEnumerable<T> QueryPoint(Vector2 point, bool approx = false);

  void QueryRay(DynamicTree<T>.RayQueryCallbackDelegate callback, in Ray ray, bool approx = false);

  void QueryRay<TState>(
    ref TState state,
    DynamicTree<T>.RayQueryCallbackDelegate<TState> callback,
    in Ray ray,
    bool approx = false);
}
