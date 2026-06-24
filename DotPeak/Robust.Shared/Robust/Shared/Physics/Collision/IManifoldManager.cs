// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Collision.IManifoldManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Physics.Collision.Shapes;

#nullable enable
namespace Robust.Shared.Physics.Collision;

internal interface IManifoldManager
{
  EdgeShape GetContactEdge();

  void ReturnEdge(EdgeShape edge);

  bool TestOverlap<T, U>(
    T shapeA,
    int indexA,
    U shapeB,
    int indexB,
    in Transform xfA,
    in Transform xfB)
    where T : IPhysShape
    where U : IPhysShape;

  void CollideCircles(
    ref Manifold manifold,
    PhysShapeCircle circleA,
    in Transform xfA,
    PhysShapeCircle circleB,
    in Transform xfB);

  void CollideEdgeAndCircle(
    ref Manifold manifold,
    EdgeShape edgeA,
    in Transform transformA,
    PhysShapeCircle circleB,
    in Transform transformB);

  void CollideEdgeAndPolygon(
    ref Manifold manifold,
    EdgeShape edgeA,
    in Transform xfA,
    PolygonShape polygonB,
    in Transform xfB);

  void CollidePolygonAndCircle(
    ref Manifold manifold,
    PolygonShape polygonA,
    in Transform xfA,
    PhysShapeCircle circleB,
    in Transform xfB);

  void CollidePolygons(
    ref Manifold manifold,
    PolygonShape polyA,
    in Transform transformA,
    PolygonShape polyB,
    in Transform transformB);
}
