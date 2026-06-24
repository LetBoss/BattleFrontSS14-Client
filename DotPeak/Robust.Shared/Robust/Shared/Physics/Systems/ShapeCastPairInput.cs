// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Systems.ShapeCastPairInput
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Physics.Collision;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Physics.Systems;

internal ref struct ShapeCastPairInput
{
  public DistanceProxy ProxyA;
  public DistanceProxy ProxyB;
  public Transform TransformA;
  public Transform TransformB;
  public Vector2 TranslationB;
  public float MaxFraction;
}
