// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Contacts.VelocityConstraintPoint
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Numerics;

#nullable disable
namespace Robust.Shared.Physics.Dynamics.Contacts;

internal struct VelocityConstraintPoint
{
  public Vector2 RelativeVelocityA;
  public Vector2 RelativeVelocityB;
  public float NormalImpulse;
  public float TangentImpulse;
  public float NormalMass;
  public float TangentMass;
  public float VelocityBias;
}
