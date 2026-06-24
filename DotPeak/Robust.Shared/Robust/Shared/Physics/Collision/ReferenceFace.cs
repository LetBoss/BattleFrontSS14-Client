// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Collision.ReferenceFace
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Numerics;

#nullable disable
namespace Robust.Shared.Physics.Collision;

public struct ReferenceFace
{
  public int i1;
  public int i2;
  public Vector2 v1;
  public Vector2 v2;
  public Vector2 normal;
  public Vector2 sideNormal1;
  public float sideOffset1;
  public Vector2 sideNormal2;
  public float sideOffset2;
}
