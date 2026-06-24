// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Systems.CastOutput
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Numerics;

#nullable disable
namespace Robust.Shared.Physics.Systems;

internal ref struct CastOutput
{
  public Vector2 Normal;
  public Vector2 Point;
  public float Fraction;
  public int Iterations;
  public bool Hit;
}
