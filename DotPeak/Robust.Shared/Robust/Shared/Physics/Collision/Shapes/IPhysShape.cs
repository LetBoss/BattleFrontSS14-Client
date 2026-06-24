// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Collision.Shapes.IPhysShape
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Robust.Shared.Physics.Collision.Shapes;

[NotContentImplementable]
public interface IPhysShape : IEquatable<IPhysShape>
{
  int ChildCount { get; }

  float Radius { get; set; }

  ShapeType ShapeType { get; }

  Box2 ComputeAABB(Transform transform, int childIndex);
}
