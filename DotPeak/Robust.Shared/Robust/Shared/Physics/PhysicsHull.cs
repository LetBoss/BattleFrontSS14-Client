// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.PhysicsHull
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Numerics;
using System.Runtime.InteropServices;

#nullable disable
namespace Robust.Shared.Physics;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct PhysicsHull
{
  public static Span<Vector2> ComputePoints(ReadOnlySpan<Vector2> points, int count)
  {
    return InternalPhysicsHull.ComputeHull(points, count).Points;
  }
}
