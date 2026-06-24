// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Collision.SimplexCache
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable disable
namespace Robust.Shared.Physics.Collision;

internal struct SimplexCache
{
  public ushort Count;
  public unsafe fixed byte IndexA[3];
  public unsafe fixed byte IndexB[3];
  public float Metric;
}
