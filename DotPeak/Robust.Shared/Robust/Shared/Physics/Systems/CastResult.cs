// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Systems.CastResult
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Physics.Dynamics;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics.Systems;

public delegate float CastResult(
  FixtureProxy proxy,
  Vector2 point,
  Vector2 normal,
  float fraction,
  ref RayResult result);
