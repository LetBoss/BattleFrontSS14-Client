// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Systems.WorldRayCastContext
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.Physics.Systems;

internal ref struct WorldRayCastContext
{
  public RayCastSystem System;
  public SharedPhysicsSystem Physics;
  public CastResult fcn;
  public QueryFilter Filter;
  public float Fraction;
  public RayResult Result;
}
