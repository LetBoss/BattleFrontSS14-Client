// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.DebugDrawingHandle
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics;

public abstract class DebugDrawingHandle
{
  public abstract Color GridFillColor { get; }

  public abstract Color RectFillColor { get; }

  public abstract Color WakeMixColor { get; }

  public abstract void DrawRect(in Box2 box, in Color color);

  public abstract void DrawRect(in Box2Rotated box, in Color color);

  public abstract void DrawCircle(Vector2 origin, float radius, in Color color);

  public abstract void DrawPolygonShape(Vector2[] vertices, in Color color);

  public abstract void DrawLine(Vector2 start, Vector2 end, in Color color);

  public abstract void SetTransform(in Matrix3x2 transform);

  public abstract Color CalcWakeColor(Color color, float wakePercent);
}
