// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Collision.EPAxis
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Numerics;

#nullable disable
namespace Robust.Shared.Physics.Collision;

public struct EPAxis
{
  public int Index;
  public float Separation;
  public EPAxisType Type;
  public Vector2 Normal;
}
