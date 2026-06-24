// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.PointLightComponentState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable disable
namespace Robust.Shared.GameObjects;

[NetSerializable]
[Serializable]
public sealed class PointLightComponentState : ComponentState
{
  public Color Color;
  public float Energy;
  public float Softness;
  public float Falloff;
  public float CurveFactor;
  public bool CastShadows;
  public bool Enabled;
  public float Radius;
  public Vector2 Offset;
}
